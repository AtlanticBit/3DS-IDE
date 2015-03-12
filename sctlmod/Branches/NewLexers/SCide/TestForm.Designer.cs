namespace SCide
{
    partial class TestForm
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
            this.scintilla1 = new ScintillaNET.Scintilla();
            ((System.ComponentModel.ISupportInitialize)(this.scintilla1)).BeginInit();
            this.SuspendLayout();
            // 
            // scintilla1
            // 
            this.scintilla1.Location = new System.Drawing.Point(12, 12);
            this.scintilla1.Name = "scintilla1";
            this.scintilla1.Size = new System.Drawing.Size(215, 195);
            this.scintilla1.Styles.BraceBad.FontName = "";
            this.scintilla1.Styles.BraceLight.FontName = "";
            this.scintilla1.Styles.ControlChar.FontName = "";
            this.scintilla1.Styles.Default.BackColor = System.Drawing.SystemColors.Window;
            this.scintilla1.Styles.IndentGuide.FontName = "";
            this.scintilla1.Styles.LastPredefined.FontName = "";
            this.scintilla1.Styles.LineNumber.FontName = "";
            this.scintilla1.TabIndex = 0;
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 456);
            this.Controls.Add(this.scintilla1);
            this.Name = "TestForm";
            this.Text = "Nothing to see here.... This is just a test form I can use to debug design-time b" +
    "ehavior";
            ((System.ComponentModel.ISupportInitialize)(this.scintilla1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ScintillaNET.Scintilla scintilla1;
    }
}