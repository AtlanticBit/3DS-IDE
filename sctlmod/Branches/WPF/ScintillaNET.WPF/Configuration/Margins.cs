using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

namespace ScintillaNET.WPF.Configuration
{
	public abstract class MarginBaseClass
	{
		public int Index { get; private set; }
		internal MarginBaseClass(int idx) { this.Index = idx; }

		private int? mAutoToggleMarkerNumber;
		public int AutoToggleMarkerNumber
		{
			get { return mAutoToggleMarkerNumber.Value; }
			set { mAutoToggleMarkerNumber = value; TryApplyConfig(); }
		}

		private bool? mIsClickable;
		public bool IsClickable
		{
			get { return mIsClickable.Value; }
			set { mIsClickable = value; TryApplyConfig(); }
		}

		private bool? mIsFoldMargin;
		public bool IsFoldMargin
		{
			get { return mIsFoldMargin.Value; }
			set { mIsFoldMargin = value; TryApplyConfig(); }
		}

		private bool? mIsMarkerMargin;
		public bool IsMarkerMargin
		{
			get { return mIsMarkerMargin.Value; }
			set { mIsMarkerMargin = value; TryApplyConfig(); }
		}

		private MarginType? mType;
		public MarginType Type
		{
			get { return mType.Value; }
			set { mType = value; TryApplyConfig(); }
		}

		private int? mWidth;
		public int Width
		{
			get { return mWidth.Value; }
			set { mWidth = value; TryApplyConfig(); }
		}

		private void TryApplyConfig()
		{
			if (mLastAppliedParent != null)
				ApplyConfig(mLastAppliedParent);
		}
		private ScintillaWPF mLastAppliedParent;
		internal void ApplyConfig(ScintillaWPF scintilla)
		{
			this.mLastAppliedParent = scintilla;

			if (mAutoToggleMarkerNumber != null)
				scintilla.Margins[Index].AutoToggleMarkerNumber = AutoToggleMarkerNumber;
			if (mIsClickable != null)
				scintilla.Margins[Index].IsClickable = IsClickable;
			if (mIsFoldMargin != null)
				scintilla.Margins[Index].IsFoldMargin = IsFoldMargin;
			if (mIsMarkerMargin != null)
				scintilla.Margins[Index].IsMarkerMargin = IsMarkerMargin;
			if (mType != null)
				scintilla.Margins[Index].Type = Type;
			if (mWidth != null)
				scintilla.Margins[Index].Width = Width;
		}

		internal void Reset(ScintillaWPF scintilla)
		{
			scintilla.Margins[Index].Reset();
		}
	}

	public sealed class Margin0 : MarginBaseClass { public Margin0() : base(0) { } }
	public sealed class Margin1 : MarginBaseClass { public Margin1() : base(1) { } }
	public sealed class Margin2 : MarginBaseClass { public Margin2() : base(2) { } }
	public sealed class Margin3 : MarginBaseClass { public Margin3() : base(3) { } }
	public sealed class Margin4 : MarginBaseClass { public Margin4() : base(4) { } }

	public sealed class Margins : ScintillaWPFConfigItem, IList
	{
		private Color? mFoldMarginColor;
		public Color FoldMarginColor
		{
			get { return mFoldMarginColor.Value; }
			set { mFoldMarginColor = value; TryApplyConfig(); }
		}

		private Color? mFoldMarginHighlightColor;
		public Color FoldMarginHighlightColor
		{
			get { return mFoldMarginHighlightColor.Value; }
			set { mFoldMarginHighlightColor = value; TryApplyConfig(); }
		}

		private int? mLeft;
		public int Left
		{
			get { return mLeft.Value; }
			set { mLeft = value; TryApplyConfig(); }
		}

		private int? mRight;
		public int Right
		{
			get { return mRight.Value; }
			set { mRight = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);

			if (mFoldMarginColor != null)
				scintilla.Margins.FoldMarginColor = FoldMarginColor;
			if (mFoldMarginHighlightColor != null)
				scintilla.Margins.FoldMarginHighlightColor = FoldMarginHighlightColor;
			if (mLeft != null)
				scintilla.Margins.Left = Left;
			if (mRight != null)
				scintilla.Margins.Right = Right;
			foreach (var m in mKnownMargins.Values)
				m.ApplyConfig(scintilla);
		}

		private readonly List<MarginBaseClass> mAddedMargins = new List<MarginBaseClass>(5);
		private readonly Dictionary<int, MarginBaseClass> mKnownMargins = new Dictionary<int, MarginBaseClass>(5);
		public int Add(object value)
		{
			if (!(value is MarginBaseClass))
				throw new ArgumentException("Attempted to add a non-margin to a margin collection!", "value");
			var mbc = (MarginBaseClass)value;
			if (mKnownMargins.ContainsKey(mbc.Index))
				throw new InvalidOperationException("Attempted to re-define a margin that has already been defined!");
			mKnownMargins.Add(mbc.Index, mbc);
			mAddedMargins.Add(mbc);
			return mbc.Index;
		}
		public void Clear()
		{
			foreach (var m in mKnownMargins.Values)
				m.Reset(mLastAppliedParent);
			mKnownMargins.Clear();
			mAddedMargins.Clear();
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
			this.Clear();
			scintilla.Margins.Reset();
		}

		public bool IsFixedSize { get { return false; } }
		public int Count { get { return mAddedMargins.Count; } }
		public void Insert(int index, object value)
		{
			if (!(value is MarginBaseClass))
				throw new ArgumentException("Attempted to add a non-margin to a margin collection!", "value");
			var mbc = (MarginBaseClass)value;
			if (mKnownMargins.ContainsKey(mbc.Index))
				throw new InvalidOperationException("Attempted to re-define a margin that has already been defined!");
			mKnownMargins.Add(mbc.Index, mbc);
			mAddedMargins.Insert(index, mbc);
		}
		public void RemoveAt(int index)
		{
			mAddedMargins[index].Reset(mLastAppliedParent);
			mKnownMargins.Remove(mAddedMargins[index].Index);
			mAddedMargins.RemoveAt(index);
		}
		public object this[int index]
		{
			get { return mAddedMargins[index]; }
			set 
			{
				if (!(value is MarginBaseClass))
					throw new ArgumentException("Attempted to add a non-margin to a margin collection!", "value");
				RemoveAt(index);
				Insert(index, value);
			}
		}

		#region IList
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool Contains(object value) { throw new NotImplementedException("Contains"); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int IndexOf(object value) { throw new NotImplementedException("IndexOf"); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Remove(object value) { throw new NotImplementedException("Remove"); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void CopyTo(Array array, int index) { throw new NotImplementedException("CopyTo"); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerator GetEnumerator() { throw new NotImplementedException("GetEnumerator"); }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsReadOnly { get { throw new NotImplementedException("IsReadOnly"); } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsSynchronized { get { throw new NotImplementedException("IsSynchronized"); } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object SyncRoot { get { throw new NotImplementedException("SyncRoot"); } }
		#endregion

	}
}
