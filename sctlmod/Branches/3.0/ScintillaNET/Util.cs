#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ScintillaNet.Properties;

#endregion Using Directives


namespace ScintillaNet
{
	// Various helper functions
	internal static class Util
	{
		#region Fields

		private static int _logPixelsY = -1;
		private static Encoding _systemEncoding;
		private static string _title;
		private static string _copyright;
		private static string _description;
		private static Version _version;

		#endregion Fields


		#region Methods

		public static string ColorToHtml(Color c)
		{
			return "#" + c.R.ToString("X2", null) + c.G.ToString("X2", null) + c.B.ToString("X2", null);
		}


		public static int ColorToRgb(Color c)
		{
			return c.R + (c.G << 8) + (c.B << 16);
		}


		public static string Format(string format, params object[] args)
		{
			// Simple wrapper around String.Format that uses
			// the current culture.

			return String.Format(CultureInfo.CurrentCulture, format, args ?? new string[0]);
		}


		private static Attribute GetAssemblyAttribute(Type assemblyType, Type attributeType)
		{
			Assembly assembly = assemblyType.Assembly;
			object[] attrs = assembly.GetCustomAttributes(attributeType, false);
			return attrs[0] as Attribute;
		}


		public static unsafe byte[] GetBytesZeroTerminated(string text, Encoding encoding)
		{
			int byteCount = encoding.GetByteCount(text);
			byte[] buffer = new byte[byteCount + 1];

			fixed (byte* bp = buffer)
			fixed (char* ch = text)
			{
				int count = encoding.GetBytes(ch, text.Length, bp, byteCount);
				Debug.Assert(count == byteCount);
			}

			// And the end cap...
			buffer[buffer.Length - 1] = 0;
			return buffer;
		}


		public static Keys GetKeys(char c)
		{
			switch (c)
			{
				case '/':
					return Keys.Oem2;
				case '`':
					return Keys.Oem3;
				case '[':
					return Keys.Oem4;
				case '\\':
					return Keys.Oem5;
				case ']':
					return Keys.Oem6;
				case '-':
					return (Keys)189;

			}

			return (Keys)Enum.Parse(typeof(Keys), c.ToString(), true);
		}


		public static Keys GetKeys(string s)
		{
			switch (s)
			{
				case "/":
					return Keys.Oem2;
				case "`":
					return Keys.Oem3;
				case "[":
					return Keys.Oem4;
				case "\\":
					return Keys.Oem5;
				case "]":
					return Keys.Oem6;
				case "-":
					return (Keys)189;
			}

			return (Keys)Enum.Parse(typeof(Keys), s, true);
		}


		public static uint GetMarkerMask(IEnumerable<int> markers)
		{
			uint mask = 0;
			foreach (int i in markers)
				mask |= ((uint)1) << i;
			return mask;
		}


		public static uint GetMarkerMask(IEnumerable<Marker> markers)
		{
			uint mask = 0;
			foreach (Marker m in markers)
				mask |= m.Mask;
			return mask;
		}


		public static string IntPtrToString(Encoding encoding, IntPtr ptr, int length)
		{
			//	null pointer = null string
			if (ptr == IntPtr.Zero)
				return null;

			//	0 length string = string.Empty
			if (length == 0)
				return string.Empty;

			byte[] buff = new byte[length];
			Marshal.Copy(ptr, buff, 0, length);

			//	We don't want to carry over the Trailing null
			if (buff[buff.Length - 1] == 0)
				length--;

			return encoding.GetString(buff, 0, length);
		}


		public static bool IsEnumDefined(int value, int minValue, int maxValue)
		{
			return ((value >= minValue) && (value <= maxValue));
		}


