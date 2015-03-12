using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Scintilla;
using Scintilla.Enums;
using Scintilla.Forms;
using Scintilla.Legacy.Configuration;
using Scintilla.Printing;

using Scintilla.Configuration;
using Scintilla.Configuration.SciTE;

namespace SCide
{
    public partial class FormScintillaContainer : Form
    {
        public FindForm findDialog = new FindForm();
        public ReplaceForm replaceDialog = new ReplaceForm();

        private PrintDocument printDocument;
        private PageSettings pageSettings;

        private Scintilla.Legacy.Configuration.Scintilla config;
        private string filename = string.Empty;

        private ScintillaConfig scintillaConfig;
        private SciTEProperties properties;
        
        public FormScintillaContainer()
        {
            InitializeComponent();        
            
            /*
            // Add Scintilla.Net Legacy Support

            // create the configuration utility.
            // you need to pass a type that exists int the assembly where the class that you use as
            // a base node for configuration.
            Scintilla.Legacy.Configuration.ConfigurationUtility cu = new Scintilla.Legacy.Configuration.ConfigurationUtility(GetType().Module.Assembly);

            // set the configuration to scintilla
            this.config = (Scintilla.Legacy.Configuration.Scintilla)cu.LoadConfiguration(typeof(Scintilla.Legacy.Configuration.Scintilla), "LegacyScintillaNET.xml");

            scintillaControl.LegacyConfiguration = this.config;

            // change the language. It automatically changes the lexer for you based on the settings
            // in the config file.
            scintillaControl.LegacyConfigurationLanguage = "C#";

            // set the language menu
            CreateLegacyLanguageMenuOptions();
            */

            // Enable smart indenting
            this.scintillaControl.SmartIndentingEnabled = true;
            this.saveToolStripMenuItem.Enabled = false;

            findDialog.Initialize(scintillaControl);
            replaceDialog.Initialize(scintillaControl);

            printDocument = new PrintDocument(scintillaControl);
            printDocument.DefaultPageSettings = pageSettings = new PageSettings();

            // Testing new Configuration Files
            FileInfo globalConfigFile = new FileInfo(Application.ExecutablePath);
            globalConfigFile = new FileInfo(globalConfigFile.Directory.FullName + @"\Configuration\global.properties");
            if (globalConfigFile.Exists)
            {
                properties = new SciTEProperties();
                SciTEPropertiesReader.Read(globalConfigFile, properties);
                scintillaConfig = new ScintillaConfig(properties);
                scintillaControl.Configuration = scintillaConfig;
                scintillaControl.ConfigurationLanguage = "cs";
            }

            CreateLanguageMenuOptions("cs");
        }

        #region File Menu

        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK) {
                filename = openFileDialog1.FileName;
                FileInfo fi = new FileInfo(filename);
                string language = properties.GetLanguageFromExtension(fi.Extension.TrimStart('.'));
                if (language != null)
                {
                    this.SelectLexerLanguage(language);
                }
                this.scintillaControl.SetText(File.ReadAllText(openFileDialog1.FileName, Encoding.UTF8));
                this.saveToolStripMenuItem.Enabled = true;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            if (filename.Length > 0) {
                File.WriteAllText(filename, this.scintillaControl.Text);
            }
            else {
                saveAsToolStripMenuItem_Click(sender, e);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK) {
                filename = saveFileDialog1.FileName;
                this.saveToolStripMenuItem.Enabled = true;

                File.WriteAllText(filename, scintillaControl.Text, Encoding.GetEncoding(scintillaControl.CodePage));
            }
        }

        PageSettings oPageSettings = new PageSettings();
        System.Drawing.Printing.PrinterSettings oPrinterSettings = new System.Drawing.Printing.PrinterSettings();

