namespace SCide
{
    partial class HtmlEditorDockContent
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HtmlEditorDockContent));
            this.menuStripEditor = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.htmlEditControl = new HTMLEditor.HtmlControl();
            this.menuStripEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripEditor
            // 
            this.menuStripEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStripEditor.Location = new System.Drawing.Point(0, 0);
            this.menuStripEditor.Name = "menuStripEditor";
            this.menuStripEditor.Size = new System.Drawing.Size(776, 24);
            this.menuStripEditor.TabIndex = 0;
            this.menuStripEditor.Text = "menuStrip1";
            this.menuStripEditor.Visible = false;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem4,
            this.closeToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.toolStripMenuItem4.MergeIndex = 2;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(144, 6);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.closeToolStripMenuItem.MergeIndex = 2;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveAsToolStripMenuItem.MergeIndex = 2;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.saveAsToolStripMenuItem.Text = "Save &As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this.saveToolStripMenuItem.MergeIndex = 2;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // htmlEditControl
            // 
            this.htmlEditControl.AbsolutePositioningEnabled = false;
            this.htmlEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlEditControl.IsDesignMode = true;
            this.htmlEditControl.IsDirty = false;
            this.htmlEditControl.Location = new System.Drawing.Point(0, 0);
            this.htmlEditControl.MultipleSelectionEnabled = false;
            this.htmlEditControl.Name = "htmlEditControl";
            this.htmlEditControl.ScriptEnabled = false;
            this.htmlEditControl.ScriptObject = null;
            this.htmlEditControl.Size = new System.Drawing.Size(776, 517);
            this.htmlEditControl.TabIndex = 0;
            // 
            // HtmlEditorDockContent
            // 
            this.ClientSize = new System.Drawing.Size(776, 517);
            this.Controls.Add(this.htmlEditControl);
            this.Controls.Add(this.menuStripEditor);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripEditor;
            this.Name = "HtmlEditorDockContent";
            this.Activated += new System.EventHandler(this.HtmlEditorDockContent_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HtmlEditorDockContent_FormClosing);
            this.menuStripEditor.ResumeLayout(false);
            this.menuStripEditor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripEditor;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButtonSave;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private HTMLEditor.HtmlControl htmlEditControl;
    }
}
