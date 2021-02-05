using Eruru.Json;

namespace Eruru.NeuralNetwork {

	public interface INeuralNetworkLoader {

		JsonObject GetModelConfig ();

		float[,] GetDenseWeights (string name, int units, int inputShape);

		float[,,,] GetConv2DWeights (string name, int width, int height, int channel, int units);

		float[] GetBiases (string name, int units);

	}

}