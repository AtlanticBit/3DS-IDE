using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using ScintillaNET;

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
        string createworkdirloc;
        string workdir;
        public IDE()
        {
            InitializeComponent();
        }

        private void IDE_Load(object sender, EventArgs e)
        {
            if (shouldNew)
            {
                createworkdirloc = ChooseFolder();
                workdir = Interaction.InputBox("What do you want to name your project?", "Setup", "");

            }
            this.scintilla1.ConfigurationManager.CustomLocation = @"C:\3DS-IDE\lua.xml";
            this.scintilla1.AutoComplete.ListString = System.IO.File.ReadAllText(@"C:\3DS-IDE\autolist");
            this.scintilla1.AutoComplete.Show();
        }
    }
}
