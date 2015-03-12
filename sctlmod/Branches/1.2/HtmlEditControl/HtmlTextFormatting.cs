// ------------------------------------------------------------
// Writer, WYSIWYG editor for HTML
// Copyright (c) 2002-2003 Lutz Roeder. All rights reserved.
// http://www.aisto.com/roeder
// ------------------------------------------------------------
// Based on HTML editor control code
// Copyright (c) 2002-2003 Nikhil Kothari. All rights reserved.
// http://www.nikhilk.net
// ------------------------------------------------------------
namespace HTMLEditor
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
	using System.Windows.Forms;

    public class HtmlTextFormatting 
    {

        // Array of the names of HTML formats that MSHTML accepts. These need to be in the same order as the enum HtmlFormat.
        private static readonly string[] formats = new string[] 
						{
                             "Normal",               // Normal
                             "Formatted",            // PreFormatted
                             "Heading 1",            // Heading1
                             "Heading 2",            // Heading2
                             "Heading 3",            // Heading3
                             "Heading 4",            // Heading4
                             "Heading 5",            // Heading5
                             "Heading 6",            // Heading6
                             "Paragraph",            // Paragraph
                             "Numbered List",        // OrderedList
                             "Bulleted List"         // UnorderedList
                         };

        private HtmlControl control;

		public HtmlTextFormatting(HtmlControl control) 
        {
            this.control = control;
        }

        /// <summary>The background color of the current text.</summary>
        public Color BackColor 
        {
            get { return this.ConvertColorFromHtml(this.control.Execute(NativeMethods.IDM_BACKCOLOR)); }
			set { this.control.Execute(NativeMethods.IDM_BACKCOLOR, new object[] { ColorTranslator.ToHtml(value) }); }
        }

        public bool CanIndent 
        {
            get { return this.control.IsEnabled(NativeMethods.IDM_INDENT); }
        }

        public bool CanSetBackColor 
        {
            get { return this.control.IsEnabled(NativeMethods.IDM_BACKCOLOR); }
        }

		public bool CanSetFontName 
        {
            get { return this.control.IsEnabled(NativeMethods.IDM_FONTNAME); }
        }

        public bool CanSetFontSize 
        {
            get { return this.control.IsEnabled(NativeMethods.IDM_FONTSIZE); }
        }

        public bool CanSetHtmlFormat 
        {
            get { return this.control.IsEnabled(NativeMethods.IDM_BLOCKFMT); }
        }

        public bool CanUnindent 
        {
            get { return this.control.IsEnabled(NativeMethods.IDM_OUTDENT); }
        }

        public string FontName 
        {
            get { return (this.control.Execute(NativeMethods.IDM_FONTNAME) as string); }
			set { this.control.Execute(NativeMethods.IDM_FONTNAME, new object[] { value }); }
        }

		public HtmlFontSize FontSize 
        {
            get 
            {
                object o = this.control.Execute(NativeMethods.IDM_FONTSIZE);
                if (o == null) 
                {
                    return HtmlFontSize.Medium;
                }
                else 
                {
                    return (HtmlFontSize)o;
                }
            }
            set 
            {
				this.control.Execute(NativeMethods.IDM_FONTSIZE, new object[] { (int) value });
            }
        }

		public bool CanSetForeColor
		{
			get { return this.control.IsEnabled(NativeMethods.IDM_FORECOLOR); }
		}

        public Color ForeColor 
        {
            get 
            {
                // Query MSHTML, convert the result, and return the color
                return this.ConvertColorFromHtml(this.control.Execute(NativeMethods.IDM_FORECOLOR));
            }
            set 
            {
                // Translate the color and execute the command
                string color = ColorTranslator.ToHtml(value);
				this.control.Execute(NativeMethods.IDM_FORECOLOR, new object[] { color });
            }
        }
        
        private Color ConvertColorFromHtml(object colorValue) 
        {
            if (colorValue != null) 
            {
                Type colorType = colorValue.GetType();
                if (colorType == typeof(int)) 
                {
                    // If the colorValue is an int, it's a Win32 color
                    return ColorTranslator.FromWin32((int)colorValue);
                }
                else if (colorType == typeof(string)) 
                {
                    // Otherwise, it's a string, so convert that
                    return ColorTranslator.FromHtml((string)colorValue);
                }
                Debug.Fail("Unexpected color type : " + colorType.FullName);
            }
            return Color.Empty;
        }

		public HtmlFormat HtmlFormat
		{
			get
			{
				string formatString = this.control.Execute(NativeMethods.IDM_BLOCKFMT, null) as string;
				if (formatString != null)
				{
					for (int i = 0; i < formats.Length; i++)
					{
						if (formatString.Equals(formats[i]))
						{
							return (HtmlFormat) i;
						}
					}
				}
				return HtmlFormat.Normal;
			}

			set
			{
				this.control.Execute(NativeMethods.IDM_BLOCKFMT, new object[] { formats[(int) value] });
			}
		}

        public void Indent() 
        {
            this.control.Execute(NativeMethods.IDM_INDENT);
        }

		public bool CanToggleBold
		{
			get { return this.control.IsEnabled(NativeMethods.IDM_BOLD); }
		}

		public bool IsBold
		{
			get { return this.control.IsChecked(NativeMethods.IDM_BOLD); }
		}

        public void ToggleBold() 
        {
            this.control.Execute(NativeMethods.IDM_BOLD);
        }

		public bool CanToggleItalic
		{
			get { return this.control.IsEnabled(NativeMethods.IDM_ITALIC); }
		}

		public bool IsItalic
		{
			get { return this.control.IsChecked(NativeMethods.IDM_ITALIC); }
		}

        public void ToggleItalics() 
        {
            this.control.Execute(NativeMethods.IDM_ITALIC);
        }

		public bool CanToggleStrikethrough
		{
			get { return this.control.IsEnabled(NativeMethods.IDM_STRIKETHROUGH); }
		}

		public bool IsStrikethrough
		{
			get { return this.control.IsChecked(NativeMethods.IDM_STRIKETHROUGH); }
		}

        public void ToggleStrikethrough() 
        {
            this.control.Execute(NativeMethods.IDM_STRIKETHROUGH);
        }

		public bool CanToggleSubscript
		{
			get { return this.control.IsEnabled(NativeMethods.IDM_SUBSCRIPT); }
		}

		public bool IsSubscript
		{
			get { return this.control.IsChecked(NativeMethods.IDM_SUBSCRIPT); }
		}

        public void ToggleSubscript() 
        {
            this.control.Execute(NativeMethods.IDM_SUBSCRIPT);
        }

		public bool CanToggleSuperscript
		{
			get { return this.control.IsEnabled(NativeMethods.IDM_SUPERSCRIPT); }
		}

		public bool IsSuperscript
		{
			get { return this.control.IsChecked(NativeMethods.IDM_SUPERSCRIPT); }
		}

		public void ToggleSuperscript() 
        {
            this.control.Execute(NativeMethods.IDM_SUPERSCRIPT);
        }

		public bool CanToggleUnderline
		{
			get { return this.control.IsEnabled(NativeMethods.IDM_UNDERLINE); }
		}

		public bool IsUnderline
		{
			get { return this.control.IsChecked(NativeMethods.IDM_UNDERLINE); }
		}

        public void ToggleUnderline() 
        {
            this.control.Execute(NativeMethods.IDM_UNDERLINE);
        }

        public void Unindent() 
        {
            this.control.Execute(NativeMethods.IDM_OUTDENT);
        }

		public HtmlAlignment Alignment
		{
			set
			{
				this.control.Execute(this.MapAlignment(value));
			}

			get
			{
				if (this.control.IsChecked(this.MapAlignment(HtmlAlignment.Left)))
				{
					return HtmlAlignment.Left;
				}

				if (this.control.IsChecked(this.MapAlignment(HtmlAlignment.Right)))
				{
					return HtmlAlignment.Right;
				}

				if (this.control.IsChecked(this.MapAlignment(HtmlAlignment.Center)))
				{
					return HtmlAlignment.Center;
				}

				return HtmlAlignment.Full;
			}
		}

		public bool CanAlign(HtmlAlignment alignment)
		{
			return this.control.IsEnabled(this.MapAlignment(alignment));
		}

		private int MapAlignment(HtmlAlignment alignment)
		{
			switch (alignment)
			{
				case HtmlAlignment.Left:
					return NativeMethods.IDM_JUSTIFYLEFT;
				case HtmlAlignment.Right:
					return NativeMethods.IDM_JUSTIFYRIGHT;
				case HtmlAlignment.Center:
					return NativeMethods.IDM_JUSTIFYCENTER;
			}
			return -1;
		}

	}
}
