#region Using Directives

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

#endregion Using Directives


namespace ScintillaNet.Dialogs
{
	public class ReplaceDialog : FindReplaceDialog
	{
		#region Properties

		[DefaultValue("Replace")]
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
			base.Title = "Replace";
		}

		#endregion Methods
	}
}
