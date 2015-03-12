using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Scintilla;

namespace SCide
{
    public partial class FormReplace : Form
    {
        public FormReplace()
        {
            InitializeComponent();
        }

        public void Initialize(ScintillaControl control) 
        {
            this.replaceControl1.Initialize(control);
            this.replaceControl1.Cancel += new CancelEventHandler(replaceControl1_Cancel);
        }

        void replaceControl1_Cancel(object sender, CancelEventArgs e)
        {
            this.replaceControl1.Reset();
            this.Hide();
        }

        public void ReplaceNext()
        {
            this.replaceControl1.ReplaceNext();
        }
    }
}