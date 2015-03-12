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
    public partial class FormFind : Form
    {
        public FormFind()
        {
            InitializeComponent();
        }

        public void Initialize(ScintillaControl control) 
        {
            this.findControl1.Initialize(control);
            this.findControl1.Cancel += new CancelEventHandler(findControl1_Cancel);
        }

        void findControl1_Cancel(object sender, CancelEventArgs e)
        {
            this.findControl1.Reset();
            this.Hide();
        }

        public void FindNext()
        {
            this.findControl1.FindNext();
        }

        public void FindPrevious()
        {
            this.findControl1.FindPrevious();
        }
    }
}