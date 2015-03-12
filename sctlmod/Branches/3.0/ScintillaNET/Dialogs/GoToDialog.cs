#region Using Directives

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

#endregion Using Directives


namespace ScintillaNet.Dialogs
{
	/// <summary>
	/// Enables users to jump to a specified line number in a <see cref="Scintilla" /> control.
	/// This class cannot be inherited
	/// </summary>
	[ToolboxBitmap(typeof(Scintilla), "Resources.GoToDialogBitmap.bmp")]
	[Description("Displays a modal dialog box that enables a user to navigate to the specified line number in a Scintilla control.")]
	public class GoToDialog : CommonDialog
	{
		#region Methods

		protected override void Initialize()
		{
			base.Title = "Go To Line";
		}

		public DialogResult ShowDialog()
		{
			return ShowDialog(null);
		}


		public virtual DialogResult ShowDialog(IWin32Window owner)
		{
			using (GoToForm gtf = new GoToForm(this))
				return gtf.ShowDialog(owner);
		}

		#endregion Methods


		#region Properties

		[DefaultValue("Go To Line")]
		public override string Title
		{
			get
			{
				return base.Title;
			}
			set
			{
				base.Title = value;
			}
		}

		#endregion Properties
	}
}
