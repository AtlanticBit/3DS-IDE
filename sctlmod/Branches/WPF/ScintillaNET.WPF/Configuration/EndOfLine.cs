using System;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class EndOfLine : ScintillaWPFConfigItem
	{
		private bool? mIsEndOfLineVisible;
		/// <summary>
		/// Gets/Sets if End of line markers are visible in the Scintilla control.
		/// </summary>
		public bool IsEndOfLineVisible
		{
			get { return mIsEndOfLineVisible.Value; }
			set { mIsEndOfLineVisible = value; TryApplyConfig(); }
		}

		private EndOfLineMode? mMode;
		/// <summary>
		/// Gets/Sets the <see cref="EndOfLineMode"/> for the document. Default is CrLf.
		/// </summary>
		/// <remarks>
		/// Changing this value does NOT change all EOL marks in a currently-loaded document.
		/// To do this, use <see cref="ConvertAllLines"/>ConvertAllLines.
		/// </remarks>
		public EndOfLineMode Mode
		{
			get { return mMode.Value; }
			set { mMode = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (this.mIsEndOfLineVisible != null)
				scintilla.EndOfLine.IsVisible = IsEndOfLineVisible;
			if (this.mMode != null)
				scintilla.EndOfLine.Mode = Mode;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
