using System;
using System.Drawing;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class Selection : ScintillaWPFConfigItem
	{
		private Color? mBackColor;
		public Color BackColor
		{
			get { return mBackColor.Value; }
			set { mBackColor = value; TryApplyConfig(); }
		}

		private Color? mBackColorUnfocused;
		public Color BackColorUnfocused
		{
			get { return mBackColorUnfocused.Value; }
			set { mBackColorUnfocused = value; TryApplyConfig(); }
		}

		private Color? mForeColor;
		public Color ForeColor
		{
			get { return mForeColor.Value; }
			set { mForeColor = value; TryApplyConfig(); }
		}

		private Color? mForeColorUnfocused;
		public Color ForeColorUnfocused
		{
			get { return mForeColorUnfocused.Value; }
			set { mForeColorUnfocused = value; TryApplyConfig(); }
		}

		private bool? mHidden;
		public bool Hidden
		{
			get { return mHidden.Value; }
			set { mHidden = value; TryApplyConfig(); }
		}

		private bool? mHideSelection;
		public bool HideSelection
		{
			get { return mHideSelection.Value; }
			set { mHideSelection = value; TryApplyConfig(); }
		}

		private SelectionMode? mMode;
		public SelectionMode Mode
		{
			get { return mMode.Value; }
			set { mMode = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (mBackColor != null)
				scintilla.Selection.BackColor = BackColor;
			if (mBackColorUnfocused != null)
				scintilla.Selection.BackColorUnfocused = BackColorUnfocused;
			if (mForeColor != null)
				scintilla.Selection.ForeColor = ForeColor;
			if (mForeColorUnfocused != null)
				scintilla.Selection.ForeColorUnfocused = ForeColorUnfocused;
			if (mHidden != null)
				scintilla.Selection.Hidden = Hidden;
			if (mHideSelection != null)
				scintilla.Selection.HideSelection = HideSelection;
			if (mMode != null)
				scintilla.Selection.Mode = Mode;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}

	}
}
