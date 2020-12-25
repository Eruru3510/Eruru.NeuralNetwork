using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Eruru.NeuralNetwork;

namespace ConsoleApp1 {

	class Program {

		static readonly NeuralNetwork NeuralNetwork = new NeuralNetwork ();
		static readonly string AssetsPath = @"..\..\..\Assets\";
		static readonly string H5Path = $@"{AssetsPath}Keras Mnist CNN.h5";
		static readonly string JsonPath = $@"{AssetsPath}Keras Mnist CNN.json";

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			Stopwatch stopWatch = new Stopwatch ();
			stopWatch.Start ();
			switch (2) {
				case 0:
					NeuralNetwork.H5ToJson (H5Path, new StreamWriter (JsonPath));
					NeuralNetwork.LoadJsonFile (JsonPath);
					break;
				case 1:
					NeuralNetwork.LoadJsonFile (JsonPath);
					break;
				case 2:
					NeuralNetwork.LoadH5 (H5Path);
					break;
			}
			stopWatch.Stop ();
			long loadTotalMilliseconds = stopWatch.ElapsedMilliseconds;
			NeuralNetwork.Summary ();
			long predictTotalMilliseconds = 0;
			for (int i = 0; i < 10; i++) {
				stopWatch.Restart ();
				Console.Write ($"实际：{i} 预测：{NeuralNetwork.PredictClasses (ConvertGrayImage (new Bitmap ($@"{AssetsPath}Mnist\{i}.jpg")))} ");
				stopWatch.Stop ();
				predictTotalMilliseconds += stopWatch.ElapsedMilliseconds;
				Console.WriteLine ($"耗时：{stopWatch.ElapsedMilliseconds}");
			}
			Console.WriteLine ($"加载耗时：{loadTotalMilliseconds} 预测耗时：{predictTotalMilliseconds} 总耗时：{loadTotalMilliseconds + predictTotalMilliseconds}");
			Console.ReadLine ();
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