		/*
		public static bool IsValidShortcut(Keys shortcut)
		{
			// Shortcut should have a key and one or more modifiers.

			Keys keyCode = (Keys)(shortcut & Keys.KeyCode);
			Keys modifiers = (Keys)(shortcut & Keys.Modifiers);

			if (shortcut == Keys.None)
			{
				return false;
			}
			else if ((keyCode == Keys.Delete) || (keyCode == Keys.Insert))
			{
				return true;
			}
			else if (((int)keyCode >= (int)Keys.F1) && ((int)keyCode <= (int)Keys.F24))
			{
				// Function keys by themselves are valid
				return true;
			}
			else if ((keyCode != Keys.None) && (modifiers != Keys.None))
			{
				switch (keyCode)
				{
					case Keys.Menu:
					case Keys.ControlKey:
					case Keys.ShiftKey:
						// shift, control and alt arent valid on their own. 
						return false;
					default:
						if (modifiers == Keys.Shift)
						{
							// shift + somekey isnt a valid modifier either
							return false;
						}
						return true;
				}
			}
			// has to have a valid keycode and valid modifier.
			return false;
		}
		*/


		/*
		public static string PrintModificationType(int modificationType)
		{
			object[,] flags = new object[,]
			{
				{ (int)Constants.SC_MOD_INSERTTEXT, "SC_MOD_INSERTTEXT" },
				{ (int)Constants.SC_MOD_DELETETEXT, "SC_MOD_DELETETEXT" },
				{ (int)Constants.SC_MOD_CHANGESTYLE, "SC_MOD_CHANGESTYLE" },
				{ (int)Constants.SC_MOD_CHANGEFOLD, "SC_MOD_CHANGEFOLD" },
				{ (int)Constants.SC_PERFORMED_USER, "SC_PERFORMED_USER" },
				{ (int)Constants.SC_PERFORMED_UNDO, "SC_PERFORMED_UNDO" },
				{ (int)Constants.SC_PERFORMED_REDO, "SC_PERFORMED_REDO" },
				{ (int)Constants.SC_MULTISTEPUNDOREDO, "SC_MULTISTEPUNDOREDO" },
				{ (int)Constants.SC_LASTSTEPINUNDOREDO, "SC_LASTSTEPINUNDOREDO" },
				{ (int)Constants.SC_MOD_CHANGEMARKER, "SC_MOD_CHANGEMARKER" },
				{ (int)Constants.SC_MOD_BEFOREINSERT, "SC_MOD_BEFOREINSERT" },
				{ (int)Constants.SC_MOD_BEFOREDELETE, "SC_MOD_BEFOREDELETE" },
				{ (int)Constants.SC_MOD_CHANGEINDICATOR, "SC_MOD_CHANGEINDICATOR" },
				{ (int)Constants.SC_MOD_CHANGELINESTATE, "SC_MOD_CHANGELINESTATE" },
				{ (int)Constants.SC_MULTILINEUNDOREDO, "SC_MULTILINEUNDOREDO" },
				{ (int)Constants.SC_STARTACTION, "SC_STARTACTION" },
				{ (int)Constants.SC_MODEVENTMASKALL, "SC_MODEVENTMASKALL" }
			};

			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < flags.GetLength(0); i++)
			{
				if ((modificationType & (int)flags[i, 0]) == (int)flags[i, 0])
				{
					if (sb.Length > 0)
						sb.Append(" | ");

					sb.Append((string)flags[i, 1]);
				}
			}

			return sb.ToString();
		}
		*/


		public static void Reset(object value)
		{
			// Reset all the design-time object properties

			if (value == null)
				return;

			foreach (PropertyInfo pi in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				object[] attributes;

				// Look for a Browsable(false) attribute)
				attributes = pi.GetCustomAttributes(typeof(BrowsableAttribute), true);
				if (attributes != null && attributes.Length > 0 && ((BrowsableAttribute)attributes[0]).Browsable == false)
					continue;

				// Look for a DefaultValue(?) attribute
				attributes = pi.GetCustomAttributes(typeof(DefaultValueAttribute), true);
				if (attributes != null && attributes.Length > 0)
				{
					pi.SetValue(value, ((DefaultValueAttribute)attributes[0]).Value, null);
					continue;
				}

				// Look for a Reset* method
				MethodInfo mi = value.GetType().GetMethod("Reset" + pi.Name, new Type[0]);
				if (mi != null)
					mi.Invoke(value, null);
			}
		}


		public static Color RgbToColor(int color)
		{
			return Color.FromArgb(color & 0x0000ff, (color & 0x00ff00) >> 8, (color & 0xff0000) >> 16);
		}


