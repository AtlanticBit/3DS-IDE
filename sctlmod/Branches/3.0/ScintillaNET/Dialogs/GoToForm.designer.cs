namespace ScintillaNet.Dialogs
{
	partial class GoToForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.textBox = new System.Windows.Forms.TextBox();
			this.lineNumberLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.Location = new System.Drawing.Point(88, 42);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(169, 42);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "Cancel";
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Location = new System.Drawing.Point(88, 12);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(156, 20);
			this.textBox.TabIndex = 1;
			// 
			// lineNumberLabel
			// 
			this.lineNumberLabel.AutoSize = true;
			this.lineNumberLabel.Location = new System.Drawing.Point(12, 15);
			this.lineNumberLabel.Name = "lineNumberLabel";
			this.lineNumberLabel.Size = new System.Drawing.Size(68, 13);
			this.lineNumberLabel.TabIndex = 0;
			this.lineNumberLabel.Text = "&Line number:";
			// 
			// GoToForm
			// 
			this.AcceptButton = this.okButton;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(256, 77);
			this.Controls.Add(this.lineNumberLabel);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GoToForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Go To Line";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Label lineNumberLabel;
	}
}