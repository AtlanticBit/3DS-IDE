using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace ScintillaNet.Configuration
{
	public class StyleConfigList : List<StyleConfig>
	{
		private bool? _inherit;
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

	}

	public class StyleConfig
	{
		private StyleCase? _case;
		public StyleCase? Case
		{
			get
			{
				return _case;
			}
			set
			{
				_case = value;
			}
		}

		private Color _foreColor;
		public Color ForeColor
		{
			get
			{
				return _foreColor;
			}
			set
			{
				_foreColor = value;
			}
		}

		private Color _backColor;
		public Color BackColor
		{
			get
			{
				return _backColor;
			}
			set
			{
				_backColor = value;
			}
		}

		private int? _number;
		public int? Number
		{
			get
			{
				return _number;
			}
			set
			{
				_number = value;
			}
		}

		private string _name;
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

		private bool? _inherit;
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

		public override string ToString()
		{
			return "Name = \"" + _name + "\" Number=" + _number.ToString();
		}
	}

	public class ResolvedStyleList : Dictionary<int, StyleConfig>
	{

		public ResolvedStyleList()
		{
			
		}

		public StyleConfig FindByName(string name)
		{
			
			foreach (StyleConfig item in this.Values)
			{
				if (item.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
					return item;
			}

			return null;
		}

	}
}
