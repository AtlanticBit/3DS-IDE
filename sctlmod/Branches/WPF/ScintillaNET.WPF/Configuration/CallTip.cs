using System;
using System.Drawing;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class CallTip : ScintillaWPFConfigItem
	{
		private Color? mBackColor;
		/// <summary>
		/// Gets/Sets the background color of all CallTips
		/// </summary>
		public Color BackColor 
		{
			get { return mBackColor.Value; }
			set { mBackColor = value; TryApplyConfig(); }
		}

		private Color? mForeColor;
		/// <summary>
		/// Gets/Sets Text color of all CallTips
		/// </summary>
		public Color ForeColor
		{
			get { return mForeColor.Value; }
			set { mForeColor = value; TryApplyConfig(); }
		}

		private int? mHighlightEnd;
		/// <summary>
		/// End position of the text to be highlighted in the CalTip
		/// </summary>
		public int HighlightEnd
		{
			get { return mHighlightEnd.Value; }
			set { mHighlightEnd = value; TryApplyConfig(); }
		}

		private int? mHighlightStart;
		/// <summary>
		/// Start position of the text to be highlighted in the CalTip
		/// </summary>
		public int HighlightStart
		{
			get { return mHighlightStart.Value; }
			set { mHighlightStart = value; TryApplyConfig(); }
		}

		private Color? mHighlightTextColor;
		/// <summary>
		/// Gets/Sets the Text Color of the portion of the CallTip that is highlighted
		/// </summary>
		public Color HighlightTextColor
		{
			get { return mHighlightTextColor.Value; }
			set { mHighlightTextColor = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (mBackColor != null)
				scintilla.CallTip.BackColor = BackColor;
			if (mForeColor != null)
				scintilla.CallTip.ForeColor = ForeColor;
			if (mHighlightEnd != null)
				scintilla.CallTip.HighlightEnd = HighlightEnd;
			if (mHighlightStart != null)
				scintilla.CallTip.HighlightStart = HighlightStart;
			if (mHighlightTextColor != null)
				scintilla.CallTip.HighlightTextColor = HighlightTextColor;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
