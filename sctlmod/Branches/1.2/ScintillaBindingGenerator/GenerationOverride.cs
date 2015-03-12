using System;
using System.Collections.Generic;
using System.Text;

namespace ScintillaBindingGenerator
{
    class GenerationOverride: SourceFile
    {

        private static GenerationOverride _instance;
        private System.Collections.Hashtable _dictionary = new System.Collections.Hashtable();


        private GenerationOverride()
        {
        }
        public static GenerationOverride instance
        {
            get
            {
                if (null == _instance)
                    _instance = new GenerationOverride();
                return _instance;
            }
        }

        public void Initialize(string filename)
        {
            ReadFile(filename);
            foreach (string line in filetext)
            {
                try
                {
                    string[] p = line.Split(' ');
                    _dictionary.Add(p[0].ToLower(), p);
                }
                catch (Exception)
                {

                }
            }
        }
        
        public void Override(Element element)
        {
            string elementname = element.name.ToLower();
            if( _dictionary.ContainsKey( elementname) )
            {
                foreach(string item in  _dictionary[elementname] as string[])
                {
                    string[] statement = item.Split('=');
                    if (statement.Length == 2)
                    {
                        System.Reflection.FieldInfo fi = element.GetType().GetField(statement[0]);
                        if (fi.FieldType.Equals(typeof(bool)))
                            fi.SetValue(element, statement[1].Equals("true"));
                        if (fi.FieldType.Equals(typeof(int)))
                            fi.SetValue(element, int.Parse(statement[1]));
                        if (fi.FieldType.Equals(typeof(string)))
                            fi.SetValue(element, statement[1]);
                    }
                }
            }
        }
    }
}