        private void pageSetupToolStripMenuItem_Click(object sender, EventArgs e) {
            PageSetupDialog oPageSetupDialog = new PageSetupDialog();

            oPageSetupDialog.PageSettings = pageSettings;
            oPageSetupDialog.PrinterSettings = pageSettings.PrinterSettings;

            oPageSetupDialog.ShowDialog();
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e) {
            PrintPreviewDialog oPrintPreviewDialog = new PrintPreviewDialog();

            oPrintPreviewDialog.Document = printDocument;

            oPrintPreviewDialog.ShowDialog();
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e) {
            PrintDialog oPrintDialog = new PrintDialog();

            oPrintDialog.Document = printDocument;
            oPrintDialog.PrinterSettings = pageSettings.PrinterSettings;

            if (oPrintDialog.ShowDialog() == DialogResult.OK) {
                printDocument.Print();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        #endregion

        #region Edit Menu

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e) {
            undoToolStripMenuItem.Enabled = scintillaControl.CanUndo;
            redoToolStripMenuItem.Enabled = scintillaControl.CanRedo;

            pasteToolStripMenuItem.Enabled = scintillaControl.CanPaste;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e) {
            scintillaControl.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e) {
            scintillaControl.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e) {
            scintillaControl.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e) {
            scintillaControl.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e) {
            scintillaControl.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e) {
            scintillaControl.DeleteBack();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e) {
            scintillaControl.SelectAll();
        }

        #endregion

        #region Search Menu

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!findDialog.Visible && !replaceDialog.Visible)
                findDialog.Show(this);
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!findDialog.Visible && !replaceDialog.Visible)
                replaceDialog.Show(this);
        }

        private void findNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            findDialog.FindNext();
        }

        private void findPreviousToolStripMenuItem_Click(object sender, EventArgs e)
        {
            findDialog.FindPrevious();
        }

        private void goToLineToolStripMenuItem_Click(object sender, EventArgs e) {
            GoToLineDialog.Show(scintillaControl);
        }

        #endregion

        #region Language Menu

        private void CreateLanguageMenuOptions(string defaultLanguage)
        {
            ToolStripMenuItem oMenuItem;

            foreach (MenuItemConfig menuItemConf in this.scintillaConfig.LanguageMenuItems)
            {
                oMenuItem = new ToolStripMenuItem(menuItemConf.Text);
                oMenuItem.Tag = menuItemConf.Value;
                oMenuItem.ShortcutKeys = menuItemConf.ShortcutKeys;
                oMenuItem.Click += new EventHandler(LanguageMenuItem_Click);

                if (menuItemConf.Value == defaultLanguage)
                {
                    oMenuItem.Checked = true;
                }

                languageToolStripMenuItem.DropDownItems.Add(oMenuItem);
            }
        }

        private void LanguageMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem oMenuItem in languageToolStripMenuItem.DropDownItems)
            {
                if (oMenuItem.Checked)
                {
                    oMenuItem.Checked = false;
                    break;
                }
            }

            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem oMenuItem = (ToolStripMenuItem)sender;

                scintillaControl.ConfigurationLanguage = oMenuItem.Tag.ToString();
                oMenuItem.Checked = true;
            }
        }

        private void SelectLexerLanguage(string language)
        {
            ToolStripMenuItem uncheckedMenuItem = null;
            ToolStripMenuItem newMenuItem = null;
            foreach (ToolStripMenuItem oMenuItem in languageToolStripMenuItem.DropDownItems)
            {
                if (language == oMenuItem.Tag.ToString())
                {
                    newMenuItem = oMenuItem;
                    if (uncheckedMenuItem != null) break;
                }
                
                if (oMenuItem.Checked)
                {
                    uncheckedMenuItem = oMenuItem;
                    oMenuItem.Checked = false;
                    if (newMenuItem != null) break;
                }
            }

            if (newMenuItem == null) uncheckedMenuItem.Checked = true;
            else if (uncheckedMenuItem != null) uncheckedMenuItem.Checked = true;

            scintillaControl.ConfigurationLanguage = language;
        }

        private void CreateLegacyLanguageMenuOptions() {
            ToolStripMenuItem oMenuItem;

            foreach (Language oLanguage in this.config.languages) {
                oMenuItem = new ToolStripMenuItem(oLanguage.name);
                oMenuItem.Click += new EventHandler(LegacyLanguageMenuItem_Click);

                if (oLanguage.name == "C#") {
                    oMenuItem.Checked = true;
                }
                                
                languageToolStripMenuItem.DropDownItems.Add(oMenuItem);
            }
        }

        private void LegacyLanguageMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem oMenuItem in languageToolStripMenuItem.DropDownItems) {
                if (oMenuItem.Checked) {
                    oMenuItem.Checked = false;
                    break;
                }
            }

            if (sender is ToolStripMenuItem) {
                ToolStripMenuItem oMenuItem = (ToolStripMenuItem) sender;

                scintillaControl.LegacyConfigurationLanguage = oMenuItem.Text;
                oMenuItem.Checked = true;
            }
        }

        #endregion

    }
}