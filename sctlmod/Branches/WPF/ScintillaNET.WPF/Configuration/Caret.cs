using System;
using System.Drawing;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class Caret : ScintillaWPFConfigItem
	{
		private int? mBlinkRate;
		/// <summary>
		/// Gets/Sets the time interval in milliseconds that the caret should blink.
		/// </summary>
		/// <remarks>
		/// This defaults to the system default value.
		/// </remarks>
		[Category("Behavior")]
		[Description("Gets/Sets the time interval in milliseconds that the caret should blink.")]
		public int BlinkRate
		{
			get { return mBlinkRate.Value; }
			set { mBlinkRate = value; TryApplyConfig(); }
		}

		private static readonly Color DefaultColor = Color.Black;
		private Color? mColor;
		/// <summary>
		/// Gets/Sets the color of the Caret.
		/// </summary>
		/// <remarks>Defaults to black</remarks>
		[Category("Appearance")]
		[Description("The color of the Caret.")]
		//[DefaultValue(typeof(Color), "Black")]
		public Color Color
		{
			get { return (mColor ?? (Color?)DefaultColor).Value; }
			set { mColor = value; TryApplyConfig(); }
		}


		private Color? mCurrentLineBackgroundColor;
		/// <summary>
		/// Gets/Sets the color of the document line where the caret currently resides
		/// </summary>
		/// <remarks>
		/// The <see cref="HighlightCurrentList"/> property must be set to true in order
		/// for this to to take effect.
		/// </remarks>
		[Category("Appearance")]
		[Description("The color of the document line where the caret currently resides.")]
		[DefaultValue(typeof(Color), "Yellow")]
		public Color CurrentLineBackgroundColor
		{
			get { return mCurrentLineBackgroundColor.Value; }
			set { mCurrentLineBackgroundColor = value; TryApplyConfig(); }
		}

		private bool? mHighlightCurrentLine;
		/// <summary>
		/// Gets/Sets if the current document line where the caret resides is highlighted.
		/// </summary>
		/// <remarks>
		/// <see cref="CurrentLineBackgroundColor"/> determines the color. 
		/// </remarks>
		public bool HighlightCurrentLine
		{
			get { return mHighlightCurrentLine.Value; }
			set { mHighlightCurrentLine = value; TryApplyConfig(); }
		}

		private bool? mIsSticky;
		/// <summary>
		/// Controls when the last position of the caret on the line is saved. When set to 
		/// <c>true</c>, the position is not saved when you type a character, a tab, paste the 
		/// clipboard content or press backspace
		/// </summary>
		/// <remarks>
		/// Defaults to <c>false</c>
		/// </remarks>
		[DefaultValue(false)]
		public bool IsSticky
		{
			get { return mIsSticky.Value; }
			set { mIsSticky = value; TryApplyConfig(); }
		}

		private CaretStyle? mStyle;
		/// <summary>
		/// Gets/Sets the <see cref="CaretStyle"/> displayed.
		/// </summary>
		[DefaultValue(typeof(CaretStyle), "Line")]
		public CaretStyle Style
		{
			get { return mStyle.Value; }
			set { mStyle = value; TryApplyConfig(); }
		}

		private int? mWidth;
		/// <summary>
		/// Gets/Sets the width in pixels of the Caret
		/// </summary>
		/// <remarks>
		/// This defaults to the system default.
		/// </remarks>
		public int Width
		{
			get { return mWidth.Value; }
			set { mWidth = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (this.mBlinkRate != null)
				scintilla.Caret.BlinkRate = BlinkRate;
			if (this.mColor != null)
				scintilla.Caret.Color = Color;
			if (this.mCurrentLineBackgroundColor != null)
			{
				if (CurrentLineBackgroundColor.A != 255)
					scintilla.Caret.CurrentLineBackgroundAlpha = CurrentLineBackgroundColor.A;
				// We have to get rid of the alpha channel information
				// for the value we set here.
				scintilla.Caret.CurrentLineBackgroundColor = Color.FromArgb(CurrentLineBackgroundColor.R, CurrentLineBackgroundColor.G, CurrentLineBackgroundColor.B);
			}
			if (this.mHighlightCurrentLine != null)
				scintilla.Caret.HighlightCurrentLine = HighlightCurrentLine;
			if (this.mIsSticky != null)
				scintilla.Caret.IsSticky = IsSticky;
			if (this.mStyle != null)
				scintilla.Caret.Style = Style;
			if (this.mWidth != null)
				scintilla.Caret.Width = Width;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
