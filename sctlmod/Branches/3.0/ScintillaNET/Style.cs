#region Using Directives

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using ScintillaNet.Design;
using System.Text;
using System.Diagnostics;
using ScintillaNet.Properties;

#endregion Using Directives


namespace ScintillaNet
{
	/// <summary>
	/// Represents the visual appearance of elements and text in a <see cref="Scintilla"/> control.
	/// </summary>
	/// <dev>
	/// One of my major goals when rewriting styles was to allow style properties to be
	/// set without first associating it with a Scintilla control. This gives developers more
	/// freedom and allows us to do some of the things we do in lexers for example.
	/// </dev>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public partial class Style : ICloneable
	{
		#region Constants

		private const string DEFAULT_FONT_NAME = "Verdana";
		private const int DEFAULT_FONT_SIZE = 8;

		#endregion Constants


		#region Fields

		private static Font _defaultFont;

		private Orphan _orphan;
		private Scintilla _scintilla;
		private int _index;

		#endregion Fields


		#region Methods

		/// <summary>
		/// Creates an exact copy of this <see cref="Style"/>.
		/// </summary>
		/// <returns>A <see cref="Style"/> that represents an exact copy of this style.</returns>
		public object Clone()
		{
			return new Style(this);
		}


		private string CreatePropertyKey(string name)
		{
			Debug.Assert(_index >= 0);
			return "Style" + _index + "." + name;
		}


		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
				handler(this, e);
		}


		internal void SetOwner(Scintilla owner, int index)
		{
			Debug.Assert(_orphan != null);

			Orphan orphan = _orphan;
			_orphan = null;

			// Make all the orphan properties real
			_scintilla = owner;
			_index = index;
			BackColor = orphan.BackColor;
			ForeColor = orphan.ForeColor;
			Font = orphan.Font;
			Case = orphan.Case;
			Link = orphan.Link;
			FillLine = orphan.FillLine;
			ReadOnly = orphan.ReadOnly;
			Visible = orphan.Visible;
		}



		private bool ShouldSerializeFont()
		{
			return Font != DefaultFont;
		}


		/// <summary>
		/// Resets all the style properties to default values.
		/// </summary>
		public virtual void Reset()
		{
			BackColor = Color.Empty;
			ForeColor = Color.Empty;
			Font = null;
			Case = StyleCase.Mixed;
			FillLine = false;
			Link = false;
			ReadOnly = false;
			Visible = true;
		}


		private void ResetFont()
		{
			Font = null;
		}


		public override string ToString()
		{
			return "Style" + _index.ToString();
		}

		#endregion Methods


		#region Properties

		/// <summary>
		/// Gets or sets the background color for the style.
		/// </summary>
		/// <value>
		/// A <see cref="Color"/> that represents the background color of the style.
		/// The default is the value of the <see cref="DefaultBackColor"/> property.
		/// </value>
		/// <exception cref="ArgumentException">
		/// The <see cref="Color"/> specified has an alpha value that is less that <see cref="Byte.MaxValue"/>.
		/// </exception>
		public virtual Color BackColor
		{
			get
			{
				if (_orphan != null)
					return _orphan.BackColor;

				// Look in the color bag first and fallback if not found
				string key = CreatePropertyKey("BackColor");
				if (_scintilla.ColorBag.ContainsKey(key))
					return _scintilla.ColorBag[key];

				return Util.RgbToColor((int)_scintilla.DirectMessage(Constants.SCI_STYLEGETBACK, (IntPtr)_index, IntPtr.Zero));
			}
			set
			{
				// Transparent colors are not allowed
				if (value != Color.Empty && value.A < Byte.MaxValue)
					throw new ArgumentException(Resources.Exception_ArgumentException_TransparentColor);

				if (value != BackColor)
				{
					if (_orphan != null)
					{
						_orphan.BackColor = value;
					}
					else
					{
						if (value.IsEmpty)
							value = DefaultBackColor;

						// Scintilla can't keep track of named colors round-trip.
						// If a color is known, we keep a local copy of it.
						string key = CreatePropertyKey("BackColor");
						if (value.IsKnownColor)
							_scintilla.ColorBag[key] = value;
						else
							_scintilla.ColorBag.Remove(key);

						_scintilla.DirectMessage(Constants.SCI_STYLESETBACK, (IntPtr)_index, (IntPtr)Util.ColorToRgb(value));
					}
				}
			}
		}


