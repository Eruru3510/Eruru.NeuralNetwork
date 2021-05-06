using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Eruru.NeuralNetwork;

namespace Test {

	public partial class DogsVsCatsForm : Form {

		readonly NeuralNetwork NeuralNetwork = new NeuralNetwork ();

		Thread Thread;

		public DogsVsCatsForm (int mode) {
			InitializeComponent ();
			NeuralNetwork.LoadH5 ($@"..\..\..\Assets\DogsVsCats.h5");
			NeuralNetwork.Summary ();
			if (mode == 0) {
				SelectMode ();
			} else {
				CaptureMode ();
			}
		}

		private void DogsVsCatsForm_FormClosed (object sender, FormClosedEventArgs e) {
			Thread?.Abort ();
		}

		void SelectMode () {
			PictureBox.Click += (sender, e) => {
				if (openFileDialog1.ShowDialog () == DialogResult.OK) {
					PictureBox.Image = Image.FromFile (openFileDialog1.FileName);
					float value = NeuralNetwork.Predict (NeuralNetworkInput.FromRGBImage (new Bitmap (PictureBox.Image, 150, 150)));
					Label.Text = $"{value} = {(value >= 0.5F ? "狗" : "猫")}";
				}
			};
		}

		void CaptureMode () {
			PictureBox.BackColor = Color.Lime;
			TopMost = true;
			SetWindowLong (Handle, GWL_EXSTYLE, WS_EX_LAYERED);
			SetLayeredWindowAttributes (Handle, 65280, 255, LWA_COLORKEY);
			Bitmap bitmap = null;
			float value = 0;
			Action capture = () => {
				Point point = PointToScreen (PictureBox.Location);
				Graphics.FromImage (bitmap).CopyFromScreen (point.X, point.Y, 0, 0, PictureBox.Size);
			};
			Action show = () => {
				Label.Text = $"{value} = {(value >= 0.5F ? "狗" : "猫")}";
			};
			Stopwatch stopwatch = new Stopwatch ();
			stopwatch.Start ();
			long time = 0, timeDelta, refreshTime = 0;
			Thread = new Thread (() => {
				while (true) {
					try {
						timeDelta = stopwatch.ElapsedMilliseconds - time;
						if (WindowState == FormWindowState.Minimized) {
							return;
						}
						if (time >= refreshTime) {
							refreshTime = time + 1000;
							bitmap = new Bitmap (PictureBox.Width, PictureBox.Height);
							Invoke (capture);
							value = NeuralNetwork.Predict (NeuralNetworkInput.FromRGBImage (new Bitmap (bitmap, 150, 150)));
							Invoke (show);
						}
						time += timeDelta;
						Thread.Sleep (1);
					} catch {

					}
				}
			}) {
				IsBackground = true
			};
			Thread.Start ();
		}

		private const uint WS_EX_LAYERED = 0x80000;
		private const int GWL_EXSTYLE = -20;
		private const int LWA_COLORKEY = 1;

		[DllImport ("user32", EntryPoint = "SetWindowLong")]
		private static extern uint SetWindowLong (IntPtr hwnd, int nIndex, uint dwNewLong);

		[DllImport ("user32", EntryPoint = "SetLayeredWindowAttributes")]
		private static extern int SetLayeredWindowAttributes (IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

	}

}