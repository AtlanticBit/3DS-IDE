using System;
using System.ComponentModel;
using System.Drawing.Design;
using ScintillaNET.Design;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class LineWrapping : ScintillaWPFConfigItem
	{
		private LineWrappingIndentMode? mIndentMode;
		/// <summary>
		/// Gets or sets how wrapped lines are indented.
		/// </summary>
		/// <returns>
		/// One of the <see cref="LineWrappingIndentMode" /> values.
		/// The default is <see cref="LineWrappingIndentMode.Fixed" />.
		/// </returns>
		/// <exception cref="InvalidEnumArgumentException">The value assigned is not one of the <see cref="LineWrappingIndentMode" /> values.</exception>
		// NEED CAT
		[Description("Indicates how wrapped lines are indented.")]
		[DefaultValue(typeof(LineWrappingIndentMode), "Fixed")]
		public LineWrappingIndentMode IndentMode
		{
			get { return mIndentMode.Value; }
			set { mIndentMode = value; TryApplyConfig(); }
		}

		private int? mIndentSize;
		/// <summary>
		/// Gets or sets the size that wrapped lines are indented when <see cref="IndentMode" /> is <see cref="LineWrappingIndentMode.Fixed" />.
		/// </summary>
		/// <returns>An <see cref="Int32" /> representing the size (in characters) that wrapped lines are indented.</returns>
		/// <exception cref="ArgumentOutOfRangeException">The value is less that zero or greater than 256.</exception>
		// NEED CAT
		[Description("The indentation size of wrapped lines.")]
		[DefaultValue(0)]
		public int IndentSize
		{
			get { return mIndentSize.Value; }
			set { mIndentSize = value; TryApplyConfig(); }
		}

		private LineWrappingMode? mMode;
		/// <summary>
		/// Gets or sets how and whether line wrapping is performed.
		/// </summary>
		/// <returns>
		/// One of the <see cref="LineWrappingMode" /> values.
		/// The default is <see cref="LineWrappingMode.None" />.
		/// </returns>
		/// <exception cref="InvalidEnumArgumentException">The value assigned is not one of the <see cref="LineWrappingMode" /> values.</exception>
		// NEED CAT
		[Description("Specifies how and whether line wrapping is performed.")]
		[DefaultValue(typeof(LineWrappingMode), "None")]
		public LineWrappingMode Mode
		{
			get { return mMode.Value; }
			set { mMode = value; TryApplyConfig(); }
		}

		private LineWrappingVisualFlags? mVisualFlags;
		/// <summary>
		/// Gets or sets the visual glyphs displayed on wrapped lines.
		/// </summary>
		/// <returns>
		/// A bitwise combination of the <see cref="LineWrappingVisualFlags" /> values.
		/// The default is <see cref="LineWrappingVisualFlags.None" />.
		/// </returns>
		// NEED CAT
		[Description("Specifies the visual indicators on wrapped lines.")] // TODO Move to resource file.
		[Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
		[DefaultValue(typeof(LineWrappingVisualFlags), "None")]
		public LineWrappingVisualFlags VisualFlags
		{
			get { return mVisualFlags.Value; }
			set { mVisualFlags = value; TryApplyConfig(); }
		}

		private LineWrappingVisualFlagsLocations? mVisualFlagsLocations;
		/// <summary>
		/// Gets or sets the location of visual glyphs displayed on wrapped lines.
		/// </summary>
		/// <returns>
		/// A bitwise combination of the <see cref="LineWrappingVisualFlagsLocations" /> values.
		/// The default is <see cref="LineWrappingVisualFlagsLocations.Default" />.
		/// </returns>
		// NEED CAT
		[Description("Specifies the location of visual indicators on wrapped lines.")]
		[Editor(typeof(FlagEnumUIEditor), typeof(UITypeEditor))]
		[DefaultValue(typeof(LineWrappingVisualFlagsLocations), "Default")]
		public LineWrappingVisualFlagsLocations VisualFlagsLocations
		{
			get { return mVisualFlagsLocations.Value; }
			set { mVisualFlagsLocations = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (this.mIndentMode != null)
				scintilla.LineWrapping.IndentMode = IndentMode;
			if (this.mIndentSize != null)
				scintilla.LineWrapping.IndentSize = IndentSize;
			if (this.mMode != null)
				scintilla.LineWrapping.Mode = Mode;
			if (this.mVisualFlags != null)
				scintilla.LineWrapping.VisualFlags = VisualFlags;
			if (this.mVisualFlagsLocations != null)
				scintilla.LineWrapping.VisualFlagsLocations = VisualFlagsLocations;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