		/// <summary>
		/// Gets or sets the display casing of text.
		/// </summary>
		/// <value>
		/// One of the <see cref="StyleCase"/> enumeration values. The default is <c>Mixed</c>.
		/// </value>
		/// <exception cref="InvalidEnumArgumentException">
		/// The specified value is not a valid <see cref="StyleCase"/> value.
		/// </exception>
		[DefaultValue(StyleCase.Mixed)]
		public virtual StyleCase Case
		{
			get
			{
				if(_orphan != null)
					return _orphan.Case;

				return (StyleCase)_scintilla.DirectMessage(Constants.SCI_STYLEGETCASE, (IntPtr)_index, IntPtr.Zero);
			}
			set
			{
				if (!Util.IsEnumDefined((int)value, (int)StyleCase.Mixed, (int)StyleCase.Lower))
					throw new InvalidEnumArgumentException("Case", (int)value, typeof(StyleCase));

				if (value != Case)
				{
					if (_orphan != null)
						_orphan.Case = value;
					else
						_scintilla.DirectMessage(Constants.SCI_STYLESETCASE, (IntPtr)_index, (IntPtr)value);
				}
			}
		}



		/// <summary>
		/// Gets the default background color of a style.
		/// </summary>
		/// <value>
		/// The default background <see cref="Color"/> of a style.
		/// The default is: 0xFF, 0xFF, 0xFF.
		/// </value>
		public static Color DefaultBackColor
		{
			get
			{
				return Color.FromArgb(0xFF, 0xFF, 0xFF);
			}
		}


		/// <summary>
		/// Gets the default font of the style.
		/// </summary>
		/// <value>The default <see cref="Font"/> of the style.</value>
		public static Font DefaultFont
		{
			get
			{
				if (_defaultFont == null)
				{
					// According to Scintilla source code the default font
					// on Windows is Verdana 8pt. The following code may seem
					// like overkill, but given how GDI+ loves to convert font
					// units this is the only guarenteed way to get a .NET
					// equivalent font to the one Scintilla uses.

					// See the following Scintlla functions for add'l info:
					// Style::Realise (Style.cxx)
					// SetLogFont (PlatWin.cxx)
					// FontCached::FontCached (PlatWin.cxx)
					// SurfaceImpl::LogPixelsY (PlatWin.cxx)
					// SurfaceImpl::DeviceHeightFont (PlatWin.cxx)
					// Platform::DefaultFont (PlatWin.cxx)
					// Platform::DefaultFontSize (PlatWin.cxx)
					// ViewStyle::ResetDefaultStyle (ViewStyle.cxx)

					int logicalHeight = NativeMethods.MulDiv(DEFAULT_FONT_SIZE, Util.LogPixelsY, 72);

					NativeMethods.LOGFONT lf = new NativeMethods.LOGFONT();
					lf.lfHeight = -(Math.Abs(logicalHeight));
					lf.lfWeight = NativeMethods.FW_NORMAL;
					lf.lfCharSet = (byte)Constants.SC_CHARSET_DEFAULT;
					lf.lfFaceName = DEFAULT_FONT_NAME;

					// Create and convert to points
					using (Font f = Font.FromLogFont(lf))
						_defaultFont = new Font(f.FontFamily, f.SizeInPoints, f.Style, GraphicsUnit.Point, f.GdiCharSet, f.GdiVerticalFont);
				}

				return _defaultFont;
			}
		}


		/// <summary>
		/// Gets the default foreground color of a style.
		/// </summary>
		/// <value>
		/// The default foreground <see cref="Color"/> of a style.
		/// The default is: 0, 0, 0.
		/// </value>
		public static Color DefaultForeColor
		{
			get
			{
				return Color.FromArgb(0, 0, 0);
			}
		}


