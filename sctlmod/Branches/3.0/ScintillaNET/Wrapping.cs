// TODO Support SCI_SETWRAPINDENTMODE
// TODO Support SCI_GETWRAPINDENTMODE

#region Using Directives

using System;
using System.ComponentModel;
using ScintillaNet.Design;

#endregion Using Directives


namespace ScintillaNet
{
	/// <summary>
	/// Represents the text wrapping options of a <see cref="Scintilla"/> control. 
	/// </summary>
	[TypeConverter(typeof(ScintillaExpandableObjectConverter))]
	public class Wrapping
	{
		#region Fields

		private Scintilla _scintilla;

		#endregion Fields


		#region Methods

		/// <summary>
		/// The number of lines displayed when a line of text is wrapped.
		/// </summary>
		/// <param name="lineIndex">The zero-based index of the line to count.</param>
		/// <returns>The numbers of display lines the line of text occupies.</returns>
		public virtual int CountDisplayLines(int lineIndex)
		{
			// TODO Some range checking?
			return (int)_scintilla.DirectMessage(Constants.SCI_WRAPCOUNT, (IntPtr)lineIndex, IntPtr.Zero);
		}


		/// <summary>
		/// Raises the <see cref="PropertyChanged"/> event.
		/// </summary>
		/// <param name="e">A <see cref="PropertyChangedEventArgs"/> that contains the event data.</param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			_scintilla.OnWrappingPropertyChanged(e);
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, e);
		}


		/// <summary>
		/// Forces the line range specified to wrap at the given pixel width. This operates independently
		/// of the current <see cref="Scintilla"/> wrapping <see cref="Mode"/> property.
		/// </summary>
		/// <param name="startLine">The zero-based line index to start wrapping.</param>
		/// <param name="endLine">The zero-based line index to stop wrapping.</param>
		/// <param name="width">
		/// The maximum width in pixels of the lines to wrap. A value of zero resets forced line wrapping.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="startLine"/> and <paramref name="endLine"/> do not specify a valid
		/// range of lines within the document.
		/// </exception>
		/// <dev>I question the usefulness of this functionality.</dev>
		public virtual void WrapLines(int startLine, int endLine, int width)
		{
			if (startLine < 0)
				throw new ArgumentOutOfRangeException("startLine", "The start line must be greater than or equal to zero.");
			if (endLine < startLine)
				throw new ArgumentOutOfRangeException("endLine", "The start line and end line must specify a valid range.");

			// Convert line indexes to positions within the line
			int startPos = (int)_scintilla.DirectMessage(Constants.SCI_POSITIONFROMLINE, (IntPtr)startLine, IntPtr.Zero);
			int endPos  = (int)_scintilla.DirectMessage(Constants.SCI_POSITIONFROMLINE, (IntPtr)endLine, IntPtr.Zero);

			if (startPos == -1)
				throw new ArgumentOutOfRangeException("startLine", "The start line specify a valid line within the document.");
			if (endPos == -1)
				throw new ArgumentOutOfRangeException("endLine", "The end line must specify a valid line within the document.");

			// Set the target positions (which Scintilla will convert back to line indexes)
			_scintilla.DirectMessage(Constants.SCI_SETTARGETSTART, (IntPtr)startPos, IntPtr.Zero);
			_scintilla.DirectMessage(Constants.SCI_SETTARGETEND, (IntPtr)endPos, IntPtr.Zero);

			_scintilla.DirectMessage(Constants.SCI_LINESSPLIT, (IntPtr)width, IntPtr.Zero);
		}

		#endregion Methods


		#region Properties

		/// <summary>
		/// Gets or sets the number of columns to indent wrapped lines of text in
		/// a <see cref="Scintilla"/> control.
		/// </summary>
		/// <value>
		/// The number of columns by which a <see cref="Scintilla"/> control indents
		/// wrapped lines of text. The default is <c>0</c>.
		/// </value>
		[DefaultValue(0), Description("The number of columns to indent wrapped text.")]
		[NotifyParentProperty(true)]
		public virtual int Indent
		{
			get { return (int)_scintilla.DirectMessage(Constants.SCI_GETWRAPSTARTINDENT, IntPtr.Zero, IntPtr.Zero); }
			set
			{
				if (value != Indent)
				{
					// TODO Some range checking?
					_scintilla.DirectMessage(Constants.SCI_SETWRAPSTARTINDENT, (IntPtr)value, IntPtr.Zero);
					OnPropertyChanged(new PropertyChangedEventArgs("Indent"));
				}
			}
		}


		/// <summary>
		/// Gets or sets the visual indicator applied to wrapped lines of text
		/// in a <see cref="Scintilla"/> control.
		/// </summary>
		/// <value>One of the <see cref="WrappingIndicator"/> enumeration values. The default is <c>None</c>.</value>
		/// <exception cref="InvalidEnumArgumentException">
		/// A value that is not within the range of valid values for the enumeration was
		/// assigned to the property.
		/// </exception>
		[DefaultValue(WrappingIndicator.None), Description("Visual indicator applied to wrapped lines of text in the control.")]
		[NotifyParentProperty(true)]
		public virtual WrappingIndicator Indicator
		{
			get
			{
				int visualFlag = (int)_scintilla.DirectMessage(Constants.SCI_GETWRAPVISUALFLAGS, IntPtr.Zero, IntPtr.Zero);
				int visualFlagLoc = (int)_scintilla.DirectMessage(Constants.SCI_GETWRAPVISUALFLAGSLOCATION, IntPtr.Zero, IntPtr.Zero);
				return (WrappingIndicator)(visualFlag | (visualFlagLoc << 16));
			}
			set
			{
				if (!Enum.IsDefined(typeof(WrappingIndicator), value))
					throw new InvalidEnumArgumentException("value", (int)value, typeof(WrappingIndicator));

				if (value != Indicator)
				{
					//
					// See the notes on the WrappingIndicator enum
					//

					int visualFlag = ((int)value & 0xFFFF);
					int visualFlagLoc = ((int)value >> 16);

					_scintilla.DirectMessage(Constants.SCI_SETWRAPVISUALFLAGS, (IntPtr)visualFlag, IntPtr.Zero);
					_scintilla.DirectMessage(Constants.SCI_SETWRAPVISUALFLAGSLOCATION, (IntPtr)visualFlagLoc, IntPtr.Zero);

					OnPropertyChanged(new PropertyChangedEventArgs("Indicator"));
				}
			}
		}


		/// <summary>
		/// Gets or sets the text wrapping behavior of a <see cref="Scintilla"/> control.
		/// </summary>
		/// <value>
		/// One of the <see cref="WrappingMode"/> enumeration values. The default is <c>None</c>.
		/// </value>
		/// <exception cref="InvalidEnumArgumentException">
		/// A value that is not within the range of valid values for the enumeration was
		/// assigned to the property.
		/// </exception>
		[DefaultValue(WrappingMode.None), Description("Indicates text wrapping behavior for this control.")]
		[NotifyParentProperty(true)]
		public virtual WrappingMode Mode
		{
			get { return (WrappingMode)_scintilla.DirectMessage(Constants.SCI_GETWRAPMODE, IntPtr.Zero, IntPtr.Zero); }
			set
			{
				if (!Enum.IsDefined(typeof(WrappingMode), value))
					throw new InvalidEnumArgumentException("value", (int)value, typeof(WrappingMode));

				if (value != Mode)
				{
					_scintilla.DirectMessage(Constants.SCI_SETWRAPMODE, (IntPtr)value, IntPtr.Zero);
					OnPropertyChanged(new PropertyChangedEventArgs("Mode"));
				}
			}
		}

		#endregion Properties


		#region Events

		/// <summary>
		/// Event that is raised when a <see cref="Wrapping"/> property is changed.
		/// </summary>
		[Description("Occurs when one of the property values changes.")]
		public event PropertyChangedEventHandler PropertyChanged;

		#endregion Events


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Wrapping"/> class
		/// for the given <see cref="Scintilla"/> control.
		/// </summary>
		/// <param name="scintilla">The <see cref="Scintilla"/> control that created this object.</param>
		/// <exception cref="ArgumentNullException"><paramref name="scintilla"/> is <c>null</c>.</exception>
		public Wrapping(Scintilla scintilla)
		{
			if (scintilla == null)
				throw new ArgumentNullException("scintilla");

			_scintilla = scintilla;
		}

		#endregion Constructors
	}
}
