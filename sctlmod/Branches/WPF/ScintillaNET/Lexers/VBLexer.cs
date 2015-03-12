using System;
using System.Collections.Generic;

namespace ScintillaNET.Lexers
{
	public abstract class VBCoreLexer : CustomLexer
	{
		protected abstract bool IsVBScript { get; }

		private const int STYLE_COMMENT = 1;
		private const int STYLE_NUMBER = 2;
		private const int STYLE_KEYWORD = 3;
		private const int STYLE_STRING = 4;
		private const int STYLE_PREPROCESSOR = 5;
		private const int STYLE_OPERATOR = 6;
		private const int STYLE_IDENTIFIER = 7;
		private const int STYLE_DATE = 8;
		private const int STYLE_KEYWORD2 = 9;
		private const int STYLE_KEYWORD3 = 10;
		private const int STYLE_KEYWORD4 = 11;

		private const int KEYWORDS_KEYWORDS = 0;
		private const int KEYWORDS_USER1 = 1;
		private const int KEYWORDS_USER2 = 2;
		private const int KEYWORDS_USER3 = 3;

		public override Dictionary<string, int> StyleNameMapping
		{
			get 
			{
				return new Dictionary<string, int>()
				{
					{ "Comment", STYLE_COMMENT },
					{ "Number", STYLE_NUMBER },
					{ "Keyword", STYLE_KEYWORD },
					{ "Keyword1", STYLE_KEYWORD },
					{ "String", STYLE_STRING },
					{ "PreProcessor", STYLE_PREPROCESSOR },
					{ "Operator", STYLE_OPERATOR },
					{ "Identifier", STYLE_IDENTIFIER },
					{ "Date", STYLE_DATE },
					{ "Keyword2", STYLE_KEYWORD2 },
					{ "Keyword3", STYLE_KEYWORD3 },
					{ "Keyword4", STYLE_KEYWORD4 },
				};
			}
		}
		public override Dictionary<string, int> KeywordNameMapping
		{
			get 
			{
				return new Dictionary<string, int>()
				{
					{ "Keywords", KEYWORDS_KEYWORDS },
					{ "User1", KEYWORDS_USER1 },
					{ "User2", KEYWORDS_USER2 },
					{ "User3", KEYWORDS_USER3 },
				};
			}
		}
		protected override bool EnableCompositeKeywords { get { return true; } }
		protected override bool LowerKeywords { get { return true; } }
		protected VBCoreLexer(Scintilla scintilla) : base(scintilla) { }

		protected override void Initialize()
		{
			EnsureCompositeKeywords(
				new KeyValuePair<int, int>(KEYWORDS_KEYWORDS, STYLE_KEYWORD),
				new KeyValuePair<int, int>(KEYWORDS_USER1, STYLE_KEYWORD2),
				new KeyValuePair<int, int>(KEYWORDS_USER2, STYLE_KEYWORD3),
				new KeyValuePair<int, int>(KEYWORDS_USER3, STYLE_KEYWORD4)
			);
			base.Initialize();
		}

		private enum State : int
		{
			Unknown = STATE_UNKNOWN,
			Identifier,
			Number,
			String,
			Comment,
			PreProcessor,
			FileNumber,
			Date,
		}

		new private State CurrentState
		{
			get { return (State)base.CurrentState; }
			set { base.CurrentState = (int)value; }
		}

		// Extended to accept accented characters
		protected override bool IsIdentifier(char c) { return c >= 0x80 || c == '.' || base.IsIdentifier(c); }
		protected override bool IsIdentifierStart(char c) { return c >= 0x80 || base.IsIdentifierStart(c); }
		private bool IsTypeCharacter(char c)
		{
			switch (c)
			{
				case '%':
				case '&':
				case '@':
				case '!':
				case '#':
				case '$':
					return true;
				default:
					return false;
			}
		}

