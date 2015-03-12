using System;
using System.ComponentModel;

using Forms = System.Windows.Forms;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class Scrolling : ScintillaWPFConfigItem
	{
		private bool? mHorizontalScrollTracking;
		/// <summary>
		/// Gets or sets whether the <see cref="Scintilla" /> control automatically increases the horizontal
		/// scroll width based on the text width.
		/// </summary>
		/// <returns>
		/// true if the <see cref="Scintilla" /> control automatically increases the horizontal scroll
		/// width based on the text width; otherwise, false. The default is true.
		/// </returns>
		[Category("Behavior")]
		[Description("Enables automatic scaling of the horizontal scroll width.")]
		[DefaultValue(true)]
		public bool HorizontalScrollTracking
		{
			get { return mHorizontalScrollTracking.Value; }
			set { mHorizontalScrollTracking = value; TryApplyConfig(); }
		}

		private int? mHorizontalScrollWidth;
		/// <summary>
		/// Gets or sets the number of pixels by which the <see cref="Scintilla" /> control can scroll horizontally.
		/// </summary>
		/// <returns>
		/// The number of pixels by which the <see cref="Scintilla" /> control can scroll horizontally.
		/// The default is 1.
		/// </returns>
		[Category("Appearance")]
		[Description("Sets the range this control can be scrolled horizontally.")]
		[DefaultValue(1)]
		public int HorizontalScrollWidth
		{
			get { return mHorizontalScrollWidth.Value; }
			set { mHorizontalScrollWidth = value; TryApplyConfig(); }
		}

		private Forms.ScrollBars? mScrollBars;
		/// <summary>
		/// Gets or sets which scroll bars should appear in a <see cref="Scintilla" /> control.
		/// </summary>
		/// <returns>
		/// One of the <see cref="ScrollBars" /> enumeration values that indicates whether
		/// a <see cref="Scintilla" /> control appears with no scroll bars, a horizontal scroll bar,
		///  a vertical scroll bar, or both. The default is <see cref="ScrollBars.Both" />.
		/// </returns>
		/// <exception cref="InvalidEnumArgumentException">The value assigned is not one of the <see cref="ScrollBars" /> values.</exception>
		[Category("Appearance")]
		[Description("Indicates which scroll bars will be shown for this control.")]
		[DefaultValue(typeof(Forms.ScrollBars), "Both")]
		public Forms.ScrollBars ScrollBars
		{
			get { return mScrollBars.Value; }
			set { mScrollBars = value; TryApplyConfig(); }
		}

		private bool? mScrollPastEnd;
		/// <summary>
		/// Gets or sets whether vertical scrolling is allowed past the last line of text
		/// in a <see cref="Scintilla" /> control
		/// </summary>
		/// <returns>
		/// true to allow vertical scrolling past the last line of text in a <see cref="Scintilla" /> control;
		/// otherwise, false. The default is false.
		/// </returns>
		[Category("Behavior")]
		[Description("Allows scrolling past the last line of text.")]
		[DefaultValue(false)]
		public bool ScrollPastEnd
		{
			get { return mScrollPastEnd.Value; }
			set { mScrollPastEnd = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (mHorizontalScrollTracking != null)
				scintilla.Scrolling.HorizontalScrollTracking = HorizontalScrollTracking;
			if (mHorizontalScrollWidth != null)
				scintilla.Scrolling.HorizontalScrollWidth = HorizontalScrollWidth;
			if (mScrollBars != null)
				scintilla.Scrolling.ScrollBars = ScrollBars;
			if (mScrollPastEnd != null)
				scintilla.Scrolling.ScrollPastEnd = ScrollPastEnd;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
