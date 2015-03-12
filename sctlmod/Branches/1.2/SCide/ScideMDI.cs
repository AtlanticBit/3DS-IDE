using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;
using WeifenLuo.WinFormsUI.Docking;

using Scintilla;
using Scintilla.Enums;
using Scintilla.Forms;
using Scintilla.Printing;
using Scintilla.Configuration;
using Scintilla.Configuration.SciTE;

namespace SCide
{
    public partial class ScideMDI : Form, IScideMDI
    {
        private const string DOCK_CONFIG_FILE = @"\DockManager.config";

        private FindForm findDialog = new FindForm();
        private ReplaceForm replaceDialog = new ReplaceForm();
        private ScintillaConfig scintillaConfig;
        
        public ScideMDI()
        {
            InitializeComponent();
        }

        private void ScideMDI_Load(object sender, EventArgs e)
        {
            // Testing new Configuration Files
            FileInfo exeFile = new FileInfo(Application.ExecutablePath);

            // You can pass a ScintillaConfigProvider class into the ScintillaConfig constructor to use custom 
            // config files, or just use the embedded resource config files in the scintillaNet assembly by passing nothing.
            scintillaConfig = new ScintillaConfig();

            DeserializeDockContent deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            string configFile = exeFile.DirectoryName + DOCK_CONFIG_FILE;
            try
            {
                if (File.Exists(configFile))
                {
                    dockPanel.LoadFromXml(configFile, deserializeDockContent);
                }
            }
            catch
            {
                if (File.Exists(configFile))
                {
                    try
                    {
                        File.Delete(configFile);
                    }
                    catch { }
                }
            }
        }

        private IDockContent FindDocument(string text)
        {
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                    if (form.Text == text)
                        return form as IDockContent;

                return null;
            }
            else
            {
                IDockContent[] documents = dockPanel.GetDocuments();

                foreach (IDockContent content in documents)
                    if (content.DockHandler.TabText == text)
                        return content;

                return null;
            }
        }

        private void CreateNewDocument()
        {
            FileEditorDockContent editor = new FileEditorDockContent(this);

            int count = 1;
            string text = "Document" + count.ToString();
            while (FindDocument(text) != null)
            {
                count++;
                text = "Document" + count.ToString();
            }
            editor.Text = text;
            editor.Show(dockPanel);
        }

        private void OpenDocument()
        {
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.InitialDirectory = Application.ExecutablePath;
            openFile.Filter = scintillaConfig.FileOpenFilter.TrimEnd('|'); //.properties["file.filterstring"];
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string fullName = openFile.FileName;
                string fileName = Path.GetFileName(fullName);

                if (FindDocument(fileName) != null)
                {
                    MessageBox.Show("The document: " + fileName + " is already open!");
                    return;
                }

                FileEditorDockContent editor = new FileEditorDockContent(this);
                editor.Text = fileName;
                editor.Show(dockPanel);
                try
                {
                    editor.FileName = fullName;
                }
                catch (Exception exception)
                {
                    editor.Close();
                    MessageBox.Show(exception.Message);
                }
            }
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            string[] parsedStrings = persistString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (parsedStrings.Length != 3)
                return null;

            if (parsedStrings[0] != typeof(FileEditorDockContent).ToString())
                return null;

            FileEditorDockContent document = new FileEditorDockContent(this);
            if (parsedStrings[1] != string.Empty)
                document.FileName = parsedStrings[1];
            if (parsedStrings[2] != string.Empty)
                document.Text = parsedStrings[2];

            return document;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.OpenDocument();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.CreateNewDocument();
        }

        private void toolStripButtonNew_Click(object sender, EventArgs e)
        {
            this.CreateNewDocument();
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            this.OpenDocument();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScideAboutBox ab = new ScideAboutBox();
            ab.ShowDialog(this);
        }

        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
            FileEditorDockContent fe = this.dockPanel.ActiveDocument as FileEditorDockContent;
            if (fe == null)
            {
                this.FindDialog.Close();
                this.ReplaceDialog.Close();
            }
        }

        private void ScideMDI_FormClosing(object sender, FormClosingEventArgs e)
        {
            FileEditorDockContent editor;
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                {
                    if (form is FileEditorDockContent)
                    {
                        editor = form as FileEditorDockContent;
                        if (!editor.SaveBeforeCloseCheck)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                
                    // Close and hidden windows.
                    if (form.Visible == false)
                    {
                        form.Close();
                    }
                }
            }
            else
            {
                IDockContent[] documents = dockPanel.GetDocuments();

                foreach (IDockContent content in documents)
                {
                    if (content is FileEditorDockContent)
                    {
                        editor = content as FileEditorDockContent;
                        if (!editor.SaveBeforeCloseCheck)
                        {
                            e.Cancel = true;
                            return;
                        }

                    }
                
                    // Close and hidden windows.
                    if (content.DockHandler.IsHidden)
                    {
                        content.DockHandler.VisibleState = DockState.Hidden;
                        content.DockHandler.Close();
                    }
                }
            }

            FileInfo exePath = new FileInfo(Application.ExecutablePath);
            string configFile = exePath.DirectoryName + DOCK_CONFIG_FILE;
            dockPanel.SaveAsXml(configFile);
        }

        #region IScideMDI Members

        public FindForm FindDialog
        {
            get { return findDialog; }
        }

        public ReplaceForm ReplaceDialog
        {
            get { return replaceDialog; }
        }

        public IScintillaConfig Configuration
        {
            get { return scintillaConfig; }
        }

        public DockPanel DockPanel
        {
            get { return dockPanel;  }
        }

       /* public SciTEProperties Properties
        {
            get { return properties; }
        }*/

        #endregion

        #region The HTML Designer has issues, especially when it comes to docking! We will probably hack this code.
        private void openInHTMLDesignerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();

            openFile.InitialDirectory = Application.ExecutablePath;
            openFile.Filter = "Web (html htm)|*.html;*.htm";
            openFile.FilterIndex = 1;
            openFile.RestoreDirectory = true;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                string fullName = openFile.FileName;
                string fileName = Path.GetFileName(fullName);

                if (FindDocument(fileName) != null)
                {
                    MessageBox.Show("The document: " + fileName + " is already open!");
                    return;
                }

                HtmlEditorDockContent editor = new HtmlEditorDockContent(this);
                editor.Text = fileName;
                editor.Show(dockPanel);
                try
                {
                    editor.FileName = fullName;
                }
                catch (Exception exception)
                {
                    editor.Close();
                    MessageBox.Show(exception.Message);
                }
            }
        }
        #endregion

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TestConfigurationForm form = new TestConfigurationForm(this.scintillaConfig);
            form.ShowDialog(this);
        }
    }
}