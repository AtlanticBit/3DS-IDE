namespace ScintillaPad
{
	partial class MainForm
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
			this.components = new System.ComponentModel.Container();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitMenuItemSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.cutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.findMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findNextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.replaceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.goToMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.selectAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.viewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusBarMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomMenuItemSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomInMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.zoomOutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.zoomResetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.formatMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.wordWrapMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.fontToolItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.unitTestMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
			this.positionStatusStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.fontDialog = new System.Windows.Forms.FontDialog();
			this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.undoContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
			this.cutContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.deleteContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.selectAllContextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.scintilla = new ScintillaNet.Scintilla();
			this.goToDialog = new ScintillaNet.Dialogs.GoToDialog();
			this.findDialog = new ScintillaNet.Dialogs.FindDialog();
			this.replaceDialog = new ScintillaNet.Dialogs.ReplaceDialog();
			this.menuStrip.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.contextMenuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.scintilla)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.editMenuItem,
            this.viewMenuItem,
            this.formatMenuItem,
            this.helpMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.menuStrip.Size = new System.Drawing.Size(632, 24);
			this.menuStrip.TabIndex = 0;
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenuItem,
            this.openMenuItem,
            this.saveMenuItem,
            this.saveAsMenuItem,
            this.exitMenuItemSeparator,
            this.exitMenuItem});
			this.fileMenuItem.Name = "fileMenuItem";
			this.fileMenuItem.Size = new System.Drawing.Size(35, 20);
			this.fileMenuItem.Text = "&File";
			// 
			// newMenuItem
			// 
			this.newMenuItem.Image = global::ScintillaPad.Properties.Resources.NewImage;
			this.newMenuItem.Name = "newMenuItem";
			this.newMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newMenuItem.Size = new System.Drawing.Size(163, 22);
			this.newMenuItem.Text = "&New";
			this.newMenuItem.Click += new System.EventHandler(this.newMenuItem_Click);
			// 
			// openMenuItem
			// 
			this.openMenuItem.Image = global::ScintillaPad.Properties.Resources.OpenImage;
			this.openMenuItem.Name = "openMenuItem";
			this.openMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openMenuItem.Size = new System.Drawing.Size(163, 22);
			this.openMenuItem.Text = "&Open...";
			this.openMenuItem.Click += new System.EventHandler(this.openMenuItem_Click);
			// 
			// saveMenuItem
			// 
			this.saveMenuItem.Image = global::ScintillaPad.Properties.Resources.SaveImage;
			this.saveMenuItem.Name = "saveMenuItem";
			this.saveMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveMenuItem.Size = new System.Drawing.Size(163, 22);
			this.saveMenuItem.Text = "&Save";
			this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
			// 
			// saveAsMenuItem
			// 
			this.saveAsMenuItem.Name = "saveAsMenuItem";
			this.saveAsMenuItem.Size = new System.Drawing.Size(163, 22);
			this.saveAsMenuItem.Text = "Save &As...";
			this.saveAsMenuItem.Click += new System.EventHandler(this.saveAsMenuItem_Click);
			// 
			// exitMenuItemSeparator
			// 
			this.exitMenuItemSeparator.Name = "exitMenuItemSeparator";
			this.exitMenuItemSeparator.Size = new System.Drawing.Size(160, 6);
			// 
			// exitMenuItem
			// 
			this.exitMenuItem.Name = "exitMenuItem";
			this.exitMenuItem.Size = new System.Drawing.Size(163, 22);
			this.exitMenuItem.Text = "E&xit";
			this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
			// 
			// editMenuItem
			// 
			this.editMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoMenuItem,
            this.toolStripSeparator4,
            this.cutMenuItem,
            this.copyMenuItem,
            this.pasteMenuItem,
            this.deleteMenuItem,
            this.toolStripSeparator2,
            this.findMenuItem,
            this.findNextMenuItem,
            this.replaceMenuItem,
            this.goToMenuItem,
            this.toolStripSeparator6,
            this.selectAllMenuItem});
			this.editMenuItem.Name = "editMenuItem";
			this.editMenuItem.Size = new System.Drawing.Size(37, 20);
			this.editMenuItem.Text = "&Edit";
			this.editMenuItem.DropDownOpening += new System.EventHandler(this.editMenuItem_DropDownOpening);
			this.editMenuItem.DropDownClosed += new System.EventHandler(this.editMenuItem_DropDownClosed);
			// 
			// undoMenuItem
			// 
			this.undoMenuItem.Image = global::ScintillaPad.Properties.Resources.UndoImage;
			this.undoMenuItem.Name = "undoMenuItem";
			this.undoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.undoMenuItem.Size = new System.Drawing.Size(174, 22);
			this.undoMenuItem.Text = "&Undo";
			this.undoMenuItem.Click += new System.EventHandler(this.undoMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(171, 6);
			// 
			// cutMenuItem
			// 
			this.cutMenuItem.Image = global::ScintillaPad.Properties.Resources.CutImage;
			this.cutMenuItem.Name = "cutMenuItem";
			this.cutMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
			this.cutMenuItem.Size = new System.Drawing.Size(174, 22);
			this.cutMenuItem.Text = "Cu&t";
			this.cutMenuItem.Click += new System.EventHandler(this.cutMenuItem_Click);
			// 
			// copyMenuItem
			// 
			this.copyMenuItem.Image = global::ScintillaPad.Properties.Resources.CopyImage;
			this.copyMenuItem.Name = "copyMenuItem";
			this.copyMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
			this.copyMenuItem.Size = new System.Drawing.Size(174, 22);
			this.copyMenuItem.Text = "&Copy";
			this.copyMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
			// 
			// pasteMenuItem
			// 
			this.pasteMenuItem.Image = global::ScintillaPad.Properties.Resources.PasteImage;
			this.pasteMenuItem.Name = "pasteMenuItem";
			this.pasteMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
			this.pasteMenuItem.Size = new System.Drawing.Size(174, 22);
			this.pasteMenuItem.Text = "&Paste";
			this.pasteMenuItem.Click += new System.EventHandler(this.pasteMenuItem_Click);
			// 
			// deleteMenuItem
			// 
			this.deleteMenuItem.Image = global::ScintillaPad.Properties.Resources.DeleteImage;
			this.deleteMenuItem.Name = "deleteMenuItem";
			this.deleteMenuItem.ShortcutKeyDisplayString = "Del";
			this.deleteMenuItem.Size = new System.Drawing.Size(174, 22);
			this.deleteMenuItem.Text = "De&lete";
			this.deleteMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(171, 6);
			// 
			// findMenuItem
			// 
			this.findMenuItem.Enabled = false;
			this.findMenuItem.Name = "findMenuItem";
			this.findMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.findMenuItem.Size = new System.Drawing.Size(174, 22);
			this.findMenuItem.Text = "&Find...";
			this.findMenuItem.Visible = false;
			this.findMenuItem.Click += new System.EventHandler(this.findMenuItem_Click);
			// 
			// findNextMenuItem
			// 
			this.findNextMenuItem.Enabled = false;
			this.findNextMenuItem.Name = "findNextMenuItem";
			this.findNextMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
			this.findNextMenuItem.Size = new System.Drawing.Size(174, 22);
			this.findNextMenuItem.Text = "Find &Next";
			this.findNextMenuItem.Visible = false;
			this.findNextMenuItem.Click += new System.EventHandler(this.findNextMenuItem_Click);
			// 
			// replaceMenuItem
			// 
			this.replaceMenuItem.Enabled = false;
			this.replaceMenuItem.Name = "replaceMenuItem";
			this.replaceMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
			this.replaceMenuItem.Size = new System.Drawing.Size(174, 22);
			this.replaceMenuItem.Text = "&Replace...";
			this.replaceMenuItem.Visible = false;
			this.replaceMenuItem.Click += new System.EventHandler(this.replaceMenuItem_Click);
			// 
			// goToMenuItem
			// 
			this.goToMenuItem.Name = "goToMenuItem";
			this.goToMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
			this.goToMenuItem.Size = new System.Drawing.Size(174, 22);
			this.goToMenuItem.Text = "&Go To...";
			this.goToMenuItem.Click += new System.EventHandler(this.goToMenuItem_Click);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(171, 6);
			// 
			// selectAllMenuItem
			// 
			this.selectAllMenuItem.Name = "selectAllMenuItem";
			this.selectAllMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
			this.selectAllMenuItem.Size = new System.Drawing.Size(174, 22);
			this.selectAllMenuItem.Text = "Select &All";
			this.selectAllMenuItem.Click += new System.EventHandler(this.selectAllMenuItem_Click);
			// 
			// viewMenuItem
			// 
			this.viewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarMenuItem,
            this.zoomMenuItemSeparator,
            this.toolStripMenuItem2});
			this.viewMenuItem.Name = "viewMenuItem";
			this.viewMenuItem.Size = new System.Drawing.Size(41, 20);
			this.viewMenuItem.Text = "&View";
			// 
			// statusBarMenuItem
			// 
			this.statusBarMenuItem.CheckOnClick = true;
			this.statusBarMenuItem.Name = "statusBarMenuItem";
			this.statusBarMenuItem.Size = new System.Drawing.Size(135, 22);
			this.statusBarMenuItem.Text = "&Status Bar";
			this.statusBarMenuItem.CheckedChanged += new System.EventHandler(this.statusBarMenuItem_CheckedChanged);
			// 
			// zoomMenuItemSeparator
			// 
			this.zoomMenuItemSeparator.Name = "zoomMenuItemSeparator";
			this.zoomMenuItemSeparator.Size = new System.Drawing.Size(132, 6);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomInMenuItem,
            this.zoomOutMenuItem,
            this.toolStripSeparator1,
            this.zoomResetMenuItem});
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(135, 22);
			this.toolStripMenuItem2.Text = "&Zoom";
			// 
			// zoomInMenuItem
			// 
			this.zoomInMenuItem.Image = global::ScintillaPad.Properties.Resources.ZoomInImage;
			this.zoomInMenuItem.Name = "zoomInMenuItem";
			this.zoomInMenuItem.ShortcutKeyDisplayString = "Ctrl++";
			this.zoomInMenuItem.Size = new System.Drawing.Size(168, 22);
			this.zoomInMenuItem.Text = "Zoom &In";
			// 
			// zoomOutMenuItem
			// 
			this.zoomOutMenuItem.Image = global::ScintillaPad.Properties.Resources.ZoomOutImage;
			this.zoomOutMenuItem.Name = "zoomOutMenuItem";
			this.zoomOutMenuItem.ShortcutKeyDisplayString = "Ctrl+-";
			this.zoomOutMenuItem.Size = new System.Drawing.Size(168, 22);
			this.zoomOutMenuItem.Text = "Zoom &Out";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
			// 
			// zoomResetMenuItem
			// 
			this.zoomResetMenuItem.Name = "zoomResetMenuItem";
			this.zoomResetMenuItem.ShortcutKeyDisplayString = "Ctrl+0";
			this.zoomResetMenuItem.Size = new System.Drawing.Size(168, 22);
			this.zoomResetMenuItem.Text = "&Reset";
			// 
			// formatMenuItem
			// 
			this.formatMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wordWrapMenuItem,
            this.fontToolItem});
			this.formatMenuItem.Name = "formatMenuItem";
			this.formatMenuItem.Size = new System.Drawing.Size(53, 20);
			this.formatMenuItem.Text = "F&ormat";
			// 
			// wordWrapMenuItem
			// 
			this.wordWrapMenuItem.CheckOnClick = true;
			this.wordWrapMenuItem.Name = "wordWrapMenuItem";
			this.wordWrapMenuItem.Size = new System.Drawing.Size(140, 22);
			this.wordWrapMenuItem.Text = "&Word Wrap";
			this.wordWrapMenuItem.CheckedChanged += new System.EventHandler(this.wordWrapMenuItem_CheckedChanged);
			// 
			// fontToolItem
			// 
			this.fontToolItem.Name = "fontToolItem";
			this.fontToolItem.Size = new System.Drawing.Size(140, 22);
			this.fontToolItem.Text = "&Font...";
			this.fontToolItem.Click += new System.EventHandler(this.fontMenuItem_Click);
			// 
			// helpMenuItem
			// 
			this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.unitTestMenuItem,
            this.aboutMenuItem});
			this.helpMenuItem.Name = "helpMenuItem";
			this.helpMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helpMenuItem.Text = "&Help";
			// 
			// unitTestMenuItem
			// 
			this.unitTestMenuItem.Name = "unitTestMenuItem";
			this.unitTestMenuItem.Size = new System.Drawing.Size(140, 22);
			this.unitTestMenuItem.Text = "&Unit Test...";
			this.unitTestMenuItem.Click += new System.EventHandler(this.unitTestMenuItem_Click);
			// 
			// aboutMenuItem
			// 
			this.aboutMenuItem.Name = "aboutMenuItem";
			this.aboutMenuItem.Size = new System.Drawing.Size(140, 22);
			this.aboutMenuItem.Text = "&About";
			this.aboutMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.positionStatusStripLabel});
			this.statusStrip.Location = new System.Drawing.Point(0, 424);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(632, 22);
			this.statusStrip.TabIndex = 1;
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(422, 17);
			this.toolStripStatusLabel1.Spring = true;
			// 
			// toolStripStatusLabel2
			// 
			this.toolStripStatusLabel2.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripStatusLabel2.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
			this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
			this.toolStripStatusLabel2.Size = new System.Drawing.Size(20, 17);
			this.toolStripStatusLabel2.Text = "   ";
			// 
			// positionStatusStripLabel
			// 
			this.positionStatusStripLabel.AutoSize = false;
			this.positionStatusStripLabel.Name = "positionStatusStripLabel";
			this.positionStatusStripLabel.Size = new System.Drawing.Size(175, 17);
			this.positionStatusStripLabel.Text = "Ln ?, Col ?";
			this.positionStatusStripLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStrip
			// 
			this.toolStrip.Location = new System.Drawing.Point(0, 24);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.toolStrip.Size = new System.Drawing.Size(632, 25);
			this.toolStrip.TabIndex = 2;
			this.toolStrip.Visible = false;
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.DefaultExt = "txt";
			this.saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
			// 
			// contextMenuStrip
			// 
			this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoContextMenuItem,
            this.toolStripSeparator5,
            this.cutContextMenuItem,
            this.copyContextMenuItem,
            this.pasteContextMenuItem,
            this.deleteContextMenuItem,
            this.toolStripSeparator3,
            this.selectAllContextMenuItem});
			this.contextMenuStrip.Name = "contextMenuStrip";
			this.contextMenuStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.contextMenuStrip.Size = new System.Drawing.Size(129, 148);
			this.contextMenuStrip.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenuStrip_Closed);
			this.contextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Opening);
			// 
			// undoContextMenuItem
			// 
			this.undoContextMenuItem.Image = global::ScintillaPad.Properties.Resources.UndoImage;
			this.undoContextMenuItem.Name = "undoContextMenuItem";
			this.undoContextMenuItem.Size = new System.Drawing.Size(128, 22);
			this.undoContextMenuItem.Text = "&Undo";
			this.undoContextMenuItem.Click += new System.EventHandler(this.undoMenuItem_Click);
			// 
			// toolStripSeparator5
			// 
			this.toolStripSeparator5.Name = "toolStripSeparator5";
			this.toolStripSeparator5.Size = new System.Drawing.Size(125, 6);
			// 
			// cutContextMenuItem
			// 
			this.cutContextMenuItem.Image = global::ScintillaPad.Properties.Resources.CutImage;
			this.cutContextMenuItem.Name = "cutContextMenuItem";
			this.cutContextMenuItem.Size = new System.Drawing.Size(128, 22);
			this.cutContextMenuItem.Text = "Cu&t";
			this.cutContextMenuItem.Click += new System.EventHandler(this.cutMenuItem_Click);
			// 
			// copyContextMenuItem
			// 
			this.copyContextMenuItem.Image = global::ScintillaPad.Properties.Resources.CopyImage;
			this.copyContextMenuItem.Name = "copyContextMenuItem";
			this.copyContextMenuItem.Size = new System.Drawing.Size(128, 22);
			this.copyContextMenuItem.Text = "&Copy";
			this.copyContextMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
			// 
			// pasteContextMenuItem
			// 
			this.pasteContextMenuItem.Image = global::ScintillaPad.Properties.Resources.PasteImage;
			this.pasteContextMenuItem.Name = "pasteContextMenuItem";
			this.pasteContextMenuItem.Size = new System.Drawing.Size(128, 22);
			this.pasteContextMenuItem.Text = "&Paste";
			this.pasteContextMenuItem.Click += new System.EventHandler(this.pasteMenuItem_Click);
			// 
			// deleteContextMenuItem
			// 
			this.deleteContextMenuItem.Image = global::ScintillaPad.Properties.Resources.DeleteImage;
			this.deleteContextMenuItem.Name = "deleteContextMenuItem";
			this.deleteContextMenuItem.Size = new System.Drawing.Size(128, 22);
			this.deleteContextMenuItem.Text = "De&lete";
			this.deleteContextMenuItem.Click += new System.EventHandler(this.deleteMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(125, 6);
			// 
			// selectAllContextMenuItem
			// 
			this.selectAllContextMenuItem.Name = "selectAllContextMenuItem";
			this.selectAllContextMenuItem.Size = new System.Drawing.Size(128, 22);
			this.selectAllContextMenuItem.Text = "Select &All";
			this.selectAllContextMenuItem.Click += new System.EventHandler(this.selectAllMenuItem_Click);
			// 
			// scintilla
			// 
			this.scintilla.ContextMenuStrip = this.contextMenuStrip;
			this.scintilla.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scintilla.Location = new System.Drawing.Point(0, 24);
			this.scintilla.Margins.Margin0.IsFoldMargin = true;
			this.scintilla.Margins.Margin1.IsFoldMargin = true;
			this.scintilla.Margins.Margin1.Width = 0;
			this.scintilla.Margins.Margin3.IsFoldMargin = true;
			this.scintilla.Margins.Margin4.IsFoldMargin = true;
			this.scintilla.Name = "scintilla";
			this.scintilla.SelectionInactiveBackColor = System.Drawing.SystemColors.Highlight;
			this.scintilla.SelectionInactiveForeColor = System.Drawing.SystemColors.HighlightText;
			this.scintilla.Size = new System.Drawing.Size(632, 400);
			this.scintilla.Styles.CallTipStyle.Font = new System.Drawing.Font("Tahoma", 8.25F);
			this.scintilla.TabIndex = 3;
			this.scintilla.ModifiedChanged += new System.EventHandler(this.scintilla_ModifiedChanged);
			this.scintilla.UpdateUI += new System.EventHandler(this.scintilla_UpdateUI);
			// 
			// goToDialog
			// 
			this.goToDialog.Scintilla = this.scintilla;
			// 
			// findDialog
			// 
			this.findDialog.Scintilla = this.scintilla;
			this.findDialog.Closed += new System.EventHandler(this.findReplaceDialog_Closed);
			// 
			// replaceDialog
			// 
			this.replaceDialog.Scintilla = this.scintilla;
			// 
			// MainForm
			// 
			this.ClientSize = new System.Drawing.Size(632, 446);
			this.Controls.Add(this.scintilla);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.contextMenuStrip.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.scintilla)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
		private System.Windows.Forms.ToolStrip toolStrip;
		private ScintillaNet.Scintilla scintilla;
		private System.Windows.Forms.ToolStripMenuItem newMenuItem;
		private System.Windows.Forms.ToolStripMenuItem openMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsMenuItem;
		private System.Windows.Forms.ToolStripSeparator exitMenuItemSeparator;
		private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ToolStripMenuItem formatMenuItem;
		private System.Windows.Forms.ToolStripMenuItem wordWrapMenuItem;
		private System.Windows.Forms.ToolStripMenuItem fontToolItem;
		private System.Windows.Forms.FontDialog fontDialog;
		private System.Windows.Forms.ToolStripMenuItem viewMenuItem;
		private System.Windows.Forms.ToolStripMenuItem statusBarMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel positionStatusStripLabel;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
		private System.Windows.Forms.ToolStripMenuItem editMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem selectAllMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem cutContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem deleteContextMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripMenuItem selectAllContextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem undoContextMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
		private System.Windows.Forms.ToolStripMenuItem goToMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private ScintillaNet.Dialogs.GoToDialog goToDialog;
		private System.Windows.Forms.ToolStripMenuItem unitTestMenuItem;
		private System.Windows.Forms.ToolStripSeparator zoomMenuItemSeparator;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripMenuItem zoomInMenuItem;
		private System.Windows.Forms.ToolStripMenuItem zoomOutMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem zoomResetMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findNextMenuItem;
		private System.Windows.Forms.ToolStripMenuItem replaceMenuItem;
		private ScintillaNet.Dialogs.FindDialog findDialog;
		private ScintillaNet.Dialogs.ReplaceDialog replaceDialog;
	}
}

