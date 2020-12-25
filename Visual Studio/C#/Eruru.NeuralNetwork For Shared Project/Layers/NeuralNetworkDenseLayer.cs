using System;
using System.Threading.Tasks;

namespace Eruru.NeuralNetwork {

	public class NeuralNetworkDenseLayer : NeuralNetworkLayerBase {

		public NeuralNetworkActivationFunctionType ActivationFunctionType { get; set; }
		public Neuron[] Neurons { get; set; }

		public NeuralNetworkDenseLayer (int[] inputShape, int[] outputShape, Neuron[] neurons, NeuralNetworkActivationFunctionType activationFunctionType) : base (inputShape, outputShape) {
			if (inputShape is null) {
				throw new ArgumentNullException (nameof (inputShape));
			}
			if (outputShape is null) {
				throw new ArgumentNullException (nameof (outputShape));
			}
			Type = NeuralNetworkLayerType.Dense;
			Neurons = neurons ?? throw new ArgumentNullException (nameof (neurons));
			ActivationFunctionType = activationFunctionType;
		}

		public override object ForwardPropagation (object inputs) {
			float[] inputValues = (float[])inputs;
			float[] outputs = new float[Neurons.Length];
			Parallel.For (0, Neurons.Length, i => {
				outputs[i] = Neurons[i].ForwardPropagation (ref inputValues, ActivationFunctionType);
			});
			switch (ActivationFunctionType) {
				case NeuralNetworkActivationFunctionType.ReLU:
					break;
				case NeuralNetworkActivationFunctionType.Softmax: {
					float max = NeuralNetworkAPI.Max (outputs);
					float sum = 0;
					for (int i = 0; i < outputs.Length; i++) {
						outputs[i] = (float)Math.Exp (outputs[i] - max);
						sum += outputs[i];
					};
					for (int i = 0; i < outputs.Length; i++) {
						outputs[i] = outputs[i] / sum;
					}
					break;
				}
				default:
					throw new NotImplementedException (ActivationFunctionType.ToString ());
			}
			return outputs;
		}

		public override void Summary (out object neuronColumn, out object activationFunctionColumn, out object paddingColumn) {
			neuronColumn = Neurons.Length;
			activationFunctionColumn = ActivationFunctionType;
			paddingColumn = "无";
		}

	}

}