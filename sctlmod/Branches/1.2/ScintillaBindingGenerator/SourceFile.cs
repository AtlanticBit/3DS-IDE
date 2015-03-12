using System;
using System.IO;

using System.Collections.Generic;
using System.Text;

namespace ScintillaBindingGenerator
{
    public class SourceFile
    {
        protected List<string> filetext = null;

        public SourceFile()
        {
        }

        protected void ReadFile(string filename)
        {
            filetext = new List<string>();

			//	WorkItem 2378: Chris Rickard 2006-08-21
			//	This file isn't written to so use the overload that specifies
			//	that it will only be open for reading. Otherwise the default is
			//	Read/Write and will throw an exception if the file is readonly.
            StreamReader sr = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read), true);
            string txt;
            do
            {
                txt = sr.ReadLine();
                if (txt != null)
                {
                    // don't use anything after deprecated.
                    if (txt.StartsWith("cat Deprecated"))
                        break;

                    // eliminate non contextual comments.
                    if (txt.StartsWith("##"))
                        continue;

                    filetext.Add(txt.Trim());
                }
            }
            while (null != txt);

            sr.Close();
        }
    }
}
