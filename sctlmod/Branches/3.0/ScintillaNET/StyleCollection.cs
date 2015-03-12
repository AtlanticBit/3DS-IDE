#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using ScintillaNet.Properties;

#endregion Using Directives


// TODO I think some of these named styles should be moved to more appropriate locations.
// For example, the line number style should be grouped with line number properties, the call tip
// style should be grouped with call tip settings, etc....

namespace ScintillaNet
{
	/// <summary>
	/// Represents a collection of <see cref="Style"/> objects in a <see cref="Scintilla"/> control.
	/// </summary>
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class StyleCollection // TODO : ICollection, IEnumerable<Style>
	{
		#region Constants

		private const int MAX_STYLE_BITS = 8;

		#endregion Constants


		#region Fields

		private Scintilla _owner;

		#endregion Fields


		#region Properties

		/// <summary>
		/// Gets or sets the style used when marking an unmatched brace.
		/// </summary>
		/// <value>The <see cref="Style"/> used to mark unmatched braces.</value>
		/// <remarks>This style represents style definition 35.</remarks>
		[Description("The style used when marking an unmatched brace.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style BraceBadStyle
		{
			get
			{
				return this[(int)Constants.STYLE_BRACEBAD];
			}
			set
			{
				this[(int)Constants.STYLE_BRACEBAD] = value;
			}
		}


		/// <summary>
		/// Gets or sets the style used when highlighting braces.
		/// </summary>
		/// <value>The <see cref="Style"/> used to highlight braces.</value>
		/// <remarks>This style represents style definition 34.</remarks>
		[Description("The style used when highlighting braces.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style BraceLightStyle
		{
			get
			{
				return this[(int)Constants.STYLE_BRACELIGHT];
			}
			set
			{
				this[(int)Constants.STYLE_BRACELIGHT] = value;
			}
		}


		/// <summary>
		/// Gets or sets the font used when displaying call tips.
		/// </summary>
		/// <value>The <see cref="Style"/> used to display call tips.</value>
		/// <remarks>
		/// This style represents style definition 38. Only the <see cref="Style.ForeColor"/>,
		/// <see cref="Style.BackColor"/>, and some <see cref="Style.Font"/> property values of
		/// this style are used when displaying call tips.
		/// </remarks>
		/// <dev>Hidden from the designer until I can determine proper default values.</dev>
		[Browsable(false)]
		public Style CallTipStyle
		{
			get
			{
				return this[(int)Constants.STYLE_CALLTIP];
			}
			set
			{
				this[(int)Constants.STYLE_CALLTIP] = value;
			}
		}


		/// <summary>
		/// Gets or sets the font used when drawing control characters.
		/// </summary>
		/// <value>The <see cref="Style"/> used to draw control characters.</value>
		/// <remarks>
		/// This style represents style definition 36. Only the <see cref="Style.Font"/>
		/// property of this style is used when drawing control characters.
		/// </remarks>
		[Description("The style used when drawing control characters.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style ControlCharStyle
		{
			get
			{
				return this[(int)Constants.STYLE_CONTROLCHAR];
			}
			set
			{
				this[(int)Constants.STYLE_CONTROLCHAR] = value;
			}
		}


		/// <summary>
		/// Gets the total number of styles available.
		/// </summary>
		/// <value>The total number of styles available.</value>
		[Browsable(false)]
		public int Count
		{
			get
			{
				int val = (int)_owner.DirectMessage(Constants.SCI_GETSTYLEBITS, IntPtr.Zero, IntPtr.Zero);
				if (val != 0)
					val = 0x1 << val;

				return val;
			}
		}


		/// <summary>
		/// Gets or sets the style properties that all styles receive when reset.
		/// </summary>
		/// <value>The <see cref="Style"/> inherited by all styles when reset.</value>
		/// <remarks>This style represents style definition 32.</remarks>
		/// <dev>Hidden from the designer since its purpose overlaps with other properties.</dev>
		[Browsable(false)]
		public Style DefaultStyle
		{
			get
			{
				return this[(int)Constants.STYLE_DEFAULT];
			}
			set
			{
				this[(int)Constants.STYLE_DEFAULT] = value;
			}
		}


		/// <summary>
		/// Gets or sets the style colors used when drawing indentation guides.
		/// </summary>
		/// <value>The <see cref="Style"/> used to draw indentation guides.</value>
		/// <remarks>
		/// This style represents style definition 37. Only the <see cref="Style.ForeColor"/>
		/// and <see cref="Style.BackColor"/> properties of this style are used when drawing
		/// indentation guides.
		/// </remarks>
		[Description("The style used when drawing indentation guides.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style IndentGuideStyle
		{
			get
			{
				return this[(int)Constants.STYLE_INDENTGUIDE];
			}
			set
			{
				this[(int)Constants.STYLE_INDENTGUIDE] = value;
			}
		}


		/// <summary>
		/// Gets or sets the style used to display line numbers in a line number margin.
		/// </summary>
		/// <value>The <see cref="Style"/> used to display line numbers.</value>
		/// <remarks>This style represents style definition 33.</remarks>
		[Description("The style used when displaying line numbers in a line number margin.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Style LineNumberStyle
		{
			get
			{
				return this[(int)Constants.STYLE_LINENUMBER];
			}
			set
			{
				this[(int)Constants.STYLE_LINENUMBER] = value;
			}
		}


		/// <summary>
		/// Gets the <see cref="Scintilla"/> control upon which the style collection
		/// performs style-related operations.
		/// </summary>
		/// <value>The <see cref="Scintlla"/> control that owns the collection.</value>
		[Browsable(false)]
		public Scintilla Scintilla
		{
			get
			{
				return _owner;
			}
		}


		private INativeScintilla NativeScintilla
		{
			get
			{
				return _owner.NativeInterface;
			}
		}

		#endregion Properties


		#region Indexers

		/// <summary>
		/// Gets or sets a <see cref="Style"/> definition by numerical index.
		/// </summary>
		/// <param name="index">The numerical index of the style definition.</param>
		/// <returns>The <see cref="Style"/> representing the definition at the specified index.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index"/> is less than 0.
		/// -or-
		/// <paramref name="index"/> is equal to or greater than <see cref="Count"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// 
		/// </exception>
		[Browsable(false)]
		public Style this[int index]
		{
			get
			{
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");

				// TODO Cache or create each time?
				return new Style(index, Scintilla);
			}
			set
			{
				if (index < 0 || index >= Count)
					throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");

				if (value != this[index])
				{
					if (value == null)
					{
						// TODO
					}
					else
					{
						if (value.Scintilla != null)
							throw new ArgumentException(Util.Format(Resources.Exception_ArgumentException_AlreadyHasOwner, "value"));

						value.SetOwner(_owner, index);
					}
				}
			}
		}

		#endregion Indexers


		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="StyleCollection"/> class for
		/// the given <see cref="Scintilla"/> control.
		/// </summary>
		/// <param name="owner">The <see cref="Scintilla"/> control that created this collection.</param>
		/// <exception cref="ArgumentNullException"><paramref name="owner"/> is <c>null</c>.</exception>
		public StyleCollection(Scintilla owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");

			_owner = owner;

			// Originally Scintilla stored indicator information alongside styles.
			// This is no longer the case and so by increasing the "style bits" we
			// can store a greater number of styles.
			_owner.DirectMessage(Constants.SCI_SETSTYLEBITS, (IntPtr)MAX_STYLE_BITS, IntPtr.Zero);
			Debug.Assert((int)_owner.DirectMessage(Constants.SCI_GETSTYLEBITS, IntPtr.Zero, IntPtr.Zero) == MAX_STYLE_BITS);


			/*
			//	Defaulting CallTip Settings to Platform defaults
			Style s = CallTip;
			s.ForeColor = SystemColors.InfoText;
			s.BackColor = SystemColors.Info;
			s.Font = SystemFonts.StatusFont;

			//	Making Line Number's BackColor have a named system color
			//	instead of just the value
			LineNumber.BackColor = SystemColors.Control;
			*/
		}

		#endregion Constructors

		public void Reset()
		{
			// TODO

		}

		#region Indexer
		

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Style this[StylesCommon index]
		{
			get
			{
				return new Style((int)index, Scintilla);
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Style this[string styleName]
		{
			get
			{
				return new Style(Scintilla.Lexing.StyleNameMap[styleName], Scintilla);
			}
		}
		#endregion


		#region Public Methods
		public void ClearAll()
		{
			NativeScintilla.StyleClearAll();
		}

		public int GetEndStyled()
		{
			return NativeScintilla.GetEndStyled();
		}

		public byte GetStyleAt(int position)
		{
			return NativeScintilla.GetStyleAt(position);
		}

		public string GetStyleNameAt(int position)
		{
			int styleNumber = GetStyleAt(position);
			foreach (KeyValuePair<string, int> map in Scintilla.Lexing.StyleNameMap)
				if (map.Value == styleNumber)
					return map.Key;

			return null;
		}

		public void ResetDefault()
		{
			NativeScintilla.StyleResetDefault();
		}

		public void ClearDocumentStyle()
		{
			NativeScintilla.ClearDocumentStyle();
		}
		#endregion

	}
}
