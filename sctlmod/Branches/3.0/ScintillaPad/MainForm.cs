#region Using Directives

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ScintillaNet;

#endregion Using Directives


namespace ScintillaPad
{
	internal partial class MainForm : Form
	{
		#region Constants

		private const string DEFAULT_FILE_TITLE = "Untitled";

		#endregion Constants


		#region Fields

		private string[] _args;
		private string _filePath;
		private string _fileTitle = DEFAULT_FILE_TITLE;
		private Settings _settings;
		private Rectangle _normalBounds;
		private bool _findActive;
		private bool _replaceActive;

		#endregion Fields


		#region Methods

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (AboutForm ab = new AboutForm())
				ab.ShowDialog(this);
		}


		private bool CanCloseFile()
		{
			DialogResult dr = DialogResult.None;

			// Notify the user if they are going to close a modified document
			if (scintilla.Modified)
			{
				string message = Util.Format(
					"The text in the {0} file has changed.{1}" +
					"Do you want to save the changes?",
					(_filePath == null ? _fileTitle : _filePath),
					Environment.NewLine);

				dr = MessageBox.Show(
					this,
					message,
					Util.AssemblyTitle,
					MessageBoxButtons.YesNoCancel,
					MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button1);

				if (dr == DialogResult.Cancel)
					return false;
			}

			if (dr == DialogResult.Yes)
				return Save();

			return true;
		}


		private void contextMenuStrip_Closed(object sender, ToolStripDropDownClosedEventArgs e)
		{
			undoContextMenuItem.Enabled = true;
			cutContextMenuItem.Enabled = true;
			copyContextMenuItem.Enabled = true;
			deleteContextMenuItem.Enabled = true;
			pasteContextMenuItem.Enabled = true;
			selectAllContextMenuItem.Enabled = true;
		}


