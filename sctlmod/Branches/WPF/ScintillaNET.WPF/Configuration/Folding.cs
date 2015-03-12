using System;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class Folding : ScintillaWPFConfigItem
	{
		private FoldFlag? mFlags;
		/// <summary>
		/// Read or change the Flags associated with a fold. The default value is 0.
		/// </summary>
		[Category("Appearance")]
		[Editor(typeof(ScintillaNET.Design.FlagEnumUIEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public FoldFlag Flags
		{
			get { return mFlags.Value; }
			set { mFlags = value; TryApplyConfig(); }
		}

		private bool? mIsFoldingEnabled;
		public bool IsFoldingEnabled
		{
			get { return mIsFoldingEnabled.Value; }
			set { mIsFoldingEnabled = value; TryApplyConfig(); }
		}

		private FoldMarkerScheme? mMarkerScheme;
		/// <summary>
		/// Read or change the Fold Marker Scheme. This changes the way Scintilla displays folds
		/// in the control. The default is BoxPlusMinus and the value Custom can be used to disable
		/// ScintillaNET changing selections made directly using MarkerCollection.FolderXX methods. 
		/// </summary>
		[DefaultValue(typeof(FoldMarkerScheme), "BoxPlusMinus")]
		public FoldMarkerScheme MarkerScheme
		{
			get { return mMarkerScheme.Value; }
			set { mMarkerScheme = value; TryApplyConfig(); }
		}

		private bool? mUseCompactFolding;
		/// <summary>
		/// Read or change the value controlling whether to use compact folding from the lexer.
		/// </summary>
		/// <remarks>This tracks the property "fold.compact"</remarks>
		public bool UseCompactFolding
		{
			get { return mUseCompactFolding.Value; }
			set { mUseCompactFolding = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (mFlags != null)
				scintilla.Folding.Flags = Flags;
			if (mIsFoldingEnabled != null)
				scintilla.Folding.IsEnabled = IsFoldingEnabled;
			if (mMarkerScheme != null)
				scintilla.Folding.MarkerScheme = MarkerScheme;
			if (mUseCompactFolding != null)
				scintilla.Folding.UseCompactFolding = UseCompactFolding;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
