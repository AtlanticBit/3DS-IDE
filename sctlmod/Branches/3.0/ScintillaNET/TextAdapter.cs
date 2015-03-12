#region Using Directives

using System;
using System.Diagnostics;
using System.Text;

#endregion Using Directives


namespace ScintillaNet
{
	// Provides an access layer for getting text in and out of Scintilla. Since Scintilla may
	// internally store text as a single byte (ASCII) or multiple bytes, (Codepage, UTF-8) a
	// byte index may not represent a character index. That along with the fact that .NET stores
	// all strings as UTF-16 means users can easily get junk data if we don't help them out.
	//
	// To accomplish this we have several routines that convert text from UTF-16 to Scintilla's
	// internal format and vice-versa. To facilitate the translation of byte index to char index
	// (and vice-versa) we maintain a cache of each line's character length.
	//
	// Very little error checking is performed here so it must be done higher up.
	//
	internal class TextAdapter
	{
		#region Fields

		private Scintilla _scintilla;
		private GapBuffer<int> _lengthCache;
		private Encoding _encoding;

		#endregion Fields


		#region Methods

		public int AnchorByteIndex()
		{
			return (int)_scintilla.DirectMessage(Constants.SCI_GETANCHOR, IntPtr.Zero, IntPtr.Zero);
		}


		public int ByteToCharIndex(int index)
		{
			// A shortcut for ASCII encoding which is a one-to-one mapping
			Encoding encoding = GetEncoding();
			if (!EncodingRequiresConversion(encoding))
				return index;

			// Sum the line lengths for every line prior to this one (cached)
			int total = 0;
			int lineIndex = (int)_scintilla.DirectMessage(Constants.SCI_LINEFROMPOSITION, (IntPtr)index, IntPtr.Zero);
			for (int i = 0; i < lineIndex; i++)
				total += GetLineLength(i, encoding);

			// Include the length from the current line start to the byte index (non-cached)
			int lineStartIndex = (int)_scintilla.DirectMessage(Constants.SCI_POSITIONFROMLINE, (IntPtr)lineIndex, IntPtr.Zero);
			total += GetCharCountOfByteRange(lineStartIndex, index, encoding);

			return total;
		}


		public int CaretByteIndex()
		{
			return (int)_scintilla.DirectMessage(Constants.SCI_GETCURRENTPOS, IntPtr.Zero, IntPtr.Zero);
		}


		private static bool EncodingRequiresConversion(Encoding encoding)
		{
			// A shortcut for ASCII encoding which is a one-to-one mapping
			if (encoding.CodePage == Constants.CODEPAGE_ASCII)
				return false;

			return true;
		}


		public unsafe int CharToByteIndex(int index)
		{
			Encoding encoding = GetEncoding();
			if (!EncodingRequiresConversion(encoding))
				return index;

			// Get the line index char index from the line start (cached)
			int lineIndex = -1;
			int lineLength = 0;
			do
			{
				lineIndex++;
				index -= lineLength;
				lineLength = GetLineLength(lineIndex, encoding);
			} while (index - lineLength > 0);

			// Get the bytes for the line containing the char index
			lineLength = (int)_scintilla.DirectMessage(Constants.SCI_LINELENGTH, (IntPtr)lineIndex, IntPtr.Zero);
			byte[] buffer = new byte[lineLength];
			fixed (byte* bp = buffer)
			{
				_scintilla.DirectMessage(Constants.SCI_GETLINE, (IntPtr)lineIndex, (IntPtr)bp);

				// Count the bytes in the line until we match the char index (non-cached)
				// TODO A binary search would perform much better here
				int byteIndex = 0;
				Decoder decoder = encoding.GetDecoder();
				for (; index > 0; byteIndex++)
					index -= decoder.GetCharCount(bp + byteIndex, 1, false);

				// Add the byte count from the start of the line to the line position
				int lineStartIndex = (int)_scintilla.DirectMessage(Constants.SCI_POSITIONFROMLINE, (IntPtr)lineIndex, IntPtr.Zero);
				return (lineStartIndex + byteIndex);
			}
		}


		/*
		private unsafe byte[] GetBytes(string text, int index, int count)
		{
			byte[] buffer;
			Encoding encoding = GetEncoding();

			fixed (char* cp = text)
			{
				int byteCount = encoding.GetByteCount(cp + index, count);
				buffer = new byte[byteCount];

				fixed (byte* bp = buffer)
				{
					int actualCount = encoding.GetBytes(cp + index, count, bp, byteCount);
					Debug.Assert(actualCount == byteCount);
				}
			}

			return buffer;
		}
		*/


		private unsafe int GetCharCountOfByteRange(int startIndex, int endIndex, Encoding encoding)
		{
			Debug.Assert(startIndex <= endIndex);

			if(!EncodingRequiresConversion(encoding))
				return endIndex - startIndex;

			// Get the text in the range and count the chars (non-cached)
			TextRange tr = new TextRange();
			tr.chrg.cpMin = startIndex;
			tr.chrg.cpMax = endIndex;

			int length;
			byte[] buffer = new byte[endIndex - startIndex + 1];
			fixed (byte* bp = buffer)
			{
				tr.lpstrText = (IntPtr)bp;
				TextRange* trp = &tr;
				length = (int)_scintilla.DirectMessage(Constants.SCI_GETTEXTRANGE, IntPtr.Zero, (IntPtr)trp);
			}

			Debug.Assert(length == buffer.Length - 1);
			return encoding.GetCharCount(buffer, 0, length);
		}