		/// <summary>
		/// Gets or sets whether the remainder of the line is filled with the <see cref="BackColor"/>
		/// when this style is used on the last character of a line.
		/// </summary>
		/// <value>
		/// <c>true</c> to fill the line with the <see cref="BackColor"/>; otherwise, <c>false</c>.
		/// </value>
		[DefaultValue(false)]
		public virtual bool FillLine
		{
			get
			{
				if (_orphan != null)
					return _orphan.FillLine;

				return (_scintilla.DirectMessage(Constants.SCI_STYLEGETEOLFILLED, (IntPtr)_index, IntPtr.Zero) != IntPtr.Zero);
			}
			set
			{
				if (value != FillLine)
				{
					if (_orphan != null)
						_orphan.FillLine = value;
					else
						_scintilla.DirectMessage(Constants.SCI_STYLESETEOLFILLED, (IntPtr)_index, (value ? (IntPtr)1 : IntPtr.Zero));
				}
			}
		}


		/// <summary>
		/// Gets or sets the font of the text displayed when using this style.
		/// </summary>
		/// <value>
		/// The <see cref="Font"/> to apply to the text displayed using this style.
		/// The default is the value of the <see cref="DefaultFont"/> property.
		/// </value>
		[Description("The font used to display text with this style.")]
		public virtual Font Font
		{
			get
			{
				if (_orphan != null)
					return _orphan.Font;

				// There's no way to reconstitute a Font object from the information
				// Scintilla gives us so we always return the local reference.
				string key = CreatePropertyKey("Font");
				if (_scintilla.PropertyBag.ContainsKey(key))
					return (Font)_scintilla.PropertyBag[key];

				return DefaultFont;
			}
			set
			{
				if (value != Font)
				{
					if (_orphan != null)
					{
						_orphan.Font = value;
					}
					else
					{
						// Update the local reference
						string key = CreatePropertyKey("Font");
						if (value == null)
						{
							value = DefaultFont;
							_scintilla.PropertyBag.Remove(key);
						}
						else
						{
							_scintilla.PropertyBag[key] = value;
						}

						// Update Scintilla. Where it's possible we check the
						// existing value to avoid uncessary repaints.

						bool bold = _scintilla.DirectMessage(Constants.SCI_STYLEGETBOLD, (IntPtr)_index, IntPtr.Zero) != IntPtr.Zero;
						if (bold != value.Bold)
							_scintilla.DirectMessage(Constants.SCI_STYLESETBOLD, (IntPtr)_index, (value.Bold ? (IntPtr)1 : IntPtr.Zero));

						bool italic = _scintilla.DirectMessage(Constants.SCI_STYLEGETITALIC, (IntPtr)_index, IntPtr.Zero) != IntPtr.Zero;
						if (italic != value.Italic)
							_scintilla.DirectMessage(Constants.SCI_STYLESETITALIC, (IntPtr)_index, (value.Italic ? (IntPtr)1 : IntPtr.Zero));

						bool underline = _scintilla.DirectMessage(Constants.SCI_STYLEGETUNDERLINE, (IntPtr)_index, IntPtr.Zero) != IntPtr.Zero;
						if (underline != value.Underline)
							_scintilla.DirectMessage(Constants.SCI_STYLESETUNDERLINE, (IntPtr)_index, (value.Underline ? (IntPtr)1 : IntPtr.Zero));

						int charSet = (int)_scintilla.DirectMessage(Constants.SCI_STYLEGETCHARACTERSET, (IntPtr)_index, IntPtr.Zero);
						if (charSet != value.GdiCharSet)
							_scintilla.DirectMessage(Constants.SCI_STYLESETCHARACTERSET, (IntPtr)_index, (IntPtr)value.GdiCharSet);

						unsafe
						{
							int fontNameLength = (int)_scintilla.DirectMessage(Constants.SCI_STYLEGETFONT, (IntPtr)_index, IntPtr.Zero);
							byte[] buffer = new byte[fontNameLength];
							fixed (byte* bp = buffer)
								_scintilla.DirectMessage(Constants.SCI_STYLEGETFONT, (IntPtr)_index, (IntPtr)bp);

							if (Encoding.ASCII.GetString(buffer) != value.Name)
							{
								buffer = Util.GetBytesZeroTerminated(value.Name, Encoding.ASCII);
								fixed (byte* bp = buffer)
									_scintilla.DirectMessage(Constants.SCI_STYLESETFONT, (IntPtr)_index, (IntPtr)bp);
							}
						}

						// .NET supports fractional font point sizes but Scintilla does not. So we can't
						// simply pass the point size of the Font object to Scintilla. To get around this
						// we have to give Scintilla an integer point size that will in effect *result*
						// in the same font height as if we had been able to use fractions. The formula
						// Scintilla (and most Win32 controls) uses to calculate the logical font height is:
						// (Point Size * LOGPIXELSY) / 72, where LOGPIXELSY is the number of pixels in one
						// device inch (usually 96). The following calculation is essentially the inverse
						// of that formula. It allows us to ignore the .NET fractional point size and
						// instead use the logical font height to calculate an integer point size. Scintila
						// will then perform its calculation to determine logical font height and if we've
						// done our math right it will match the font height we started with in .NET. All of
						// this is so that Scintilla will use the same logical font height that we are
						// using. Figuring this out was two days of my life I will never get back.

						int sizeInPoints = DEFAULT_FONT_SIZE;
						IntPtr hDC = NativeMethods.GetDC(IntPtr.Zero);
						if (hDC != IntPtr.Zero)
						{
							IntPtr hFont = IntPtr.Zero;
							try
							{
								hFont = Font.ToHfont();
								if (NativeMethods.SelectObject(hDC, hFont) != IntPtr.Zero)
								{
									NativeMethods.TEXTMETRIC tm;
									if (NativeMethods.GetTextMetrics(hDC, out tm))
										sizeInPoints = NativeMethods.MulDiv((tm.tmHeight - tm.tmInternalLeading), 72, Util.LogPixelsY);
								}
							}
							finally
							{
								if (hFont != IntPtr.Zero)
									NativeMethods.DeleteObject(hFont);
								if (hDC != IntPtr.Zero)
									NativeMethods.ReleaseDC(IntPtr.Zero, hDC);
							}
						}

						if (sizeInPoints != (int)_scintilla.DirectMessage(Constants.SCI_STYLEGETSIZE, (IntPtr)_index, IntPtr.Zero))
							_scintilla.DirectMessage(Constants.SCI_STYLESETSIZE, (IntPtr)_index, (IntPtr)sizeInPoints);
					}
				}
			}
		}


