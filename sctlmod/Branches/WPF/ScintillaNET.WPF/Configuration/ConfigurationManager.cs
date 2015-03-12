using System;
using System.ComponentModel;
using ScintillaNET.Configuration;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class ConfigurationManager : ScintillaWPFConfigItem
	{
		private bool? mClearIndicators;
		public bool ClearIndicators
		{
			get { return mClearIndicators.Value; }
			set { mClearIndicators = value; TryApplyConfig(); }
		}

		private bool? mClearKeyBindings;
		public bool ClearKeyBindings
		{
			get { return mClearKeyBindings.Value; }
			set { mClearKeyBindings = value; TryApplyConfig(); }
		}

		private bool? mClearMargins;
		public bool ClearMargins
		{
			get { return mClearMargins.Value; }
			set { mClearMargins = value; TryApplyConfig(); }
		}

		private bool? mClearMarkers;
		public bool ClearMarkers
		{
			get { return mClearMarkers.Value; }
			set { mClearMarkers = value; TryApplyConfig(); }
		}

		private bool? mClearSnippets;
		public bool ClearSnippets
		{
			get { return mClearSnippets.Value; }
			set { mClearSnippets = value; TryApplyConfig(); }
		}

		private bool? mClearStyles;
		public bool ClearStyles
		{
			get { return mClearStyles.Value; }
			set { mClearStyles = value; TryApplyConfig(); }
		}

		private string mCustomLocation;
		public string CustomLocation 
		{
			get { return mCustomLocation; }
			set { mCustomLocation = value; TryApplyConfig(); }
		}

		private bool? mIsBuiltInEnabled;
		public bool IsBuiltInEnabled
		{
			get { return mIsBuiltInEnabled.Value; }
			set { mIsBuiltInEnabled = value; TryApplyConfig(); }
		}

		private bool? mIsUserEnabled;
		public bool IsUserEnabled
		{
			get { return mIsUserEnabled.Value; }
			set { mIsUserEnabled = value; TryApplyConfig(); }
		}

		private string mLanguage;
		public string Language 
		{
			get { return mLanguage; }
			set { mLanguage = value; TryApplyConfig(); }
		}

		private ConfigurationLoadOrder? mLoadOrder;
		public ConfigurationLoadOrder LoadOrder
		{
			get { return mLoadOrder.Value; }
			set { mLoadOrder = value; TryApplyConfig(); }
		}

		private bool? mUseXmlReader;
		[DefaultValue(true)]
		public bool UseXmlReader
		{
			get { return mUseXmlReader.Value; }
			set { mUseXmlReader = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (this.mClearIndicators != null)
				scintilla.ConfigurationManager.ClearIndicators = ClearIndicators;
			if (this.mClearKeyBindings != null)
				scintilla.ConfigurationManager.ClearKeyBindings = ClearKeyBindings;
			if (this.mClearMargins != null)
				scintilla.ConfigurationManager.ClearMargins = ClearMargins;
			if (this.mClearMarkers != null)
				scintilla.ConfigurationManager.ClearMarkers = ClearMarkers;
			if (this.mClearSnippets != null)
				scintilla.ConfigurationManager.ClearSnippets = ClearSnippets;
			if (this.mClearStyles != null)
				scintilla.ConfigurationManager.ClearStyles = ClearStyles;
			if (this.CustomLocation != null)
				scintilla.ConfigurationManager.CustomLocation = CustomLocation;
			if (this.mIsBuiltInEnabled != null)
				scintilla.ConfigurationManager.IsBuiltInEnabled = IsBuiltInEnabled;
			if (this.mIsUserEnabled != null)
				scintilla.ConfigurationManager.IsUserEnabled = IsUserEnabled;
			if (this.mLoadOrder != null)
				scintilla.ConfigurationManager.LoadOrder = LoadOrder;
			if (this.mUseXmlReader != null)
				scintilla.ConfigurationManager.UseXmlReader = UseXmlReader;

			// Language is done last because it ends up
			// calling Configure, for which we want all
			// the other values already set.
			if (this.Language != null)
				scintilla.ConfigurationManager.Language = Language;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
