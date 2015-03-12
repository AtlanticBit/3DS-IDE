using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Scintilla.Configuration;

namespace SCide
{
    public partial class TestConfigurationForm : Form
    {
        public TestConfigurationForm(IScintillaConfig conf)
        {
            InitializeComponent();
            
            // Turn on double-buffering, so the form looks better. 
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.DoubleBuffer, true);

            styleConfigControl1.Configuration = conf;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}