		/// <summary>
		/// Gets or sets the foreground color for the style.
		/// </summary>
		/// <value>
		/// A <see cref="Color"/> that represents the foreground color of the style.
		/// The default is the value of the <see cref="DefaultForeColor"/> property.
		/// </value>
		/// <exception cref="ArgumentException">
		/// The <see cref="Color"/> specified has an alpha value that is less that <see cref="Byte.MaxValue"/>.
		/// </exception>
		public virtual Color ForeColor
		{
			get
			{
				if (_orphan != null)
					return _orphan.ForeColor;

				// Look in the color bag first and fallback if not found
				string key = CreatePropertyKey("ForeColor");
				if (_scintilla.ColorBag.ContainsKey(key))
					return _scintilla.ColorBag[key];

				return Util.RgbToColor((int)_scintilla.DirectMessage(Constants.SCI_STYLEGETFORE, (IntPtr)_index, IntPtr.Zero));
			}
			set
			{
				// Transparent colors are not allowed
				if (value != Color.Empty && value.A < Byte.MaxValue)
					throw new ArgumentException(Resources.Exception_ArgumentException_TransparentColor);

				if (value != ForeColor)
				{
					if (_orphan != null)
					{
						_orphan.ForeColor = value;
					}
					else
					{
						if (value.IsEmpty)
							value = DefaultForeColor;

						// Scintilla can't keep track of named colors round-trip.
						// If a color is known, we keep a local copy of it.
						string key = CreatePropertyKey("ForeColor");
						if (value.IsKnownColor)
							_scintilla.ColorBag[key] = value;
						else
							_scintilla.ColorBag.Remove(key);

						_scintilla.DirectMessage(Constants.SCI_STYLESETFORE, (IntPtr)_index, (IntPtr)Util.ColorToRgb(value));
					}
				}
			}
		}


