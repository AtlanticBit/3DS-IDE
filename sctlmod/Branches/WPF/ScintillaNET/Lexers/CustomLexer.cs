using System;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace ScintillaNET.Lexers
{
	public abstract class CustomLexer
	{
		#region Custom Lexer Registration
		private sealed class CustomLexerDescription
		{
			public Type Type { get; private set; }
			public Dictionary<string, int> KeywordNames { get; private set; }
			public Dictionary<string, int> StyleNames { get; private set; }
			public CustomLexerDescription(CustomLexer lex)
			{
				this.KeywordNames = new Dictionary<string, int>();
				this.StyleNames = new Dictionary<string, int>() { { "document_default", STYLE_DEFAULT } };
				this.Type = lex.GetType();
				foreach (var kv in lex.KeywordNameMapping)
				{
					this.KeywordNames.Add(String.Intern(kv.Key.ToLower()), kv.Value);
				}
				foreach (var kv in lex.StyleNameMapping)
				{
					this.StyleNames.Add(String.Intern(kv.Key.ToLower()), kv.Value);
				}
			}
		}
		private static readonly Dictionary<string, CustomLexerDescription> KnownCustomLexers = new Dictionary<string, CustomLexerDescription>();
		static CustomLexer()
		{
			foreach (var t in typeof(CustomLexer).Assembly.GetTypes())
			{
				if (BasedOnCustomLexer(t) && (t.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract)
				{
					var l = (CustomLexer)Activator.CreateInstance(t, new object[] { null });
					KnownCustomLexers.Add(((string)t.GetProperty("LexerName").GetValue(l, null)).ToLower(), new CustomLexerDescription(l));
				}
			}
		}
		private static bool BasedOnCustomLexer(Type t)
		{
			if (t.BaseType == typeof(CustomLexer))
				return true;
			if (t.BaseType != null && BasedOnCustomLexer(t.BaseType))
				return true;
			return false;
		}

		internal static bool TryGetCustomLexer(string name, out Type lexerType) 
		{
			CustomLexerDescription desc;
			if (KnownCustomLexers.TryGetValue(name.ToLower(), out desc))
			{
				lexerType = desc.Type;
				return true;
			}
			lexerType = null;
			return false;
		}
		internal static bool IsCustomLexer(string name) { return KnownCustomLexers.ContainsKey(name.ToLower()); }
		internal static void LoadKeywordNameMap(string lexerName, Dictionary<string, int> keywordNameMap)
		{
			var knm = KnownCustomLexers[lexerName].KeywordNames;
			foreach (var kv in knm)
			{
				keywordNameMap.Remove(kv.Key);
				keywordNameMap.Add(kv.Key, kv.Value);
			}
		}
		internal static void LoadStyleNameMap(string lexerName, Dictionary<string, int> styleNameMap)
		{
			var snm = KnownCustomLexers[lexerName].StyleNames;
			foreach (var kv in snm)
			{
				styleNameMap.Remove(kv.Key);
				styleNameMap.Add(kv.Key, kv.Value);
			}
		}
		#endregion

		private readonly Scintilla mScintilla;
		protected Scintilla Scintilla { get { return mScintilla; } }
		/// <summary>
		/// The name of this lexer.
		/// </summary>
		/// <remarks>
		/// This should not rely on <see cref="Initialize"/>
		/// having already been called when this property is
		/// retrieved.
		/// </remarks>
		public abstract string LexerName { get; }
		/// <summary>
		/// A mapping between the names of styles
		/// and their indexes.
		/// </summary>
		/// <remarks>
		/// This should not rely on <see cref="Initialize"/>
		/// having already been called when this property is
		/// retrieved.
		/// </remarks>
		public abstract Dictionary<string, int> StyleNameMapping { get; }
		/// <summary>
		/// A mapping between the names of keyword 
		/// lists and their indexes.
		/// </summary>
		/// <remarks>
		/// This should not rely on <see cref="Initialize"/>
		/// having already been called when this property is
		/// retrieved.
		/// </remarks>
		public abstract Dictionary<string, int> KeywordNameMapping { get; }

		protected CustomLexer(Scintilla scintilla)
		{
			this.mScintilla = scintilla;
			// Scintilla will be passed in as null when
			// creating the instance for registration.
			if (scintilla != null)
			{
				Initialize();
			}
		}

		protected virtual void Initialize()
		{
			if (!UseDynamicOptions)
				UpdateProperties();
			InitializeIndicators(Scintilla.Indicators);
		}

		protected const int STYLE_DEFAULT = 0;

		protected const int INDICATOR_ERROR = 3;
		protected virtual void InitializeIndicators(Indicators indicators)
		{
			indicators[INDICATOR_ERROR].Color = Color.Red;
			indicators[INDICATOR_ERROR].DrawMode = IndicatorDrawMode.Overlay;
			indicators[INDICATOR_ERROR].Style = IndicatorStyle.Squiggle;
		}

		protected void SetStyle(int style) { SetStyle(style, CurrentPosition - CurrentBasePosition); }
		protected void SetStyle(int style, int length)
		{
			if (length > 0)
				mScintilla.NativeInterface.SetStyling(length, style);
			CurrentBasePosition = CurrentPosition;
		}

		protected void StartStyling()
		{
			if (!mLexerForcedRestyle)
				mScintilla.NativeInterface.StartStyling(StartPosition, Constants.STYLE_MAX);
		}

		protected const int STATE_UNKNOWN = -1;
		private bool mLexerForcedRestyle = false;
		internal void Style(int startPosition, string text)
		{
			mLexerForcedRestyle = false;
		Start:
			Text = text;
			StartPosition = startPosition;
			CurrentLine = mScintilla.NativeInterface.LineFromPosition(startPosition);
			CurrentColumn = mScintilla.NativeInterface.GetColumn(startPosition);
			CurrentPosition = 0;
			CurrentBasePosition = 0;
			CurrentState = STATE_UNKNOWN;
			mScintilla.NativeInterface.SetIndicatorCurrent(INDICATOR_ERROR);
			mScintilla.NativeInterface.IndicatorClearRange(startPosition, text.Length);

			UpdateKeywords();
			if (UseDynamicOptions)
				UpdateProperties();

			InitializeStateFromStyle(mScintilla.NativeInterface.GetStyleAt(mScintilla.NativeInterface.GetLineEndPosition(CurrentLine - 1)));

			Style();
			if (mDelayedStyle != null)
			{
				startPosition = mDelayedStyle.Start;
				text = mDelayedStyle.Text;
				mDelayedStyle = null;
				mLexerForcedRestyle = true;
				goto Start;
			}
		}
		// We have to delay this because
		// otherwise we end up calling 
		// ourselves recursively and overflow
		// the stack at about 900 lines.
		private Range mDelayedStyle = null;
		protected void StyleNextLine()
		{
			var len = Scintilla.NativeInterface.GetLength();
			var startPos = mScintilla.NativeInterface.GetLineEndPosition(CurrentLine);
			var endPos = mScintilla.NativeInterface.GetLineEndPosition(CurrentLine + 1);
			if (endPos > len)
				endPos = len - 1;
			if (startPos > len)
				endPos = len - 1;
			if (startPos == endPos)
				return;
			this.mDelayedStyle = new Range(startPos, endPos, Scintilla);
		}

		protected virtual void InitializeStateFromStyle(int style)
		{
			// We don't do anything here by default.
		}
		protected abstract void Style();

		// TODO: Allow auto-display of calltips with the error message
		protected void MarkSyntaxError(int position, int length)
		{
			mScintilla.NativeInterface.SetIndicatorCurrent(INDICATOR_ERROR);
			mScintilla.NativeInterface.IndicatorFillRange(StartPosition + position, length);
		}
		protected void MarkCurrentAsSyntaxError() { MarkSyntaxError(CurrentBasePosition, CurrentPosition - CurrentBasePosition); }

		#region Options
		/// <summary>
		/// If true, the lexer will re-apply all options
		/// every time <see cref="Style"/> is called. Default is false.
		/// </summary>
		protected virtual bool UseDynamicOptions { get { return false; } }
		public class LexerOptions
		{
			public virtual void ResetProperties()
			{

			}
			protected virtual bool? Bool(string value)
			{
				switch (value.ToLower())
				{
					case "true":
					case "yes":
					case "1":
						return true;
					case "false":
					case "no":
					case "0":
						return false;
					default:
						bool b;
						if (!bool.TryParse(value, out b))
							return null;
						return b;
				}
			}
			public virtual void SetProperty(string name, string value)
			{
				Console.WriteLine("WARNING: Unknown lexer property '" + name + "'!");
			}
		}
		private LexerOptions mOptions = new LexerOptions();
		protected virtual LexerOptions Options
		{
			get { return mOptions; }
			set { mOptions = value; }
		}
		private void UpdateProperties()
		{
			foreach (var kv in Scintilla.Lexing.AssignedProperties)
			{
				Options.SetProperty(kv.Key, kv.Value);
			}
		}
		internal void ClearProperties()
		{
			Options.ResetProperties();
		}
		#endregion

		#region Keywords
		/// <summary>
		/// If true, all keywords will be entered into a single table which
		/// maps each keyword to a style, rather than separate tables for each
		/// keyword set.
		/// </summary>
		protected virtual bool EnableCompositeKeywords { get { return false; } }
		/// <summary>
		/// If true, all keywords will be treated as
		/// lowercase.
		/// </summary>
		protected virtual bool LowerKeywords { get { return false; } }
		private readonly Dictionary<int, Hashtable> mKeywords = new Dictionary<int, Hashtable>();
		protected Dictionary<int, Hashtable> Keywords { get { return mKeywords; } }
		private readonly Dictionary<int, int> mKeywordStyleMap = new Dictionary<int, int>(16);
		private readonly Dictionary<string, int> mCompositeKeywords = new Dictionary<string, int>();
		protected Dictionary<string, int> CompositeKeywords { get { return mCompositeKeywords; } }

		protected virtual void EnsureKeywords(params int[] keywordsIndexes)
		{
			Keywords.Clear();
			foreach (var k in keywordsIndexes)
				Keywords[k] = new Hashtable();
		}
		protected virtual void EnsureCompositeKeywords(params KeyValuePair<int, int>[] keywordStyleMaps)
		{
			if (!EnableCompositeKeywords)
				throw new Exception("This method is only valid if EnableCompositeKeywords is true!");
			mKeywordStyleMap.Clear();
			foreach (var kv in keywordStyleMaps)
				mKeywordStyleMap.Add(kv.Key, kv.Value);
		}
		private void UpdateKeywords()
		{
			if (Scintilla.Lexing.Keywords.Modified)
			{
				if (EnableCompositeKeywords)
				{
					foreach (var kv in mKeywordStyleMap)
					{
						var words = mScintilla.Lexing.Keywords[kv.Key];
						if (!String.IsNullOrEmpty(words))
						{
							var itms = words.Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ').Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
							foreach (var s in itms)
							{
								var str = s;
								if (LowerKeywords)
									str = str.ToLower();
								mCompositeKeywords.Add(str, kv.Value);
							}
						}
					}
				}
				foreach (var kv in Keywords)
				{
					kv.Value.Clear();
					var words = mScintilla.Lexing.Keywords[kv.Key];
					if (!String.IsNullOrEmpty(words))
					{
						var itms = words.Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ').Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
						foreach (var s in itms)
						{
							var str = s;
							if (LowerKeywords)
								str = str.ToLower();
							kv.Value.Add(str, null);
						}
					}
				}
				Scintilla.Lexing.Keywords.Modified = false;
			}
		}
		#endregion

		#region Folding
		private uint mCurrentLineFlags = 0;
		private uint mFoldDepth = Constants.SC_FOLDLEVELBASE;
		protected uint FoldDepth { get { return mFoldDepth; } }
		protected void EnterFoldLevel()
		{
			mFoldDepth++;
			mCurrentLineFlags = Constants.SC_FOLDLEVELHEADERFLAG;
		}
		protected void LeaveFoldLevel()
		{
			mFoldDepth--;
			CurrentLine--;
			UpdateCurrentFoldLevel();
			CurrentLine++;
		}
		protected void TryLeaveFoldLevel()
		{
			if (mFoldDepth > Constants.SC_FOLDLEVELBASE)
				LeaveFoldLevel();
		}
		private void UpdateCurrentFoldLevel()
		{
			if (mCurrentLineFlags != 0)
				mFoldDepth--;
			mScintilla.NativeInterface.SetFoldLevel(CurrentLine, mFoldDepth | mCurrentLineFlags);
			if (mCurrentLineFlags != 0)
				mFoldDepth++;
		}
		#endregion

		#region Styling
		/// <summary>
		/// The position in the document where the <see cref="Text"/> value is located.
		/// </summary>
		protected int StartPosition { get; private set; }
		/// <summary>
		/// The current position within the <see cref="Text"/>.
		/// </summary>
		protected int CurrentPosition { get; private set; }

		/// <summary>
		/// The current character being processed.
		/// </summary>
		protected char CurrentCharacter { get { return Peek(); } }
		/// <summary>
		/// The previous character processed.
		/// </summary>
		protected char PreviousCharacter
		{
			get
			{
				if (CurrentPosition < 1)
					throw new ArgumentOutOfRangeException("CurrentPosition", "The current position is at the start of the Text, there are no previous characters to read!");
				if (CurrentPosition > Text.Length)
					throw new ArgumentOutOfRangeException("CurrentPosition", "The current position is past the end of the Text, no characters can be read!");
				return Text[CurrentPosition - 1];
			}
		}
		/// <summary>
		/// The next character to be processed. This will
		/// be a NUL byte if there is no next character.
		/// </summary>
		protected char NextCharacter
		{
			get
			{
				if (CurrentPosition + 1 >= Text.Length)
					return '\0';
				return Text[CurrentPosition + 1];
			}
		}
		/// <summary>
		/// A character <i>distance</i> characters ahead. This will
		/// be a NUL byte if that would be past the end of the text.
		/// </summary>
		protected char Lookahead(int distance)
		{
			if (CurrentPosition + distance >= Text.Length)
				return '\0';
			return Text[CurrentPosition + distance];
		}

		/// <summary>
		/// Retrieve the next character to be read without
		/// consuming it.
		/// </summary>
		/// <returns>The character read.</returns>
		protected char Peek()
		{
			if (CurrentPosition >= Text.Length)
				throw new ArgumentOutOfRangeException("CurrentPosition", "The current position is past the end of the Text, no characters can be read!");
			return Text[CurrentPosition];
		}
		/// <summary>
		/// Retrieve the next character to be read and
		/// consume it.
		/// </summary>
		/// <returns>The character read.</returns>
		protected char Read()
		{
			if (CurrentPosition >= Text.Length)
				throw new ArgumentOutOfRangeException("CurrentPosition", "The current position is past the end of the Text, no characters can be read!");
			char c = Text[CurrentPosition];
			Consume();
			return c;
		}
		/// <summary>
		/// Consume the specified number of characters.
		/// </summary>
		/// <param name="count">The number of characters to consume.</param>
		protected void Consume(int count)
		{
			for (int i = 0; i < count; i++)
				Consume();
		}
		/// <summary>
		/// Consume the current character being read.
		/// </summary>
		protected void Consume()
		{
			switch (Peek())
			{
				case '\r':
					if (CurrentPosition + 1 < Text.Length && Text[CurrentPosition + 1] == '\n')
						CurrentPosition++;
					goto case '\n';
				case '\n':
					UpdateCurrentFoldLevel();
					CurrentLine++;
					CurrentColumn = 0;
					mCurrentLineFlags = 0;
					break;
				default:
					CurrentColumn++;
					break;
			}
			CurrentPosition++;
		}
		/// <summary>
		/// True if we are at the end of the text to be processed.
		/// </summary>
		protected bool EndOfText
		{
			get { return CurrentPosition >= Text.Length; }
		}

		protected string GetRange(int start, int end)
		{
			return Text.Substring(start, end - start);
		}


		/// <summary>
		/// The position where the style currently being processed started.
		/// </summary>
		protected int CurrentBasePosition { get; private set; }

		/// <summary>
		/// The text currently being processed.
		/// </summary>
		protected string Text { get; private set; }

		/// <summary>
		/// The line number currently being processed.
		/// </summary>
		protected int CurrentLine { get; private set; }
		/// <summary>
		/// The column number currently being processed.
		/// </summary>
		protected int CurrentColumn { get; private set; }

		private int mCurrentState = STATE_UNKNOWN;
		/// <summary>
		/// The state of the current lexer.
		/// </summary>
		/// <remarks>
		/// This should be hidden by any state-based
		/// lexers, in favor of a version using their
		/// enum instead.
		/// </remarks>
		protected int CurrentState
		{
			get { return mCurrentState; }
			set
			{
				// If the states ever become more precise
				// than by-line, this should be updated.
				mCurrentState = value;
				//Scintilla.NativeInterface.SetLineState(CurrentLine, mState);
			}
		}
		#endregion


		// Helper methods for lexing

		protected virtual void ConsumeUntilEOL(int style)
		{
			while (!EndOfText)
			{
				if (IsEndOfLine(CurrentCharacter))
				{
					Consume();
					SetStyle(style);
					return;
				}
				else
					Consume();
			}
			// May want an option to mark an error
			// if this point is reached.
		}

		protected virtual void ConsumeWhitespace()
		{
			while (!EndOfText && IsWhitespace(CurrentCharacter))
				Consume();
			SetStyle(STYLE_DEFAULT);
		}

		protected virtual void ConsumeString(int style, char escapeCharacter, bool doubleQuoteEscapes = false, bool breakAtEndOfLine = true, char endChar = '"')
		{
			bool closed = false;
			while (!EndOfText)
			{
				if (CurrentCharacter == escapeCharacter)
				{
					Consume(2);
				}
				else if (CurrentCharacter == endChar)
				{
					if (doubleQuoteEscapes && NextCharacter == endChar)
					{
						Consume(2);
					}
					else
					{
						closed = true;
						Consume();
						break;
					}
				}
				else if (breakAtEndOfLine && IsEndOfLine(CurrentCharacter))
				{
					MarkSyntaxError(CurrentBasePosition, CurrentPosition - CurrentBasePosition);
					break;
				}
				else
					Consume();
			}
			if (!closed)
				MarkSyntaxError(CurrentBasePosition, CurrentPosition - CurrentBasePosition);
			SetStyle(style);
		}


		/// <summary>
		/// Determine if the specified character is within
		/// the set [ \t\f\v\r\n].
		/// </summary>
		/// <param name="c">The character to process.</param>
		/// <returns>True if the character is within the set, otherwise false.</returns>
		protected virtual bool IsWhitespace(char c)
		{
			switch (c)
			{
				case ' ':  // Space
				case '\t': // Tab
				case '\f': // Form Feed
				case '\v': // Vertical Tab
				case '\r': // Carriage Return
				case '\n': // Newline
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Determine if the specified character is within
		/// the set [\r\n].
		/// </summary>
		/// <param name="c">The character to process.</param>
		/// <returns>True if the character is within the set, otherwise false.</returns>
		protected virtual bool IsEndOfLine(char c)
		{
			switch (c)
			{
				case '\r':
				case '\n':
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Determine if the specified character is within
		/// the set [0-9eE.+-].
		/// </summary>
		/// <param name="c">The character to process.</param>
		/// <returns>True if the character is within the set, otherwise false.</returns>
		protected virtual bool IsNumber(char c)
		{
			switch (c)
			{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case 'e':
				case 'E':
				case '.':
				case '-':
				case '+':
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Determine if the specified character is within
		/// the set [%^&amp;*()-+=|{}[]:;&lt;&gt;,/?!.~].
		/// </summary>
		/// <param name="c">The character to process.</param>
		/// <returns>True if the character is within the set, otherwise false.</returns>
		protected virtual bool IsOperator(char c)
		{
			switch (c)
			{
				case '%':
				case '^':
				case '&':
				case '*':
				case '(':
				case ')':
				case '-':
				case '+':
				case '=':
				case '|':
				case '{':
				case '}':
				case '[':
				case ']':
				case ':':
				case ';':
				case '<':
				case '>':
				case ',':
				case '/':
				case '?':
				case '!':
				case '.':
				case '~':
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Determine if the specified character is within
		/// the set [0-9].
		/// </summary>
		/// <param name="c">The character to process.</param>
		/// <returns>True if the character is within the set, otherwise false.</returns>
		protected virtual bool IsDigit(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// Determine if the specified character is within
		/// the set [0-9a-fA-F].
		/// </summary>
		/// <param name="c">The character to process.</param>
		/// <returns>True if the character is within the set, otherwise false.</returns>
		protected virtual bool IsHexDigit(char c)
		{
			if (
				(c >= '0' && c <= '9') ||
				(c >= 'a' && c <= 'f') ||
				(c >= 'A' && c <= 'F')
			)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determine if the specified character is within
		/// the set [a-zA-Z_].
		/// </summary>
		/// <param name="c">The character to process.</param>
		/// <returns>True if the character is within the set, otherwise false.</returns>
		protected virtual bool IsIdentifierStart(char c)
		{
			if (
				(c >= 'a' && c <= 'z') ||
				(c >= 'A' && c <= 'Z') ||
				(c == '_')
			)
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Determine if the specified character is within
		/// the set [a-zA-Z0-9_].
		/// </summary>
		/// <param name="c">The character to process.</param>
		/// <returns>True if the character is within the set, otherwise false.</returns>
		protected virtual bool IsIdentifier(char c)
		{
			if (
				(c >= 'a' && c <= 'z') ||
				(c >= 'A' && c <= 'Z') ||
				(c >= '0' && c <= '9') ||
				(c == '_')
			)
			{
				return true;
			}
			return false;
		}
	}
}
