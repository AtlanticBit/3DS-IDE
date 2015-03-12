using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SCide
{
 public partial class Form1 : Form
 {
  public Form1()
  {
   InitializeComponent();
   scintillaControl1.IndicatorStyle[1] = Scintilla.Enums.IndicatorStyle.Diagonal;
   scintillaControl1.SetText("thisis a test\n\n");
   MessageBox.Show(scintillaControl1.Text);
  }

     private void scintillaControl1_Click(object sender, EventArgs e)
     {

     }
 }
}