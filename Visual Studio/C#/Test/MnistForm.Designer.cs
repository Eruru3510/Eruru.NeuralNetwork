namespace Test {
	partial class MnistForm {
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose (bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent () {
			this.PictureBox = new System.Windows.Forms.PictureBox();
			this.Label_Result = new System.Windows.Forms.Label();
			this.Button_Clear = new System.Windows.Forms.Button();
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
			this.PictureBox.Size = new System.Drawing.Size(373, 324);
			this.PictureBox.TabIndex = 0;
			this.PictureBox.TabStop = false;
			this.PictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseDown);
			this.PictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseMove);
			this.PictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.PictureBox_MouseUp);
			// 
			// Label_Result
			// 
			this.Label_Result.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.Label_Result.Font = new System.Drawing.Font("宋体", 10F);
			this.Label_Result.Location = new System.Drawing.Point(5, 335);
			this.Label_Result.Name = "Label_Result";
			this.Label_Result.Size = new System.Drawing.Size(290, 23);
			this.Label_Result.TabIndex = 2;
			this.Label_Result.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// Button_Clear
			// 
			this.Button_Clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.Button_Clear.Location = new System.Drawing.Point(303, 334);
			this.Button_Clear.Name = "Button_Clear";
			this.Button_Clear.Size = new System.Drawing.Size(75, 23);
			this.Button_Clear.TabIndex = 3;
			this.Button_Clear.Text = "清空";
			this.Button_Clear.UseVisualStyleBackColor = true;
			this.Button_Clear.Click += new System.EventHandler(this.Button_Clear_Click);
			// 
			// MnistForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(384, 361);
			this.Controls.Add(this.Button_Clear);
			this.Controls.Add(this.Label_Result);
			this.Controls.Add(this.PictureBox);
			this.Name = "MnistForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox PictureBox;
		private System.Windows.Forms.Label Label_Result;
		private System.Windows.Forms.Button Button_Clear;
	}
}

