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
using System.IO;

namespace _3DS_IDE
{
    public partial class IDE : Form
    {
        private string keywords = System.IO.File.ReadAllText(@"C:\3DS-IDE\lua.complete");
        List<string> keywordList;
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
            keywordList = new List<string>(keywords.Split(' '));
            keywordList.Sort(); 
            if (shouldNew)
            {
                createworkdirloc = ChooseFolder();
                workdir = Interaction.InputBox("What do you want to name your project?", "Setup", "");

            }
            /*string[] filePaths = Directory.GetFiles(@"c:\Maps\", "*.txt",
                                         SearchOption.TopDirectoryOnly);*/
            this.scintilla1.ConfigurationManager.CustomLocation = @"C:\3DS-IDE\lua.xml";
            this.scintilla1.ConfigurationManager.Language = "lua";
            this.scintilla1.AutoLaunchAutoComplete = true;
            this.scintilla1.ConfigurationManager.Configure();
            //this.scintilla1.AutoComplete.ListString = System.IO.File.ReadAllText(@"C:\3DS-IDE\autolist"); //that's wrong i guess
        }
    }
}
