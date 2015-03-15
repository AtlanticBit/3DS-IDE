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
        void cmd(string command)
        {
            System.Diagnostics.Process.Start("CMD.exe", command);
        }
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
        //setup variables
        bool shouldNew = Properties.Settings.Default.shouldNew;
        string createworkdirloc;
        string workdirname;
        string workdir;

        //project variables
        string projectname;
        string projectversion;
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
                workdirname = Interaction.InputBox("What do you want to name your project?\nPlease name it with a name that is suitable for folders\nor 3DS-IDE will crash(sorry ;-;)", "Setup", "");
                string workdir = createworkdirloc + @"\" + workdirname;
                projectname = workdirname;
                cmd("mkdir " + workdir);
                cmd("cd " + workdir + @" && C:\3DS-IDE\newproject.bat");
                string[] projectfile = { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<name>" + projectname + @"</name>", "Third line" };
                try
                { 
                    if (File.Exists(workdir + @"\project.lua3dsproj"))
                    {
                        MessageBox.Show("File already exists!\nApp WILL crash now!\nError code: 12");
                    }
                    File.Create(workdir + @"\project.lua3dsproj");
                    System.IO.File.WriteAllLines(workdir + @"\project.lua3dsproj", projectfile);  
                }

                catch (Exception ex)
                {
                    MessageBox.Show(@"Error:" + ex.ToString());
                }
            }
            /*string[] filePaths = Directory.GetFiles(@"c:\Maps\", "*.txt",
                                         SearchOption.TopDirectoryOnly);*/ // commented because crashed IDEview
            this.scintilla1.ConfigurationManager.CustomLocation = @"C:\3DS-IDE\lua.xml";
            this.scintilla1.ConfigurationManager.Language = "lua";
            this.scintilla1.AutoLaunchAutoComplete = true;
            this.scintilla1.ConfigurationManager.Configure();
            //this.scintilla1.AutoComplete.ListString = System.IO.File.ReadAllText(@"C:\3DS-IDE\autolist"); //that's wrong i guess
        }
    }
}
