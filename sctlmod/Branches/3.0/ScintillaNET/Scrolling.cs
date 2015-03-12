// TODO Implement SCI_SETXCARETPOLICY
// TODO Implement SCI_SETYCARETPOLICY
// TODO Implement SCI_SETVISIBLEPOLICY

#region Using Directives

using System;
using System.ComponentModel;
using System.Windows.Forms;
using ScintillaNet.Design;

#endregion Using Directives


namespace ScintillaNet
{
	/// <summary>
	/// Represents the scrolling options of a <see cref="Scintilla"/> control. 
	/// </summary>
	[TypeConverter(typeof(ScintillaExpandableObjectConverter))]
	public class Scrolling
	{
		#region Fields

		private Scintilla _scintilla;

		#endregion Fields


		#region Methods

		/// <summary>
		/// Raises the <see cref="PropertyChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			_scintilla.OnScrollingPropertyChanged(e);
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, e);
		}


		/// <summary>
		/// Scrolls the display by the specified number of lines and columns.
		/// </summary>
		/// <param name="lines">The number of display lines to scroll. Positive numbers scroll down, negative numbers scroll up.</param>
		/// <param name="columns">The number of columns to scroll. Positive numbers scroll right, negative numbers scroll left.</param>
		public virtual void Scroll(int lines, int columns)
		{
			// NOTE: We reverse the order of the params
			_scintilla.DirectMessage(Constants.SCI_LINESCROLL, (IntPtr)columns, (IntPtr)lines);
		}


		/// <summary>
		/// Scrolls the contents of the control to the current caret position.
		/// </summary>
		public virtual void ScrollToCaret()
		{
			_scintilla.DirectMessage(Constants.SCI_SCROLLCARET, IntPtr.Zero, IntPtr.Zero);
		}

		#endregion Methods


		#region Properties

		/// <summary>
		/// Gets or sets whether the <see cref="Scintilla"/> control automatically increases the horizontal
		/// scroll width based on the text width.
		/// </summary>
		/// <value>
		/// <c>true</c> is the <see cref="Scintilla"/> control automatically increases the horizontal scroll
		/// width based on the text width; otherwise, <c>false</c>. The default is <c>true</c>.
		/// </value>
		[DefaultValue(true), Description("Enables automatic scaling of the horizontal scroll width.")]
		[NotifyParentProperty(true)]
		public virtual bool HorizontalScrollTracking
		{
			get { return (_scintilla.DirectMessage(Constants.SCI_GETSCROLLWIDTHTRACKING, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero); }
			set
			{
				if (value != HorizontalScrollTracking)
				{
					_scintilla.DirectMessage(Constants.SCI_SETSCROLLWIDTHTRACKING, (value ? (IntPtr)1 : IntPtr.Zero), IntPtr.Zero);
					OnPropertyChanged(new PropertyChangedEventArgs("HorizontalScrollTracking"));
				}
			}
		}


		/// <summary>
		/// Gets or sets the number of pixels by which the <see cref="Scintilla"/> control is scrolled horizontally.
		/// </summary>
		/// <value>
		/// The number of pixels by which the <see cref="Scintilla"/> control is scrolled horizontally.
		/// The default is <c>0</c>.
		/// </value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int HorizontalScrollOffset
		{
			get { return (int)_scintilla.DirectMessage(Constants.SCI_GETXOFFSET, IntPtr.Zero, IntPtr.Zero); }
			set
			{
				if (value != HorizontalScrollOffset)
				{
					_scintilla.DirectMessage(Constants.SCI_SETXOFFSET, (IntPtr)value, IntPtr.Zero);
					OnPropertyChanged(new PropertyChangedEventArgs("HorizontalScrollOffset"));
				}
			}
		}


		/// <summary>
		/// Gets or sets the number of pixels by which the <see cref="Scintilla"/> control can scroll horizontally.
		/// </summary>
		/// <value>
		/// The number of pixels by which the <see cref="Scintilla"/> control can scroll horizontally.
		/// The default is <c>1</c>.
		/// </value>
		[DefaultValue(1), Description("Sets the rage this control can be scrolled horizontally.")]
		[NotifyParentProperty(true)]
		public virtual int HorizontalScrollWidth
		{
			get { return (int)_scintilla.DirectMessage(Constants.SCI_GETSCROLLWIDTH, IntPtr.Zero, IntPtr.Zero); }
			set
			{
				if (value != HorizontalScrollWidth)
				{
					_scintilla.DirectMessage(Constants.SCI_SETSCROLLWIDTH, (IntPtr)value, IntPtr.Zero);
					OnPropertyChanged(new PropertyChangedEventArgs("HorizontalScrollWidth"));
				}
			}
		}


		/// <summary>
		/// Gets or sets which scroll bars should appear in a <see cref="Scintilla"/> control.
		/// </summary>
		/// <value>
		/// One of the <see cref="ScrollBars"/> enumeration values that indicates whether
		/// a <see cref="Scintilla"/> control appears with no scroll bars, a horizontal scroll bar, 
		/// a vertical scroll bar, or both. The default is <c>ScrollBars.Both</c>.
		/// </value>
		/// <exception cref="InvalidEnumArgumentException">
		/// A value that is not within the range of valid values for the enumeration was
		/// assigned to the property.
		/// </exception>
		[DefaultValue(ScrollBars.Both), Description("Indicates which scroll bars will be shown for this control.")]
		[NotifyParentProperty(true)]
		public virtual ScrollBars ScrollBars
		{
			get
			{
				bool h = (_scintilla.DirectMessage(Constants.SCI_GETHSCROLLBAR, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero);
				bool v = (_scintilla.DirectMessage(Constants.SCI_GETVSCROLLBAR, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero);

				if (h && v)
					return ScrollBars.Both;
				else if (h)
					return ScrollBars.Horizontal;
				else if (v)
					return ScrollBars.Vertical;
				else
					return ScrollBars.None;
			}
			set
			{
				if (!Enum.IsDefined(typeof(ScrollBars), value))
					throw new InvalidEnumArgumentException("value", (int)value, typeof(ScrollBars));

				if (value != ScrollBars)
				{
					bool h = (value & ScrollBars.Horizontal) == ScrollBars.Horizontal;
					bool v = (value & ScrollBars.Vertical) == ScrollBars.Vertical;
					_scintilla.DirectMessage(Constants.SCI_SETHSCROLLBAR, (h ? (IntPtr)1 : IntPtr.Zero), IntPtr.Zero);
					_scintilla.DirectMessage(Constants.SCI_SETVSCROLLBAR, (v ? (IntPtr)1 : IntPtr.Zero), IntPtr.Zero);
					OnPropertyChanged(new PropertyChangedEventArgs("ScrollBars"));
				}
			}
		}


		/// <summary>
		/// Gets or sets whether vertical scrolling is allowed past the last line of text
		/// in a <see cref="Scintilla" /> control
		/// </summary>
		/// <value>
		/// <c>true</c> to allow vertical scrolling past the last line of text in a <see cref="Scintilla"/> control;
		/// otherwise, <c>false</c>. The default is <c>true</c>.
		/// </value>
		[DefaultValue(false), Description("Allows scrolling past the last line of text.")]
		[NotifyParentProperty(true)]
		public virtual bool ScrollPastEnd
		{
			get { return (_scintilla.DirectMessage(Constants.SCI_GETENDATLASTLINE, IntPtr.Zero, IntPtr.Zero) == IntPtr.Zero); }
			set
			{
				if (value != ScrollPastEnd)
				{
					_scintilla.DirectMessage(Constants.SCI_SETENDATLASTLINE, (value ? IntPtr.Zero : (IntPtr)1), IntPtr.Zero);
					OnPropertyChanged(new PropertyChangedEventArgs("ScrollPastEnd"));
				}
			}
		}

		#endregion Properties


		#region Events

		/// <summary>
		/// Event that is raised when a <see cref="Scrolling"/> property is changed.
		/// </summary>
		[Description("Occurs when one of the property values changes.")]
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion Events


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Scrolling"/> class
		/// for the given <see cref="Scintilla"/> control.
		/// </summary>
		/// <param name="scintilla">The <see cref="Scintilla"/> control that created this object.</param>
		/// <exception cref="ArgumentNullException"><paramref name="scintilla"/> is <c>null</c>.</exception>
		public Scrolling(Scintilla scintilla)
		{
			if (scintilla == null)
				throw new ArgumentNullException("scintilla");

			_scintilla = scintilla;
		}

		#endregion Constructors
	}
}
