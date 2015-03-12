#region Using Directives

using System;
using System.Drawing;
using System.Diagnostics;

#endregion Using Directives


namespace ScintillaNet
{
	partial class Style
	{
		// To give users the ability to set style properties without the style first
		// having been associated with a Scintilla control requires us to store that
		// information until we make that association. This is where we do that.
		private class Orphan
		{
			#region Fields

			public StyleCase Case;
			public bool FillLine;
			public bool Visible;
			public bool ReadOnly;
			public bool Link;

			private Color _backColor;
			private Color _foreColor;
			private Font _font;

			#endregion Fields


			#region Properties

			public Color BackColor
			{
				get
				{
					if (_backColor.IsEmpty)
						return DefaultBackColor;

					return _backColor;
				}
				set
				{
					_backColor = value;
				}
			}


			public Font Font
			{
				get
				{
					if (_font == null)
						return DefaultFont;

					return _font;
				}
				set
				{
					_font = value;
				}
			}


			public Color ForeColor
			{
				get
				{
					if (_foreColor.IsEmpty)
						return DefaultForeColor;

					return _foreColor;
				}
				set
				{
					_foreColor = value;
				}
			}

			#endregion Properties


			#region Constructors

			public Orphan()
			{
				Visible = true;
			}

			#endregion Constructors
		}
	}
}
