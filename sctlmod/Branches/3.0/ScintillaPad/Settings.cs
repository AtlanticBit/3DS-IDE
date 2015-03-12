#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

#endregion Using Directives


namespace ScintillaPad
{
	// Settings are stored in an INI (compatible) file. I personally can't
	// stand the Visual Studio settings designer and API. Our version is also
	// better than strait XML because it's faster, easier to work with, and
	// easier to upgrade from version to version.
	internal sealed class Settings
	{
		#region Constants

		// Keys
		public const string MainFormBounds = "MainForm.Bounds";
		public const string ViewStatusBar = "View.StatusBar";
		public const string ViewZoomFactor = "View.ZoomFactor";
		public const string FormatWordWrap = "Format.WordWrap";
		public const string FormatFont = "Format.Font";

		#endregion Constants


		#region Fields

		// Use RegexBuddy if you need an explanation ;)
		private static readonly Regex _keyValueRegex = new Regex(@"^\s*(?<key>[^\s\=\;]+)\s*[=]\s*(?:""(?<value>[^""\\]*(?:\\.[^""\\]*)*)""|(?<value>[^\s\\\;]*(?:\\.[^\s\\\;]*)*))");
		private static readonly Regex _unescapeCharsRegex = new Regex(@"(?:\\\;|\\""|\\r|\\n|\\\\)");
		private static readonly Regex _escapeCharsRegex = new Regex(@"[\;\""\r\n\\]");
		private static readonly Regex _whitespaceCharsRegex = new Regex(@"\s");

		private string _filePath;
		private Dictionary<string, object> _cache;

		#endregion Fields


		#region Methods

		public T Get<T>(string key)
		{
			Debug.Assert(!String.IsNullOrEmpty(key), "The 'key' argument was null or empty.");

			// This really shouldn't happen, so just return
			// the type default if there is no key.
			object value;
			if (!_cache.TryGetValue(key, out value))
				return default(T);

			// After loading the settings, all values in the cache
			// are string types. If T is not a string type, convert
			// the value and update the cache.
			if (value != null)
			{
				if (typeof(T) != typeof(string) && typeof(T) != value.GetType())
				{
					TypeConverter tc = TypeDescriptor.GetConverter(typeof(T));
					value = tc.ConvertFrom(value);
					_cache[key] = value;
				}
			}

			return (T)value;
		}


		public void Load()
		{
			using (StreamReader sr = new StreamReader(_filePath))
			{
				string line;
				while ((line = sr.ReadLine()) != null)
				{
					// Read the name value pairs (we don't use sections)
					Match m = _keyValueRegex.Match(line);
					if (!m.Success)
						continue;

					string key = m.Groups["key"].Value;
					string value = m.Groups["value"].Value;
					
					// Unescape
					value = _unescapeCharsRegex.Replace(value, (me) =>
						{
							switch (me.Value)
							{
								case "\\;":
									return ";";
								case "\\\"":
									return "\"";
								case "\\r":
									return "\r";
								case "\\n":
									return "\n";
								case "\\\\":
									return "\\";
								default:
									return me.Value;
							}
						});

					// Add the key if we recognize it
					if (_cache.ContainsKey(key))
						_cache[key] = value;
				}
			}
		}


		public void Reset()
		{
			_cache.Clear();

			_cache[MainFormBounds] = Rectangle.Empty;
			_cache[ViewStatusBar] = false;
			_cache[FormatWordWrap] = true;
			_cache[FormatFont] = new Font("Lucida Console", 9.75f);
			_cache[ViewZoomFactor] = 0;
		}


		public void Save()
		{
			using (StreamWriter sw = new StreamWriter(_filePath))
			{
				// ... a friendly reminder :)
				sw.WriteLine("; This might void your warranty!");
				sw.WriteLine();
				sw.WriteLine("; Manually changing these settings can be harmful to the stability, security");
				sw.WriteLine("; and performance of this application. You should only modify this file if you");
				sw.WriteLine("; are sure of what you are doing.");
				sw.WriteLine();

				foreach (KeyValuePair<string, object> kvp in _cache)
				{
					sw.Write(kvp.Key);
					sw.Write('=');

					// Convert to string
					object obj = kvp.Value ?? String.Empty;
					TypeConverter tc = TypeDescriptor.GetConverter(obj.GetType());
					string value = tc.ConvertToString(obj);

					// Escape
					value = _escapeCharsRegex.Replace(value, (me) =>
					{
						switch (me.Value)
						{
							case ";":
								return "\\;";
							case "\"":
								return "\\\"";
							case "\r":
								return "\\r";
							case "\n":
								return "\\n";
							case "\\":
								return "\\\\";
							default:
								return me.Value;
						}
					});

					if (_whitespaceCharsRegex.Match(value).Success)
					{
						// Bracket the value in double-quotes
						sw.Write("\"");
						sw.Write(value);
						sw.WriteLine("\"");
					}
					else
					{
						sw.WriteLine(value);
					}
				}
			}
		}


		public void Set(string key, object value)
		{
			Debug.Assert(!String.IsNullOrEmpty(key), "The 'key' argument cannot be null or empty.");
			Debug.Assert(!key.Contains("="), "The '=' char is not allowed in key names.");

			_cache[key] = value;
		}

		#endregion Methods


		#region Constructors

		public Settings(string filePath)
		{
			Debug.Assert(!String.IsNullOrEmpty(filePath));

			_cache = new Dictionary<string, object>();
			_filePath = filePath;

			// Prime the cache with usable keys (and values)
			Reset();
		}

		#endregion Constructors
	}
}
