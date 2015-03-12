#region Using Directives

using System;
using System.Collections.ObjectModel;
using System.Text;

#endregion Using Directives


namespace ScintillaNET.Configuration
{
    public class KeyWordConfig
    {
        #region Fields

        private bool? _inherit;
		private int? _list;
		private string _name;
        private string _value;

        #endregion Fields


        #region Properties

        public bool? Inherit
        {
            get
            {
                return _inherit;
            }
            set
            {
                _inherit = value;
            }
        }


        public int? List
        {
            get
            {
                return _list;
            }
            set
            {
                _list = value;
            }
        }


		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}


        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        #endregion Properties


        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the KeyWordConfig class.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <param name="inherit"></param>
        public KeyWordConfig(int list, string value, bool? inherit)
        {
            _list = list;
            _value = value;
            _inherit = inherit;
        }

		public KeyWordConfig(string name, string value, bool? inherit)
		{
			_name = name;
			_value = value;
			_inherit = inherit;
		}

		public KeyWordConfig(string name, int? list, string value, bool? inherit)
		{
			_name = name;
			_list = list;
			_value = value;
			_inherit = inherit;
		}
        #endregion Constructors
    }
}
