using System;
using System.Windows.Forms;

namespace Test {

	public partial class Form1 : Form {

		public Form1 () {
			InitializeComponent ();
		}

		private void MnistButton_Click (object sender, EventArgs e) {
			new MnistForm ().ShowDialog ();
		}

		private void DogsVsCatsButton_Click (object sender, EventArgs e) {
			new DogsVsCatsForm (1).ShowDialog ();
		}

		private void button1_Click (object sender, EventArgs e) {
			new DogsVsCatsForm (0).ShowDialog ();
		}
	}

}