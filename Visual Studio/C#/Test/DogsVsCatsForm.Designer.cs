
namespace Test {
	partial class DogsVsCatsForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose (bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent () {
			this.PictureBox = new System.Windows.Forms.PictureBox();
			this.Label = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// PictureBox
			// 
			this.PictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.PictureBox.Location = new System.Drawing.Point(5, 5);
			this.PictureBox.Name = "PictureBox";
			this.PictureBox.Size = new System.Drawing.Size(775, 530);
			this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.PictureBox.TabIndex = 0;
			this.PictureBox.TabStop = false;
			// 
			// Label
			// 
			this.Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Label.Font = new System.Drawing.Font("宋体", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.Label.Location = new System.Drawing.Point(5, 540);
			this.Label.Name = "Label";
			this.Label.Size = new System.Drawing.Size(775, 23);
			this.Label.TabIndex = 2;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// DogsVsCatsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.Label);
			this.Controls.Add(this.PictureBox);
			this.Name = "DogsVsCatsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "DogsVsCatsForm";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DogsVsCatsForm_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox PictureBox;
		private System.Windows.Forms.Label Label;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
	}
}