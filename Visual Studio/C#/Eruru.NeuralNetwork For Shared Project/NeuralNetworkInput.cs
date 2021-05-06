using System;
using System.Drawing;

namespace Eruru.NeuralNetwork {

	public static class NeuralNetworkInput {

#if NET20_OR_GREATER
		public static float[,,] FromGrayImage (Bitmap bitmap) {
			if (bitmap is null) {
				throw new ArgumentNullException (nameof (bitmap));
			}
			float[,,] outputs = new float[bitmap.Height, bitmap.Width, 1];
			for (int y = 0; y < bitmap.Height; y++) {
				for (int x = 0; x < bitmap.Width; x++) {
					outputs[y, x, 0] = bitmap.GetPixel (x, y).GetBrightness ();
				}
			}
			return outputs;
		}

		public static float[,,] FromRGBImage (Bitmap bitmap) {
			if (bitmap is null) {
				throw new ArgumentNullException (nameof (bitmap));
			}
			int channel = 3;
			float[,,] outputs = new float[bitmap.Height, bitmap.Width, channel];
			for (int y = 0; y < bitmap.Height; y++) {
				for (int x = 0; x < bitmap.Width; x++) {
					for (int c = 0; c < channel; c++) {
						Color color = bitmap.GetPixel (x, y);
						float value;
						switch (c) {
							case 0:
								value = color.R;
								break;
							case 1:
								value = color.G;
								break;
							case 2:
								value = color.B;
								break;
							default:
								throw new NotImplementedException ();
						}
						outputs[y, x, c] = value / 255;
					}
				}
			}
			return outputs;
		}
#endif

	}

}