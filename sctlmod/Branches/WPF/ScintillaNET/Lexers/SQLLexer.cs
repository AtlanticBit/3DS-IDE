using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace ScintillaNET.Lexers
{
	public class SQLLexer : CustomLexer
	{
		protected virtual bool IsMySQL { get { return false; } }

		private const int STYLE_COMMENT = 1;
		private const int STYLE_COMMENTLINE = 2;
		private const int STYLE_COMMENTDOC = 3;
		private const int STYLE_NUMBER = 4;
		private const int STYLE_WORD = 5;
		private const int STYLE_STRING = 6;
		private const int STYLE_CHARACTER = 7;
		private const int STYLE_SQLPLUS = 8;
		private const int STYLE_SQLPLUS_PROMPT = 9;
		private const int STYLE_OPERATOR = 10;
		private const int STYLE_IDENTIFIER = 11;
		private const int STYLE_SQLPLUS_COMMENT = 13;
		private const int STYLE_COMMENTLINEDOC = 15;
		private const int STYLE_WORD2 = 16;
		private const int STYLE_COMMENTDOCKEYWORD = 17;
		private const int STYLE_COMMENTDOCKEYWORDERROR = 18;
		private const int STYLE_USER1 = 19;
		private const int STYLE_USER2 = 20;
		private const int STYLE_USER3 = 21;
		private const int STYLE_USER4 = 22;
		private const int STYLE_QUOTEDIDENTIFIER = 23;
		
		private const int KEYWORDS_KEYWORDS1 = 0;
		private const int KEYWORDS_KEYWORDS2 = 1;
		private const int KEYWORDS_PLDOC = 2;
		private const int KEYWORDS_SQLPLUS = 3;
		private const int KEYWORDS_USER1 = 4;
		private const int KEYWORDS_USER2 = 5;
		private const int KEYWORDS_USER3 = 6;
		private const int KEYWORDS_USER4 = 7;

		public override string LexerName { get { return "sql"; } }
		public override Dictionary<string, int> StyleNameMapping
		{
			get 
			{
				return new Dictionary<string, int>()
				{
					{ "Comment", STYLE_COMMENT },
					{ "CommentLine", STYLE_COMMENTLINE },
					{ "CommentDoc", STYLE_COMMENTDOC },
					{ "Number", STYLE_NUMBER },
					{ "Word", STYLE_WORD },
					{ "String", STYLE_STRING },
					{ "Character", STYLE_CHARACTER },
					{ "SqlPlus", STYLE_SQLPLUS },
					{ "SqlPlus_Prompt", STYLE_SQLPLUS_PROMPT },
					{ "Operator", STYLE_OPERATOR },
					{ "Identifier", STYLE_IDENTIFIER },
					{ "SqlPlus_Comment", STYLE_SQLPLUS_COMMENT },
					{ "CommentLineDoc", STYLE_COMMENTLINEDOC },
					{ "Word2", STYLE_WORD2 },
					{ "CommentDocKeyword", STYLE_COMMENTDOCKEYWORD },
					{ "CommentDocKeywordError", STYLE_COMMENTDOCKEYWORDERROR },
					{ "User1", STYLE_USER1 },
					{ "User2", STYLE_USER2 },
					{ "User3", STYLE_USER3 },
					{ "User4", STYLE_USER4 },
					{ "QuotedIdentifier", STYLE_QUOTEDIDENTIFIER },
				};
			}
		}
		public override Dictionary<string, int> KeywordNameMapping
		{
			get 
			{
				return new Dictionary<string, int>()
				{
					{ "Keywords", KEYWORDS_KEYWORDS1 },
					{ "Database Objects", KEYWORDS_KEYWORDS2 },
					{ "PLDoc", KEYWORDS_PLDOC },
					{ "SQL*Plus", KEYWORDS_SQLPLUS },
					{ "SqlPlus", KEYWORDS_SQLPLUS },
					{ "User Keywords 1", KEYWORDS_USER1 },
					{ "User1", KEYWORDS_USER1 },
					{ "User Keywords 2", KEYWORDS_USER2 },
					{ "User2", KEYWORDS_USER2 },
					{ "User Keywords 3", KEYWORDS_USER3 },
					{ "User3", KEYWORDS_USER3 },
					{ "User Keywords 4", KEYWORDS_USER4 },
					{ "User4", KEYWORDS_USER4 },
				};
			}
		}
		protected override bool LowerKeywords { get { return true; } }
		protected override bool EnableCompositeKeywords { get { return true; } }
		public SQLLexer(Scintilla scintilla) : base(scintilla) { }

		new protected sealed class LexerOptions : CustomLexer.LexerOptions
		{
			public bool Fold { get; set; }
			/// <summary>
			/// This option enables SQL folding on the "ELSE" and "ELSIF" lines of an IF statement.
			/// </summary>
			[Description("This option enables SQL folding on the \"ELSE\" and \"ELSIF\" lines of an IF statement.")]
			[DefaultValue(false)]
			public bool FoldAtElse { get; set; }
			[DefaultValue(false)]
			public bool FoldComment { get; set; }
			[DefaultValue(false)]
			public bool FoldCompact { get; set; }
			[DefaultValue(false)]
			public bool FoldOnlyBegin { get; set; }
			[DefaultValue(false)]
			public bool SQLBackticksIdentifier { get; set; }
			/// <summary>
			/// If <c>true</c>, a line beginning with '#' will be a comment.
			/// </summary>
			[Description("If true a line beginning with '#' will be a comment.")]
			[DefaultValue(false)]
			public bool SQLNumberSignComment { get; set; }
			/// <summary>
			/// Enables backslash as an escape character in SQL.
			/// </summary>
			[Description("Enables backslash as an escape character in SQL.")]
			[DefaultValue(false)]
			public bool SQLBackslashEscapes { get; set; }
			/// <summary>
			/// If <c>true</c>, identifiers will be allowed to contain
			/// dots. (recommended for Oracle PL/SQL objects)
			/// </summary>
			[Description("If true, identifiers will be allowed to contain dots.")]
			[DefaultValue(false)]
			public bool SQLAllowDottedWord { get; set; }

			public override void ResetProperties()
			{
				this.Fold = false;
				this.FoldComment = false;
				this.FoldCompact = false;
				this.FoldOnlyBegin = false;
				this.SQLBackticksIdentifier = false;
				this.SQLNumberSignComment = false; 
				this.SQLBackslashEscapes = false;
				this.SQLAllowDottedWord = false;
			}

			public override void SetProperty(string name, string value)
			{
				switch (name.ToLower())
				{
					case "fold":
						this.Fold = (Bool(value) ?? (bool?)false).Value;
						break;
					case "fold.sql.at.else":
						this.FoldAtElse = (Bool(value) ?? (bool?)false).Value;
						break;
					case "fold.comment":
						this.FoldComment = (Bool(value) ?? (bool?)false).Value;
						break;
					case "fold.compact":
						this.FoldCompact = (Bool(value) ?? (bool?)false).Value;
						break;
					case "fold.sql.only.begin":
						this.FoldOnlyBegin = (Bool(value) ?? (bool?)false).Value;
						break;
					case "lexer.sql.backticks.identifier":
						this.SQLBackticksIdentifier = (Bool(value) ?? (bool?)false).Value;
						break;
					case "lexer.sql.numbersign.comment":
						this.SQLNumberSignComment = (Bool(value) ?? (bool?)false).Value;
						break;
					case "sql.backslash.escapes":
						this.SQLBackslashEscapes = (Bool(value) ?? (bool?)false).Value;
						break;
					case "lexer.sql.allow.dotted.word":
						this.SQLAllowDottedWord = (Bool(value) ?? (bool?)false).Value;
						break;
					default:
						base.SetProperty(name, value);
						break;
				}
			}
		}
		new protected LexerOptions Options
		{
			get { return (LexerOptions)base.Options; }
		}

		protected override void Initialize()
		{
			base.Options = new LexerOptions();
			// The PLDoc and SQL Plus keywords aren't in the composite list.
			EnsureKeywords(KEYWORDS_PLDOC, KEYWORDS_SQLPLUS);
			EnsureCompositeKeywords(
				new KeyValuePair<int, int>(KEYWORDS_KEYWORDS1, STYLE_WORD),
				new KeyValuePair<int, int>(KEYWORDS_KEYWORDS2, STYLE_WORD2),
				new KeyValuePair<int, int>(KEYWORDS_USER1, STYLE_USER1),
				new KeyValuePair<int, int>(KEYWORDS_USER2, STYLE_USER2),
				new KeyValuePair<int, int>(KEYWORDS_USER3, STYLE_USER3),
				new KeyValuePair<int, int>(KEYWORDS_USER4, STYLE_USER4)
			);
			base.Initialize();
		}

		private enum State : int
		{
			Unknown = STATE_UNKNOWN,
			Number,
			Identifier,
			QuotedIdentifier,
			Comment,
			CommentDoc,
			CommentLine,
			CommentLineDoc,
			SqlPlus_Comment,
			SqlPlus_Prompt,
			Character,
		}

		new private State CurrentState
		{
			get { return (State)base.CurrentState; }
			set { base.CurrentState = (int)value; }
		}

		protected override void InitializeStateFromStyle(int style)
		{
			switch (style)
			{
				case STYLE_COMMENT:
					CurrentState = State.Comment;
					break;
				case STYLE_COMMENTDOC:
					CurrentState = State.CommentDoc;
					break;
				// Otherwise we don't need to carry the
				// state on to the next line.
				default:
					break;
			}
		}

		protected override void Style()
		{
			var sA = Scintilla.Styles[STYLE_COMMENT];
			StartStyling();

			while (!EndOfText)
			{
				switch (CurrentState)
				{
					case State.Unknown:
						bool consumed = false;
						switch (CurrentCharacter)
						{
							case '#':
								if (!Options.SQLNumberSignComment)
									goto default;
								CurrentState = State.CommentLineDoc;
								break;
							case '`':
								if (!Options.SQLBackticksIdentifier)
									goto default;
								CurrentState = State.QuotedIdentifier;
								break;
							case '/':
								if (NextCharacter != '*')
									goto default;
								if (Lookahead(2) == '*' || Lookahead(2) == '!')
									CurrentState = State.CommentDoc;
								else
									CurrentState = State.Comment;
								Consume();
								break;
							case '-':
								if (NextCharacter != '-')
									goto default;
								CurrentState = State.CommentLine;
								break;
							case '\'':
								CurrentState = State.Character;
								break;
							case '"':
								Consume();
								ConsumeString(STYLE_STRING, '\\', true);
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
					case State.Number:
						if (!IsNumber(CurrentCharacter))
						{
							SetStyle(STYLE_NUMBER);
							CurrentState = State.Unknown;
						}
						else
							Consume();
						break;
					case State.Identifier:
						if (!IsIdentifier(CurrentCharacter) && (!Options.SQLAllowDottedWord || CurrentCharacter != '.'))
						{
							string ident = GetRange(CurrentBasePosition, CurrentPosition).ToLower();
							int style;
							if (CompositeKeywords.TryGetValue(ident, out style))
								SetStyle(style);
							else
								SetStyle(STYLE_IDENTIFIER);
							CurrentState = State.Unknown;
						}
						else
							Consume();
						break;
					case State.QuotedIdentifier:
						if (CurrentCharacter == '`')
						{
							if (NextCharacter == '`')
								Consume(2);
							else
							{
								Consume();
								SetStyle(STYLE_QUOTEDIDENTIFIER);
								CurrentState = State.Unknown;
							}
						}
						else
							Consume();
						break;
					case State.Comment:
						if (CurrentCharacter == '*' && NextCharacter == '/')
						{
							Consume(2);
							SetStyle(STYLE_COMMENT);
							CurrentState = State.Unknown;
						}
						else
							Consume();
						break;
					case State.CommentDoc:
						switch (CurrentCharacter)
						{
							case '*':
								if (NextCharacter != '/')
									goto default;
								Consume(2);
								SetStyle(STYLE_COMMENTDOC);
								CurrentState = State.Unknown;
								break;
							case '@':
							case '\\':
								if ((IsWhitespace(PreviousCharacter) || PreviousCharacter == '*') && !IsWhitespace(NextCharacter))
								{
									SetStyle(STYLE_COMMENTDOC);
									ConsumeCommentDocKeyword(true);
								}
								else
									Consume();
								break;
							default:
								Consume();
								break;
						}
						break;
					case State.CommentLine:
						ConsumeUntilEOL(STYLE_COMMENTLINE);
						CurrentState = State.Unknown;
						break;
					case State.CommentLineDoc:
						ConsumeUntilEOL(STYLE_COMMENTLINEDOC);
						CurrentState = State.Unknown;
						break;
					case State.SqlPlus_Comment:
						ConsumeUntilEOL(STYLE_SQLPLUS_COMMENT);
						CurrentState = State.Unknown;
						break;
					case State.SqlPlus_Prompt:
						ConsumeUntilEOL(STYLE_SQLPLUS_PROMPT);
						CurrentState = State.Unknown;
						break;
					case State.Character:
						switch (CurrentCharacter)
						{
							case '\\':
								if (!Options.SQLBackslashEscapes)
									goto default;
								Consume(2);
								break;
							case '\'':
								if (NextCharacter == '"')
								{
									Consume(2);
								}
								else
								{
									Consume();
									SetStyle(STYLE_CHARACTER);
									CurrentState = State.Unknown;
								}
								break;
							default:
								if (IsEndOfLine(CurrentCharacter))
								{
									MarkCurrentAsSyntaxError();
									SetStyle(STYLE_CHARACTER);
									CurrentState = State.Unknown;
								}
								else
									Consume();
								break;
						}
						break;
					default:
						throw new Exception("Unknown State!");
				}
			}
			switch (CurrentState)
			{
				case State.Unknown: break;
				case State.Number:
					SetStyle(STYLE_NUMBER);
					break;
				case State.Identifier:
					break;
				case State.QuotedIdentifier:
					MarkCurrentAsSyntaxError();
					SetStyle(STYLE_QUOTEDIDENTIFIER);
					break;
				case State.Comment:
					SetStyle(STYLE_COMMENT);
					StyleNextLine();
					break;
				case State.CommentDoc:
					SetStyle(STYLE_COMMENTDOC);
					StyleNextLine();
					break;
				case State.CommentLine:
					SetStyle(STYLE_COMMENTLINE);
					break;
				case State.CommentLineDoc:
					SetStyle(STYLE_COMMENTLINEDOC);
					break;
				case State.SqlPlus_Comment:
					SetStyle(STYLE_SQLPLUS_COMMENT);
					break;
				case State.SqlPlus_Prompt:
					SetStyle(STYLE_SQLPLUS_PROMPT);
					break;
				case State.Character:
					MarkCurrentAsSyntaxError();
					SetStyle(STYLE_CHARACTER);
					break;
				default:
					throw new Exception("Unknown State!");
			}

		}

		private void ConsumeCommentDocKeyword(bool blockComment)
		{
			while (!EndOfText)
			{
				switch (CurrentCharacter)
				{
					case '$':
					case '@':
					case '\\':
					case '&':
					case '<':
					case '>':
					case '#':
					case '{':
					case '}':
					case '[':
					case ']':
						Consume();
						break;
					case '*':
						if (blockComment && NextCharacter == '/')
						{
							MarkSyntaxError(CurrentBasePosition, CurrentPosition - CurrentBasePosition);
							SetStyle(STYLE_COMMENTDOCKEYWORDERROR);
							return;
						}
						goto default;
					default:
						if (CurrentCharacter >= 'a' && CurrentCharacter <= 'z')
							goto case '$';
						string cur = GetRange(CurrentBasePosition, CurrentPosition).ToLower();
						if (!IsWhitespace(CurrentCharacter) || !Keywords[KEYWORDS_PLDOC].Contains(cur.Substring(1)))
						{
							MarkSyntaxError(CurrentBasePosition, CurrentPosition - CurrentBasePosition);
							SetStyle(STYLE_COMMENTDOCKEYWORDERROR);
						}
						else
							SetStyle(STYLE_COMMENTDOCKEYWORD);
						return;
				}
			}
		}

	}
}