		/// <summary>
		/// Gets the style definition index of this style.
		/// </summary>
		/// <value>
		/// The style definition index of this style when being used in
		/// a <see cref="Scintilla"/> control; otherwise, -1.
		/// </value>
		[Browsable(false)]
		public int Index
		{
			get
			{
				return _index;
			}
		}


		/// <summary>
		/// Gets or sets whether text using this style exhibits hyperlink behavior.
		/// </summary>
		/// <value>
		/// <c>true</c> if text using this style accepts hyperlink style mouse clicks; otherwise, <c>false</c>.
		/// </value>
		[DefaultValue(false)]
		public virtual bool Link
		{
			get
			{
				if (_orphan != null)
					return _orphan.Link;

				return (_scintilla.DirectMessage(Constants.SCI_STYLEGETHOTSPOT, (IntPtr)_index, IntPtr.Zero) != IntPtr.Zero);
			}
			set
			{
				if (value != Link)
				{
					if (_orphan != null)
						_orphan.Link = value;
					else
						_scintilla.DirectMessage(Constants.SCI_STYLESETHOTSPOT, (IntPtr)_index, (value ? (IntPtr)1 : IntPtr.Zero));
				}
			}
		}


		/// <summary>
		/// Gets the <see cref="Scintilla"/> control the <see cref="Style"/> is located in.
		/// </summary>
		/// <value>
		/// A <see cref="Scintilla"/> control that represents the control that contains the <see cref="Style"/>.
		/// </value>
		[Browsable(false)]
		public Scintilla Scintilla
		{
			get
			{
				return _scintilla;
			}
		}


		/// <summary>
		/// Gets or sets whether text using this style can be user modified.
		/// </summary>
		/// <value>
		/// <c>true</c> if text in this style cannot be modified by the user; otherwise, <c>false</c>.
		/// </value>
		[DefaultValue(false)]
		public virtual bool ReadOnly
		{
			get
			{
				if (_orphan != null)
					return _orphan.ReadOnly;

				return (_scintilla.DirectMessage(Constants.SCI_STYLEGETCHANGEABLE, (IntPtr)_index, IntPtr.Zero) == IntPtr.Zero);
			}
			set
			{
				if (value != ReadOnly)
				{
					if (_orphan != null)
						_orphan.ReadOnly = value;
					else
						_scintilla.DirectMessage(Constants.SCI_STYLESETCHANGEABLE, (IntPtr)_index, (value ? IntPtr.Zero : (IntPtr)1));
				}
			}
		}


		/// <summary>
		/// Gets or sets the visibility of text using this style.
		/// </summary>
		/// <value>
		/// <c>true</c> if text with this style is visible; otherwise, <c>false</c>.
		/// </value>
		[DefaultValue(true)]
		public virtual bool Visible
		{
			get
			{
				if (_orphan != null)
					return _orphan.Visible;

				return (_scintilla.DirectMessage(Constants.SCI_STYLEGETVISIBLE, (IntPtr)_index, IntPtr.Zero) != IntPtr.Zero);
			}
			set
			{
				if (value != Visible)
				{
					if (_orphan != null)
						_orphan.Visible = value;
					else
						_scintilla.DirectMessage(Constants.SCI_STYLESETVISIBLE, (IntPtr)_index, (value ? (IntPtr)1 : IntPtr.Zero));
				}
			}
		}

		#endregion Properties


		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion Events


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="Style"/> class using default property values.
		/// </summary>
		public Style()
		{
			_orphan = new Orphan();
			_index = -1;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="Style"/> class using the property values of the
		/// specified <see cref="Style"/>.
		/// </summary>
		/// <param name="style">The <see cref="Style"/> used as a template to provide initial property values.</param>
		/// <exception cref="ArgumentNullException"><paramref name="style"/> is <c>null</c>.</exception>
		public Style(Style style)
		{
			if (style == null)
				throw new ArgumentNullException("style");
		}


		internal Style(int index, Scintilla owner)
		{
			Debug.Assert(index >= 0);
			Debug.Assert(owner != null);

			_index = index;
			_scintilla = owner;
		}

		#endregion Constructors
	}
}
