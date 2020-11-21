namespace Eruru.NeuralNetwork {

	public class Neuron {

		public float[] Weights { get; set; }
		public float Bias { get; set; }

		public Neuron (int inputNumber, float bias) {
			Weights = new float[inputNumber];
			Bias = bias;
		}

		public float ForwardPropagation (ref float[] inputs, NeuralNetworkActivationFunctionType activationFunctionType) {
			float sum = 0;
			for (int i = 0; i < Weights.Length; i++) {
				sum += inputs[i] * Weights[i];
			};
			return NeuralNetworkAPI.ActivationFunction (activationFunctionType, sum + Bias);
		}

	}

}