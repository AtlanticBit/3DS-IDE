#region Using Directives

using System;
using System.Reflection;
using System.Globalization;
using ScintillaNet;

#endregion Using Directives


namespace ScintillaPad
{
	internal static class Util
	{
		#region Fields

		private static string _assemblyCopyright;
		private static string _assemblyDescription;
		private static string _assemblyTitle;
		private static Version _assemblyVersion;

		private static string _scintillaAssemblyTitle;
		private static string _scintillaAssemblyCopyright;
		private static Version _scintillaAssemblyVersion;

		#endregion Fields


		#region Properties

		public static string AssemblyCopyright
		{
			get
			{
				if (_assemblyCopyright == null)
				{
					Attribute attr = GetAssemblyAttribute(typeof(Util), typeof(AssemblyCopyrightAttribute));
					_assemblyCopyright = ((AssemblyCopyrightAttribute)attr).Copyright;
				}

				return _assemblyCopyright;
			}
		}


		public static string AssemblyDescription
		{
			get
			{
				if (_assemblyDescription == null)
				{
					Attribute attr = GetAssemblyAttribute(typeof(Util), typeof(AssemblyDescriptionAttribute));
					_assemblyDescription = ((AssemblyDescriptionAttribute)attr).Description;
				}

				return _assemblyDescription;
			}
		}


		public static string AssemblyTitle
		{
			get
			{
				if (_assemblyTitle == null)
				{
					Attribute attr = GetAssemblyAttribute(typeof(Util), typeof(AssemblyTitleAttribute));
					_assemblyTitle = ((AssemblyTitleAttribute)attr).Title;
				}

				return _assemblyTitle;
			}
		}


		public static Version AssemblyVersion
		{
			get
			{
				if (_assemblyVersion == null)
				{
					Assembly assembly = typeof(Util).Assembly;
					_assemblyVersion = assembly.GetName().Version;
				}

				return _assemblyVersion;
			}
		}


		public static string ScintillaAssemblyCopyright
		{
			get
			{
				if (_scintillaAssemblyCopyright == null)
				{
					Attribute attr = GetAssemblyAttribute(typeof(Scintilla), typeof(AssemblyCopyrightAttribute));
					_scintillaAssemblyCopyright = ((AssemblyCopyrightAttribute)attr).Copyright;
				}

				return _scintillaAssemblyCopyright;
			}
		}


		public static string ScintillaAssemblyTitle
		{
			get
			{
				if (_scintillaAssemblyTitle == null)
				{
					Attribute attr = GetAssemblyAttribute(typeof(Scintilla), typeof(AssemblyTitleAttribute));
					_scintillaAssemblyTitle = ((AssemblyTitleAttribute)attr).Title;
				}

				return _scintillaAssemblyTitle;
			}
		}


		public static Version ScintillaAssemblyVersion
		{
			get
			{
				if (_scintillaAssemblyVersion == null)
				{
					Assembly assembly = typeof(Scintilla).Assembly;
					_scintillaAssemblyVersion = assembly.GetName().Version;
				}

				return _scintillaAssemblyVersion;
			}
		}

		#endregion Properties


		#region Methods

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

		#endregion Methods
	}
}
