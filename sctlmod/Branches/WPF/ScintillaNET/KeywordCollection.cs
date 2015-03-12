#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;

#endregion Using Directives


namespace ScintillaNET
{
    public class KeywordCollection : TopLevelHelper
    {
        #region Fields

        private string[] _keywords;
		private bool _modified = false;

        #endregion Fields

		internal bool Modified
		{
			get { return _modified; }
			set { _modified = value; }
		}

        #region Indexers

        public string this[int keywordSet]
        {
            get
            {
                return _keywords[keywordSet];
            }
            set
            {
				if (_keywords[keywordSet] != value)
				{
					_keywords[keywordSet] = value;
					NativeScintilla.SetKeywords(keywordSet, value);
					_modified = true;
				}
            }
        }


        public string this[string keywordListName]
        {
            get
            {
                return this[Scintilla.Lexing.KeywordNameMap[keywordListName]];
            }
            set
            {
				this[Scintilla.Lexing.KeywordNameMap[keywordListName]] = value;
            }
        }

        #endregion Indexers


        #region Constructors

        internal KeywordCollection(Scintilla scintilla) : base(scintilla)
        {
			_keywords = new string[Constants.KEYWORDSET_MAX];
			for (int i = 0; i < _keywords.Length; i++)
				_keywords[i] = "";
        }

        #endregion Constructors
    }
}
