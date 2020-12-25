using System;

namespace Eruru.NeuralNetwork {

	public class NeuralNetworkDropoutLayer : NeuralNetworkLayerBase {

		public NeuralNetworkDropoutLayer (int[] inputShape, int[] outputShape) : base (inputShape, outputShape) {
			if (inputShape is null) {
				throw new ArgumentNullException (nameof (inputShape));
			}
			if (outputShape is null) {
				throw new ArgumentNullException (nameof (outputShape));
			}
			Type = NeuralNetworkLayerType.Dropout;
		}

		public override object ForwardPropagation (object inputs) {
			return inputs;
		}

		public override void Summary (out object neuronColumn, out object activationFunctionColumn, out object paddingColumn) {
			neuronColumn = "无";
			activationFunctionColumn = "无";
			paddingColumn = "无";
		}

	}

}