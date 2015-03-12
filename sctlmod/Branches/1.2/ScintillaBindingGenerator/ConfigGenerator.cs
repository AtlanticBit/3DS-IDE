using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScintillaBindingGenerator
{
    class ConfigGenerator : SourceFile
    {
        private List<LanugageElement> languages = null;
        private Scintilla scintilla;
        public static string INDENT = "    ";
        public static string xmlTemplateFile;
        
        public ConfigGenerator(string filename, Scintilla scintilla, string xmlTemplate)
        {
            this.scintilla = scintilla;
            xmlTemplateFile = xmlTemplate;
            ReadFile(filename);
            Languages = new List<LanugageElement>();

            string comment = "";

            LanugageElement l = null;
            String cat = null;
            
            // create properties here
            foreach (string line in filetext)
            {
                String ln = line.Trim();


                if (ln.Length == 0)
                {
                    comment = "";
                    continue;
                }

                // don't use anything after deprecated.
                if (ln.StartsWith("cat Deprecated"))
                    break;

                // eliminate non contextual comments.
                if (ln.StartsWith("##"))
                    continue;

                if (ln.StartsWith("#"))
                {
                    // this is a comment. build to the current comment string.
                    comment += ln.Substring(2) + "\n";
                    continue;
                }

                if (ln.StartsWith("lang"))
                {
                    // this is a langugage
                    l = new LanugageElement(ln);
                    Languages.Add(l);
                    continue;
                }

                if (l != null)
                    if (ln.StartsWith("*"))
                    {
                        // this is a keyword category
                        cat = ln.Substring(1).Trim();
                    }

                if (l != null && cat != null)
                    if (ln.StartsWith("!"))
                    {
                        String kwds = "         ";
                        // this is a keyword set.
                        if (l.keyword_contents.ContainsKey(cat))
                        {
                            kwds = l.keyword_contents[cat].ToString();
                            l.keyword_contents.Remove(cat);
                        }
                        kwds += "\n         " + ln.Substring(1).Trim();
                        l.keyword_contents[cat] = kwds;
                    }

                if (l != null && cat != null)
                    if (ln.StartsWith("$")) //borrow from another language
                    {
                        string[] two = ln.Substring(1).Trim().Split(' ');

                        String kwds = "";

                        foreach (LanugageElement lng in Languages)
                        {
                            if (lng.language.Equals(two[0]))
                            {
                                if (lng.keyword_contents.ContainsKey(two[1]))
                                    kwds = lng.keyword_contents[two[1]].ToString();
                                break;
                            }
                        }

                        l.keyword_contents[cat] = kwds;
                    }
            }
        }


        public void processMasterFile(string masterfilename )
        {
            SourceFile MasterFile = new SourceFile();
            MasterFile.ReadFile(masterfilename);
            
            
            List<string> resultFile = new List<string>();

            bool inRegion = false;
            foreach (string aLine in MasterFile.filetext)
            {
                String ln = aLine.Trim();
                if (inRegion)
                {

                    if (ln.IndexOf("#endregion") == -1)
                        continue;

                    inRegion = false;
                }
                else
                {
                    if (ln.IndexOf("#region \"xml-includes\"") > 0)
                    {
                        resultFile.Add(aLine);
                        inRegion = true;
                        emitXMLIncludes(resultFile);
                        continue;
                    }

                    if (ln.IndexOf("#region \"xml-style-classes\"") > 0)
                    {
                        resultFile.Add(aLine);
                        inRegion = true;
                        emitXMLStyles(resultFile);
                        continue;
                    }

                }
                resultFile.Add(aLine);
            }
            MasterFile.filetext = resultFile;
            MasterFile.WriteFile(masterfilename);
        }        
        public void EmitXMLFiles(string destinationPath)
        {
            // process includes
            foreach (LanugageElement l in languages)
            {
                // include will be in the same directory as the setting files. [arg0]

                string xmlFile = destinationPath + l.language + ".xml";
                SourceFile theFile = new SourceFile();
                if (!File.Exists(xmlFile))
                    File.Copy(xmlTemplateFile, xmlFile);

                theFile.ReadFile(xmlFile);
                processLanguageFile(l, theFile);
                theFile.WriteFile(xmlFile);

            }

        }

        public void processLanguageFile(LanugageElement language, SourceFile file)
        {

            List<string> resultFile = new List<string>();

            bool inRegion = false;
            foreach (string aLine in file.filetext)
            {
                String ln = aLine.Trim();
                if (inRegion)
                {

                    if (ln.IndexOf("#endregion") == -1)
                        continue;

                    inRegion = false;
                }
                else
                {
                    if (ln.IndexOf("#region \"xml-languages\"") > 0)
                    {
                        resultFile.Add(aLine);
                        inRegion = true;
                        emitXMLLanguage(language, resultFile);
                        continue;
                    }

                    if (ln.IndexOf("#region \"xml-style-classes\"") > 0)
                    {
                        resultFile.Add(aLine);
                        inRegion = true;
                        emitXMLStyles(language, resultFile);
                        continue;
                    }
                    if (ln.IndexOf("#region \"xml-keyword-classes\"") > 0)
                    {
                        resultFile.Add(aLine);
                        inRegion = true;
                        emitXMLKeywords(language, resultFile);
                        continue;
                    }

                    /*
                                        if( ln.cont "#region \"xml-globals\"" )
                                        {
                                            resultFile.Add( aLine );
                                            inRegion = true;
                                            emitXMLGlobals(resultFile);
                                            continue;
                                        }
                                        if( ln == "#region \"xml-style-classes\"" )
                                        {
                                            resultFile.Add( aLine );
                                            inRegion = true;
                                            emitXMLStyles(resultFile);
                                            continue;
                                        }
                                        if( ln == "#region \"xml-keyword-classes\"" )
                                        {
                                            resultFile.Add( aLine );
                                            inRegion = true;
                                            emitXMLKeywords(resultFile);
                                            continue;
                                        }
                    */
                }
                resultFile.Add(aLine);
            }
            file.filetext  = resultFile;
        }
        

		public void emitXMLIncludes( List<string> file )
		{
            foreach (LanugageElement l in Languages)
			{
				file.Add( String.Format("{1}{1}<include file=\"settings\\{0}.xml\" />", l.language , INDENT ) );
			}
		}
		public void emitXMLGlobals( List<string> file )
		{
		}
		public void emitXMLKeywords( LanugageElement theLang ,List<string> file )
		{
			// find lexer
			LexerElement theLexer = null;
			foreach(LexerElement lx in scintilla.Lexers )
			{
                if (lx.value.ToUpper().Equals(theLang.lexer.ToUpper()))
					theLexer = lx;
			}

			foreach( string s in theLang.keywords )
			{
				if( theLang.keyword_contents.ContainsKey(s) )
					file.Add( string.Format( "      <keyword-class name=\"{1}-{0}\" >{2}\n      </keyword-class>", s, theLang.language, theLang.keyword_contents[s]) );
				else
					file.Add( string.Format( "      <keyword-class name=\"{1}-{0}\" >\n      </keyword-class>", s, theLang.language) );
			}
		}
		public void emitXMLStyles( List<string> file )
		{
			Hashtable stylenames = new Hashtable();
			Hashtable stylecomments = new Hashtable();

            foreach (LexerElement theLexer in scintilla.Lexers)
				foreach( string s in theLexer.styles )
				{
					if( s.Equals("max") )
						continue;

					if( stylenames.ContainsKey(s) )
					{
						string comment = stylecomments[s].ToString();
						comment += ", "+ theLexer.name ;
						stylecomments.Remove( s );
						stylecomments[s] = comment;
						continue;
					}

					stylecomments[s] = theLexer.name;
					stylenames.Add( s,string.Format( @"      <style-class name=""{0}"" fore=""default-fore"" back=""default-back"" size=""default-font-size"" font=""default-font"" />", s) );

					//file.Add( string.Format( @"      <style-class name=""{0}"" fore=""default-fore"" back=""default-back"" size=""default-font-size"" font=""default-font"" />", s) );
				}
			string[] keys = new string[stylenames.Count];

			stylenames.Keys.CopyTo( keys , 0);
			Array.Sort( keys );

			foreach( string key in keys )
			{
				file.Add( "      <!-- Used in "+stylecomments[key]+" -->" );
				file.Add( stylenames[key].ToString() );
			}
		}

        public void emitXMLStyles(LanugageElement theLang, List<string> file)
		{
			// find lexer
            LexerElement theLexer = null;
            foreach (LexerElement lx in scintilla.Lexers)
			{
                if (lx.value.ToUpper().Equals(theLang.lexer.ToUpper()))
					theLexer = lx;
			}

			foreach( string s in theLexer.styles )
			{
				if( s.Equals("max") )
					continue;

				file.Add( string.Format( @"      <style-class name=""{1}-{0}"" inherit-style=""{0}"" />", s, theLang.language) );
			}
		}


        public void emitXMLLanguage(LanugageElement Language, List<string> file)
		{
            LexerElement theLexer = null;
            foreach (LexerElement lx in scintilla.Lexers)
			{
				if( lx.value.ToUpper().Equals( Language.lexer.ToUpper() ) )
					theLexer = lx;
			}
			int stylebits = 5;
			switch( theLexer.name.ToUpper() )
			{
				case "HTML":
					stylebits=7;
					break;
				case "XML":
					stylebits=7;
					break;
				case "ASP":
					stylebits=7;
					break;
				case "PHP":
					stylebits=7;
					break;
			}
			// lexer line
			String sb = string.Format("         <lexer name=\"{0}\" style-bits=\"{1}\"/>\n" , theLexer.name, stylebits );

			sb+="          <editor-style caret-fore=\"default-caret-fore\" selection-fore=\"default-selection-fore\" selection-back=\"default-selection-back\" />\n";
			sb+="          <character-class inherit=\"alphanumeric\" ></character-class>\n";
			

			// file extensions
			sb+="         <file-extensions>";
			foreach( string s in Language.filemasks )
				sb+=" "+s;
			sb+="</file-extensions>\n";

			// use keywords
			sb+="         <use-keywords>\n";
			int n=0;
			foreach( string s in Language.keywords )
			{
				sb+="            <keyword key=\""+n+"\" class=\""+Language.language+"-"+s+"\"/>\n";
				n++;
			}
			sb+="         </use-keywords>\n";

			// use styles
			sb+="         <use-styles>\n";
			foreach( string s in theLexer.styles )
			{
				if( s.Equals("max") )
					continue;

				sb+="            <style name=\""+s+"\" class=\""+ Language.language +"-"+s+"\" />\n";
			}
			sb+="         </use-styles>\n";


			file.Add( string.Format("{0}{0}<language name=\"{2}\">\n{1}\n{0}{0}</language>","   ", sb.ToString(), Language.language  ));
		}            
        
        public List<LanugageElement> Languages
        {
            get { return languages; }
            set { languages = value; }
        }

    }
}
