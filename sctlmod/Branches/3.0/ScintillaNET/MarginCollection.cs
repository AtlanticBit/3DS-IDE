#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;

#endregion Using Directives


namespace ScintillaNet
{
	[TypeConverterAttribute(typeof(System.ComponentModel.ExpandableObjectConverter))]
	public class MarginCollection : TopLevelHelper, ICollection<Margin>
	{
		#region Individual Margin Elements

		#region Margin0
		private Margin _margin0;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Margin Margin0
		{
			get
			{
				return _margin0;
			}
		}

		private bool ShouldSerializeMargin0()
		{
			return _margin0.ShouldSerialize();
		}

		private void ResetMargin0()
		{
			_margin0.Reset();
		}
		#endregion

		#region Margin1
		private Margin _margin1;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Margin Margin1
		{
			get
			{
				return _margin1;
			}
		}

		private bool ShouldSerializeMargin1()
		{
			return _margin1.ShouldSerialize();
		}

		private void ResetMargin1()
		{
			_margin0.Reset();
		}
		#endregion

		#region Margin2
		private Margin _margin2;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Margin Margin2
		{
			get
			{
				return _margin2;
			}
		}

		private bool ShouldSerializeMargin2()
		{
			return _margin2.ShouldSerialize();
		}

		private void ResetMargin2()
		{
			_margin0.Reset();
		}

		#endregion

		#region Margin3
		private Margin _margin3;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Margin Margin3
		{
			get
			{
				return _margin3;
			}
		}

		private bool ShouldSerializeMargin3()
		{
			return _margin3.ShouldSerialize();
		}

		private void ResetMargin3()
		{
			_margin0.Reset();
		}
		#endregion

		#region Margin4
		private Margin _margin4;
		[Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Margin Margin4
		{
			get
			{
				return _margin4;
			}
		}

		private bool ShouldSerializeMargin4()
		{
			return _margin4.ShouldSerialize();
		}

		private void ResetMargin4()
		{
			_margin0.Reset();
		}
		#endregion
		#endregion

		protected internal MarginCollection(Scintilla scintilla)
			: base(scintilla)
		{
			_margin0 = new Margin(scintilla, 0);
			_margin1 = new Margin(scintilla, 1);
			_margin2 = new Margin(scintilla, 2);
			_margin3 = new Margin(scintilla, 3);
			_margin4 = new Margin(scintilla, 4);

			_margin2.IsFoldMargin = true;
			_margin2.IsClickable = true;
		}

		public Margin this[int number]
		{
			get
			{
				Debug.Assert(number >= 0, "Number must be greater than or equal to 0");
				Debug.Assert(number < 5, "Number must be less than 5");

				switch (number)
				{
					case 0:
						return _margin0;
					case 1:
						return _margin1;
					case 2:
						return _margin2;
					case 3:
						return _margin3;
					case 4:
						return _margin4;
				}

				throw new ArgumentException("number", "Number must be greater than or equal to 0 AND less than 5");
			}
		}

		internal bool ShouldSerialize()
		{
			return ShouldSerializeFoldMarginColor() ||
				ShouldSerializeFoldMarginHighlightColor() ||
				ShouldSerializeMargin0() ||
				ShouldSerializeMargin1() ||
				ShouldSerializeMargin2() ||
				ShouldSerializeMargin3() ||
				ShouldSerializeMargin4();
		}

		public void Reset()
		{
			ResetMargin0();
			ResetMargin1();
			ResetMargin2();
			ResetMargin3();
			ResetMargin4();
		}

		#region FoldMarginColor
		public Color FoldMarginColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("Margins.FoldMarginColor"))
					return Scintilla.ColorBag["Margins.FoldMarginColor"];

				return Color.Transparent;
			}
			set
			{
				if (value == Color.Transparent)
					Scintilla.ColorBag.Remove("Margins.FoldMarginColor");
				else
					Scintilla.ColorBag["Margins.FoldMarginColor"] = value;


				NativeScintilla.SetFoldMarginColour(true, Util.ColorToRgb(value));

			}
		}

		private bool ShouldSerializeFoldMarginColor()
		{
			return FoldMarginColor != Color.Transparent;
		}

		private void ResetFoldMarginColor()
		{
			FoldMarginColor = Color.Transparent;
		}
		#endregion

		#region FoldMarginHighlightColor
		public Color FoldMarginHighlightColor
		{
			get
			{
				if (Scintilla.ColorBag.ContainsKey("Margins.FoldMarginHighlightColor"))
					return Scintilla.ColorBag["Margins.FoldMarginHighlightColor"];

				return Color.Transparent;
			}
			set
			{
				if (value == Color.Transparent)
					Scintilla.ColorBag.Remove("Margins.FoldMarginHighlightColor");
				else
					Scintilla.ColorBag["Margins.FoldMarginHighlightColor"] = value;


				NativeScintilla.SetFoldMarginColour(true, Util.ColorToRgb(value));

			}
		}

		private bool ShouldSerializeFoldMarginHighlightColor()
		{
			return FoldMarginHighlightColor != Color.Transparent;
		}

		private void ResetFoldMarginHighlightColor()
		{
			FoldMarginHighlightColor = Color.Transparent;
		}
		#endregion

		#region ICollection<Margin> Members

		public void Add(Margin item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Clear()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool Contains(Margin item)
		{
			return true;
		}

		public void CopyTo(Margin[] array, int arrayIndex)
		{
			Array.Copy(ToArray(), 0, array, arrayIndex, 5);
		}

		public Margin[] ToArray()
		{
			return new Margin[]
			{
				_margin0,
				_margin1,
				_margin2,
				_margin3,
				_margin4
			};
		}

		[Browsable(false)]
		public int Count
		{
			get { return 5; }
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool Remove(Margin item)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IEnumerable<Margin> Members

		public IEnumerator<Margin> GetEnumerator()
		{
			return new List<Margin>(ToArray()).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return new List<Margin>(ToArray()).GetEnumerator();
		}

		#endregion


	}
}
