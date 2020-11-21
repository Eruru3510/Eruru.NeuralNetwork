using System;
using System.Collections.Generic;
using Eruru.Json;
using HDF5DotNet;

namespace Eruru.NeuralNetwork {

	public class NeuralNetwork {

		public List<NeuralNetworkLayer> Layers { get; set; } = new List<NeuralNetworkLayer> ();

		public void Load (string h5file) {
			if (h5file is null) {
				throw new ArgumentNullException (nameof (h5file));
			}
			H5FileId h5FileId = H5F.open (h5file, H5F.OpenMode.ACC_RDONLY);
			H5GroupId h5GroupId = H5G.open (h5FileId, "/");
			H5GroupId h5GroupIdModelWeights = H5G.open (h5GroupId, "model_weights");
			JsonObject ModelConfig = JsonObject.Parse (NeuralNetworkAPI.GetAttributeValue (h5GroupId, "model_config"));
			int[] inputShape = null;
			foreach (JsonObject layer in ModelConfig["config"]["layers"].Array) {
				string className = layer["class_name"];
				if (className == "InputLayer") {
					continue;
				}
				JsonObject config = layer["config"];
				string name = config["name"];
				JsonValue activation = config["activation"];
				JsonValue dataFormat = config["data_format"];
				JsonValue padding = config["padding"];
				if (inputShape is null) {
					JsonArray batchInputShape = config["batch_input_shape"];
					batchInputShape.RemoveAt (0);
					inputShape = JsonConvert.Deserialize<int[]> (batchInputShape);
				}
				switch (className) {
					case nameof (NeuralNetworkLayerType.Dense): {
						int units = config["units"];
						NeuralNetworkAPI.GetDataSet (h5GroupIdModelWeights, $"{name}/{name}/kernel:0", out float[,] weights, units, inputShape[0]);
						NeuralNetworkAPI.GetDataSet (h5GroupIdModelWeights, $"{name}/{name}/bias:0", out float[] biases, units);
						Neuron[] neurons = new Neuron[units];
						for (int i = 0; i < neurons.Length; i++) {
							neurons[i] = new Neuron (inputShape[0], biases[i]);
							for (int n = 0; n < inputShape[0]; n++) {
								neurons[i].Weights[n] = weights[n, i];
							}
						}
						int[] outputShape = new int[] { units };
						Add (new NeuralNetworkDenseLayer (neurons, inputShape, outputShape, GetActivationFunctionType (activation)));
						inputShape = outputShape;
						break;
					}
					case nameof (NeuralNetworkLayerType.Conv2D): {
						JsonArray kernelSize = config["kernel_size"];
						JsonArray strides = config["strides"];
						int units = config["filters"];
						int width = kernelSize[1];
						int height = kernelSize[0];
						int channel = inputShape[2];
						int strideX = strides[1];
						int strideY = strides[0];
						NeuralNetworkAPI.GetDataSet (h5GroupIdModelWeights, $"{name}/{name}/kernel:0", out float[,,,] weights, width, height, channel, units);
						NeuralNetworkAPI.GetDataSet (h5GroupIdModelWeights, $"{name}/{name}/bias:0", out float[] biases, units);
						Kernel[] kernels = new Kernel[units];
						for (int i = 0; i < kernels.Length; i++) {
							kernels[i] = new Kernel (width, height, channel, strideX, strideY, (float)biases[i]);
							for (int c = 0; c < channel; c++) {
								for (int y = 0; y < height; y++) {
									for (int x = 0; x < width; x++) {
										kernels[i].Weights[y, x, c] = (float)weights[y, x, c, i];
									}
								}
							}
						}
						NeuralNetworkPaddingType paddingType = GetPaddingType (padding);
						int[] outputShape = new int[] {
							NeuralNetworkAPI.CalculateOutputLength (inputShape[0], height, strideY, paddingType),
							NeuralNetworkAPI.CalculateOutputLength (inputShape[1], width, strideX, paddingType),
							units
						};
						Add (new NeuralNetworkConv2DLayer (kernels, GetActivationFunctionType (activation), paddingType, inputShape, outputShape));
						inputShape = outputShape;
						break;
					}
					case nameof (NeuralNetworkLayerType.MaxPooling2D): {
						JsonArray poolSize = config["pool_size"];
						JsonArray strides = config["strides"];
						int width = poolSize[1];
						int height = poolSize[0];
						int strideX = strides[1];
						int strideY = strides[0];
						NeuralNetworkPaddingType paddingType = GetPaddingType (padding);
						int[] outputShape = new int[] {
							NeuralNetworkAPI.CalculateOutputLength (inputShape[0], height, strideY, paddingType),
							NeuralNetworkAPI.CalculateOutputLength (inputShape[1], width, strideX, paddingType),
							inputShape[2]
						};
						Add (new NeuralNetworkMaxPooling2DLayer (width, height, strideX, strideY, paddingType, inputShape, outputShape));
						inputShape = outputShape;
						break;
					}
					case nameof (NeuralNetworkLayerType.Dropout): {
						Add (new NeuralNetworkDropoutLayer (config["rate"], inputShape, inputShape));
						break;
					}
					case nameof (NeuralNetworkLayerType.Flatten): {
						NeuralNetworkDataFormatType dataFormatType = GetDataFormatType (config["data_format"]);
						int sum = 1;
						NeuralNetworkLayer lastLayer = Layers[Layers.Count - 1];
						foreach (int length in lastLayer.OutputShape) {
							sum *= length;
						}
						int[] outputShape = new int[] { sum };
						Add (new NeuralNetworkFlattenLayer (inputShape, outputShape, dataFormatType));
						inputShape = outputShape;
						break;
					}
					default:
						throw new NotImplementedException (className);
				}
			}
			H5G.close (h5GroupIdModelWeights);
			H5G.close (h5GroupId);
			H5F.close (h5FileId);
			NeuralNetworkActivationFunctionType GetActivationFunctionType (string name) {
				switch (name) {
					case "relu":
						return NeuralNetworkActivationFunctionType.ReLU;
					case "softmax":
						return NeuralNetworkActivationFunctionType.Softmax;
					default:
						throw new NotImplementedException (name);
				}
			}
			NeuralNetworkDataFormatType GetDataFormatType (string name) {
				switch (name) {
					case "channels_last":
						return NeuralNetworkDataFormatType.ChannelsLast;
					default:
						throw new NotImplementedException (name);
				}
			}
			NeuralNetworkPaddingType GetPaddingType (string name) {
				switch (name) {
					case "valid":
						return NeuralNetworkPaddingType.Valid;
					case "same":
						return NeuralNetworkPaddingType.Same;
					default:
						throw new NotImplementedException (name);
				}
			}
			NeuralNetworkLayer Add (NeuralNetworkLayer layer) {
				Layers.Add (layer);
				return layer;
			}
		}

