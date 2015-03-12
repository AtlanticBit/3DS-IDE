using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ScintillaNet
{
	// The only reason I didn't yank this out with the rest of the selection changes
	// is because snippets and search make heavy use of these properties. When those
	// can be rewritten, this can be completely removed.
	[Obsolete("The Selection class is deprecated because it uses byte-based offsets. Alternatives that use char-based offsets are available on the Scintilla control.")]
	public class Selection : TopLevelHelper
	{
		protected internal Selection(Scintilla scintilla) : base(scintilla) 
		{			
		}


		public bool IsRectangle
		{
			get
			{
				return NativeScintilla.SelectionIsRectangle();
			}
		}


		public PositionRange Range
		{
			get
			{
				return new PositionRange(NativeScintilla.GetSelectionStart(), NativeScintilla.GetSelectionEnd(), Scintilla);
			}
			set
			{
				NativeScintilla.SetSel(value.Start, value.End);
			}
		}
		

		public int Length
		{
			get
			{
				return Math.Abs(End - Start);
			}			
			set
			{	//hope this is ok, seemed like an easy feature to add, just these lines, hope it doesn't break anything
				this.End = this.Start + value;
			}
		}


		public SelectionMode Mode
		{
			get
			{
				return (SelectionMode)NativeScintilla.GetSelectionMode();
			}
			set
			{
				NativeScintilla.SetSelectionMode((int)value);
			}
		}


		public int Start
		{
			get
			{
				return NativeScintilla.GetSelectionStart();
			}
			set
			{
				NativeScintilla.SetSelectionStart(value);
			}
		}


		public int End
		{
			get
			{
				return NativeScintilla.GetSelectionEnd();
			}
			set
			{
				NativeScintilla.SetSelectionEnd(value);
			}
		}
	}
}
