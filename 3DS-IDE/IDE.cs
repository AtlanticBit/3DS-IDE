﻿using System;
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
using System.Xml;


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
        string projectversion = "1.0.0";
        string projectauthor;
        string currentlyopenedtab;
        public IDE()
        {
            InitializeComponent();
        }
        void replacestring(string filepath, string whattoreplace, string forwhattoreplace)
        {
            var fileContents = System.IO.File.ReadAllText(filepath);
            fileContents = fileContents.Replace(whattoreplace, forwhattoreplace);
            System.IO.File.WriteAllText(filepath, fileContents);
        }
        private void IDE_Load(object sender, EventArgs e)
        {
            keywordList = new List<string>(keywords.Split(' '));
            keywordList.Sort(); 
            Setup:
            if (shouldNew)
            {
                createworkdirloc = ChooseFolder();
                workdirname = Interaction.InputBox("What do you want to name your project?\nPlease name it with a name that is suitable for folders\nor 3DS-IDE will crash(sorry ;-;)", "Setup", "");
                string workdir = createworkdirloc + @"\" + workdirname;
                projectname = workdirname;
                cmd("mkdir " + workdir);
                cmd("cd " + workdir + @" && C:\3DS-IDE\newproject.bat");
                string author = Interaction.InputBox("Who is the project author?");
                string[] projectfile = { "<?xml version=\"1.0\" encoding=\"utf-8\"?>", "<name>" + projectname + @"</name>", "<author>" + author + @"</author>" };
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
                if (!File.Exists(workdir + @"\code\index.lua"))
                {
                    MessageBox.Show("File doesn't exist!\nWarning code: main.lua");
                    File.Create(workdir + @"\code\index.lua");
                }
            }
            else
            {
                MessageBox.Show("Please choose a folder where a project exists", "Setup");
                workdir = ChooseFolder();
                XmlDocument doc = new XmlDocument();
                if (File.Exists(workdir + @"\project.lua3dsproj"))
                {
                    doc.Load(workdir + @"\project.lua3dsproj");
                    XmlNodeList elemList = doc.GetElementsByTagName("name");
                    projectname = elemList[0].InnerText;
                    elemList = doc.GetElementsByTagName("author");
                    projectauthor = elemList[0].InnerText;
                    if(!File.Exists(workdir + @"\code\index.lua"))
                    {
                        MessageBox.Show("File doesn't exist!\nWarning code: main.lua");
                        File.Create(workdir + @"\code\index.lua");
                    }
                }
                else
                {
                    MessageBox.Show("There is no project here!");
                    goto Setup;
                }
            }
            //setup done

            string[] filePaths = Directory.GetFiles(workdir + @"\code\", "*.lua",
                                         SearchOption.TopDirectoryOnly); // commented because crashed IDEview
            listBox1.DataSource = filePaths;
            this.scintilla1.ConfigurationManager.CustomLocation = @"C:\3DS-IDE\lua.xml";
            this.scintilla1.ConfigurationManager.Language = "lua";
            this.scintilla1.AutoLaunchAutoComplete = true;
            this.scintilla1.ConfigurationManager.Configure();
            //this.scintilla1.AutoComplete.ListString = System.IO.File.ReadAllText(@"C:\3DS-IDE\autolist"); //that's wrong i guess
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