		protected override void Style()
		{
			StartStyling();

			while (!EndOfText)
			{
				switch (CurrentState)
				{
					case State.Unknown:
						bool consumed = false;
						switch (CurrentCharacter)
						{
							case '\'':
								CurrentState = State.Comment;
								break;
							case '"':
								CurrentState = State.String;
								break;
							case '#':
								if (CurrentColumn == 0)
									CurrentState = State.PreProcessor;
								else
									CurrentState = State.FileNumber;
								break;
							case '&':
								Consume();
								switch (CurrentCharacter)
								{
									case 'h':
									case 'H':
										CurrentState = State.Number;
										break;
									case 'o':
									case 'O':
										CurrentState = State.Number;
										break;
									default:
										if (IsWhitespace(CurrentCharacter))
										{
											SetStyle(STYLE_OPERATOR);
											consumed = true;
										}
										else
										{
											MarkSyntaxError(CurrentPosition - 1, 2);
											Consume();
											SetStyle(STYLE_DEFAULT);
											consumed = true;
										}
										break;
								}
								break;
							case '[':
								CurrentState = State.Identifier;
								break;
							case '\\':
								Consume();
								SetStyle(STYLE_OPERATOR);
								consumed = true;
								break;
							default:
								if (
									IsDigit(CurrentCharacter) || 
									(CurrentCharacter == '.' && IsDigit(NextCharacter))
								)
								{
									CurrentState = State.Number;
								}
								else if (IsIdentifierStart(CurrentCharacter))
								{
									CurrentState = State.Identifier;
								}
								else if (IsOperator(CurrentCharacter))
								{
									Consume();
									SetStyle(STYLE_OPERATOR);
									consumed = true;
								}
								else if (IsWhitespace(CurrentCharacter))
								{
									ConsumeWhitespace();
									consumed = true;
								}
								else
								{
									MarkSyntaxError(CurrentPosition, 1);
									Consume();
									SetStyle(STYLE_DEFAULT);
									consumed = true;
								}
								break;
						}
						if (!consumed)
							Consume();
						break;
					case State.Identifier:
						if (!IsIdentifier(CurrentCharacter))
						{
							bool skipType = false;
							if (!IsVBScript && IsTypeCharacter(CurrentCharacter))
							{
								Consume();
								skipType = true;
							}
							if (CurrentCharacter == ']')
								Consume();
							var ident = GetRange(CurrentBasePosition, CurrentPosition).ToLower();
							if (skipType)
								ident = ident.Substring(0, ident.Length - 1);
							if (ident == "rem")
							{
								CurrentState = State.Comment;
								break;
							}
							else
							{
								int style;
								if (CompositeKeywords.TryGetValue(ident, out style))
									SetStyle(style);
								else
									SetStyle(STYLE_IDENTIFIER);
							}
							CurrentState = State.Unknown;
						}
						else
							Consume();
						break;
					case State.Number:
						if (!IsNumber(CurrentCharacter) && !IsHexDigit(CurrentCharacter))
						{
							SetStyle(STYLE_NUMBER);
							CurrentState = State.Unknown;
						}
						else
							Consume();
						break;
					case State.String:
						if (CurrentCharacter == '"')
						{
							if (NextCharacter == '"')
							{
								Consume(2);
							}
							else
							{
								if (NextCharacter == 'c')
									Consume();
								Consume();
								SetStyle(STYLE_STRING);
								CurrentState = State.Unknown;
							}
						}
						else if (IsEndOfLine(CurrentCharacter))
						{
							MarkSyntaxError(CurrentBasePosition, CurrentPosition - CurrentBasePosition);
							SetStyle(STYLE_STRING);
							CurrentState = State.Unknown;
						}
						else
							Consume();
						break;
					case State.Comment:
						if (IsEndOfLine(CurrentCharacter))
						{
							Consume();
							SetStyle(STYLE_COMMENT);
							CurrentState = State.Unknown;
						}
						else
							Consume();
						break;
					case State.PreProcessor:
						if (IsEndOfLine(CurrentCharacter))
						{
							Consume();
							SetStyle(STYLE_PREPROCESSOR);
							CurrentState = State.Unknown;
						}
						else
							Consume();
						break;
					case State.FileNumber:
						switch (CurrentCharacter)
						{
							case '\r':
							case '\n':
							case ',':
								SetStyle(STYLE_NUMBER);
								CurrentState = State.Unknown;
								break;
							case '#':
								Consume();
								SetStyle(STYLE_DATE);
								CurrentState = State.Unknown;
								break;
							default:
								if (IsDigit(CurrentCharacter))
								{
									if (CurrentPosition - CurrentBasePosition > 3)
										CurrentState = State.Date;
									else
										Consume();
								}
								else
									CurrentState = State.Date;
								break;
						}
						break;
					case State.Date:
						if (CurrentCharacter == '#')
						{
							Consume();
							SetStyle(STYLE_DATE);
							CurrentState = State.Unknown;
						}
						else if (IsEndOfLine(CurrentCharacter))
						{
							MarkSyntaxError(CurrentBasePosition, CurrentPosition - CurrentBasePosition);
							SetStyle(STYLE_DATE);
							CurrentState = State.Unknown;
						}
						else
							Consume();
						break;
					default:
						throw new Exception("Unknown State!");
				}
			}
		}
	}

	public sealed class VBLexer : VBCoreLexer
	{
		public override string LexerName { get { return "vb"; } }
		protected override bool IsVBScript { get { return false; } }

		public VBLexer(Scintilla scintilla) : base(scintilla) { }
	}

	public sealed class VBScriptLexer : VBCoreLexer
	{
		public override string LexerName { get { return "vbscript"; } }
		protected override bool IsVBScript { get { return true; } }

		public VBScriptLexer(Scintilla scintilla) : base(scintilla) { }
	}

	public sealed class SmallBasicLexer : VBCoreLexer
	{
		public override string LexerName { get { return "smallbasic"; } }
		protected override bool IsVBScript { get { return false; } }

		protected override bool IsIdentifier(char c) { return c >= 0x80 || base.IsIdentifier(c); }

		public SmallBasicLexer(Scintilla scintilla) : base(scintilla) { }
	}
}
