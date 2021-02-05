using System;
using System.IO;
using Eruru.Json;

namespace Eruru.NeuralNetwork {

	public class NeuralNetworkJsonLoader : INeuralNetworkLoader {

		readonly JsonObject Data;

		public NeuralNetworkJsonLoader (TextReader textReader) {
			if (textReader is null) {
				throw new ArgumentNullException (nameof (textReader));
			}
			Data = JsonObject.Load (textReader);
		}

		public float[] GetBiases (string name, int units) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return JsonConvert.Deserialize<float[]> (Data[name]["bias"]);
		}

		public float[,,,] GetConv2DWeights (string name, int width, int height, int channel, int units) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return JsonConvert.Deserialize<float[,,,]> (Data[name]["kernel"]);
		}

		public float[,] GetDenseWeights (string name, int units, int inputShape) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			return JsonConvert.Deserialize<float[,]> (Data[name]["kernel"]);
		}

		public JsonObject GetModelConfig () {
			return Data[NeuralNetworkKeyword.ModelConfig];
		}

	}

}