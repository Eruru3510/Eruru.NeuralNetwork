using System;
#if NET40_OR_GREATER
using System.Threading.Tasks;
#endif

namespace Eruru.NeuralNetwork {

	public class NeuralNetworkMaxPooling2DLayer : NeuralNetworkLayer {

		public int Width;
		public int Height;
		public int StrideX;
		public int StrideY;
		public NeuralNetworkPaddingType PaddingType;

		public NeuralNetworkMaxPooling2DLayer (int[] inputShape, int[] outputShape, int width, int height, int strideX, int strideY, NeuralNetworkPaddingType paddingType) : base (inputShape, outputShape) {
			if (inputShape is null) {
				throw new ArgumentNullException (nameof (inputShape));
			}
			if (outputShape is null) {
				throw new ArgumentNullException (nameof (outputShape));
			}
			Type = NeuralNetworkLayerType.MaxPooling2D;
			Width = width;
			Height = height;
			StrideX = strideX;
			StrideY = strideY;
			PaddingType = paddingType;
		}

		public override object ForwardPropagation (object inputs) {
			float[,,] inputValues = (float[,,])inputs;
			int inputWidth = inputValues.GetLength (1);
			int inputHeight = inputValues.GetLength (0);
			int inputChannel = inputValues.GetLength (2);
			int outputWidth = OutputShape[1];
			int outputHeight = OutputShape[0];
			int paddingX;
			int paddingY;
			switch (PaddingType) {
				case NeuralNetworkPaddingType.Valid:
					paddingX = 0;
					paddingY = 0;
					break;
				case NeuralNetworkPaddingType.Same:
					paddingX = (Width - 1) / 2;
					paddingY = (Height - 1) / 2;
					break;
				default:
					throw new NotImplementedException (PaddingType.ToString ());
			}
			int startX = 0 - paddingX;
			int startY = 0 - paddingY;
			int endX = inputWidth + paddingX - Width;
			int endY = inputHeight + paddingY - Height;
			float[,,] outputs = new float[outputHeight, outputWidth, inputChannel];
#if NET40_OR_GREATER
			Parallel.For (0, inputChannel, c => {
				int outputX = 0;
				int outputY = 0;
				for (int y = startY; y <= endY; y += StrideY, outputY++) {
					for (int x = startX; x <= endX; x += StrideX, outputX++) {
						bool first = true;
						float max = 0;
						int ex = x + Width;
						int eY = y + Height;
						for (int cy = y; cy < eY; cy++) {
							for (int cx = x; cx < ex; cx++) {
								float value;
								if (cy < 0 || cy >= endY || cx < 0 || cx >= endX) {
									value = 0;
								} else {
									value = inputValues[cy, cx, c];
								}
								if (first || max < value) {
									first = false;
									max = value;
								}
							}
						}
						outputs[outputY, outputX, c] = max;
					}
					outputX = 0;
				}
			});
#else
			for (int c = 0; c < inputChannel; c++) {
				int outputX = 0;
				int outputY = 0;
				for (int y = startY; y <= endY; y += StrideY, outputY++) {
					for (int x = startX; x <= endX; x += StrideX, outputX++) {
						bool first = true;
						float max = 0;
						int ex = x + Width;
						int eY = y + Height;
						for (int cy = y; cy < eY; cy++) {
							for (int cx = x; cx < ex; cx++) {
								float value;
								if (cy < 0 || cy >= endY || cx < 0 || cx >= endX) {
									value = 0;
								} else {
									value = inputValues[cy, cx, c];
								}
								if (first || max < value) {
									first = false;
									max = value;
								}
							}
						}
						outputs[outputY, outputX, c] = max;
					}
					outputX = 0;
				}
			}
#endif
			return outputs;
		}

		public override void Summary (out object neuronColumn, out object activationFunctionColumn, out object paddingColumn) {
			neuronColumn = "无";
			activationFunctionColumn = "无";
			paddingColumn = PaddingType;
		}

	}

}