using System;
using System.Collections.Generic;
using System.IO;
using Eruru.Json;

namespace Eruru.NeuralNetwork {

	public class NeuralNetwork {

		public List<NeuralNetworkLayer> Layers { get; set; } = new List<NeuralNetworkLayer> ();

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
					batchInputShape.RemoveAt (0);
					List<int> inputShapes = JsonConvert.Deserialize<List<int>> (batchInputShape);
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
						NeuralNetworkNeuron[] neurons = new NeuralNetworkNeuron[units];
						for (int i = 0; i < neurons.Length; i++) {
							neurons[i] = new NeuralNetworkNeuron (inputShape[0], biases[i]);
							for (int n = 0; n < inputShape[0]; n++) {
								neurons[i].Weights[n] = weights[n, i];
							}
						}
						int[] outputShape = new int[] { units };
						Layers.Add (new NeuralNetworkDenseLayer (
							inputShape,
							outputShape,
							neurons,
							NeuralNetworkApi.GetActivationFunctionType (activation)
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
						NeuralNetworkKernel[] kernels = new NeuralNetworkKernel[units];
						for (int i = 0; i < kernels.Length; i++) {
							kernels[i] = new NeuralNetworkKernel (width, height, channel, strideX, strideY, biases[i]);
							for (int c = 0; c < channel; c++) {
								for (int y = 0; y < height; y++) {
									for (int x = 0; x < width; x++) {
										kernels[i].Weights[y, x, c] = weights[y, x, c, i];
									}
								}
							}
						}
						NeuralNetworkPaddingType paddingType = NeuralNetworkApi.GetPaddingType (padding);
						int[] outputShape = new int[] {
							NeuralNetworkApi.CalculateOutputLength (inputShape[0], height, strideY, paddingType),
							NeuralNetworkApi.CalculateOutputLength (inputShape[1], width, strideX, paddingType),
							units
						};
						Layers.Add (new NeuralNetworkConv2DLayer (
							inputShape,
							outputShape,
							kernels,
							NeuralNetworkApi.GetActivationFunctionType (activation),
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
						NeuralNetworkPaddingType paddingType = NeuralNetworkApi.GetPaddingType (padding);
						int[] outputShape = new int[] {
							NeuralNetworkApi.CalculateOutputLength (inputShape[0], height, strideY, paddingType),
							NeuralNetworkApi.CalculateOutputLength (inputShape[1], width, strideX, paddingType),
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
						NeuralNetworkDataFormatType dataFormatType = NeuralNetworkApi.GetDataFormatType (config["data_format"]);
						int sum = 1;
						NeuralNetworkLayer lastLayer = Layers[Layers.Count - 1];
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

#if NET40_OR_GREATER
		public void LoadH5 (string path) {
			if (path is null) {
				throw new ArgumentNullException (nameof (path));
			}
			using (NeuralNetworkH5Loader loader = new NeuralNetworkH5Loader (path)) {
				Load (loader);
			}
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
#endif

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

		public void Summary () {
			Console.Write (NeuralNetworkApi.PadRight ("层"));
			Console.Write (NeuralNetworkApi.PadRight ("输入"));
			Console.Write (NeuralNetworkApi.PadRight ("神经元"));
			Console.Write (NeuralNetworkApi.PadRight ("激活函数"));
			Console.Write (NeuralNetworkApi.PadRight ("Padding"));
			Console.Write (NeuralNetworkApi.PadRight ("输出"));
			Console.WriteLine ();
			foreach (var layer in Layers) {
				Console.Write (NeuralNetworkApi.PadRight (layer.Type));
				Console.Write (NeuralNetworkApi.PadRight (NeuralNetworkApi.Shape (layer.InputShape)));
				layer.Summary (out object neuronColumn, out object activationFunctionColumn, out object paddingColumn);
				Console.Write (NeuralNetworkApi.PadRight (neuronColumn));
				Console.Write (NeuralNetworkApi.PadRight (activationFunctionColumn));
				Console.Write (NeuralNetworkApi.PadRight (paddingColumn));
				Console.Write (NeuralNetworkApi.PadRight (NeuralNetworkApi.Shape (layer.OutputShape)));
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
			return NeuralNetworkApi.IndexOfMax ((float[])ForwardPropagation (inputs));
		}

		public float Predict (object inputs) {
			if (inputs is null) {
				throw new ArgumentNullException (nameof (inputs));
			}
			return ((float[])ForwardPropagation (inputs))[0];
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