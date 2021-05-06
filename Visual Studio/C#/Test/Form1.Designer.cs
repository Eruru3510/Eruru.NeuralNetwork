
namespace Test {
	partial class Form1 {
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
			this.MnistButton = new System.Windows.Forms.Button();
			this.DogsVsCatsButton = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// MnistButton
			// 
			this.MnistButton.Location = new System.Drawing.Point(15, 25);
			this.MnistButton.Name = "MnistButton";
			this.MnistButton.Size = new System.Drawing.Size(100, 23);
			this.MnistButton.TabIndex = 0;
			this.MnistButton.Text = "Mnist手写数字";
			this.MnistButton.UseVisualStyleBackColor = true;
			this.MnistButton.Click += new System.EventHandler(this.MnistButton_Click);
			// 
			// DogsVsCatsButton
			// 
			this.DogsVsCatsButton.Location = new System.Drawing.Point(120, 25);
			this.DogsVsCatsButton.Name = "DogsVsCatsButton";
			this.DogsVsCatsButton.Size = new System.Drawing.Size(160, 23);
			this.DogsVsCatsButton.TabIndex = 1;
			this.DogsVsCatsButton.Text = "猫狗大战（实时）";
			this.DogsVsCatsButton.UseVisualStyleBackColor = true;
			this.DogsVsCatsButton.Click += new System.EventHandler(this.DogsVsCatsButton_Click);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(120, 55);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(160, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "猫狗大战（选择图片）";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(304, 112);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.DogsVsCatsButton);
			this.Controls.Add(this.MnistButton);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button MnistButton;
		private System.Windows.Forms.Button DogsVsCatsButton;
		private System.Windows.Forms.Button button1;
	}
}