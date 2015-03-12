// Yes, it's true that Windows comes with its own find and replace
// dialogs, but I would rather make our own then have to do all that
// interop just to say we used the built-in one.

#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

#endregion Using Directives


namespace ScintillaNet.Dialogs
{
	internal partial class FindReplaceForm : Form
	{
		#region Fields

		private bool _findMode;
		private CommonDialog _dialog;
		private Scintilla _scintilla;
		private string _messageCaption;

		#endregion Fields


		#region Methods

		private void closeButton_Click(object sender, EventArgs e)
		{
			Close();
		}


		private void findComboBox_TextUpdate(object sender, EventArgs e)
		{
			bool enabled = findComboBox.Text.Length > 0;

			findNextButton.Enabled = enabled;
			replaceButton.Enabled = enabled;
			replaceAllButton.Enabled = enabled;
		}


		private void findNextButton_Click(object sender, EventArgs e)
		{
			Debug.Assert(findComboBox.Text.Length > 0);

			FindFlags flags = FindFlags.None;
			if (matchWordsCheckBox.Checked)
				flags |= FindFlags.WholeWord;
			if (matchCaseCheckBox.Checked)
				flags |= FindFlags.MatchCase;

			Range range;
			if (downRadioButton.Checked)
				range = _scintilla.FindNext(findComboBox.Text, flags);
			else
				range = _scintilla.FindPrevious(findComboBox.Text, flags);

			if (range.IsEmpty)
			{
				string message = Util.Format("Cannot find \"{0}\".", findComboBox.Text);
				MessageBox.Show(this, message, _messageCaption ?? String.Empty, MessageBoxButtons.OK, MessageBoxIcon.Information);
				findComboBox.Select();
			}
		}

		#endregion Methods


		#region Constructors

		public FindReplaceForm()
		{
			InitializeComponent();
		}


		public FindReplaceForm(CommonDialog dialog) : this()
		{
			Debug.Assert(dialog.Scintilla != null);

			_dialog = dialog;
			_scintilla = dialog.Scintilla;
			_messageCaption = dialog.MessageCaption;
			_findMode = dialog is FindDialog;
			Text = dialog.Title;

			if (_findMode)
			{
				// Change the display for finding
				replaceWithLabel.Visible = false;
				replaceComboBox.Visible = false;
				replaceButton.Visible = false;
				replaceAllButton.Visible = false;

				int height = replaceComboBox.Height;
				matchPanel.Anchor &= ~AnchorStyles.Bottom;
				matchPanel.Location = new Point(matchPanel.Location.X, matchPanel.Location.Y - height);
				directionGroupBox.Anchor &= ~AnchorStyles.Bottom;
				directionGroupBox.Location = new Point(directionGroupBox.Location.X, directionGroupBox.Location.Y - height);
				Height = Height - height;

				closeButton.Bounds = replaceButton.Bounds;
			}

			findComboBox.Select();
		}

		#endregion Constructors
	}
}
