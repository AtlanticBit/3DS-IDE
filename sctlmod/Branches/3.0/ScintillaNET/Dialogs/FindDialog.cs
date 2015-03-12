#region Using Directives

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

#endregion Using Directives


namespace ScintillaNet.Dialogs
{
	/// <summary>
	/// Lets users search a <see cref="Scintilla"/> control for matching text
	/// from a Windows Forms application.
	/// </summary>
	[Description("Displays a modeless dialog box that enables a user to search for text in a Scintilla control.")]
	public class FindDialog : FindReplaceDialog
	{
		#region Properties

		[DefaultValue("Find")]
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


		#region Methods

		protected override void Initialize()
		{
			base.Title = "Find";
		}

		#endregion Methods
	}
}
