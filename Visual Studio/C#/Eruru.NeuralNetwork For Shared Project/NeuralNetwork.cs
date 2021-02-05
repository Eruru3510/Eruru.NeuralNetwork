using System;
using System.Collections.Generic;
using System.IO;
using Eruru.Json;

namespace Eruru.NeuralNetwork {

	public class NeuralNetwork {

		public List<NeuralNetworkLayerBase> Layers { get; set; } = new List<NeuralNetworkLayerBase> ();

		public void Load (INeuralNetworkLoader loader, JsonTextWriter textWriter = null) {
			if (loader is null) {
				throw new ArgumentNullException (nameof (loader));
			}
			JsonObject modelConfig = loader.GetModelConfig ();
			if (textWriter != null) {
				textWriter.BeginObject ();
				textWriter.Write (NeuralNetworkKeyword.ModelConfig);
				new JsonTextBuilder (new JsonValueReader (modelConfig), textWriter).BuildObject ();
			}
			int[] inputShape = null;
			Layers.Clear ();
			foreach (JsonObject layer in modelConfig["config"]["layers"].Array) {
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
					List<int> inputShapes = JsonConvert.Deserialize<List<int>> (batchInputShape);
					inputShapes.RemoveAt (0);
					inputShape = inputShapes.ToArray ();
				}
				switch (className) {
					case nameof (NeuralNetworkLayerType.Dense): {
						int units = config["units"];
						float[,] weights = loader.GetDenseWeights (name, units, inputShape[0]);
						float[] biases = loader.GetBiases (name, units);
						if (textWriter != null) {
							WriteLayer (textWriter, name, weights, biases);
						}
						Neuron[] neurons = new Neuron[units];
						for (int i = 0; i < neurons.Length; i++) {
							neurons[i] = new Neuron (inputShape[0], biases[i]);
							for (int n = 0; n < inputShape[0]; n++) {
								neurons[i].Weights[n] = weights[n, i];
							}
						}
						int[] outputShape = new int[] { units };
						Layers.Add (new NeuralNetworkDenseLayer (
							inputShape,
							outputShape,
							neurons,
							NeuralNetworkAPI.GetActivationFunctionType (activation)
						));
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
						float[,,,] weights = loader.GetConv2DWeights (name, width, height, channel, units);
						float[] biases = loader.GetBiases (name, units);
						if (textWriter != null) {
							WriteLayer (textWriter, name, weights, biases);
						}
						Kernel[] kernels = new Kernel[units];
						for (int i = 0; i < kernels.Length; i++) {
							kernels[i] = new Kernel (width, height, channel, strideX, strideY, biases[i]);
							for (int c = 0; c < channel; c++) {
								for (int y = 0; y < height; y++) {
									for (int x = 0; x < width; x++) {
										kernels[i].Weights[y, x, c] = weights[y, x, c, i];
									}
								}
							}
						}
						NeuralNetworkPaddingType paddingType = NeuralNetworkAPI.GetPaddingType (padding);
						int[] outputShape = new int[] {
							NeuralNetworkAPI.CalculateOutputLength (inputShape[0], height, strideY, paddingType),
							NeuralNetworkAPI.CalculateOutputLength (inputShape[1], width, strideX, paddingType),
							units
						};
						Layers.Add (new NeuralNetworkConv2DLayer (
							inputShape,
							outputShape,
							kernels,
							NeuralNetworkAPI.GetActivationFunctionType (activation),
							paddingType
						));
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
						NeuralNetworkPaddingType paddingType = NeuralNetworkAPI.GetPaddingType (padding);
						int[] outputShape = new int[] {
							NeuralNetworkAPI.CalculateOutputLength (inputShape[0], height, strideY, paddingType),
							NeuralNetworkAPI.CalculateOutputLength (inputShape[1], width, strideX, paddingType),
							inputShape[2]
						};
						Layers.Add (new NeuralNetworkMaxPooling2DLayer (inputShape, outputShape, width, height, strideX, strideY, paddingType));
						inputShape = outputShape;
						break;
					}
					case nameof (NeuralNetworkLayerType.Dropout): {
						Layers.Add (new NeuralNetworkDropoutLayer (inputShape, inputShape));
						break;
					}
					case nameof (NeuralNetworkLayerType.Flatten): {
						NeuralNetworkDataFormatType dataFormatType = NeuralNetworkAPI.GetDataFormatType (config["data_format"]);
						int sum = 1;
						NeuralNetworkLayerBase lastLayer = Layers[Layers.Count - 1];
						foreach (int length in lastLayer.OutputShape) {
							sum *= length;
						}
						int[] outputShape = new int[] { sum };
						Layers.Add (new NeuralNetworkFlattenLayer (inputShape, outputShape, dataFormatType));
						inputShape = outputShape;
						break;
					}
					default:
						throw new NotImplementedException (className);
				}
			}
			if (textWriter != null) {
				textWriter.EndObject ();
			}
		}

		public void LoadH5 (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			using (NeuralNetworkH5Loader loader = new NeuralNetworkH5Loader (path)) {
				Load (loader);
			}
		}

		public void LoadJson (string text) {
			if (text is null) {
				throw new ArgumentNullException (nameof (text));
			}

			LoadJson (new StringReader (text));
		}
		public void LoadJson (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			Load (new NeuralNetworkJsonLoader (textReader));
		}

		public void LoadJsonFile (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			LoadJson (new StreamReader (path));
		}

		public static void H5ToJson (string h5Path, TextWriter textWriter) {
			if (h5Path is null) {
				throw new ArgumentNullException (nameof (h5Path));
			}
			if (textWriter is null) {
				throw new ArgumentNullException (nameof (textWriter));
			}
			using (JsonTextWriter jsonTextWriter = new JsonTextWriter (textWriter)) {
				using (NeuralNetworkH5Loader loader = new NeuralNetworkH5Loader (h5Path)) {
					new NeuralNetwork ().Load (loader, jsonTextWriter);
				}
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

		void WriteLayer (JsonTextWriter textWriter, string name, object weights, float[] biases) {
			textWriter.Write (name);
			textWriter.BeginObject ();
			textWriter.Write ("kernel");
			new JsonTextBuilder (new JsonSerializer (weights), textWriter).BuildArray ();
			textWriter.Write ("bias");
			new JsonTextBuilder (new JsonSerializer (biases), textWriter).BuildArray ();
			textWriter.EndObject ();
		}

	}

}