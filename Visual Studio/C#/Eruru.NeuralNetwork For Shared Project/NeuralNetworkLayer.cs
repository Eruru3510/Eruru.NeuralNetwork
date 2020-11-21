using System;

namespace Eruru.NeuralNetwork {

	public abstract class NeuralNetworkLayer {

		public NeuralNetworkLayerType Type { get; set; }
		public int[] InputShape { get; set; }
		public int[] OutputShape { get; set; }

		public NeuralNetworkLayer (int[] inputShape, int[] outputShape) {
			InputShape = inputShape ?? throw new ArgumentNullException (nameof (inputShape));
			OutputShape = outputShape ?? throw new ArgumentNullException (nameof (outputShape));
		}

		public abstract object ForwardPropagation (object inputs);

		public abstract void Summary (out object neuronColumn, out object activationFunctionColumn, out object paddingColumn);

	}

}