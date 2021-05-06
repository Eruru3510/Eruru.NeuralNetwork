#if NET40_OR_GREATER
using System;
using Eruru.Json;
using HDF5DotNet;

namespace Eruru.NeuralNetwork {

	public class NeuralNetworkH5Loader : IDisposable, INeuralNetworkLoader {

		readonly H5FileId H5FileId;
		readonly H5GroupId H5GroupId;
		readonly H5GroupId H5GroupIdModelWeights;

		public NeuralNetworkH5Loader (string h5File) {
			if (h5File is null) {
				throw new ArgumentNullException (nameof (h5File));
			}
			H5FileId = H5F.open (h5File, H5F.OpenMode.ACC_RDONLY);
			H5GroupId = H5G.open (H5FileId, "/");
			H5GroupIdModelWeights = H5G.open (H5GroupId, "model_weights");
		}

		public JsonObject GetModelConfig () {
			return JsonObject.Parse (NeuralNetworkApi.GetAttributeValue (H5GroupId, NeuralNetworkKeyword.ModelConfig));
		}

		public float[,] GetDenseWeights (string name, int units, int inputShape) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			NeuralNetworkApi.GetDataSet (H5GroupIdModelWeights, $"{name}/{name}/kernel:0", out float[,] weights, units, inputShape);
			return weights;
		}

		public float[,,,] GetConv2DWeights (string name, int width, int height, int channel, int units) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			NeuralNetworkApi.GetDataSet (H5GroupIdModelWeights, $"{name}/{name}/kernel:0", out float[,,,] weights, width, height, channel, units);
			return weights;
		}

		public float[] GetBiases (string name, int units) {
			if (name is null) {
				throw new ArgumentNullException (nameof (name));
			}
			NeuralNetworkApi.GetDataSet (H5GroupIdModelWeights, $"{name}/{name}/bias:0", out float[] biases, units);
			return biases;
		}

		public void Dispose () {
			H5G.close (H5GroupIdModelWeights);
			H5G.close (H5GroupId);
			H5F.close (H5FileId);
		}

	}

}
#endif