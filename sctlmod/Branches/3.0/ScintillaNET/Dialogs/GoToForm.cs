#region Using Directives

using System;
using System.Windows.Forms;
using Win32 = ScintillaNet.NativeMethods;

#endregion Using Directives


namespace ScintillaNet.Dialogs
{
	internal partial class GoToForm : Form
	{
		#region Fields

		private string _messageCaption;
		private Scintilla _scintilla;

		#endregion Fields


		#region Methods

		private void okButton_Click(object sender, EventArgs e)
		{
			int lineNumber;
			if (!int.TryParse(textBox.Text, out lineNumber) || lineNumber < 1 || lineNumber > _scintilla.LineCount)
			{
				MessageBox.Show(this, "Line number out of range.", _messageCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
				textBox.Select();
				return;
			}

			_scintilla.GoTo(lineNumber - 1);
			DialogResult = DialogResult.OK;
		}

		#endregion Methods


		#region Constructors

		public GoToForm()
		{
			InitializeComponent();
		}


		public GoToForm(GoToDialog dialog) : this()
		{
			_scintilla = dialog.Scintilla;
			_messageCaption = dialog.MessageCaption;

			// Accept only numeric input in the text box
			int styles = (int)Win32.GetWindowLong(textBox.Handle, Win32.GWL_STYLE);
			Win32.SetWindowLong(textBox.Handle, Win32.GWL_STYLE, (IntPtr)(styles |= Win32.ES_NUMBER));

			textBox.Text = (_scintilla.GetLine() + 1).ToString();
			textBox.Select();
		}

		#endregion Constructors
	}
}
