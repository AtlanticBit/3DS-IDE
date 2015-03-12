#region Using Directives

using System;
using System.Windows.Forms;

#endregion Using Directives


namespace ScintillaNet.Dialogs
{
	public abstract class FindReplaceDialog : CommonDialog
	{



		#region Fields

		private FindReplaceForm _dialog;

		#endregion Fields


		#region Methods

		private void Dialog_FormClosed(object sender, FormClosedEventArgs e)
		{
			_dialog.FormClosed -= new FormClosedEventHandler(Dialog_FormClosed);
			_dialog = null;
		}


		/// <summary>
		/// Shows the dialog with the specified owner to the user.
		/// </summary>
		/// <param name="owner">
		/// Any object that implements IWin32Window and represents the top-level window that will own this dialog.
		/// </param>
		/// <exception cref="ArgumentException">
		/// The form specified in the <paramref name="owner"/> parameter is the same as the form being shown.
		/// </exception>
		public virtual void Show(IWin32Window owner)
		{
			if (_dialog == null)
			{
				_dialog = new FindReplaceForm(this);
				_dialog.FormClosed += new FormClosedEventHandler(Dialog_FormClosed);
				_dialog.Show(owner);
			}
			else
			{
				_dialog.Activate();
			}
		}

		#endregion Methods
	}
}
