using System;
#if NET40_OR_GREATER
using System.Threading.Tasks;
#endif

namespace Eruru.NeuralNetwork {

	public class NeuralNetworkConv2DLayer : NeuralNetworkLayer {

		public NeuralNetworkKernel[] Kernels { get; set; }
		public NeuralNetworkActivationFunctionType ActivationFunctionType { get; set; }
		public NeuralNetworkPaddingType PaddingType { get; set; }

		public NeuralNetworkConv2DLayer (int[] inputShape, int[] outputShape, NeuralNetworkKernel[] kernels, NeuralNetworkActivationFunctionType activationFunctionType, NeuralNetworkPaddingType paddingType) : base (inputShape, outputShape) {
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
#if NET40_OR_GREATER
			Parallel.For (0, Kernels.Length, i => {
				Kernels[i].ForwardPropagation (ref inputValues, ref outputs, i, ActivationFunctionType, PaddingType);
			});
#else
			for (int i = 0; i < Kernels.Length; i++) {
				Kernels[i].ForwardPropagation (ref inputValues, ref outputs, i, ActivationFunctionType, PaddingType);
			}
#endif
			return outputs;
		}

		public override void Summary (out object neuronColumn, out object activationFunctionColumn, out object paddingColumn) {
			neuronColumn = Kernels.Length;
			activationFunctionColumn = ActivationFunctionType;
			paddingColumn = PaddingType;
		}

	}

}