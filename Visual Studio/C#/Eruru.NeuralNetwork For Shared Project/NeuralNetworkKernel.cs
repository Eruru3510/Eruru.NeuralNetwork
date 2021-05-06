using System;

namespace Eruru.NeuralNetwork {

	public class NeuralNetworkKernel {

		public float[,,] Weights { get; set; }
		public int StrideX { get; set; }
		public int StrideY { get; set; }
		public float Bias { get; set; }

		public NeuralNetworkKernel (int width, int height, int channel, int strideX, int strideY, float bias) {
			Weights = new float[height, width, channel];
			StrideX = strideX;
			StrideY = strideY;
			Bias = bias;
		}

		public void ForwardPropagation (ref float[,,] inputs, ref float[,,] outputs, int channel, NeuralNetworkActivationFunctionType activationFunctionType, NeuralNetworkPaddingType paddingType) {
			int kernelWidth = Weights.GetLength (1);
			int kernelHeight = Weights.GetLength (0);
			int inputWidth = inputs.GetLength (1);
			int inputHeight = inputs.GetLength (0);
			int inputChannel = inputs.GetLength (2);
			int outputX = 0;
			int outputY = 0;
			int paddingX;
			int paddingY;
			switch (paddingType) {
				case NeuralNetworkPaddingType.Valid:
					paddingX = 0;
					paddingY = 0;
					break;
				case NeuralNetworkPaddingType.Same:
					paddingX = (kernelWidth - 1) / 2;
					paddingY = (kernelHeight - 1) / 2;
					break;
				default:
					throw new NotImplementedException (paddingType.ToString ());
			}
			int startX = 0 - paddingX;
			int startY = 0 - paddingY;
			int endX = inputWidth + paddingX - kernelWidth;
			int endY = inputHeight + paddingY - kernelHeight;
			for (int ky = startY; ky <= endY; ky += StrideY, outputY++) {
				for (int kx = startX; kx <= endX; kx += StrideX, outputX++) {
					float sum = 0;
					for (int c = 0; c < inputChannel; c++) {
						for (int wy = 0; wy < kernelHeight; wy++) {
							for (int wx = 0; wx < kernelWidth; wx++) {
								float value;
								int ix = kx + wx;
								int iy = ky + wy;
								if (iy < 0 || iy >= endY || ix < 0 || ix >= endX) {
									value = 0;
								} else {
									value = inputs[iy, ix, c];
								}
								sum += Weights[wy, wx, c] * value;
							}
						}
					}
					outputs[outputY, outputX, channel] = NeuralNetworkApi.ActivationFunction (activationFunctionType, sum + Bias);
				}
				outputX = 0;
			}
		}

	}

}