using System;
using System.Threading.Tasks;

namespace Eruru.NeuralNetwork {

	public class NeuralNetworkConv2DLayer : NeuralNetworkLayer {

		public Kernel[] Kernels { get; set; }
		public NeuralNetworkActivationFunctionType ActivationFunctionType { get; set; }
		public NeuralNetworkPaddingType PaddingType { get; set; }

		public NeuralNetworkConv2DLayer (Kernel[] kernels, NeuralNetworkActivationFunctionType activationFunctionType, NeuralNetworkPaddingType paddingType, int[] inputShape, int[] outputShape) : base (inputShape, outputShape) {
			if (inputShape is null) {
				throw new ArgumentNullException (nameof (inputShape));
			}
			if (outputShape is null) {
				throw new ArgumentNullException (nameof (outputShape));
			}
			Type = NeuralNetworkLayerType.Conv2D;
			Kernels = kernels ?? throw new ArgumentNullException (nameof (kernels));
			ActivationFunctionType = activationFunctionType;
			PaddingType = paddingType;
		}

		public override object ForwardPropagation (object inputs) {
			float[,,] inputValues = (float[,,])inputs;
			float[,,] outputs = new float[OutputShape[0], OutputShape[1], OutputShape[2]];
			Parallel.For (0, Kernels.Length, i => {
				Kernels[i].ForwardPropagation (ref inputValues, ref outputs, i, ActivationFunctionType, PaddingType);
			});
			return outputs;
		}

		public override void Summary (out object neuronColumn, out object activationFunctionColumn, out object paddingColumn) {
			neuronColumn = Kernels.Length;
			activationFunctionColumn = ActivationFunctionType;
			paddingColumn = PaddingType;
		}

	}

}