using System;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class DocumentNavigation : ScintillaWPFConfigItem
	{
		private bool? mIsDocumentNavigationEnabled;
		/// <summary>
		/// Gets/Sets whether Document Navigation is tracked. Defaults to true.
		/// </summary>
		[DefaultValue(true)]
		public bool IsDocumentNavigationEnabled
		{
			get { return mIsDocumentNavigationEnabled.Value; }
			set { mIsDocumentNavigationEnabled = value; TryApplyConfig(); }
		}

		private int? mMaxHistorySize;
		/// <summary>
		/// Maximum number of places the document navigation remembers. Defaults to 50.
		/// </summary>
		/// <remarks>
		/// When the max value is reached the oldest entries are removed.
		/// </remarks>
		[DefaultValue(50)]
		public int MaxHistorySize
		{
			get { return mMaxHistorySize.Value; }
			set { mMaxHistorySize = value; TryApplyConfig(); }
		}

		private int? mNavigationPointTimeout;
		/// <summary>
		/// Time in milliseconds to wait before a Navigation Point is set. Default is 200
		/// </summary>
		/// <remarks>
		/// In text editing, the current caret position is constantly changing. Rather than capture every
		/// change in position, ScintillaNET captures the current position [NavigationPointTimeout]ms after a 
		/// position changes, only then is it eligible for another snapshot
		/// </remarks>
		[DefaultValue(200)]
		public int NavigationPointTimeout
		{
			get { return mNavigationPointTimeout.Value; }
			set { mNavigationPointTimeout = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (this.mIsDocumentNavigationEnabled != null)
				scintilla.DocumentNavigation.IsEnabled = IsDocumentNavigationEnabled;
			if (this.mMaxHistorySize != null)
				scintilla.DocumentNavigation.MaxHistorySize = MaxHistorySize;
			if (this.mNavigationPointTimeout != null)
				scintilla.DocumentNavigation.NavigationPointTimeout = NavigationPointTimeout;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
			scintilla.DocumentNavigation.Reset();
		}
	}
}
