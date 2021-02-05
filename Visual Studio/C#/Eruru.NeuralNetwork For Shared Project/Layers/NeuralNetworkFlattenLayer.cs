using System;

namespace Eruru.NeuralNetwork {

	public class NeuralNetworkFlattenLayer : NeuralNetworkLayer {

		public NeuralNetworkDataFormatType DataFormatType { get; set; }

		public NeuralNetworkFlattenLayer (int[] inputShape, int[] outputShape, NeuralNetworkDataFormatType dataFormatType) : base (inputShape, outputShape) {
			if (inputShape is null) {
				throw new ArgumentNullException (nameof (inputShape));
			}
			if (outputShape is null) {
				throw new ArgumentNullException (nameof (outputShape));
			}
			Type = NeuralNetworkLayerType.Flatten;
			InputShape = inputShape ?? throw new ArgumentNullException (nameof (inputShape));
			OutputShape = outputShape ?? throw new ArgumentNullException (nameof (outputShape));
			DataFormatType = dataFormatType;
		}

		public override object ForwardPropagation (object inputs) {
			float[,,] inputValues = (float[,,])inputs;
			float[] outputs = new float[inputValues.Length];
			int width = inputValues.GetLength (1);
			int height = inputValues.GetLength (0);
			int channel = inputValues.GetLength (2);
			int index = 0;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					for (int c = 0; c < channel; c++) {
						outputs[index++] = inputValues[y, x, c];
					}
				}
			}
			return outputs;
		}

		public override void Summary (out object neuronColumn, out object activationFunctionColumn, out object paddingColumn) {
			neuronColumn = "无";
			activationFunctionColumn = "无";
			paddingColumn = "无";
		}

	}

}