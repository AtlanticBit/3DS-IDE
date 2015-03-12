namespace ScintillaNet.Dialogs
{
	partial class FindReplaceForm
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
			this.findNextButton = new System.Windows.Forms.Button();
			this.replaceButton = new System.Windows.Forms.Button();
			this.findComboBox = new System.Windows.Forms.ComboBox();
			this.findWhatLabel = new System.Windows.Forms.Label();
			this.directionGroupBox = new System.Windows.Forms.GroupBox();
			this.downRadioButton = new System.Windows.Forms.RadioButton();
			this.upRadioButton = new System.Windows.Forms.RadioButton();
			this.replaceAllButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.replaceWithLabel = new System.Windows.Forms.Label();
			this.replaceComboBox = new System.Windows.Forms.ComboBox();
			this.matchWordsCheckBox = new System.Windows.Forms.CheckBox();
			this.matchCaseCheckBox = new System.Windows.Forms.CheckBox();
			this.matchPanel = new System.Windows.Forms.Panel();
			this.directionGroupBox.SuspendLayout();
			this.matchPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// findNextButton
			// 
			this.findNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.findNextButton.Enabled = false;
			this.findNextButton.Location = new System.Drawing.Point(324, 12);
			this.findNextButton.Name = "findNextButton";
			this.findNextButton.Size = new System.Drawing.Size(80, 23);
			this.findNextButton.TabIndex = 6;
			this.findNextButton.Text = "&Find Next";
			this.findNextButton.Click += new System.EventHandler(this.findNextButton_Click);
			// 
			// replaceButton
			// 
			this.replaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.replaceButton.Enabled = false;
			this.replaceButton.Location = new System.Drawing.Point(324, 41);
			this.replaceButton.Name = "replaceButton";
			this.replaceButton.Size = new System.Drawing.Size(80, 23);
			this.replaceButton.TabIndex = 7;
			this.replaceButton.Text = "&Replace";
			// 
			// findComboBox
			// 
			this.findComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.findComboBox.Location = new System.Drawing.Point(85, 12);
			this.findComboBox.Name = "findComboBox";
			this.findComboBox.Size = new System.Drawing.Size(228, 21);
			this.findComboBox.TabIndex = 1;
			this.findComboBox.TextUpdate += new System.EventHandler(this.findComboBox_TextUpdate);
			// 
			// findWhatLabel
			// 
			this.findWhatLabel.AutoSize = true;
			this.findWhatLabel.Location = new System.Drawing.Point(12, 17);
			this.findWhatLabel.Name = "findWhatLabel";
			this.findWhatLabel.Size = new System.Drawing.Size(56, 13);
			this.findWhatLabel.TabIndex = 0;
			this.findWhatLabel.Text = "Fi&nd what:";
			// 
			// directionGroupBox
			// 
			this.directionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.directionGroupBox.Controls.Add(this.downRadioButton);
			this.directionGroupBox.Controls.Add(this.upRadioButton);
			this.directionGroupBox.Location = new System.Drawing.Point(177, 70);
			this.directionGroupBox.Name = "directionGroupBox";
			this.directionGroupBox.Size = new System.Drawing.Size(136, 56);
			this.directionGroupBox.TabIndex = 5;
			this.directionGroupBox.TabStop = false;
			this.directionGroupBox.Text = "Direction";
			// 
			// downRadioButton
			// 
			this.downRadioButton.AutoSize = true;
			this.downRadioButton.Checked = true;
			this.downRadioButton.Location = new System.Drawing.Point(68, 20);
			this.downRadioButton.Name = "downRadioButton";
			this.downRadioButton.Size = new System.Drawing.Size(53, 17);
			this.downRadioButton.TabIndex = 1;
			this.downRadioButton.TabStop = true;
			this.downRadioButton.Text = "&Down";
			// 
			// upRadioButton
			// 
			this.upRadioButton.AutoSize = true;
			this.upRadioButton.Location = new System.Drawing.Point(15, 20);
			this.upRadioButton.Name = "upRadioButton";
			this.upRadioButton.Size = new System.Drawing.Size(39, 17);
			this.upRadioButton.TabIndex = 0;
			this.upRadioButton.Text = "&Up";
			// 
			// replaceAllButton
			// 
			this.replaceAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.replaceAllButton.Enabled = false;
			this.replaceAllButton.Location = new System.Drawing.Point(324, 70);
			this.replaceAllButton.Name = "replaceAllButton";
			this.replaceAllButton.Size = new System.Drawing.Size(80, 23);
			this.replaceAllButton.TabIndex = 8;
			this.replaceAllButton.Text = "Replace &All";
			// 
			// closeButton
			// 
			this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Location = new System.Drawing.Point(324, 99);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(80, 23);
			this.closeButton.TabIndex = 9;
			this.closeButton.Text = "Close";
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// replaceWithLabel
			// 
			this.replaceWithLabel.AutoSize = true;
			this.replaceWithLabel.Location = new System.Drawing.Point(12, 42);
			this.replaceWithLabel.Name = "replaceWithLabel";
			this.replaceWithLabel.Size = new System.Drawing.Size(72, 13);
			this.replaceWithLabel.TabIndex = 2;
			this.replaceWithLabel.Text = "R&eplace with:";
			// 
			// replaceComboBox
			// 
			this.replaceComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.replaceComboBox.Location = new System.Drawing.Point(85, 39);
			this.replaceComboBox.Name = "replaceComboBox";
			this.replaceComboBox.Size = new System.Drawing.Size(228, 21);
			this.replaceComboBox.TabIndex = 3;
			// 
			// matchWordsCheckBox
			// 
			this.matchWordsCheckBox.AutoSize = true;
			this.matchWordsCheckBox.Location = new System.Drawing.Point(3, 8);
			this.matchWordsCheckBox.Name = "matchWordsCheckBox";
			this.matchWordsCheckBox.Size = new System.Drawing.Size(118, 17);
			this.matchWordsCheckBox.TabIndex = 0;
			this.matchWordsCheckBox.Text = "Match &whole words";
			// 
			// matchCaseCheckBox
			// 
			this.matchCaseCheckBox.AutoSize = true;
			this.matchCaseCheckBox.Location = new System.Drawing.Point(3, 31);
			this.matchCaseCheckBox.Name = "matchCaseCheckBox";
			this.matchCaseCheckBox.Size = new System.Drawing.Size(82, 17);
			this.matchCaseCheckBox.TabIndex = 1;
			this.matchCaseCheckBox.Text = "Match &case";
			// 
			// matchPanel
			// 
			this.matchPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.matchPanel.Controls.Add(this.matchWordsCheckBox);
			this.matchPanel.Controls.Add(this.matchCaseCheckBox);
			this.matchPanel.Location = new System.Drawing.Point(12, 70);
			this.matchPanel.Name = "matchPanel";
			this.matchPanel.Size = new System.Drawing.Size(159, 56);
			this.matchPanel.TabIndex = 4;
			// 
			// FindReplaceForm
			// 
			this.AcceptButton = this.findNextButton;
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(416, 138);
			this.Controls.Add(this.matchPanel);
			this.Controls.Add(this.replaceComboBox);
			this.Controls.Add(this.replaceWithLabel);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.replaceAllButton);
			this.Controls.Add(this.directionGroupBox);
			this.Controls.Add(this.findWhatLabel);
			this.Controls.Add(this.findComboBox);
			this.Controls.Add(this.replaceButton);
			this.Controls.Add(this.findNextButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FindReplaceForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "FindReplaceForm";
			this.directionGroupBox.ResumeLayout(false);
			this.directionGroupBox.PerformLayout();
			this.matchPanel.ResumeLayout(false);
			this.matchPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button findNextButton;
		private System.Windows.Forms.Button replaceButton;
		private System.Windows.Forms.ComboBox findComboBox;
		private System.Windows.Forms.Label findWhatLabel;
		private System.Windows.Forms.GroupBox directionGroupBox;
		private System.Windows.Forms.Button replaceAllButton;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.RadioButton downRadioButton;
		private System.Windows.Forms.RadioButton upRadioButton;
		private System.Windows.Forms.Label replaceWithLabel;
		private System.Windows.Forms.ComboBox replaceComboBox;
		private System.Windows.Forms.CheckBox matchWordsCheckBox;
		private System.Windows.Forms.CheckBox matchCaseCheckBox;
		private System.Windows.Forms.Panel matchPanel;
	}
}