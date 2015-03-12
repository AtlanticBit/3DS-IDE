using System;
using System.Collections.Generic;
using System.Text;

namespace ScintillaBindingGenerator
{
    class NameNormalizer : SourceFile
    {

        private static NameNormalizer _instance;
        private System.Collections.Hashtable _dictionary = new System.Collections.Hashtable();
        
        
        private NameNormalizer()
        {
        }
        public static NameNormalizer instance
        {
            get
            {
                if (null == _instance)
                    _instance = new NameNormalizer();
                return _instance;
            }   
        }
        
        public void Initialize(string filename)
        {
            ReadFile(filename);
            foreach( string line in filetext )
            {
                try
                {
                    string[] p = line.Split(' ');
                    _dictionary.Add(p[0].ToLower(), p[1]);
                }catch( Exception )
                {
                    
                }
            }
        }
        
        public string FixName( string name )
        {
            string result = name;
            result = FixFunnyPatterns(result);
            result = Alias(result);
            result = FixCase(result);
            result = FixUnderscores(result);
            result = result.Replace("Colour", "Color"); 
            return result;
        }
        
        public string Alias(string name)
        {
            string result = name;

            if (_dictionary.ContainsKey(name.ToLower()))
                result = _dictionary[name.ToLower()].ToString();

            return result;
        }
        
        public string FixCase(string name)
        {
            if (char.IsLower(name[0]))
                name = char.ToUpper(name[0]) + name.Substring(1);
            return name;
        }

        private string clipAndAlias(string name , string pattern, string fixedpattern )
        {
            string result = name;
            if (result.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase))
                result = fixedpattern + FixName(result.Substring(pattern.Length));
            return result;
        }
        public string FixFunnyPatterns(string name )
        {
            string result = name;

            result = clipAndAlias(result, "ja_", "AspJavaScript");
            result = clipAndAlias(result, "j_", "JavaScript");
            result = clipAndAlias(result, "sgml_", "Sgml");
            result = clipAndAlias(result, "php_", "Php");
            result = clipAndAlias(result, "ba_", "AspVBScript");
            result = clipAndAlias(result, "b_", "VBScript");
            result = clipAndAlias(result, "p_", "Python");
            result = clipAndAlias(result, "pa_", "AspPython");
            
            return result;
        }
        
        public string FixUnderscores(string name)
        {
            string result = name;
            
            
            int i = result.IndexOf('_'); 
            while( i > -1 )
            {
                try
                {
                    result = result.Substring(0, i) + char.ToUpper(result[i + 1]) + result.Substring(i + 2);
                    i = result.IndexOf('_');
                }
                catch (Exception)
                {
                    System.Console.WriteLine("{0}--{1}" , name,result);
                    break;
                }
            }
            return result;
        }
        
    }
}
