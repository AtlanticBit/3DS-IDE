#region Using Directives

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion Using Directives


namespace InterfaceParser
{
	// Parses the Scintilla.iface file and spits out some formatted declarations
	// because I'm tired of copying these by hand (and I don't much care
	// for having to edit Scintilla's python scripts).
	internal static class Program
	{
		#region Fields

		private static readonly Regex _featureRegex = new Regex(@"^(?<feature>cat|fun|get|set|val|evt|enu|lex)\s+(?<define>.*)");
		private static readonly Regex _keyValueRegex = new Regex(@"(?<key>\w+)\s*=\s*(?<value>-?(?:\dx)?\d+)");
		private static readonly Regex _prefixRefex = new Regex(@"[^_]+_");

		#endregion Fields


		#region Methods

		private static void Main(string[] args)
		{
			try
			{
				MainImp(args);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}


		private static void MainImp(string[] args)
		{
			if (args.Length == 0 || args[0] == "/?")
			{
				Console.WriteLine("Please specify the path to the Scintilla.iface file.");
				Console.WriteLine("EXAMPLE: InterfaceParser.exe Scintilla.iface > output.txt");
				return;
			}

			Dictionary<string, List<Constant>> constants = new Dictionary<string, List<Constant>>();
			List<string> lexers = new List<string>();
			using (StreamReader sr = new StreamReader(args[0]))
			{
				string line = null;
				while ((line = sr.ReadLine()) != null)
				{
					// Find definitions (and ignore the rest)
					Match m = _featureRegex.Match(line);
					if (!m.Success)
						continue;

					string prefix = null;
					string constant = null;
					string value = null;

					string feature = m.Groups["feature"].Value;
					string define = m.Groups["define"].Value;

					if (feature == "cat")
					{
						if (!define.StartsWith("Deprecated"))
							continue;

						// This is the end of the list for us
						break;
					}
					else
					{
						// Parse out the constants and organize them by prefix
						Match kvp = _keyValueRegex.Match(define);
						constant = kvp.Groups["key"].Value.ToUpper();
						value = kvp.Groups["value"].Value;

						if (feature == "lex" || feature == "enu")
						{
							continue;
						}
						if (feature == "fun" || feature == "get" || feature == "set")
						{
							prefix = "SCI_";
						}
						else if (feature == "evt")
						{
							prefix = "SCN_";
						}
						else
						{
							prefix = _prefixRefex.Match(constant).Value;
							
							// Lexer states have a longer prefix
							if (prefix == "SCE_")
								prefix += _prefixRefex.Match(constant, 4).Value;
						}
					}

					if (!constants.ContainsKey(prefix))
						constants.Add(prefix, new List<Constant>());

					constants[prefix].Add(new Constant
						{
							Name = (constant.StartsWith(prefix) ? constant : prefix + constant),
							Value = value
						});
				}
			}

			// Print the results to the console. Use a pipe to save as file.
			foreach (KeyValuePair<string, List<Constant>> kvp in constants)
			{
				Console.WriteLine("public const uint");
				for (int i = 0; i < kvp.Value.Count; i++)
				{
					Console.Write("\t");
					Console.Write(kvp.Value[i].Name);
					Console.Write(" = ");
					if (kvp.Value[i].Value.StartsWith("-"))
						Console.Write("unchecked((uint)");
					Console.Write(kvp.Value[i].Value);
					if(kvp.Value[i].Value.StartsWith("-"))
						Console.Write(")");
					Console.WriteLine(i == kvp.Value.Count - 1 ? ";" : ",");
				}
				Console.WriteLine();
			}
		}

		#endregion Methods


		#region Types

		private struct Constant
		{
			public string Name;
			public string Value;
		}

		#endregion Types
	}
}
