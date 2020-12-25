using System;
using System.Drawing;
using System.Windows.Forms;
using Eruru.NeuralNetwork;

namespace WindowsFormsApp1 {

	public partial class Form1 : Form {

		readonly NeuralNetwork NeuralNetwork = new NeuralNetwork ();
		readonly int Radius = 5;

		Graphics Graphics;
		bool IsDraring;

		public Form1 () {
			InitializeComponent ();
		}

		private void Form1_Load (object sender, EventArgs e) {
			NeuralNetwork.LoadH5 (@"..\..\..\Assets\Keras Mnist CNN.h5");
			NeuralNetwork.Summary ();
			Clear ();
		}

		private void PictureBox_MouseDown (object sender, MouseEventArgs e) {
			IsDraring = true;
		}

		private void PictureBox_MouseMove (object sender, MouseEventArgs e) {
			if (IsDraring) {
				Graphics.FillEllipse (Brushes.White, e.X - Radius, e.Y - Radius, Radius * 2, Radius * 2);
				PictureBox.Refresh ();
			}
		}

		private void PictureBox_MouseUp (object sender, MouseEventArgs e) {
			IsDraring = false;
			Bitmap bitmap = ScaleBitmap (new Bitmap (PictureBox.Image));
			Label_Result.Text = $"识别为：{NeuralNetwork.PredictClasses (ParseGrayImage (bitmap))}";
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

		Bitmap ScaleBitmap (Bitmap bitmap) {
			Bitmap newBitmap = new Bitmap (28, 28);
			Graphics graphics = Graphics.FromImage (newBitmap);
			graphics.DrawImage (bitmap, 0, 0, newBitmap.Width, newBitmap.Height);
			return newBitmap;
		}

		static float[,,] ParseGrayImage (Bitmap bitmap) {
			float[,,] outputs = new float[bitmap.Height, bitmap.Width, 1];
			for (int y = 0; y < bitmap.Height; y++) {
				for (int x = 0; x < bitmap.Width; x++) {
					Color color = bitmap.GetPixel (x, y);
					outputs[y, x, 0] = color.GetBrightness ();
				}
			}
			return outputs;
		}

	}

}