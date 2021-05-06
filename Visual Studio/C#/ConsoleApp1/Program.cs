using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Eruru.NeuralNetwork;

namespace ConsoleApp1 {

	class Program {

		static readonly string AssetsPath = @"..\..\..\Assets\";

		static void Main (string[] args) {
			Console.Title = nameof (ConsoleApp1);
			DogsVsCats ();
			Mnist ();
			Console.ReadLine ();
		}

		static void Mnist () {
			NeuralNetwork neuralNetwork = new NeuralNetwork ();
			string h5Path = $@"{AssetsPath}Keras Mnist CNN.h5";
			string jsonPath = $@"{AssetsPath}Keras Mnist CNN.json";
			Stopwatch stopwatch = new Stopwatch ();
			stopwatch.Start ();
			switch (1) {
				case 0:
					NeuralNetwork.H5ToJson (h5Path, new StreamWriter (jsonPath));
					neuralNetwork.LoadJsonFile (jsonPath);
					break;
				case 1:
					neuralNetwork.LoadJsonFile (jsonPath);
					break;
				case 2:
					neuralNetwork.LoadH5 (h5Path);
					break;
				default:
					throw new NotImplementedException ();
			}
			stopwatch.Stop ();
			long loadTotalMilliseconds = stopwatch.ElapsedMilliseconds;
			neuralNetwork.Summary ();
			long predictTotalMilliseconds = 0;
			for (int i = 0; i < 10; i++) {
				float[,,] inputs = NeuralNetworkInput.FromGrayImage (new Bitmap ($@"{AssetsPath}Mnist\{i}.jpg"));
				stopwatch.Reset ();
				stopwatch.Start ();
				Console.Write ($"实际：{i} 预测：{neuralNetwork.PredictClasses (inputs)} ");
				stopwatch.Stop ();
				predictTotalMilliseconds += stopwatch.ElapsedMilliseconds;
				Console.WriteLine ($"耗时：{stopwatch.ElapsedMilliseconds}");
			}
			Console.WriteLine ($"加载耗时：{loadTotalMilliseconds} 预测耗时：{predictTotalMilliseconds} 总耗时：{loadTotalMilliseconds + predictTotalMilliseconds}");
		}

		static void DogsVsCats () {
			NeuralNetwork neuralNetwork = new NeuralNetwork ();
			Stopwatch stopwatch = new Stopwatch ();
			stopwatch.Start ();
			neuralNetwork.LoadH5 ($@"{AssetsPath}DogsVsCats.h5");
			stopwatch.Stop ();
			long loadTime = stopwatch.ElapsedMilliseconds;
			long predictTotalMilliseconds = 0;
			neuralNetwork.Summary ();
			Predict ($@"{AssetsPath}\Dog.jpg", "狗");
			Predict ($@"{AssetsPath}\Cat.jpg", "猫");
			Console.WriteLine ($"加载耗时：{loadTime} 预测耗时：{predictTotalMilliseconds} 总耗时：{loadTime + predictTotalMilliseconds}");
			void Predict (string path, string actual) {
				float[,,] inputs = NeuralNetworkInput.FromRGBImage (new Bitmap (Image.FromFile (path), 150, 150));
				stopwatch.Restart ();
				Console.Write ($"实际：{actual} 预测：{Classification (neuralNetwork.Predict (inputs))} ");
				stopwatch.Stop ();
				predictTotalMilliseconds += stopwatch.ElapsedMilliseconds;
				Console.WriteLine ($"耗时：{stopwatch.ElapsedMilliseconds}");
			}
			string Classification (float value) {
				if (value >= 0.5F) {
					return "狗";
				}
				return "猫";
			}
		}

	}

}