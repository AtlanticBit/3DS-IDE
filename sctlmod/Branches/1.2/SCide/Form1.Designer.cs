namespace SCide
{
 partial class Form1
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
      this.scintillaControl1 = new Scintilla.ScintillaControl();
      this.SuspendLayout();
      // 
      // scintillaControl1
      // 
      this.scintillaControl1.Location = new System.Drawing.Point(12, 12);
      this.scintillaControl1.Name = "scintillaControl1";
      this.scintillaControl1.Size = new System.Drawing.Size(491, 417);
      this.scintillaControl1.TabIndex = 0;
      this.scintillaControl1.Text = "scintillaControl1";
      this.scintillaControl1.Click += new System.EventHandler(this.scintillaControl1_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(827, 685);
      this.Controls.Add(this.scintillaControl1);
      this.Name = "Form1";
      this.Text = "SCide";
      this.ResumeLayout(false);

  }

  #endregion

     private Scintilla.ScintillaControl scintillaControl1;





}
}

