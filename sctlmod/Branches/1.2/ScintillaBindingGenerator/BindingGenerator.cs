using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ScintillaBindingGenerator
{
    class BindingGenerator
    {
        // source files
        public static FileInfo scintilla = null;
        public static FileInfo languages = null;
        public static FileInfo dictionary = null;
        public static FileInfo overrides = null;

        public static int ShowUsage(string error)
        {
            if( !string.IsNullOrEmpty(error) )
                System.Console.WriteLine("\n\nError:\n   {0}", error);

            System.Console.WriteLine("\n\nUsage:");
            System.Console.WriteLine("---------");
            System.Console.WriteLine("ScintillaBindingGenerator /source:<dir> /dest:<dir>");


            return 1;
        }

        public static string ParseArg(string target, string argument)
        {
            if (argument.StartsWith(string.Format("/{0}:", target), StringComparison.CurrentCultureIgnoreCase))
                return argument.Substring(target.Length + 2);
            return null;
        }

        static int Main(string[] args)
        {
            System.Console.WriteLine("Scintilla Binding Generator 6.0 (c) 2006 Garrett W. Serack");
            if (args.Length != 2)
                return ShowUsage("Invalid Number of Arguments.");

            string source = null;
            string dest = null;

            for (int i = 0; i < args.Length; i++)
            {
                if (null == source)
                    source = ParseArg("source", args[i]);

                if (null == dest)
                    dest = ParseArg("dest", args[i]);
            }

            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(dest))
                return ShowUsage("Arguments can not have empty values.");

            System.Console.WriteLine("Reading Configuration From [{0}]", source);

            scintilla = new FileInfo(string.Format("{0}\\Scintilla.iface", source));
            System.Console.WriteLine("   File:[{0}] Exists:[{1}]", scintilla.FullName, scintilla.Exists);

            languages = new FileInfo(string.Format("{0}\\languages.iface", source));
            System.Console.WriteLine("   File:[{0}] Exists:[{1}]", languages.FullName, languages.Exists);

            dictionary = new FileInfo(string.Format("{0}\\dictionary.txt", source));
            System.Console.WriteLine("   File:[{0}] Exists:[{1}]", dictionary.FullName, dictionary.Exists);

            overrides = new FileInfo(string.Format("{0}\\overrides.txt", source));
            System.Console.WriteLine("   File:[{0}] Exists:[{1}]", overrides.FullName, overrides.Exists);

            if (null == scintilla || null == languages || null == dictionary || null == overrides)
                return ShowUsage("Missing Source File");

            NameNormalizer.instance.Initialize(dictionary.FullName);
            GenerationOverride.instance.Initialize(overrides.FullName);
            
            System.Console.WriteLine("Writing files to [{0}]", dest);

            
            Scintilla gen = new Scintilla(scintilla.FullName);
            foreach (Element element in gen.Enums)
                GenerationOverride.instance.Override(element);
            foreach (Element element in gen.Lexers)
                GenerationOverride.instance.Override(element);
            foreach (Element element in gen.Events)
                GenerationOverride.instance.Override(element);
            foreach (Element element in gen.Functions)
                GenerationOverride.instance.Override(element);
            foreach (Element element in gen.Gets)
                GenerationOverride.instance.Override(element);
            foreach (Element element in gen.Languages)
                GenerationOverride.instance.Override(element);
            foreach (Element element in gen.Sets)
                GenerationOverride.instance.Override(element);
            foreach (Element element in gen.Values)
                GenerationOverride.instance.Override(element);
            


            System.Console.WriteLine("   {0}\\Enums.generated.cs", dest);
            gen.EmitEnums( string.Format("{0}\\Enums.generated.cs", dest ) );

            System.Console.WriteLine("   {0}\\Lexers.generated.cs", dest);
            gen.EmitLexers(string.Format("{0}\\Lexers.generated.cs", dest));

            System.Console.WriteLine("   {0}\\ScintillaControl.generated.cs", dest);
            gen.EmitScintillaControl(string.Format("{0}\\ScintillaControl.generated.cs", dest));

            return 0;
        }
    }
}
