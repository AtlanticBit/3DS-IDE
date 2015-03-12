using System;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class AutoComplete : ScintillaWPFConfigItem
	{
		private bool? mAutoHide;
		/// <summary>
		/// By default, the list is cancelled if there are no viable matches (the user has typed characters that no longer match a list entry). 
		/// If you want to keep displaying the original list, set AutoHide to false. 
		/// </summary>
		public bool AutoHide
		{
			get { return mAutoHide.Value; }
			set { mAutoHide = value; TryApplyConfig(); }
		}

		private bool? mAutomaticLengthEntered;
		/// <summary>
		/// Gets or Sets the last automatically calculated LengthEntered used with <see cref="Show" />.
		/// </summary>
		public bool AutomaticLengthEntered
		{
			get { return mAutomaticLengthEntered.Value; }
			set { mAutomaticLengthEntered = value; TryApplyConfig(); }
		}

		private bool? mCancelAtStart;
		/// <summary>
		/// The default behavior is for the list to be cancelled if the caret moves before the location it was at when the list was displayed. 
		/// By setting this property to false, the list is not cancelled until the caret moves before the first character of the word being completed.
		/// </summary>
		public bool CancelAtStart
		{
			get { return mCancelAtStart.Value; }
			set { mCancelAtStart = value; TryApplyConfig(); }
		}

		private bool? mDropRestOfWord;
		/// <summary>
		/// When an item is selected, any word characters following the caret are first erased if dropRestOfWord is set to true.
		/// </summary>
		/// <remarks>Defaults to false</remarks>
		public bool DropRestOfWord
		{
			get { return mDropRestOfWord.Value; }
			set { mDropRestOfWord = value; TryApplyConfig(); }
		}

		private string mFillUpCharacters;
		/// <summary>
		/// List of characters (no separated) that causes the AutoComplete window to accept the current
		/// selection.
		/// </summary>
		public string FillUpCharacters 
		{
			get { return mFillUpCharacters; }
			set { mFillUpCharacters = value; TryApplyConfig(); }
		}

		private char? mImageSeparator;
		/// <summary>
		/// Autocompletion list items may display an image as well as text. Each image is first registered with an integer type. 
		/// Then this integer is included in the text of the list separated by a '?' from the text. For example, "fclose?2 fopen" 
		/// displays image 2 before the string "fclose" and no image before "fopen". 
		/// </summary>
		public char ImageSeparator
		{
			get { return mImageSeparator.Value; }
			set { mImageSeparator = value; TryApplyConfig(); }
		}

		private bool? mIsCaseSensitive;
		/// <summary>
		///     Gets or Sets if the comparison of words to the AutoComplete <see cref="List"/> are case sensitive.
		/// </summary>
		/// <remarks>Defaults to true</remarks>
		public bool IsCaseSensitive
		{
			get { return mIsCaseSensitive.Value; }
			set { mIsCaseSensitive = value; TryApplyConfig(); }
		}

		private char? mListSeparator;
		/// <summary>
		/// Character used to split <see cref="ListString"/> to convert to a List.
		/// </summary>
		[TypeConverter(typeof(ScintillaNET.WhitespaceStringConverter))]
		public char ListSeparator
		{
			get { return mListSeparator.Value; }
			set { mListSeparator = value; TryApplyConfig(); }
		}

		private string mListString;
		/// <summary>
		/// List of words to display in the AutoComplete window.
		/// </summary>
		/// <remarks>
		/// The list of words separated by <see cref="ListSeparator"/> which
		/// is " " by default.
		/// </remarks>
		public string ListString 
		{
			get { return mListString; }
			set { mListString = value; TryApplyConfig(); }
		}

		private int? mMaxHeight;
		/// <summary>
		/// Get or set the maximum number of rows that will be visible in an autocompletion list. If there are more rows in the list, then a vertical scrollbar is shown
		/// </summary>
		/// <remarks>Defaults to 5</remarks>
		public int MaxHeight
		{
			get { return mMaxHeight.Value; }
			set { mMaxHeight = value; TryApplyConfig(); }
		}

		private int? mMaxWidth;
		/// <summary>
		/// Get or set the maximum width of an autocompletion list expressed as the number of characters in the longest item that will be totally visible. 
		/// </summary>
		/// <remarks>
		/// If zero (the default) then the list's width is calculated to fit the item with the most characters. Any items that cannot be fully displayed 
		/// within the available width are indicated by the presence of ellipsis.
		/// </remarks>
		public int MaxWidth
		{
			get { return mMaxWidth.Value; }
			set { mMaxWidth = value; TryApplyConfig(); }
		}

		private bool? mSingleLineAccept;
		/// <summary>
		/// If you set this value to true and a list has only one item, it is automatically added and no list is displayed.
		/// The default is to display the list even if there is only a single item.
		/// </summary>
		public bool SingleLineAccept
		{
			get { return mSingleLineAccept.Value; }
			set { mSingleLineAccept = value; TryApplyConfig(); }
		}

		private string mStopCharacters;
		/// <summary>
		/// List of characters (no separator) that causes the AutoComplete window to cancel.
		/// </summary>
		public string StopCharacters 
		{
			get { return mStopCharacters; }
			set { mStopCharacters = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (this.mAutoHide != null)
				scintilla.AutoComplete.AutoHide = AutoHide;
			if (this.mAutomaticLengthEntered != null)
				scintilla.AutoComplete.AutomaticLengthEntered = AutomaticLengthEntered;
			if (this.mCancelAtStart != null)
				scintilla.AutoComplete.CancelAtStart = CancelAtStart;
			if (this.mDropRestOfWord != null)
				scintilla.AutoComplete.DropRestOfWord = DropRestOfWord;
			if (this.FillUpCharacters != null)
				scintilla.AutoComplete.FillUpCharacters = FillUpCharacters;
			if (this.mImageSeparator != null)
				scintilla.AutoComplete.ImageSeparator = ImageSeparator;
			if (this.mIsCaseSensitive != null)
				scintilla.AutoComplete.IsCaseSensitive = IsCaseSensitive;
			if (this.mListSeparator != null)
				scintilla.AutoComplete.ListSeparator = ListSeparator;
			if (this.ListString != null)
				scintilla.AutoComplete.ListString = ListString;
			if (this.mMaxHeight != null)
				scintilla.AutoComplete.MaxHeight = MaxHeight;
			if (this.mMaxWidth != null)
				scintilla.AutoComplete.MaxWidth = MaxWidth;
			if (this.mSingleLineAccept != null)
				scintilla.AutoComplete.SingleLineAccept = SingleLineAccept;
			if (this.StopCharacters != null)
				scintilla.AutoComplete.StopCharacters = StopCharacters;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
