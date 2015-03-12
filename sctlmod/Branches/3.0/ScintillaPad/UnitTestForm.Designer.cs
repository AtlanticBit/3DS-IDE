namespace ScintillaPad
{
	partial class UnitTestForm
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
			this.startButton = new System.Windows.Forms.Button();
			this.stopButton = new System.Windows.Forms.Button();
			this.listView = new System.Windows.Forms.ListView();
			this.nameColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.stateColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.resetButton = new System.Windows.Forms.Button();
			this.textBox = new System.Windows.Forms.TextBox();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// startButton
			// 
			this.startButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.startButton.Location = new System.Drawing.Point(110, 508);
			this.startButton.Name = "startButton";
			this.startButton.Size = new System.Drawing.Size(75, 23);
			this.startButton.TabIndex = 0;
			this.startButton.Text = "Start";
			this.startButton.Click += new System.EventHandler(this.startButton_Click);
			// 
			// stopButton
			// 
			this.stopButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.stopButton.Enabled = false;
			this.stopButton.Location = new System.Drawing.Point(191, 508);
			this.stopButton.Name = "stopButton";
			this.stopButton.Size = new System.Drawing.Size(75, 23);
			this.stopButton.TabIndex = 1;
			this.stopButton.Text = "Stop";
			this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
			// 
			// listView
			// 
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumnHeader,
            this.stateColumnHeader});
			this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listView.FullRowSelect = true;
			this.listView.GridLines = true;
			this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView.HideSelection = false;
			this.listView.Location = new System.Drawing.Point(0, 0);
			this.listView.MultiSelect = false;
			this.listView.Name = "listView";
			this.listView.Size = new System.Drawing.Size(432, 238);
			this.listView.TabIndex = 0;
			this.listView.UseCompatibleStateImageBehavior = false;
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.SelectedIndexChanged += new System.EventHandler(this.listView_SelectedIndexChanged);
			// 
			// nameColumnHeader
			// 
			this.nameColumnHeader.Text = "Name";
			this.nameColumnHeader.Width = 342;
			// 
			// stateColumnHeader
			// 
			this.stateColumnHeader.Text = "State";
			this.stateColumnHeader.Width = 84;
			// 
			// resetButton
			// 
			this.resetButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.resetButton.Location = new System.Drawing.Point(272, 508);
			this.resetButton.Name = "resetButton";
			this.resetButton.Size = new System.Drawing.Size(75, 23);
			this.resetButton.TabIndex = 2;
			this.resetButton.Text = "Reset";
			this.resetButton.Click += new System.EventHandler(this.resetButton_Click);
			// 
			// textBox
			// 
			this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox.Location = new System.Drawing.Point(0, 0);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox.Size = new System.Drawing.Size(432, 248);
			this.textBox.TabIndex = 0;
			this.textBox.WordWrap = false;
			// 
			// splitContainer
			// 
			this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer.Location = new System.Drawing.Point(12, 12);
			this.splitContainer.Name = "splitContainer";
			this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.listView);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.textBox);
			this.splitContainer.Size = new System.Drawing.Size(432, 490);
			this.splitContainer.SplitterDistance = 238;
			this.splitContainer.TabIndex = 5;
			// 
			// UnitTestForm
			// 
			this.AcceptButton = this.startButton;
			this.ClientSize = new System.Drawing.Size(456, 543);
			this.Controls.Add(this.splitContainer);
			this.Controls.Add(this.resetButton);
			this.Controls.Add(this.stopButton);
			this.Controls.Add(this.startButton);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(285, 275);
			this.Name = "UnitTestForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Unit Test";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UnitTestForm_FormClosing);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel2.ResumeLayout(false);
			this.splitContainer.Panel2.PerformLayout();
			this.splitContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button startButton;
		private System.Windows.Forms.Button stopButton;
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.ColumnHeader nameColumnHeader;
		private System.Windows.Forms.ColumnHeader stateColumnHeader;
		private System.Windows.Forms.Button resetButton;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.SplitContainer splitContainer;
	}
}