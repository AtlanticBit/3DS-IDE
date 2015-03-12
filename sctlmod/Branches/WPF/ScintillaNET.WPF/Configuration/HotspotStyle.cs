using System;
using System.Drawing;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class HotspotStyle : ScintillaWPFConfigItem
	{
		private Color? mActiveBackColor;
		public Color ActiveBackColor
		{
			get { return mActiveBackColor.Value; }
			set { mActiveBackColor = value; TryApplyConfig(); }
		}

		private Color? mActiveForeColor;
		public Color ActiveForeColor
		{
			get { return mActiveForeColor.Value; }
			set { mActiveForeColor = value; TryApplyConfig(); }
		}

		private bool? mActiveUnderline;
		public bool ActiveUnderline
		{
			get { return mActiveUnderline.Value; }
			set { mActiveUnderline = value; TryApplyConfig(); }
		}

		private bool? mSingleLine;
		public bool SingleLine
		{
			get { return mSingleLine.Value; }
			set { mSingleLine = value; TryApplyConfig(); }
		}

		private bool? mUseActiveBackColor;
		public bool UseActiveBackColor
		{
			get { return mUseActiveBackColor.Value; }
			set { mUseActiveBackColor = value; TryApplyConfig(); }
		}

		private bool? mUseActiveForeColor;
		public bool UseActiveForeColor
		{
			get { return mUseActiveForeColor.Value; }
			set { mUseActiveForeColor = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (mActiveBackColor != null)
				scintilla.HotspotStyle.ActiveBackColor = ActiveBackColor;
			if (mActiveForeColor != null)
				scintilla.HotspotStyle.ActiveForeColor = ActiveForeColor;
			if (mActiveUnderline != null)
				scintilla.HotspotStyle.ActiveUnderline = ActiveUnderline;
			if (mSingleLine != null)
				scintilla.HotspotStyle.SingleLine = SingleLine;
			if (mUseActiveBackColor != null)
				scintilla.HotspotStyle.UseActiveBackColor = UseActiveBackColor;
			if (mUseActiveForeColor != null)
				scintilla.HotspotStyle.UseActiveForeColor = UseActiveForeColor;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