		private void contextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			undoContextMenuItem.Enabled = scintilla.CanUndo || scintilla.CanRedo;
			cutContextMenuItem.Enabled = scintilla.CanCut;
			copyContextMenuItem.Enabled = scintilla.CanCopy;
			deleteContextMenuItem.Enabled = scintilla.CanDelete;
			pasteContextMenuItem.Enabled = scintilla.CanPaste;
			selectAllContextMenuItem.Enabled = (scintilla.GetLength() != 0);
		}


		private void copyMenuItem_Click(object sender, EventArgs e)
		{
			scintilla.Copy();
		}


		private void cutMenuItem_Click(object sender, EventArgs e)
		{
			scintilla.Cut();
		}


		private void deleteMenuItem_Click(object sender, EventArgs e)
		{
			scintilla.Delete();
		}


		private void editMenuItem_DropDownClosed(object sender, EventArgs e)
		{
			undoMenuItem.Enabled = true;
			cutMenuItem.Enabled = true;
			copyMenuItem.Enabled = true;
			deleteMenuItem.Enabled = true;
			pasteMenuItem.Enabled = true;
		}


		private void editMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			undoMenuItem.Enabled = scintilla.CanUndo || scintilla.CanRedo;
			cutMenuItem.Enabled = scintilla.CanCut;
			copyMenuItem.Enabled = scintilla.CanCopy;
			deleteMenuItem.Enabled = scintilla.CanDelete;
			pasteMenuItem.Enabled = scintilla.CanPaste;
		}


		private void exitMenuItem_Click(object sender, EventArgs e)
		{
			// A chance to save is handled in the closing event
			Close();
		}


		private void findMenuItem_Click(object sender, EventArgs e)
		{
			if (!_replaceActive)
			{
				_findActive = true;
				findDialog.Show(this);
			}
		}


		private void findNextMenuItem_Click(object sender, EventArgs e)
		{

		}


		private void findReplaceDialog_Closed(object sender, EventArgs e)
		{
			_findActive = false;
			_replaceActive = false;
		}


		private void fontMenuItem_Click(object sender, EventArgs e)
		{
			fontDialog.Font = scintilla.Font;
			if (fontDialog.ShowDialog(this) == DialogResult.OK)
				scintilla.Font = fontDialog.Font;
		}


		private void goToMenuItem_Click(object sender, EventArgs e)
		{
			goToDialog.ShowDialog(this);
		}


		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			// Give the user a chance to save before exiting
			if (!CanCloseFile())
			{
				e.Cancel = true;
				return;
			}

			// Update the settings
			_settings.Set(Settings.MainFormBounds, (WindowState == FormWindowState.Normal ? Bounds : _normalBounds));
			_settings.Set(Settings.ViewStatusBar, statusBarMenuItem.Checked);
			_settings.Set(Settings.FormatWordWrap, wordWrapMenuItem.Checked);
			_settings.Set(Settings.FormatFont, scintilla.Font);
			_settings.Set(Settings.ViewZoomFactor, scintilla.ZoomFactor);

			// Save the settings
			try
			{
				_settings.Save();
			}
			catch
			{
				// Just swallow for now
			}
		}


		private void MainForm_Shown(object sender, EventArgs e)
		{
			// Open any file passed at startup
			if (_args != null && _args.Length > 0)
			{
				if (!OpenFile(_args[0], true))
					Close();
			}
		}


		private void NewFile()
		{
			_filePath = null;
			_fileTitle = DEFAULT_FILE_TITLE;
			scintilla.Clear();
			scintilla.Modified = false;
			scintilla.Scrolling.HorizontalScrollWidth = 1;
			UpdateCaption();
		}


		private void newMenuItem_Click(object sender, EventArgs e)
		{
			if (!CanCloseFile())
				return;

			NewFile();
		}


		private void openMenuItem_Click(object sender, EventArgs e)
		{
			if (!CanCloseFile())
				return;

			openFileDialog.FilterIndex = 1;
			openFileDialog.FileName = "*.txt";
			DialogResult dr = openFileDialog.ShowDialog(this);
			if (dr == DialogResult.OK)
				OpenFile(openFileDialog.FileName, false);
		}


		private bool OpenFile(string filePath, bool canCancel)
		{
			try
			{
				scintilla.Text = File.ReadAllText(filePath);
				_filePath = filePath;
				_fileTitle = Path.GetFileName(filePath);
				scintilla.Modified = false;
				UpdateCaption();
			}
			catch
			{
				string message = Util.Format(
					"There was an error opening the {0} file.{1}" +
					"Do you want to create a new file?",
					filePath,
					Environment.NewLine);

				DialogResult dr = MessageBox.Show(
					this,
					message,
					Util.AssemblyTitle,
					(canCancel ? MessageBoxButtons.YesNoCancel : MessageBoxButtons.YesNo),
					MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button1);

				if (dr == DialogResult.Cancel)
					return false;
				else if (dr == DialogResult.Yes)
					NewFile();
			}

			return true;
		}


		private void pasteMenuItem_Click(object sender, EventArgs e)
		{
			scintilla.Paste();
		}


		private void replaceMenuItem_Click(object sender, EventArgs e)
		{
			if (!_findActive)
			{
				_replaceActive = true;
				replaceDialog.Show(this);
			}
		}


		private void saveAsMenuItem_Click(object sender, EventArgs e)
		{
			SaveAs();
		}


		private bool Save()
		{
			if (_filePath == null)
				return SaveAs();
			
			return SaveFile(_filePath);
		}


		private bool SaveAs()
		{
			saveFileDialog.FilterIndex = 1;
			saveFileDialog.FileName = (_filePath == null ? "*.txt" : _filePath);
			DialogResult dr = saveFileDialog.ShowDialog(this);
			if (dr == DialogResult.OK)
				return SaveFile(saveFileDialog.FileName);

			return false;
		}


		private bool SaveFile(string filePath)
		{
			try
			{
				File.WriteAllText(filePath, scintilla.Text);
				_filePath = filePath;
				_fileTitle = Path.GetFileName(filePath);
				scintilla.Modified = false;
				UpdateCaption();
			}
			catch
			{
				string message = Util.Format(
					"There was an error saving the {0} file.",
					filePath);

				DialogResult dr = MessageBox.Show(
					this,
					message,
					Util.AssemblyTitle,
					MessageBoxButtons.RetryCancel,
					MessageBoxIcon.Warning,
					MessageBoxDefaultButton.Button1);

				if (dr == DialogResult.Retry)
					return SaveFile(filePath);

				return false;
			}

			return true;
		}


		private void saveMenuItem_Click(object sender, EventArgs e)
		{
			Save();
		}


		private void scintilla_ModifiedChanged(object sender, EventArgs e)
		{
			UpdateCaption();
		}


		private void scintilla_UpdateUI(object sender, EventArgs e)
		{
			// Update the status display
			int index = scintilla.GetByteIndex();
			int line = scintilla.GetLineFromByteIndex(index) + 1;
			int col = scintilla.GetColumnFromByteIndex(index) + 1;

			positionStatusStripLabel.Text = Util.Format("Ln {0}, Col {1}", line, col);
		}


		private void selectAllMenuItem_Click(object sender, EventArgs e)
		{
			scintilla.SelectAll();
		}


		private void SetStatusBarVisible()
		{
			if (wordWrapMenuItem.Checked)
			{
				statusStrip.Visible = false;
				statusBarMenuItem.Enabled = false;
			}
			else
			{
				statusBarMenuItem.Enabled = true;
				statusStrip.Visible = statusBarMenuItem.Checked;
			}
		}


		private void statusBarMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			SetStatusBarVisible();
		}


		private void UpdateCaption()
		{
			Text = Util.Format(
				"{0}{1} - {2}",
				(scintilla.Modified ? "*" : null),
				_fileTitle,
				Util.AssemblyTitle);
		}


		private void undoMenuItem_Click(object sender, EventArgs e)
		{
			if (scintilla.CanRedo)
				scintilla.Redo();
			else
				scintilla.Undo();
		}


		private void unitTestMenuItem_Click(object sender, EventArgs e)
		{
			using (UnitTestForm utf = new UnitTestForm())
				utf.ShowDialog(this);
		}


		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case NativeMethods.WM_SYSCOMMAND:
					if (m.WParam == (IntPtr)NativeMethods.SC_MAXIMIZE || m.WParam == (IntPtr)NativeMethods.SC_MINIMIZE)
					{
						// The window bounds stored in the settings file for the next
						// application start must be the bounds of a window in "normal"
						// window state, not "maximized" or "minimized" window states.
						// So we keep a copy of the most recent normal window state bounds.
						if(WindowState == FormWindowState.Normal)
							_normalBounds = Bounds;
					}
					break;
			}

			base.WndProc(ref m);
		}


		private void wordWrapMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			scintilla.Wrapping.Mode = (wordWrapMenuItem.Checked ? WrappingMode.Word : WrappingMode.None);
			SetStatusBarVisible();
		}

		#endregion Methods


		#region Constructors

		public MainForm()
		{
			InitializeComponent();

			// Load the settings
			string settingsFilePath = typeof(Program).Assembly.Location + ".ini";
			_settings = new Settings(settingsFilePath);
			try
			{
				_settings.Load();
			}
			catch (FileNotFoundException)
			{
				// This could happen the first time the application is run
				_settings.Reset();
			}
			catch
			{
				// Just swallow for now
			}


			// Apply settings
			Rectangle bounds = _settings.Get<Rectangle>(Settings.MainFormBounds);
			if (!bounds.IsEmpty)
			{
				StartPosition = FormStartPosition.Manual;
				Bounds = bounds;
			}

			statusBarMenuItem.Checked = _settings.Get<bool>(Settings.ViewStatusBar);
			wordWrapMenuItem.Checked = _settings.Get<bool>(Settings.FormatWordWrap);
			scintilla.Font = _settings.Get<Font>(Settings.FormatFont);
			SetStatusBarVisible();

			// Zoom
			scintilla.ZoomFactor = _settings.Get<int>(Settings.ViewZoomFactor);
			zoomInMenuItem.Click += (s, e) => scintilla.ZoomIn();
			zoomOutMenuItem.Click += (s, e) => scintilla.ZoomOut();
			zoomResetMenuItem.Click += (s, e) => scintilla.ZoomFactor = 0;


			// Strangely the VS Designer doesn't let you set the form icon
			// to an embedded resource and instead makes a copy in the RESX
			// file... yuck! So we'll just set it manually.
			Icon = Properties.Resources.ApplicationIcon;


			// Initialize
			UpdateCaption();
			aboutMenuItem.Text = Util.Format("About {0}", Util.AssemblyTitle);
			goToDialog.MessageCaption = Util.AssemblyTitle;
			findDialog.MessageCaption = Util.AssemblyTitle;
			scintilla.Select();
		}


		public MainForm(string[] args) : this()
		{
			_args = args;
		}

		#endregion Constructors
	}
}
