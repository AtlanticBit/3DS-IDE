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
    public partial class FileEditorDockContent : DockContent, IScintillaEditControl
    {
        private string fileName = string.Empty;
        private IScideMDI scideMDI;

        private PrintDocument printDocument;
        private PageSettings pageSettings;

        public FileEditorDockContent(IScideMDI scideMDI)
        {
            InitializeComponent();

            this.scideMDI = scideMDI;
            this.scintillaEditor.AddShortcuts(this.menuStripEditor.Items);

            scintillaEditor.Configure = scideMDI.Configuration.Configure;
            scintillaEditor.ConfigurationLanguage = "cs";

            printDocument = new PrintDocument(scintillaEditor);
            printDocument.DefaultPageSettings = pageSettings = new PageSettings();

            CreateLanguageMenuOptions("cs");

            scintillaEditor.SmartIndentType = Scintilla.Enums.SmartIndent.CPP;
            scintillaEditor.IsBraceMatching = true;
        }

        private void FileEditorDockContent_Activated(object sender, EventArgs e)
        {
            scideMDI.FindDialog.Initialize(scintillaEditor);
            scideMDI.ReplaceDialog.Initialize(scintillaEditor);
            scintillaEditor.GrabFocus();
        }

        public string FileName
        {
            get { return fileName; }
            set
            {
                if (value != string.Empty)
                {
                    FileInfo efInfo = new FileInfo(value);
                    if (efInfo.Exists)
                    {
                        string extension = efInfo.Extension.TrimStart('.');
                        if (scideMDI.Configuration.ExtensionLanguages.ContainsKey(extension))
                            this.SelectLexerLanguage(scideMDI.Configuration.ExtensionLanguages[extension]);

                        this.scintillaEditor.Text = File.ReadAllText(value, Encoding.UTF8);

                        this.Activate();
                    }
                }

                scintillaEditor.EmptyUndoBuffer();
                scintillaEditor.SetSavePoint();

                fileName = value;
                this.ToolTipText = value;
            }
        }

        public bool SaveBeforeCloseCheck
        {
            get
            {
                bool returnVal = true;
                if (scintillaEditor.CanUndo)
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
            if (string.IsNullOrEmpty(fileName))
            {
                this.SaveAs();
            }
            else
            {
                FileInfo file = new FileInfo(fileName);
                if (file.Exists && file.IsReadOnly)
                {
                    MessageBox.Show(this, "The file you're trying to save is locked or read only.", 
                        "Cannot save file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    try
                    {
                        File.WriteAllText(fileName, scintillaEditor.Text, Encoding.GetEncoding(scintillaEditor.CodePage));

                        scintillaEditor.EmptyUndoBuffer();
                        scintillaEditor.SetSavePoint();

                    }
                    catch
                    {
                        MessageBox.Show(this, "The file you're trying to save is locked or read only.", 
                            "Cannot save file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void SaveAs()
        {
            SaveFileDialog saveFile = new SaveFileDialog();

            saveFile.InitialDirectory = Application.ExecutablePath;
            saveFile.Filter = scideMDI.Configuration.FileOpenFilter.TrimEnd('|');
            saveFile.FilterIndex = 1;
            saveFile.RestoreDirectory = true;

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                FileInfo file = new FileInfo(saveFile.FileName);
                this.fileName = file.FullName;
                //FileInfo file = new FileInfo(fileName);
                this.Text = file.Name;

                File.WriteAllText(fileName, this.scintillaEditor.Text, Encoding.GetEncoding(scintillaEditor.CodePage));

                scintillaEditor.EmptyUndoBuffer();
                scintillaEditor.SetSavePoint();
            }
        }

        private void PageSetup()
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
            this.PageSetup();
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.PrintPreview();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Print();
        }

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

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
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
        }

        private void FileEditorDockContent_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!SaveBeforeCloseCheck)
            {
                e.Cancel = true;
            }
        }

        #region Language Menu

        private void CreateLanguageMenuOptions(string defaultLanguage)
        {
            ToolStripMenuItem menuItem;

            foreach (MenuItemConfig menuItemConf in this.scideMDI.Configuration.LanguageMenuItems)
            {
                menuItem = new ToolStripMenuItem(menuItemConf.Text);
                menuItem.Tag = menuItemConf.Value;
                menuItem.ShortcutKeys = menuItemConf.ShortcutKeys;
                menuItem.Click += new EventHandler(LanguageMenuItem_Click);

                if (menuItemConf.Value == defaultLanguage)
                {
                    menuItem.Checked = true;
                }

                languageToolStripMenuItem.DropDownItems.Add(menuItem);
            }
        }

        private void LanguageMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem menuItem in languageToolStripMenuItem.DropDownItems)
            {
                if (menuItem.Checked)
                {
                    menuItem.Checked = false;
                    break;
                }
            }

            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem menuItem = sender as ToolStripMenuItem;

                if (menuItem != null)
                {
                    scintillaEditor.ConfigurationLanguage = menuItem.Tag.ToString();
                    menuItem.Checked = true;
                }
            }
        }

        private void SelectLexerLanguage(string language)
        {
            ToolStripMenuItem uncheckedMenuItem = null;
            ToolStripMenuItem newMenuItem = null;
            foreach (ToolStripMenuItem menuItem in languageToolStripMenuItem.DropDownItems)
            {
                if (language == menuItem.Tag.ToString())
                {
                    newMenuItem = menuItem;
                    if (uncheckedMenuItem != null) break;
                }

                if (menuItem.Checked)
                {
                    uncheckedMenuItem = menuItem;
                    menuItem.Checked = false;
                    if (newMenuItem != null) break;
                }
            }

            if (newMenuItem == null) uncheckedMenuItem.Checked = true;
            else if (uncheckedMenuItem != null) newMenuItem.Checked = true;

            scintillaEditor.ConfigurationLanguage = language;
        }

        #endregion


        #region IScintillaEditControl Members

        public Scintilla.ScintillaControl ScintillaEditor
        {
            get { return scintillaEditor; }
        }

        #endregion
    }
}