		public static bool ShouldSerialize(object value)
		{
			// Determine if the object should be design-time serialized

			if (value == null)
				return false;

			foreach (PropertyInfo pi in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				object[] attributes;

				// Look for a Browsable(false) attribute
				attributes = pi.GetCustomAttributes(typeof(BrowsableAttribute), true);
				if (attributes != null && attributes.Length > 0 && ((BrowsableAttribute)attributes[0]).Browsable == false)
					continue;

				// Look for a DefaultValue(?) attribute
				attributes = pi.GetCustomAttributes(typeof(DefaultValueAttribute), true);
				if (attributes != null && attributes.Length > 0)
				{
					object v1 = pi.GetValue(value, null);
					object v2 = ((DefaultValueAttribute)attributes[0]).Value;
					if (v1 == v2 || v1.Equals(v2))
						continue;
				}

				// Look for a ShouldSerialize* method
				MethodInfo mi = value.GetType().GetMethod("ShouldSerialize" + pi.Name, new Type[0]);
				if (mi != null && (bool)mi.Invoke(value, null) == false)
					continue;

				return true;
			}

			return false;
		}


		/*
		public static int SignedHiWord(IntPtr hiWord)
		{
			return (short)(((int)(long)hiWord >> 0x10) & 0xffff);
		}
		*/


		public static int SignedLoWord(IntPtr loWord)
		{
			return (short)((int)(long)loWord & 0xffff);
		}


		public static void ValidateIndex(int index, string indexParamName, int min, string minParamName, int max, string maxParamName, string rangeParamName)
		{
			if (index < min || index > max)
			{
				string message = Format(Resources.Argument_InvalidIndex, index, indexParamName, minParamName, maxParamName);
				Debug.Assert(rangeParamName == null || rangeParamName.Length > 0);
				if (rangeParamName == null)
					throw new ArgumentOutOfRangeException(indexParamName, message);
				else
					throw new ArgumentException(message, rangeParamName);
			}
		}

		#endregion Methods


		#region Properties

		public static string Copyright
		{
			get
			{
				if (_copyright == null)
				{
					Attribute attr = GetAssemblyAttribute(typeof(Scintilla), typeof(AssemblyCopyrightAttribute));
					_copyright = ((AssemblyCopyrightAttribute)attr).Copyright;
				}

				return _copyright;
			}
		}


		public static string Description
		{
			get
			{
				if (_description == null)
				{
					Attribute attr = GetAssemblyAttribute(typeof(Scintilla), typeof(AssemblyDescriptionAttribute));
					_description = ((AssemblyDescriptionAttribute)attr).Description;
				}

				return _description;
			}
		}


		public static int LogPixelsY
		{
			get
			{
				// Get the number of pixels per logical inch along the screen height
				// (and cache it).
				if (_logPixelsY == -1)
				{
					IntPtr hDC = NativeMethods.GetDC(IntPtr.Zero);
					if (hDC != IntPtr.Zero)
					{
						_logPixelsY = NativeMethods.GetDeviceCaps(hDC, NativeMethods.LOGPIXELSY);
						NativeMethods.ReleaseDC(IntPtr.Zero, hDC);
					}
				}

				return _logPixelsY;
			}
		}


		public static Encoding SystemEncoding
		{
			get
			{
				// Get the default Win32 char encoding, aka TCHAR
				// (and cache it).
				if (_systemEncoding == null)
				{
					if (Marshal.SystemDefaultCharSize == 1)
						_systemEncoding = Encoding.Default;
					else
						_systemEncoding = Encoding.Unicode;
				}

				return _systemEncoding;
			}
		}


		public static string Title
		{
			get
			{
				if (_title == null)
				{
					Attribute attr = GetAssemblyAttribute(typeof(Scintilla), typeof(AssemblyTitleAttribute));
					_title = ((AssemblyTitleAttribute)attr).Title;
				}

				return _title;
			}
		}


		public static Version Version
		{
			get
			{
				if (_version == null)
				{
					Assembly assembly = typeof(Scintilla).Assembly;
					_version = assembly.GetName().Version;
				}

				return _version;
			}
		}

		#endregion Properties
	}
}
