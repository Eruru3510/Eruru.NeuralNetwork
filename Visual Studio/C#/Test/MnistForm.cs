using System;
using System.Drawing;
using System.Windows.Forms;
using Eruru.NeuralNetwork;

namespace Test {

	public partial class MnistForm : Form {

		readonly NeuralNetwork NeuralNetwork = new NeuralNetwork ();
		readonly static int Radius = 20;
		readonly Pen Pen = new Pen (Color.White, Radius);

		Graphics Graphics;
		Point Point;
		bool IsDrawing;

		public MnistForm () {
			InitializeComponent ();
		}

		private void Form1_Load (object sender, EventArgs e) {
			NeuralNetwork.LoadH5 (@"..\..\..\Assets\Keras Mnist CNN.h5");
			NeuralNetwork.Summary ();
			Clear ();
		}

		private void PictureBox_MouseDown (object sender, MouseEventArgs e) {
			IsDrawing = true;
			Point = e.Location;
		}

		private void PictureBox_MouseMove (object sender, MouseEventArgs e) {
			if (IsDrawing) {
				Graphics.DrawLine (Pen, Point, e.Location);
				Point = e.Location;
				PictureBox.Refresh ();
			}
		}

		private void PictureBox_MouseUp (object sender, MouseEventArgs e) {
			IsDrawing = false;
			Bitmap bitmap = new Bitmap (PictureBox.Image, 28, 28);
			Label_Result.Text = $"识别为：{NeuralNetwork.PredictClasses (NeuralNetworkInput.FromGrayImage (bitmap))}";
		}

		private void Button_Clear_Click (object sender, EventArgs e) {
			Clear ();
		}

		void Clear () {
			PictureBox.Image = new Bitmap (PictureBox.Width, PictureBox.Height);
			Graphics = Graphics.FromImage (PictureBox.Image);
			Graphics.Clear (Color.Black);
			PictureBox.Refresh ();
			Label_Result.Text = string.Empty;
		}

	}

}