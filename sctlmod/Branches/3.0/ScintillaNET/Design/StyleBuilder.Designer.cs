namespace ScintillaNet.Design
{
	partial class StyleBuilder
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
			this._propertyGrid = new System.Windows.Forms.PropertyGrid();
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._previewScintilla = new ScintillaNet.Scintilla();
			this._previewLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this._previewScintilla)).BeginInit();
			this.SuspendLayout();
			// 
			// _propertyGrid
			// 
			this._propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._propertyGrid.Location = new System.Drawing.Point(12, 13);
			this._propertyGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this._propertyGrid.Name = "_propertyGrid";
			this._propertyGrid.Size = new System.Drawing.Size(417, 302);
			this._propertyGrid.TabIndex = 0;
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.Location = new System.Drawing.Point(273, 470);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 4;
			this._okButton.Text = "OK";
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._cancelButton.Location = new System.Drawing.Point(354, 470);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 5;
			this._cancelButton.Text = "Cancel";
			// 
			// _previewScintilla
			// 
			this._previewScintilla.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._previewScintilla.Location = new System.Drawing.Point(12, 347);
			this._previewScintilla.Margins.Margin0.IsFoldMargin = true;
			this._previewScintilla.Margins.Margin1.IsFoldMargin = true;
			this._previewScintilla.Margins.Margin1.Width = 0;
			this._previewScintilla.Margins.Margin3.IsFoldMargin = true;
			this._previewScintilla.Margins.Margin4.IsFoldMargin = true;
			this._previewScintilla.Name = "_previewScintilla";
			this._previewScintilla.Size = new System.Drawing.Size(417, 109);
			this._previewScintilla.Styles.DefaultStyle.BackColor = System.Drawing.SystemColors.Window;
			this._previewScintilla.TabIndex = 0;
			// 
			// _previewLabel
			// 
			this._previewLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._previewLabel.AutoSize = true;
			this._previewLabel.Location = new System.Drawing.Point(9, 328);
			this._previewLabel.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
			this._previewLabel.Name = "_previewLabel";
			this._previewLabel.Size = new System.Drawing.Size(53, 16);
			this._previewLabel.TabIndex = 6;
			this._previewLabel.Text = "Preview";
			// 
			// StyleBuilder
			// 
			this.AcceptButton = this._okButton;
			this.CancelButton = this._cancelButton;
			this.ClientSize = new System.Drawing.Size(441, 505);
			this.Controls.Add(this._previewLabel);
			this.Controls.Add(this._previewScintilla);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this._propertyGrid);
			this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "StyleBuilder";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Style Builder";
			((System.ComponentModel.ISupportInitialize)(this._previewScintilla)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PropertyGrid _propertyGrid;
		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
		private Scintilla _previewScintilla;
		private System.Windows.Forms.Label _previewLabel;
	}
}