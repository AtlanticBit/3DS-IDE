using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using ScintillaNET.Configuration;

namespace ScintillaNET.Configuration
{
    /// <summary>
    /// Provides methods for loading common language configurations in a <see cref="Scintilla" /> control.
    /// </summary>
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class ConfigurationManager : TopLevelHelper
    {
        #region Fields

        private static Dictionary<string, Dictionary<string, int>> _keywordListNames;
        private static Dictionary<string, Dictionary<string, int>> _styleNames;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Applies the specified predefined configuration.
        /// </summary>
        /// <param name="language">One of the <see cref="ConfigurationLanguage" /> enumeration values.</param>
        public void Apply(ConfigurationLanguage language)
        {
            // Map to an embedded resource
            var resourceName = string.Format(CultureInfo.InvariantCulture, "ScintillaNET.Configuration.Builtin.{0}.xml", language.ToString().ToLower());
            var stream = GetType().Assembly.GetManifestResourceStream(resourceName);

            Apply(stream);
        }

        /// <summary>
        /// Applies the language configuration from the specified XML stream.
        /// </summary>
        /// <param name="stream">The configuration XML stream.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream" /> is null.</exception>
        /// <exception cref="ArgumentException">An XML configuration could not be read from <paramref name="stream" />.</exception>
        public void Apply(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            try
            {
                // Our parser is intentionally unforgiving...
                var configuration = new XmlDocument();
                configuration.Load(stream);
                var root = configuration.DocumentElement;

                // Lexing
                var lexingNode = root.SelectSingleNode("lexing");
                if (lexingNode != null)
                {
                    ApplyAttributesToProperties(lexingNode.Attributes.Cast<XmlAttribute>(), typeof(Lexing), Scintilla.Lexing);
                    foreach (XmlNode keywordsNode in lexingNode.SelectNodes("keywords"))
                    {
                        var keywordsName = keywordsNode.Attributes["name"].Value;
                        var keywordsIndex = GetKeywordListNames(lexingNode.Attributes["lexer"].Value)[keywordsName];
                        Scintilla.Lexing.Keywords[keywordsIndex] = keywordsNode.InnerText.Trim();
                    }
                }

                // Styles
                var stylesNode = root.SelectSingleNode("styles");
                if (stylesNode != null)
                {
                    ApplyAttributesToProperties(stylesNode.Attributes.Cast<XmlAttribute>().Where(a => a.Name != "names"), typeof(StyleCollection), Scintilla.Styles);
                    var language = (root.SelectSingleNode("lexing/@lexer") ?? root.SelectSingleNode("styles/@names")).Value; // Alternate
                    foreach (XmlNode styleNode in stylesNode.SelectNodes("style"))
                    {
                        var styleName = styleNode.Attributes["name"].Value;
                        var styleIndex = GetStyleNames(language)[styleName];
                        var style = Scintilla.Styles[styleIndex];
                        ApplyAttributesToProperties(styleNode.Attributes.Cast<XmlAttribute>().Where(a => a.Name != "name"), style.GetType(), style);
                    }
                }

                // Snippets
                var snippetsNode = root.SelectSingleNode("snippets");
                if (snippetsNode != null)
                {
                    ApplyAttributesToProperties(snippetsNode.Attributes.Cast<XmlAttribute>(), typeof(SnippetManager), Scintilla.Snippets);
                    foreach (XmlNode snippetNode in snippetsNode.SelectNodes("snippet"))
                    {
                        var shortcut = snippetNode.Attributes["shortcut"].Value;
                        var snippet = new Snippet(shortcut, snippetNode.InnerText.Trim());
                        ApplyAttributesToProperties(snippetNode.Attributes.Cast<XmlAttribute>().Where(a => (a.Name != "shortcut" && a.Name != "code")), typeof(Snippet), snippet);
                        Scintilla.Snippets.List.Remove(snippet.Shortcut);
                        Scintilla.Snippets.List.Add(snippet);
                    }
                }
            }
            catch (Exception innerException)
            {
                throw new ArgumentException("Unable to read configuration XML from stream.", innerException);
            }
        }

        private static void ApplyAttributesToProperties(IEnumerable<XmlAttribute> attributes, Type type, object obj)
        {
            foreach (var attr in attributes)
            {
                var propInfo = type.GetProperty(attr.Name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                var converter = TypeDescriptor.GetConverter(propInfo.PropertyType);
                propInfo.SetValue(obj, converter.ConvertFromString(attr.Value), null);
            }
        }

        private static Dictionary<string, int> GetKeywordListNames(string language)
        {
            if (_keywordListNames == null)
                _keywordListNames = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);

            Dictionary<string, int> map = null;
            if (!_keywordListNames.TryGetValue(language, out map))
            {
                // Lay load embedded resource
                map = new Dictionary<string, int>();
                var resourceStream = typeof(ConfigurationManager).Assembly.GetManifestResourceStream("ScintillaNET.Configuration.Builtin.LexerKeywordListNames." + language.ToLower() + ".txt");
                var reader = new StreamReader(resourceStream, Encoding.UTF8); // Don't close stream

                string line = null;
                int index = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length > 0)
                        map[line] = index;

                    index++;
                }
            }

            return map;
        }

        private static Dictionary<string, int> GetStyleNames(string language)
        {
            if (_styleNames == null)
                _styleNames = new Dictionary<string, Dictionary<string, int>>(StringComparer.OrdinalIgnoreCase);

            Dictionary<string, int> map = null;
            if (!_styleNames.TryGetValue(language, out map))
            {
                // Lay load embedded resource
                map = new Dictionary<string, int>();
                var resourceStream = typeof(ConfigurationManager).Assembly.GetManifestResourceStream("ScintillaNET.Configuration.Builtin.LexerStyleNames." + language.ToLower() + ".txt");
                var reader = new StreamReader(resourceStream, Encoding.UTF8); // Don't close stream

                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2)
                        map[parts[0].Trim()] = int.Parse(parts[1].Trim());
                }
            }

            return map;
        }

        #endregion Methods

        #region Constructors

        internal ConfigurationManager(Scintilla scintilla) : base(scintilla)
        {
        }

        #endregion Constructors
    }
}

