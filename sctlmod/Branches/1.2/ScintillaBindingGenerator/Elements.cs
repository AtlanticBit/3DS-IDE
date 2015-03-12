using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ScintillaBindingGenerator
{
    public abstract class Element 
    {
        public string comment;
        public string line;
        public string name;
        public string value;
        public string[] values;

        public bool emitted = false;

        public abstract Regex rx { get; }
        public virtual Type clrReturnType
        {
            get
            {
                return null ;
            }
        }

        public Element(string line, string comment )
        {
            this.line = line;
            this.comment = comment;
        }
    }

    public class ValueElement : Element
    {
        private List<LexerElement> lastLexerList;
        private static Regex RX = new Regex("^val (.*?) *= *(.*) *$", RegexOptions.Compiled);
        public override Regex rx
        { get { return RX; } }

        public ValueElement(string line, string comment)
            : base(line, comment)
        {
            Match m = rx.Match(line);
            name = m.Groups[1].Value;
            value = m.Groups[2].Value;
        }

        public ValueElement(string line, string comment, List<LexerElement> lastLexerList)
            : this(line, comment)
        {
            this.lastLexerList = lastLexerList;
        }

        public List<LexerElement> LexerList
        {
            get 
            {
                return lastLexerList;
            }
        }

    }

    public class EnumElement : Element
    {
        public static Regex RX = new Regex("^enu *(.*?) *= *(.*) *$", RegexOptions.Compiled);
        public override Regex rx
        { get { return RX; } }

        public EnumElement(string line, string comment)
            : base(line, comment)
        {
            Match m = rx.Match(line);
            name = m.Groups[1].Value;
            values = m.Groups[2].Value.Trim().Split(' ');
        }

    }

    public class LexerElement : Element
    {
        public string prefix;
        public static Regex RX = new Regex("^lex *(.*?) *= *(.*?)$", RegexOptions.Compiled);
        public override Regex rx
        { get { return RX; } }

        public LexerElement(string line, string comment)
            : base(line, comment)
        {
            Match m = rx.Match(line);
            name = m.Groups[1].Value;
            string[] sp = m.Groups[2].Value.Trim().Split();

            value= sp[0];
            prefix = sp[1];

        }

    }

    public abstract class featureElement : Element
    {
        public string returnType;

        public string p1type;
        public string p2type;
        public string p1name;
        public string p2name;
        public int index;


        public featureElement(string line, string comment)
            : base(line, comment)
        {
        }
        public override Type clrReturnType
        {
            get
            {

                switch (returnType)
                {

                    case "void":
                        return null;

                    case "int":
                    case "position":
                    case "integer":
                    case "colour":
                    case "keymod":
                        return typeof(int);

                    case "bool":
                        return typeof(bool);

                    case "string":
                        return typeof(string);
                }
                return null;
            }
        }

        public Type clrP1Type
        {
            get
            {
                switch (p1type)
                {

                    case "void":
                        return null;

                    case "int":
                    case "position":
                    case "integer":
                    case "colour":
                    case "keymod":
                        return typeof(int);

                    case "bool":
                        return typeof(bool);

                    case "string":
                        return typeof(string);
                }
                return null;
            }
        }

        public Type clrP2Type
        {
            get
            {
                switch (p2type)
                {

                    case "void":
                        return null;

                    case "int":
                    case "position":
                    case "integer":
                    case "colour":
                    case "keymod":
                        return typeof(int);

                    case "bool":
                        return typeof(bool);

                    case "string":
                        return typeof(string);
                }
                return null;
            }
        }


        public FunctionTypes FunctionType
        {
            get
            {
                FunctionTypes result = FunctionTypes.INVALID;

                if (string.IsNullOrEmpty(returnType))
                    result = FunctionTypes.ret_void;
                else switch (returnType)
                    {

                        case "void":
                            result = FunctionTypes.ret_void;
                            break;

                        case "int":
                        case "position":
                        case "integer":
                        case "colour":
                        case "keymod":
                            result = FunctionTypes.ret_int;
                            break;

                        case "bool":
                            result = FunctionTypes.ret_bool;
                            break;
                    }

                if (string.IsNullOrEmpty(p1name))
                    result = result | FunctionTypes.p1_void;
                else switch (p1type)
                    {
                        case "void":
                            result = result | FunctionTypes.p1_void;
                            break;

                        case "int":
                        case "position":
                        case "integer":
                        case "colour":
                        case "keymod":
                            result = result | FunctionTypes.p1_int;
                            break;

                        case "bool":
                            result = result | FunctionTypes.p1_bool;
                            break;

                        case "string":
                            result = result | FunctionTypes.p1_string;
                            break;
                    }

                if (string.IsNullOrEmpty(p2name))
                    result = result | FunctionTypes.p2_void;
                else switch (p2type)
                    {
                        case "void":
                            result = result | FunctionTypes.p2_void;
                            break;

                        case "int":
                        case "position":
                        case "integer":
                        case "colour":
                        case "keymod":
                            result = result | FunctionTypes.p2_int;
                            break;

                        case "bool":
                            result = result | FunctionTypes.p2_bool;
                            break;

                        case "string":
                            result = result | FunctionTypes.p2_string;
                            break;

                        case "stringresult":
                            result = result | FunctionTypes.p2_stringresult;
                            break;

                        case "cells":
                            result = result | FunctionTypes.p2_cells;
                            break;

                        case "formatrange":
                            result = result | FunctionTypes.p2_formatrange;
                            break;

                        case "textrange":
                            result = result | FunctionTypes.p2_textrange;
                            break;

                        case "findtext":
                            result = result | FunctionTypes.p2_findtext;
                            break;
                    }

                return result;
            }
        }


    }

    public class SetAccessorElement : featureElement
    {
        public static Regex RX = new Regex("^set *(.*?) (Set)?(.*?) *= *(\\d*) *\\((.*?),(.*?)\\) *$", RegexOptions.Compiled);
        public override Regex rx
        { get { return RX; } }

        public SetAccessorElement(string line, string comment)
            : base(line, comment)
        {
            Match m = rx.Match(line);
            returnType = m.Groups[1].Value;
            name = m.Groups[3].Value;
            index = int.Parse(m.Groups[4].Value);

            String[] sp = m.Groups[5].Value.Trim().Split(' ');
            if (sp.Length == 2)
            {
                p1type = sp[0];
                p1name = sp[1];
            }
            sp = m.Groups[6].Value.Trim().Split(' ');
            if (sp.Length == 2)
            {
                p2type = sp[0];
                p2name = sp[1];
            }

        }

    }
    public class GetAccessorElement : featureElement
    {
        public static Regex RX = new Regex("^get *(.*?) (Get)?(.*?) *= *(\\d*) *\\((.*?),(.*?)\\) *$", RegexOptions.Compiled);
        public override Regex rx
        { get { return RX; } }

        public GetAccessorElement(string line, string comment)
            : base(line, comment)
        {
            Match m = rx.Match(line);
            returnType = m.Groups[1].Value;
            name = m.Groups[3].Value;
            index = int.Parse(m.Groups[4].Value);

            String[] sp = m.Groups[5].Value.Trim().Split(' ');
            if (sp.Length == 2)
            {
                p1type = sp[0];
                p1name = sp[1];
            }
            sp = m.Groups[6].Value.Trim().Split(' ');
            if (sp.Length == 2)
            {
                p2type = sp[0];
                p2name = sp[1];
            }
        }

    }


    [Flags]
    public enum FunctionTypes
    {
        INVALID         = 0x00000000,

        ret_void        = 0x00000001,
        ret_int         = 0x00000002,
        ret_bool        = 0x00000004,

        p1_void         = 0x00000100,
        p1_int          = 0x00000200,
        p1_bool         = 0x00000400,
        p1_string       = 0x00000800,

        p2_void         = 0x00010000,
        p2_int          = 0x00020000,
        p2_bool         = 0x00040000,
        p2_string       = 0x00080000,
        p2_stringresult = 0x00100000,
        p2_textrange    = 0x00200000,
        p2_cells        = 0x00400000,
        p2_findtext     = 0x00800000,
        p2_formatrange  = 0x01000000,
    }


    public class FunctionElement : featureElement
    {
        public static Regex RX = new Regex( "^fun *(.*?) (.*?) *= *(\\d*) *\\((.*?),(.*?)\\) *$" , RegexOptions.Compiled );
        public override Regex rx
        { get { return RX; } }


        public FunctionElement(string line, string comment)
            : base(line, comment)
        {
            Match m = rx.Match(line);
            returnType = m.Groups[1].Value;
            name = m.Groups[2].Value;
            index = int.Parse(m.Groups[3].Value);

            String[] sp = m.Groups[4].Value.Trim().Split(' ');
            if (sp.Length == 2)
            {
                p1type = sp[0];
                p1name = sp[1];
            }
            sp = m.Groups[5].Value.Trim().Split(' ');
            if (sp.Length == 2)
            {
                p2type = sp[0];
                p2name = sp[1];
            }
        }

    }
    public class EventElement : Element
    {
        public static Regex RX = new Regex( "^evt *(.*?) (.*?) *= *(\\d*) *\\((.*?)\\) *$" , RegexOptions.Compiled );
        public string index;
        public string type;
        public string parameters;

        public override Regex rx
        { get { return RX; } }

        public EventElement(string line, string comment)
            : base(line, comment)
        {
            Match m = rx.Match(line);
            type = m.Groups[1].Value;
            name = m.Groups[2].Value;
            index = m.Groups[3].Value;

            parameters = m.Groups[4].Value.Trim();
        }

    }

    public class LanugageElement : Element
    {
        private static Regex RX = new Regex("^val (.*?) *= *(.*) *$", RegexOptions.Compiled);
        public override Regex rx
        { get { return RX; } }

        public LanugageElement(string line, string comment)
            : base(line, comment)
        {
        }

    }

}