		public void Summary () {
			Console.Write (NeuralNetworkAPI.PadRight ("层"));
			Console.Write (NeuralNetworkAPI.PadRight ("输入"));
			Console.Write (NeuralNetworkAPI.PadRight ("神经元"));
			Console.Write (NeuralNetworkAPI.PadRight ("激活函数"));
			Console.Write (NeuralNetworkAPI.PadRight ("Padding"));
			Console.Write (NeuralNetworkAPI.PadRight ("输出"));
			Console.WriteLine ();
			foreach (var layer in Layers) {
				Console.Write (NeuralNetworkAPI.PadRight (layer.Type));
				Console.Write (NeuralNetworkAPI.PadRight (NeuralNetworkAPI.Shape (layer.InputShape)));
				layer.Summary (out object neuronColumn, out object activationFunctionColumn, out object paddingColumn);
				Console.Write (NeuralNetworkAPI.PadRight (neuronColumn));
				Console.Write (NeuralNetworkAPI.PadRight (activationFunctionColumn));
				Console.Write (NeuralNetworkAPI.PadRight (paddingColumn));
				Console.Write (NeuralNetworkAPI.PadRight (NeuralNetworkAPI.Shape (layer.OutputShape)));
				Console.WriteLine ();
			}
		}

		public object ForwardPropagation (object inputs) {
			if (inputs is null) {
				throw new ArgumentNullException (nameof (inputs));
			}
			object outputs = inputs;
			foreach (var layer in Layers) {
				outputs = layer.ForwardPropagation (outputs);
			}
			return outputs;
		}

		public int PredictClasses (object inputs) {
			if (inputs is null) {
				throw new ArgumentNullException (nameof (inputs));
			}
			return NeuralNetworkAPI.IndexOfMax ((float[])ForwardPropagation (inputs));
		}

	}

}