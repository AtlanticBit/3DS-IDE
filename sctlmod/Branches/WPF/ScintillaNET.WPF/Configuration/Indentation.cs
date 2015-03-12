using System;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class Indentation : ScintillaWPFConfigItem
	{
		private bool? mBackspaceUnindents;
		public bool BackspaceUnindents
		{
			get { return mBackspaceUnindents.Value; }
			set { mBackspaceUnindents = value; TryApplyConfig(); }
		}

		private int? mIndentWidth;
		public int IndentWidth
		{
			get { return mIndentWidth.Value; }
			set { mIndentWidth = value; TryApplyConfig(); }
		}

		private bool? mShowGuides;
		public bool ShowGuides
		{
			get { return mShowGuides.Value; }
			set { mShowGuides = value; TryApplyConfig(); }
		}

		private SmartIndent? mSmartIndentType;
		public SmartIndent SmartIndentType
		{
			get { return mSmartIndentType.Value; }
			set { mSmartIndentType = value; TryApplyConfig(); }
		}

		private bool? mTabIndents;
		public bool TabIndents
		{
			get { return mTabIndents.Value; }
			set { mTabIndents = value; TryApplyConfig(); }
		}

		private int? mTabWidth;
		public int TabWidth
		{
			get { return mTabWidth.Value; }
			set { mTabWidth = value; TryApplyConfig(); }
		}

		private bool? mUseTabs;
		public bool UseTabs
		{
			get { return mUseTabs.Value; }
			set { mUseTabs = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (mBackspaceUnindents != null)
				scintilla.Indentation.BackspaceUnindents = BackspaceUnindents;
			if (mIndentWidth != null)
				scintilla.Indentation.IndentWidth = IndentWidth;
			if (mShowGuides != null)
				scintilla.Indentation.ShowGuides = ShowGuides;
			if (mSmartIndentType != null)
				scintilla.Indentation.SmartIndentType = SmartIndentType;
			if (mTabIndents != null)
				scintilla.Indentation.TabIndents = TabIndents;
			if (mTabWidth != null)
				scintilla.Indentation.TabWidth = TabWidth;
			if (mUseTabs != null)
				scintilla.Indentation.UseTabs = UseTabs;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}

	}
}
