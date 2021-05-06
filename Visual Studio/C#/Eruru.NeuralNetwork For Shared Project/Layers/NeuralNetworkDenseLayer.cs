using System;
#if NET40_OR_GREATER
using System.Threading.Tasks;
#endif

namespace Eruru.NeuralNetwork {

	public class NeuralNetworkDenseLayer : NeuralNetworkLayer {

		public NeuralNetworkActivationFunctionType ActivationFunctionType { get; set; }
		public NeuralNetworkNeuron[] Neurons { get; set; }

		public NeuralNetworkDenseLayer (int[] inputShape, int[] outputShape, NeuralNetworkNeuron[] neurons, NeuralNetworkActivationFunctionType activationFunctionType) : base (inputShape, outputShape) {
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
#if NET40_OR_GREATER
			Parallel.For (0, Neurons.Length, i => {
				outputs[i] = Neurons[i].ForwardPropagation (ref inputValues, ActivationFunctionType);
			});
#else
			for (int i = 0; i < Neurons.Length; i++) {
				outputs[i] = Neurons[i].ForwardPropagation (ref inputValues, ActivationFunctionType);
			}
#endif
			switch (ActivationFunctionType) {
				case NeuralNetworkActivationFunctionType.ReLU:
					break;
				case NeuralNetworkActivationFunctionType.Softmax: {
					float max = NeuralNetworkApi.Max (outputs);
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
				case NeuralNetworkActivationFunctionType.Sigmoid:
					break;
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