		public Encoding GetEncoding()
		{
			// Getting the current code page is cheap and checking it
			// ensures that we always have the correct encoding.
			int codePage = (int)_scintilla.DirectMessage(Constants.SCI_GETCODEPAGE, IntPtr.Zero, IntPtr.Zero);
			if ((codePage == 0 && _encoding.CodePage != Constants.CODEPAGE_ASCII) || codePage != _encoding.CodePage)
			{
				_encoding = System.Text.Encoding.GetEncoding(codePage == 0 ? Constants.CODEPAGE_ASCII : codePage);
				
				// The line cache is now invalid
				for (int i = 0; i < _lengthCache.Count; i++)
					_lengthCache[i] = -1;
			}

			return _encoding;
		}


		private unsafe int GetLineLength(int index)
		{
			Encoding encoding = GetEncoding();
			return GetLineLength(index, encoding);
		}


		private unsafe int GetLineLength(int index, Encoding encoding)
		{
			// A shortcut for ASCII encoding which is a one-to-one mapping
			if (!EncodingRequiresConversion(encoding))
				return (int)_scintilla.DirectMessage(Constants.SCI_LINELENGTH, (IntPtr)index, IntPtr.Zero);

			int length = _lengthCache[index];
			if (length == -1)
			{
				// TODO Upgrade to Scintilla 1.77 so we can use SCI_GETCHARACTERPOINTER?
				int lineLength = (int)_scintilla.DirectMessage(Constants.SCI_LINELENGTH, (IntPtr)index, IntPtr.Zero);
				byte[] buffer = new byte[lineLength];

				fixed (byte* bp = buffer)
				{
					lineLength = (int)_scintilla.DirectMessage(Constants.SCI_GETLINE, (IntPtr)index, (IntPtr)bp);
					Debug.Assert(lineLength == buffer.Length);
				}

				_lengthCache[index] = length = encoding.GetCharCount(buffer);
			}

			return length;
		}


		public unsafe string GetText()
		{
			int length = TotalByteLength() + 1;
			byte[] buffer = new byte[length];
			fixed (byte* bp = buffer)
				length = (int)_scintilla.DirectMessage(Constants.SCI_GETTEXT, (IntPtr)length, (IntPtr)bp);

			return GetEncoding().GetString(buffer, 0, length);
		}


		public unsafe string GetSelectedText()
		{
			int length = (int)_scintilla.DirectMessage(Constants.SCI_GETSELTEXT, IntPtr.Zero, IntPtr.Zero);
			byte[] buffer = new byte[length];
			fixed (byte* bp = buffer)
				_scintilla.DirectMessage(Constants.SCI_GETSELTEXT, (IntPtr.Zero), (IntPtr)bp);

			return GetEncoding().GetString(buffer, 0, length - 1); // Skip the terminating null
		}


		public void ModifiedCallback(int position, int linesDelta)
		{
			// Obviously it's critical that we get every change notification
			// from Scintilla to keep the line cache in sync. For that reason,
			// Scintilla explicitly passes us the information rather than us
			// listening for an event... which could pass through who knows what.

			// Invalidate the lengths cache for the modified lines
			int index = (int)_scintilla.DirectMessage(Constants.SCI_LINEFROMPOSITION, (IntPtr)position, IntPtr.Zero);
			_lengthCache[index] = -1;

			if (linesDelta > 0)
			{
				for (int i = 0; i < linesDelta; i++)
					_lengthCache.Insert(index + 1, -1);
			}
			else if (linesDelta < 0)
			{
				for (int i = 0; i > linesDelta; i--)
					_lengthCache.RemoveAt(index + 1);
			}
		}


		public unsafe void SetSelectedText(string text)
		{
			byte[] buffer = (String.IsNullOrEmpty(text) ? new byte[] { 0 } : Util.GetBytesZeroTerminated(text, GetEncoding()));
			fixed (byte* bp = buffer)
				_scintilla.DirectMessage(Constants.SCI_REPLACESEL, IntPtr.Zero, (IntPtr)bp);
		}


		public unsafe void SetText(string text)
		{
			byte[] buffer = (String.IsNullOrEmpty(text) ? new byte[] { 0 } : Util.GetBytesZeroTerminated(text, GetEncoding()));
			fixed (byte* bp = buffer)
				_scintilla.DirectMessage(Constants.SCI_SETTEXT, IntPtr.Zero, (IntPtr)bp);
		}


		public int TotalByteLength()
		{
			return (int)_scintilla.DirectMessage(Constants.SCI_GETLENGTH, IntPtr.Zero, IntPtr.Zero);
		}


		public int TotalCharLength()
		{
			// Sum the lengths of all the lines
			int length = 0;
			for (int i = 0; i < _lengthCache.Count; i++)
				length += GetLineLength(i);

			return length;
		}

		#endregion Methods


		#region Constructors

		public TextAdapter(Scintilla scintilla)
		{
			_scintilla = scintilla;
			_encoding = Encoding.UTF8;
			_lengthCache = new GapBuffer<int>();
			_lengthCache.Insert(0, -1);
		}

		#endregion Constructors
	}
}
