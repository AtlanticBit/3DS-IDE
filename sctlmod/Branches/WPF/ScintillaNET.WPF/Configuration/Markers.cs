using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;

namespace ScintillaNET.WPF.Configuration
{
	public abstract class MarkerBaseClass
	{
		public int Index { get; private set; }
		internal MarkerBaseClass(int idx) { this.Index = idx; }

		private const int DefaultAlpha = 255;
		private int? mAlpha;
		[DefaultValue(DefaultAlpha)]
		public int Alpha
		{
			get { return (mAlpha ?? (int?)DefaultAlpha).Value; }
			set { mAlpha = value; TryApplyConfig(); }
		}

		private Color? mBackColor;
		public Color BackColor
		{
			get { return mBackColor.Value; }
			set { mBackColor = value; TryApplyConfig(); }
		}

		private Color? mForeColor;
		[DefaultValue(typeof(Color), "Gray")]
		public Color ForeColor
		{
			get { return mForeColor.Value; }
			set { mForeColor = value; TryApplyConfig(); }
		}

		private MarkerSymbol? mSymbol;
		/// <summary>
		/// Gets or sets the marker symbol.
		/// </summary>
		/// <returns>One of the <see cref="MarkerSymbol" /> values. The default is <see cref="MarkerSymbol.Circle" />.</returns>
		/// <exception cref="InvalidEnumArgumentException">The value assigned is not one of the <see cref="MarkerSymbol" /> values.</exception>
		public MarkerSymbol Symbol
		{
			get { return mSymbol.Value; }
			set { mSymbol = value; TryApplyConfig(); }
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
			if (mAlpha != null)
				scintilla.Markers[Index].Alpha = Alpha;
			if (mBackColor != null)
				scintilla.Markers[Index].BackColor = BackColor;
			if (mForeColor != null)
				scintilla.Markers[Index].ForeColor = ForeColor;
			if (mSymbol != null)
				scintilla.Markers[Index].Symbol = Symbol;
		}

		internal void Reset(ScintillaWPF scintilla)
		{
			scintilla.Markers[Index].Reset();
			Alpha = 0xff;
			BackColor = Color.White;
			ForeColor = Color.Gray;
			switch (Index)
			{
				case Constants.SC_MARKNUM_FOLDER:
					Symbol = MarkerSymbol.BoxPlus;
					break;
				case Constants.SC_MARKNUM_FOLDEREND:
					Symbol = MarkerSymbol.BoxPlusConnected;
					break;
				case Constants.SC_MARKNUM_FOLDEROPEN:
					Symbol = MarkerSymbol.BoxMinus;
					break;
				case Constants.SC_MARKNUM_FOLDEROPENMID:
					Symbol = MarkerSymbol.BoxMinusConnected;
					break;
				case Constants.SC_MARKNUM_FOLDERMIDTAIL:
					ForeColor = Color.White;
					BackColor = Color.Gray;
					Symbol = MarkerSymbol.TCorner;
					break;
				case Constants.SC_MARKNUM_FOLDERSUB:
					ForeColor = Color.White;
					BackColor = Color.Gray;
					Symbol = MarkerSymbol.VLine;
					break;
				case Constants.SC_MARKNUM_FOLDERTAIL:
					ForeColor = Color.White;
					BackColor = Color.Gray;
					Symbol = MarkerSymbol.LCorner;
					break;
				default:
					Symbol = MarkerSymbol.Circle;
					break;
			}
		}
	}
	
	public sealed class FolderMarker : MarkerBaseClass { public FolderMarker() : base(Constants.SC_MARKNUM_FOLDER) { } }
	public sealed class FolderEndMarker : MarkerBaseClass { public FolderEndMarker() : base(Constants.SC_MARKNUM_FOLDEREND) { } }
	public sealed class FolderOpenMarker : MarkerBaseClass { public FolderOpenMarker() : base(Constants.SC_MARKNUM_FOLDEROPEN) { } }
	public sealed class FolderOpenMidMarker : MarkerBaseClass { public FolderOpenMidMarker() : base(Constants.SC_MARKNUM_FOLDEROPENMID) { } }
	public sealed class FolderOpenMidTailMarker : MarkerBaseClass { public FolderOpenMidTailMarker() : base(Constants.SC_MARKNUM_FOLDERMIDTAIL) { } }
	public sealed class FolderSubMarker : MarkerBaseClass { public FolderSubMarker() : base(Constants.SC_MARKNUM_FOLDERSUB) { } }
	public sealed class FolderTailMarker : MarkerBaseClass { public FolderTailMarker() : base(Constants.SC_MARKNUM_FOLDERTAIL) { } }

	public sealed class Markers : ScintillaWPFConfigItem, IList
	{
		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);

			foreach (var m in mKnownMarkers.Values)
				m.ApplyConfig(scintilla);
		}

		private readonly List<MarkerBaseClass> mAddedMarkers = new List<MarkerBaseClass>(7);
		private readonly Dictionary<int, MarkerBaseClass> mKnownMarkers = new Dictionary<int, MarkerBaseClass>(7);
		public int Add(object value)
		{
			if (!(value is MarkerBaseClass))
				throw new ArgumentException("Attempted to add a non-marker to a marker collection!", "value");
			var mbc = (MarkerBaseClass)value;
			if (mKnownMarkers.ContainsKey(mbc.Index))
				throw new InvalidOperationException("Attempted to re-define a marker that has already been defined!");
			mKnownMarkers.Add(mbc.Index, mbc);
			mAddedMarkers.Add(mbc);
			return mbc.Index;
		}
		public void Clear()
		{
			foreach (var m in mKnownMarkers.Values)
				m.Reset(mLastAppliedParent);
			mKnownMarkers.Clear();
			mAddedMarkers.Clear();
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
			this.Clear();
			scintilla.Markers.Reset();
		}
		public bool IsFixedSize { get { return false; } }
		public int Count { get { return mAddedMarkers.Count; } }

		public void Insert(int index, object value)
		{
			if (!(value is MarkerBaseClass))
				throw new ArgumentException("Attempted to add a non-marker to a marker collection!", "value");
			var mbc = (MarkerBaseClass)value;
			if (mKnownMarkers.ContainsKey(mbc.Index))
				throw new InvalidOperationException("Attempted to re-define a marker that has already been defined!");
			mKnownMarkers.Add(mbc.Index, mbc);
			mAddedMarkers.Insert(index, mbc);
		}
		public void RemoveAt(int index) 
		{
			mAddedMarkers[index].Reset(mLastAppliedParent);
			mKnownMarkers.Remove(mAddedMarkers[index].Index);
			mAddedMarkers.RemoveAt(index);
		}
		public object this[int index]
		{
			get { return mAddedMarkers[index]; }
			set
			{
				if (!(value is MarkerBaseClass))
					throw new ArgumentException("Attempted to add a non-marker to a marker collection!", "value");
				RemoveAt(index);
				Insert(index, value);
			}
		}

		#region IList
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool Contains(object value) { throw new NotImplementedException(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int IndexOf(object value) { throw new NotImplementedException(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Remove(object value) { throw new NotImplementedException(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void CopyTo(Array array, int index) { throw new NotImplementedException(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerator GetEnumerator() { throw new NotImplementedException(); }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsReadOnly { get { throw new NotImplementedException(); } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsSynchronized { get { throw new NotImplementedException(); } }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object SyncRoot { get { throw new NotImplementedException(); } }
		#endregion

	}
}
