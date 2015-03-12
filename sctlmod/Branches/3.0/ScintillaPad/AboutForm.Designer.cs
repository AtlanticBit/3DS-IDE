namespace ScintillaPad
{
	partial class AboutForm
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
			this.titleLabel = new System.Windows.Forms.Label();
			this.versionLabel = new System.Windows.Forms.Label();
			this.copyrightLabel = new System.Windows.Forms.Label();
			this.websiteLinkLabel = new System.Windows.Forms.LinkLabel();
			this.graphicsPanel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.scintillaCopyrightLabel = new System.Windows.Forms.Label();
			this.licenseTextBox = new System.Windows.Forms.TextBox();
			this.scintillaVersionLabel = new System.Windows.Forms.Label();
			this.scintillaTitleLabel = new System.Windows.Forms.Label();
			this.borderPanel = new System.Windows.Forms.Panel();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.okButton.Location = new System.Drawing.Point(403, 333);
			this.okButton.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			// 
			// titleLabel
			// 
			this.titleLabel.AutoSize = true;
			this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.titleLabel.Location = new System.Drawing.Point(1, 1);
			this.titleLabel.Margin = new System.Windows.Forms.Padding(1);
			this.titleLabel.Name = "titleLabel";
			this.titleLabel.Size = new System.Drawing.Size(32, 13);
			this.titleLabel.TabIndex = 1;
			this.titleLabel.Text = "Title";
			// 
			// versionLabel
			// 
			this.versionLabel.AutoSize = true;
			this.versionLabel.Location = new System.Drawing.Point(1, 16);
			this.versionLabel.Margin = new System.Windows.Forms.Padding(1);
			this.versionLabel.Name = "versionLabel";
			this.versionLabel.Size = new System.Drawing.Size(42, 13);
			this.versionLabel.TabIndex = 2;
			this.versionLabel.Text = "Version";
			// 
			// copyrightLabel
			// 
			this.copyrightLabel.AutoSize = true;
			this.copyrightLabel.Location = new System.Drawing.Point(1, 31);
			this.copyrightLabel.Margin = new System.Windows.Forms.Padding(1);
			this.copyrightLabel.Name = "copyrightLabel";
			this.copyrightLabel.Size = new System.Drawing.Size(54, 13);
			this.copyrightLabel.TabIndex = 3;
			this.copyrightLabel.Text = "Copyright";
			// 
			// websiteLinkLabel
			// 
			this.websiteLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.websiteLinkLabel.AutoSize = true;
			this.websiteLinkLabel.Location = new System.Drawing.Point(13, 338);
			this.websiteLinkLabel.Name = "websiteLinkLabel";
			this.websiteLinkLabel.Size = new System.Drawing.Size(159, 13);
			this.websiteLinkLabel.TabIndex = 2;
			this.websiteLinkLabel.TabStop = true;
			this.websiteLinkLabel.Text = "http://scintillanet.codeplex.com";
			this.websiteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.websiteLinkLabel_LinkClicked);
			// 
			// graphicsPanel
			// 
			this.graphicsPanel.BackColor = System.Drawing.Color.Black;
			this.graphicsPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.graphicsPanel.Location = new System.Drawing.Point(0, 0);
			this.graphicsPanel.Name = "graphicsPanel";
			this.graphicsPanel.Size = new System.Drawing.Size(490, 75);
			this.graphicsPanel.TabIndex = 5;
			this.graphicsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.graphicsPanel_Paint);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.scintillaCopyrightLabel, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.licenseTextBox, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.scintillaVersionLabel, 1, 1);
			this.tableLayoutPanel1.Controls.Add(this.scintillaTitleLabel, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.titleLabel, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.versionLabel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.copyrightLabel, 0, 2);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 92);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 9, 3, 6);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(466, 229);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// scintillaCopyrightLabel
			// 
			this.scintillaCopyrightLabel.AutoSize = true;
			this.scintillaCopyrightLabel.Location = new System.Drawing.Point(234, 31);
			this.scintillaCopyrightLabel.Margin = new System.Windows.Forms.Padding(1);
			this.scintillaCopyrightLabel.Name = "scintillaCopyrightLabel";
			this.scintillaCopyrightLabel.Size = new System.Drawing.Size(92, 13);
			this.scintillaCopyrightLabel.TabIndex = 8;
			this.scintillaCopyrightLabel.Text = "Scintilla Copyright";
			// 
			// licenseTextBox
			// 
			this.licenseTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.SetColumnSpan(this.licenseTextBox, 2);
			this.licenseTextBox.Location = new System.Drawing.Point(1, 48);
			this.licenseTextBox.Margin = new System.Windows.Forms.Padding(1, 3, 1, 1);
			this.licenseTextBox.Multiline = true;
			this.licenseTextBox.Name = "licenseTextBox";
			this.licenseTextBox.ReadOnly = true;
			this.licenseTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.licenseTextBox.Size = new System.Drawing.Size(464, 180);
			this.licenseTextBox.TabIndex = 0;
			// 
			// scintillaVersionLabel
			// 
			this.scintillaVersionLabel.AutoSize = true;
			this.scintillaVersionLabel.Location = new System.Drawing.Point(234, 16);
			this.scintillaVersionLabel.Margin = new System.Windows.Forms.Padding(1);
			this.scintillaVersionLabel.Name = "scintillaVersionLabel";
			this.scintillaVersionLabel.Size = new System.Drawing.Size(80, 13);
			this.scintillaVersionLabel.TabIndex = 8;
			this.scintillaVersionLabel.Text = "Scintilla Version";
			// 
			// scintillaTitleLabel
			// 
			this.scintillaTitleLabel.AutoSize = true;
			this.scintillaTitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.scintillaTitleLabel.Location = new System.Drawing.Point(234, 1);
			this.scintillaTitleLabel.Margin = new System.Windows.Forms.Padding(1);
			this.scintillaTitleLabel.Name = "scintillaTitleLabel";
			this.scintillaTitleLabel.Size = new System.Drawing.Size(81, 13);
			this.scintillaTitleLabel.TabIndex = 8;
			this.scintillaTitleLabel.Text = "Scintilla Title";
			// 
			// borderPanel
			// 
			this.borderPanel.BackColor = System.Drawing.SystemColors.ControlDark;
			this.borderPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.borderPanel.Location = new System.Drawing.Point(0, 75);
			this.borderPanel.Name = "borderPanel";
			this.borderPanel.Size = new System.Drawing.Size(490, 5);
			this.borderPanel.TabIndex = 7;
			// 
			// AboutForm
			// 
			this.AcceptButton = this.okButton;
			this.CancelButton = this.okButton;
			this.ClientSize = new System.Drawing.Size(490, 368);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.borderPanel);
			this.Controls.Add(this.graphicsPanel);
			this.Controls.Add(this.websiteLinkLabel);
			this.Controls.Add(this.okButton);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AboutForm";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Label titleLabel;
		private System.Windows.Forms.Label versionLabel;
		private System.Windows.Forms.Label copyrightLabel;
		private System.Windows.Forms.LinkLabel websiteLinkLabel;
		private System.Windows.Forms.Panel graphicsPanel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.Panel borderPanel;
		private System.Windows.Forms.Label scintillaCopyrightLabel;
		private System.Windows.Forms.Label scintillaVersionLabel;
		private System.Windows.Forms.Label scintillaTitleLabel;
		private System.Windows.Forms.TextBox licenseTextBox;
	}
}