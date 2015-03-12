using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

using Scintilla.Configuration.SciTE;
using Scintilla.Configuration;
using Scintilla.Printing;
using Scintilla.Forms;

using WeifenLuo.WinFormsUI.Docking;

namespace SCide
{
    /// <summary>
    /// </summary>
    public partial class HtmlEditorDockContent : DockContent
    {
        private string fileName = string.Empty;
        private IScideMDI scideMDI;

        //private PrintDocument printDocument;
        //private PageSettings pageSettings;

        public HtmlEditorDockContent(IScideMDI scideMDI)
        {
            InitializeComponent();

            this.scideMDI = scideMDI;
            //this.scintillaEditor.AddShortcuts(this.menuStripEditor.Items);

            //scintillaEditor.SmartIndentingEnabled = true;
            //scintillaEditor.Configuration = scideMDI.Configuration;
            //scintillaEditor.ConfigurationLanguage = "cs";

            //printDocument = new PrintDocument(scintillaEditor);
            //printDocument.DefaultPageSettings = pageSettings = new PageSettings();

            //CreateLanguageMenuOptions("cs");
        }

        private void HtmlEditorDockContent_Activated(object sender, EventArgs e)
        {
            //scideMDI.FindDialog.Initialize(scintillaEditor);
            //scideMDI.ReplaceDialog.Initialize(scintillaEditor);
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                htmlEditControl.IsDesignMode = true;
                if ((string.IsNullOrEmpty(value)) || (!File.Exists(value)))
                {
                    fileName = Path.GetTempFileName() + ".html";
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }

                    Directory.CreateDirectory(fileName);
                    FileInfo efInfo = new FileInfo(fileName);
                    fileName = efInfo.FullName;
                    this.Text = efInfo.Name;

                    StreamWriter writer = new StreamWriter(this.fileName);
                    writer.Write("<html><body>Hello World!</body></html>");
                    writer.Close();

                }
                else
                {
                    //StreamReader reader = File.OpenText(this.fileName);
                    //this.htmlControl.LoadHtml(reader.ReadToEnd(), this.url);
                    //reader.Close();

                    FileInfo efInfo = new FileInfo(value);
                    if (efInfo.Exists)
                    {
                        fileName = efInfo.FullName;
                        this.Text = efInfo.Name;
                        htmlEditControl.LoadHtml(File.ReadAllText(fileName), fileName);

                        this.Activate();
                    }
                }

                fileName = value;
                this.ToolTipText = value;
            }
        }

        public bool SaveBeforeCloseCheck
        {
            get
            {
                bool returnVal = true;
                if (htmlEditControl.IsDirty)
                {
                    DialogResult result = MessageBox.Show(this, "The current document has changed since it was last saved. Would you like to save now?",
                        "Save file before close?",
                        MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        this.Save();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        returnVal = false;
                    }
                }
                return returnVal;
            }
        }

        protected override string GetPersistString()
        {
            return GetType().ToString() + "," + FileName + "," + Text;
        }

        private void Save()
        {
            //htmlEditControl.Save();
            if (string.IsNullOrEmpty(fileName))
            {
                this.SaveAs();
            }
            else
            {
                //htmlEditControl.Save();
                File.WriteAllText(fileName, htmlEditControl.SaveHtml());//, Encoding.GetEncoding(scintillaEditor.CodePage));
            }
        }

        private void SaveAs()
        {
           // htmlEditControl.SaveAs();
            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.InitialDirectory = Application.ExecutablePath;
            saveFile.Filter = scideMDI.Configuration.FileOpenFilter.TrimEnd('|');
            saveFile.FilterIndex = 1;
            saveFile.RestoreDirectory = true;

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                FileInfo file = new FileInfo(saveFile.FileName);
                //this.fileName = file.FullName;
                //FileInfo file = new FileInfo(fileName);
                //this.Text = file.Name;

                File.WriteAllText(file.FullName, htmlEditControl.SaveHtml()); //, Encoding.GetEncoding(scintillaEditor.CodePage));

                this.FileName = file.FullName;

                //scintillaEditor.EmptyUndoBuffer();
                //scintillaEditor.SetSavePoint();
            }
        }

        /*private void PageSetup()
        {
            PageSetupDialog pageSetupDialog = new PageSetupDialog();

            pageSetupDialog.PageSettings = pageSettings;
            pageSetupDialog.PrinterSettings = pageSettings.PrinterSettings;

            pageSetupDialog.ShowDialog();
        }

        private void PrintPreview()
        {
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

            printPreviewDialog.Document = printDocument;
            printPreviewDialog.ShowDialog();
        }

        private void Print()
        {
            PrintDialog printDialog = new PrintDialog();

            printDialog.Document = printDocument;
            printDialog.PrinterSettings = pageSettings.PrinterSettings;

            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.PrintPreview();
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.PrintPreview();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Print();
        }*/

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.SaveAs();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!scideMDI.FindDialog.Visible && !scideMDI.ReplaceDialog.Visible)
                scideMDI.FindDialog.Show(this);
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!scideMDI.FindDialog.Visible && !scideMDI.ReplaceDialog.Visible)
                scideMDI.ReplaceDialog.Show(this);
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scideMDI.FindDialog.FindNext();
        }

        private void findPrevToolStripMenuItem_Click(object sender, EventArgs e)
        {
            scideMDI.FindDialog.FindPrevious();
        }

        private void goToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GoToLineDialog.Show(scintillaEditor);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.scintillaEditor.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.scintillaEditor.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.scintillaEditor.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.scintillaEditor.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.scintillaEditor.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.scintillaEditor.ReplaceSelection(string.Empty);
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            undoToolStripMenuItem.Enabled = scintillaEditor.CanUndo;
            redoToolStripMenuItem.Enabled = scintillaEditor.CanRedo;
            pasteToolStripMenuItem.Enabled = scintillaEditor.CanPaste;
        }*/

        private void HtmlEditorDockContent_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!SaveBeforeCloseCheck)
            {
                e.Cancel = true;
            }
        }
    }
}

