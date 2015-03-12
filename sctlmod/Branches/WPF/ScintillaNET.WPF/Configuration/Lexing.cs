using System;
using System.ComponentModel;

namespace ScintillaNET.WPF.Configuration
{
	public sealed class Lexing : ScintillaWPFConfigItem
	{
		private Lexer? mLexer;
		public Lexer Lexer
		{
			get { return mLexer.Value; }
			set { mLexer = value; TryApplyConfig(); }
		}

		private string mLexerName;
		public string LexerName 
		{
			get { return mLexerName; }
			set { mLexerName = value; TryApplyConfig(); }
		}
		
		private string mLineCommentPrefix;
		public string LineCommentPrefix 
		{
			get { return mLineCommentPrefix; }
			set { mLineCommentPrefix = value; TryApplyConfig(); }
		}
		
		private string mStreamCommentPrefix;
		public string StreamCommentPrefix 
		{
			get { return mStreamCommentPrefix; }
			set { mStreamCommentPrefix = value; TryApplyConfig(); }
		}

		private string mStreamCommentSuffix;
#warning Note to self: this property is spelled wrong in the main library.
		public string StreamCommentSuffix 
		{
			get { return mStreamCommentSuffix; }
			set { mStreamCommentSuffix = value; TryApplyConfig(); }
		}

		internal override void ApplyConfig(ScintillaWPF scintilla)
		{
			base.ApplyConfig(scintilla);
			if (mLexer != null)
				scintilla.Lexing.Lexer = Lexer;
			if (LexerName != null)
				scintilla.Lexing.LexerName = LexerName;
			if (LineCommentPrefix != null)
				scintilla.Lexing.LineCommentPrefix = LineCommentPrefix;
			if (StreamCommentPrefix != null)
				scintilla.Lexing.StreamCommentPrefix = StreamCommentPrefix;
			if (StreamCommentSuffix != null)
				scintilla.Lexing.StreamCommentSufix = StreamCommentSuffix;
		}

		internal override void Reset(ScintillaWPF scintilla)
		{
#warning Need to reset things to their default values.
		}
	}
}
