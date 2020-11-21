using System;
using System.Diagnostics;
using System.Drawing;
using Eruru.NeuralNetwork;

namespace ConsoleApp1 {

	class Program {

		static NeuralNetwork NeuralNetwork = new NeuralNetwork ();

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			MnistCNN ();
			Console.ReadLine ();
		}

		static void MnistCNN () {
			NeuralNetwork.Load (@"Keras Mnist CNN.h5");
			NeuralNetwork.Summary ();
			Stopwatch stopwatch = new Stopwatch ();
			stopwatch.Start ();
			for (int i = 0; i < 10; i++) {
				Console.WriteLine ($"实际：{i} 预测：{NeuralNetwork.PredictClasses (ConvertGrayImage (new Bitmap ($"{i}.jpg")))}");
			}
			stopwatch.Stop ();
			Console.WriteLine ($"耗时：{stopwatch.ElapsedMilliseconds}毫秒");
		}

		static void Test () {
			NeuralNetwork.Load (@"Test.h5");
			NeuralNetwork.Summary ();
			Stopwatch stopwatch = new Stopwatch ();
			stopwatch.Start ();
			for (int i = 0; i < 10; i++) {
				Console.WriteLine ($"实际：{i} 预测：{NeuralNetwork.PredictClasses (ConvertGrayImage (new Bitmap ($"{i}.jpg")))}");
			}
			stopwatch.Stop ();
			Console.WriteLine ($"耗时：{stopwatch.ElapsedMilliseconds}毫秒");
		}

		static float[] ConvertImage (Bitmap bitmap) {
			float[] outputs = new float[bitmap.Width * bitmap.Height];
			for (int y = 0; y < bitmap.Height; y++) {
				for (int x = 0; x < bitmap.Width; x++) {
					outputs[y * bitmap.Width + x] = bitmap.GetPixel (x, y).GetBrightness ();
				}
			}
			return outputs;
		}

		static float[,,] ConvertGrayImage (Bitmap bitmap) {
			float[,,] outputs = new float[bitmap.Height, bitmap.Width, 1];
			for (int y = 0; y < bitmap.Height; y++) {
				for (int x = 0; x < bitmap.Width; x++) {
					outputs[y, x, 0] = bitmap.GetPixel (x, y).GetBrightness ();
				}
			}
			return outputs;
		}

	}

}