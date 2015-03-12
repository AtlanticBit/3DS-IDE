using System;
using System.Drawing;
using System.Collections.Generic;

namespace ScintillaNET.Lexers
{
	public sealed class IniLexer : CustomLexer
	{
		public override string LexerName { get { return "ini"; } }
		public override Dictionary<string, int> StyleNameMapping
		{
			get 
			{
				return new Dictionary<string, int>()
				{
					{ "Key", STYLE_KEY },
					{ "Value", STYLE_VALUE },
					{ "Assignment", STYLE_ASSIGNMENT },
					{ "Section", STYLE_SECTION },
					{ "Comment", STYLE_COMMENT },
					{ "Quoted", STYLE_QUOTED },
				}; 
			}
		}
		// This lexer has no keywords.
		public override Dictionary<string, int> KeywordNameMapping { get { return new Dictionary<string,int>(); } }

		private const int STYLE_KEY = 1;
		private const int STYLE_VALUE = 2;
		private const int STYLE_ASSIGNMENT = 3;
		private const int STYLE_SECTION = 4;
		private const int STYLE_COMMENT = 5;
		private const int STYLE_QUOTED = 6;

		public IniLexer(Scintilla scintilla) : base(scintilla) { }

		protected override void Initialize()
		{
			base.Initialize();
			Scintilla.Indentation.SmartIndentType = SmartIndent.None;
		}

		private enum State : int
		{
			Unknown = STATE_UNKNOWN,
			Whitespace,
			Section,
			Comment,
			Key,
			Value,
		}

		new private State CurrentState
		{
			get { return (State)base.CurrentState; }
			set { base.CurrentState = (int)value; }
		}
		protected override void Style()
		{
			StartStyling();

			while (!EndOfText)
			{
				switch (CurrentState)
				{
					case State.Unknown:
						switch (CurrentCharacter)
						{
							case '[':
								TryLeaveFoldLevel();
								EnterFoldLevel();
								CurrentState = State.Section;
								break;
							case ';':
								CurrentState = State.Comment;
								break;
							default:
								if (IsWhitespace(CurrentCharacter))
								{
									CurrentState = State.Whitespace;
								}
								else if (IsIdentifier(CurrentCharacter))
								{
									CurrentState = State.Key;
								}
								else
								{
									SetStyle(STYLE_DEFAULT);
									MarkSyntaxError(CurrentPosition, 1);
								}
								break;
						}
						Consume();
						break;
					case State.Whitespace:
						if (IsWhitespace(CurrentCharacter))
							Consume();
						else
						{
							SetStyle(STYLE_DEFAULT);
							CurrentState = State.Unknown;
						}
						break;
					case State.Section:
						if (CurrentCharacter == ']')
						{
							Consume();
							SetStyle(STYLE_SECTION);
							CurrentState = State.Unknown;
						}
						else
							Consume();
						break;
					case State.Comment:
						if (IsEndOfLine(CurrentCharacter))
						{
							SetStyle(STYLE_COMMENT);
							CurrentState = State.Unknown;
						}
						else
							Consume();
						break;
					case State.Key:
						switch (CurrentCharacter)
						{
							case ';':
								SetStyle(STYLE_KEY);
								CurrentState = State.Comment;
								break;
							case '=':
								SetStyle(STYLE_KEY);
								Consume();
								SetStyle(STYLE_ASSIGNMENT);
								CurrentState = State.Value;
								break;
							default:
								if (IsEndOfLine(CurrentCharacter))
								{
									SetStyle(STYLE_KEY);
									CurrentState = State.Unknown;
								}
								else
									Consume();
								break;
						}
						break;
					case State.Value:
						switch (CurrentCharacter)
						{
							case ';':
								SetStyle(STYLE_VALUE);
								CurrentState = State.Comment;
								break;
							case '"':
								SetStyle(STYLE_VALUE);
								Consume();
								ConsumeString(STYLE_QUOTED, '\\');
								break;
							case '=':
							case ',':
							case '(':
							case ')':
								SetStyle(STYLE_VALUE);
								Consume();
								SetStyle(STYLE_ASSIGNMENT);
								CurrentState = State.Value;
								break;
							default:
								if (IsEndOfLine(CurrentCharacter))
								{
									SetStyle(STYLE_VALUE);
									CurrentState = State.Unknown;
								}
								else
									Consume();
								break;
						}
						break;
					default:
						throw new Exception("Unknown state!");
				}
			}

			switch (CurrentState)
			{
				case State.Unknown: break;
				case State.Whitespace:
					SetStyle(STYLE_DEFAULT);
					break;
				case State.Section:
					SetStyle(STYLE_SECTION);
					break;
				case State.Comment:
					SetStyle(STYLE_COMMENT);
					break;
				case State.Key:
					SetStyle(STYLE_KEY);
					break;
				case State.Value:
					SetStyle(STYLE_VALUE);
					break;
				default:
					throw new Exception("Unknown state!");
			}
		}
	}
}
