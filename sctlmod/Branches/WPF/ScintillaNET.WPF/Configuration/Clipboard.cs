using System;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class Clipboard : ScintillaWPFConfigItem
	{
		private bool? mConvertLineBreaksOnPaste;
		/// <summary>
		/// Gets or sets whether pasted line break characters are converted to match the document's end-of-line mode.
		/// </summary>
		/// <returns>
		/// true if line break characters are converted; otherwise, false.
		/// The default is true.
		/// </returns>
		[Category("Behavior")]
		[Description("Indicates whether line breaks are converted to match the document's end-of-line mode when pasted.")]
		[DefaultValue(true)]
		public bool ConvertLineBreaksOnPaste
		{
			get { return mConvertLineBreaksOnPaste.Value; }
			set { mConvertLineBreaksOnPaste = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (mConvertLineBreaksOnPaste != null)
				scintilla.Clipboard.ConvertLineBreaksOnPaste = ConvertLineBreaksOnPaste;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
