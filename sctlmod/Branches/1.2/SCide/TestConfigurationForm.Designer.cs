namespace SCide
{
    partial class TestConfigurationForm
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
            this.styleConfigControl1 = new Scintilla.Forms.Configuration.StyleConfigControl();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // styleConfigControl1
            // 
            this.styleConfigControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.styleConfigControl1.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.styleConfigControl1.Location = new System.Drawing.Point(0, 1);
            this.styleConfigControl1.MinimumSize = new System.Drawing.Size(620, 473);
            this.styleConfigControl1.Name = "styleConfigControl1";
            this.styleConfigControl1.Size = new System.Drawing.Size(657, 473);
            this.styleConfigControl1.TabIndex = 0;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(570, 480);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 1;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // TestConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 510);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.styleConfigControl1);
            this.Name = "TestConfigurationForm";
            this.Text = "TestConfigurationForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Scintilla.Forms.Configuration.StyleConfigControl styleConfigControl1;
        private System.Windows.Forms.Button buttonClose;
    }
}