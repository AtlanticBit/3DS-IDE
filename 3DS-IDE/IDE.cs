using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace _3DS_IDE
{
    public partial class IDE : Form
    {
        public string ChooseFolder()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                return folderBrowserDialog1.SelectedPath;
            }
            else
            {
                MessageBox.Show("Please choose one this time!", "A folder was not chosen!");
                ChooseFolder();
                return "failed";
            }
        }
        bool shouldNew = Properties.Settings.Default.shouldNew;
        string workdir;
        public IDE()
        {
            InitializeComponent();
        }

        private void IDE_Load(object sender, EventArgs e)
        {
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            if (shouldNew)
            {
                workdir = ChooseFolder();
            }
        }
    }
}
