using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace ScintillaBindingGenerator
{
    public class Scintilla  : SourceFile
    {
        private static string PERFORM = "SendMessageDirect";
        private static string PERFORMASSTRINGRESULT = "PerformAsStringResult";

        CodeGeneratorOptions options = new CodeGeneratorOptions();

        private List<ValueElement> values = null;
        private List<EnumElement> enums = null;
        private List<LexerElement> lexers = null;
        private List<SetAccessorElement> sets = null;
        private List<GetAccessorElement> gets= null;
        private List<FunctionElement> functions = null;
        private List<EventElement> events = null;
        private List<LanugageElement> languages = null;

        public Scintilla(string filename)
        {
            // read file
            ReadFile(filename);
            options.BlankLinesBetweenMembers = false;
            options.BracingStyle = "C";

            Values = new List<ValueElement>();
            Enums = new List<EnumElement>();
            Lexers = new List<LexerElement> ();
            Sets = new List<SetAccessorElement>();
            Gets = new List<GetAccessorElement>();
            Functions = new List<FunctionElement>();
            Events = new List<EventElement>();
            Languages = new List<LanugageElement>();

            List<LexerElement> lastLexerSet = new List<LexerElement>();
            bool clearLastLexer = false;
            
            string comment = "";

            // create properties here
            foreach (string line in filetext)
            {
                if (line.Length == 0)
                {
                    // reset the comment string.
                    comment = "";
                    continue;
                }

                if (line.StartsWith("#"))
                {
                    // this is a comment. build to the current comment string.
                    comment += line.Substring(2) + "\n";
                    continue;
                }

                if (line.StartsWith("val"))
                {
                    clearLastLexer = true;
                    if (lastLexerSet.Count > 0)
                    {
                        Values.Add(new ValueElement(line, comment, lastLexerSet));
                    }
                    else
                    {
                        Values.Add(new ValueElement(line, comment));
                    }
                    continue;
                }

                if (line.StartsWith("set"))
                {
                    // this is a set accessor
                    Sets.Add(new SetAccessorElement(line, comment));
                    continue;
                }

                if (line.StartsWith("get"))
                {
                    // this is a get accessor
                    Gets.Add(new GetAccessorElement(line, comment));
                    continue;
                }

                if (line.StartsWith("fun"))
                {
                    // this is a function
                    Functions.Add(new FunctionElement(line, comment));
                    continue;
                }

                if (line.StartsWith("enu"))
                {
                    // this is an enum
                    Enums.Add(new EnumElement(line, comment));
                    continue;
                }

                if (line.StartsWith("evt"))
                {
                    // this is an enum
                    Events.Add(new EventElement(line, comment));
                    continue;
                }

                if (line.StartsWith("lex"))
                {
                    LexerElement e = new LexerElement(line, comment);
                    if (clearLastLexer)
                    {
                        lastLexerSet = new List<LexerElement>();
                        clearLastLexer = false;
                    }
                    lastLexerSet.Add(e);

                    // this is an enum
                    Lexers.Add(e);
                    continue;
                }
            }
        }

        public List<ValueElement> Values
        {
            get { return values; }
            set { values = value; }
        }

        public List<EnumElement> Enums
        {
            get { return enums; }
            set { enums = value; }
        }

        public List<LexerElement> Lexers
        {
            get { return lexers; }
            set { lexers = value; }
        }

        public List<SetAccessorElement> Sets
        {
            get { return sets; }
            set { sets = value; }
        }

        public List<GetAccessorElement> Gets
        {
            get { return gets; }
            set { gets = value; }
        }

        public List<FunctionElement> Functions
        {
            get { return functions; }
            set { functions = value; }
        }

        public List<EventElement> Events
        {
            get { return events; }
            set { events = value; }
        }

        public List<LanugageElement> Languages
        {
            get { return languages; }
            set { languages = value; }
        }

        public void EmitEnums(string filename)
        {
            CSharpCodeProvider cg = new CSharpCodeProvider();
            CodeNamespace cnamespace = new CodeNamespace("Scintilla.Enums");
            cnamespace.Imports.Add(new CodeNamespaceImport("System"));
            cnamespace.Imports.Add(new CodeNamespaceImport("System.Collections"));
            cnamespace.Imports.Add(new CodeNamespaceImport("System.Windows.Forms"));
            cnamespace.Imports.Add(new CodeNamespaceImport("System.Runtime.InteropServices"));

            foreach (EnumElement element in Enums)
            {
                CodeTypeDeclaration co = new CodeTypeDeclaration(element.name);
                co.IsEnum = true;
                cnamespace.Types.Add(co);
                foreach (ValueElement value in Values)
                {
                    foreach (string s in element.values)
                    {
                        if (value.name.StartsWith(s))
                        {
                            CodeMemberField field = new CodeMemberField();
                            string name = value.name.Substring(s.Length).ToLower();
                            if (name.StartsWith("_"))
                                name = name.Substring(1);
                            if (name.StartsWith("8859"))
                                name = "CharSet" + name;
                            name = NameNormalizer.instance.FixName( name );
                            field.Name = name;
                            field.InitExpression = new CodeSnippetExpression(value.value);
                            co.Members.Add(field);
                        }
                    }
                }
            }

            CodeTypeDeclaration eventEnum = new CodeTypeDeclaration("Events");
            eventEnum.IsEnum = true;
            eventEnum.BaseTypes.Add( new CodeTypeReference( typeof(uint) ) );
            cnamespace.Types.Add(eventEnum);

            foreach (EventElement element in Events)
            {
                CodeMemberField field = new CodeMemberField();
                string name = element.name;
                name = NameNormalizer.instance.FixName(name);
                if (name == "Zoom")
                    name = "SCZoom";
                if (name == "DoubleClick")
                    name = "SCDoubleClick";
                if (name == "Key")
                    name = "SCKey";
                field.Name = name;

                field.InitExpression = new CodeSnippetExpression(element.index);
                eventEnum.Members.Add(field);
            }


			//	WorkItem 2378: Chris Rickard 2006-08-21 See writeOverReadOnlyFile() comments
			writeOverReadOnlyFile(filename, cg, cnamespace);
        }

        public void EmitLexers(string filename)
        {
            CSharpCodeProvider cg = new CSharpCodeProvider();

            CodeNamespace cnamespace = new CodeNamespace("Scintilla.Lexers");
            cnamespace.Imports.Add(new CodeNamespaceImport("System"));
            cnamespace.Imports.Add(new CodeNamespaceImport("System.Collections"));
            cnamespace.Imports.Add(new CodeNamespaceImport("System.Windows.Forms"));
            cnamespace.Imports.Add(new CodeNamespaceImport("System.Runtime.InteropServices"));

            List<ValueElement> commonstyles = new List<ValueElement>();
            int stylelen = 0;
            foreach (EnumElement element in Enums)
            {
                if (element.name.Equals("StylesCommon"))
                {
                    foreach (ValueElement value in Values)
                    {
                        foreach (string s in element.values)
                        {
                            if (value.name.StartsWith(s))
                            {
                                commonstyles.Add(value);
                                stylelen = s.Length;
                            }
                        }
                    }
                }
            }

            List<string> usedKeys = new List<string>();
            foreach( LexerElement element in Lexers )
            {
                CodeTypeDeclaration co = new CodeTypeDeclaration(NameNormalizer.instance.FixName(element.name.ToLower()));
                co.IsEnum = true;
                cnamespace.Types.Add(co);
                foreach (ValueElement value in Values)
                {
                    bool foundMatchingLexer = value.name.StartsWith(element.prefix);
                    if (foundMatchingLexer && (value.LexerList != null))
                    {
                        if (!value.LexerList.Contains(element)) foundMatchingLexer = false;
                    }

                    //if ( ((value.LexerList != null) && (value.LexerList.Contains(element))) ||
                     //   (value.LexerList == null && value.name.StartsWith(element.prefix)) )
                    if (foundMatchingLexer) 
                    {
                        usedKeys.Add(value.value);
                        CodeMemberField field = new CodeMemberField();
                        string name = value.name.Substring(element.prefix.Length).ToLower();

                        if (name.StartsWith("_"))
                            name = name.Substring(1);

                        name = NameNormalizer.instance.FixName(name);

                        field.Name = name;
                        field.InitExpression = new CodeSnippetExpression(value.value);
                        co.Members.Add(field);
                    }
                }

                foreach (ValueElement ve in commonstyles)
                {
                    if (!usedKeys.Contains(ve.value))
                    {
                        CodeMemberField field = new CodeMemberField();
                        string name = ve.name.Substring(stylelen).ToLower();

                        if (name.StartsWith("_"))
                            name = name.Substring(1);
                        if (name.Equals("default"))
                            name = "GlobalDefault";

                        name = NameNormalizer.instance.FixName(name);

                        field.Name = name;
                        field.InitExpression = new CodeSnippetExpression(ve.value);
                        co.Members.Add(field);
                    }
                }
            }

			//	WorkItem 2378: Chris Rickard 2006-08-21 See writeOverReadOnlyFile() comments
			writeOverReadOnlyFile(filename, cg, cnamespace);
        }

        protected CodeMethodInvokeExpression Perform(int index)
        {
            CodeMethodInvokeExpression perform = new CodeMethodInvokeExpression();
            perform.Method = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), PERFORM);
            perform.Parameters.Add(new CodePrimitiveExpression(index));

            return perform;
        }
        protected CodeMethodInvokeExpression Perform(int index, string param1)
        {
            CodeMethodInvokeExpression perform = new CodeMethodInvokeExpression();
            perform.Method = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), PERFORM);
            perform.Parameters.Add(new CodePrimitiveExpression(index));
            perform.Parameters.Add(new CodeSnippetExpression(param1));

            return perform;
        }
        protected CodeMethodInvokeExpression Perform(int index, string param1, string param2)
        {
            CodeMethodInvokeExpression perform = new CodeMethodInvokeExpression();
            perform.Method = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), PERFORM);
            perform.Parameters.Add(new CodePrimitiveExpression(index));
            perform.Parameters.Add(new CodeSnippetExpression(param1));
            perform.Parameters.Add(new CodeSnippetExpression(param2));
            return perform;
        }
        protected CodeMethodInvokeExpression PerformP1Void(int index, string param2)
        {
            CodeMethodInvokeExpression perform = new CodeMethodInvokeExpression();
            perform.Method = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), PERFORM);
            perform.Parameters.Add(new CodePrimitiveExpression(index));
            perform.Parameters.Add(new CodeSnippetExpression("VOID.NULL"));
            perform.Parameters.Add(new CodeSnippetExpression(param2));
            return perform;
        }

        protected CodeMethodInvokeExpression PerformAsStringResult(int index)
        {
            CodeMethodInvokeExpression perform = new CodeMethodInvokeExpression();

            perform.Method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(new CodeTypeReference("ScintillaControl")), PERFORMASSTRINGRESULT);
            perform.Parameters.Add(new CodePrimitiveExpression(index));

            return perform;
        }


        protected CodeExpression AsBool(CodeExpression expr)
        {

            CodeBinaryOperatorExpression asbool = new CodeBinaryOperatorExpression(expr,CodeBinaryOperatorType.IdentityInequality,new CodePrimitiveExpression(0) );
            return asbool;
        }

        protected CodeMemberMethod NewMethod(Element element)
        {
            CodeMemberMethod method = new CodeMemberMethod();

            method.Name = NameNormalizer.instance.FixName( element.name );
            if (element as GetAccessorElement != null || element as SetAccessorElement != null)
                method.Comments.Add(new CodeCommentStatement(new CodeComment("NOTE: originally a property:" + element.name)));

            //method.Comments.Add(new CodeCommentStatement(new CodeComment(string.Format("<summary>\n {0}\n </summary>", element.comment.Replace("\n", " ")), true)));
			method.Comments.Add(new CodeCommentStatement(new CodeComment(string.Format("<include file='..\\Help\\GeneratedInclude.xml' path='root/Methods/Method[@name=\"{0}\"]/*' />", method.Name), true)));
            method.Attributes = MemberAttributes.Public;
            return method;
        }

        protected CodeMemberProperty NewProperty(Element element)
        {
            CodeMemberProperty property = new CodeMemberProperty();
            property.CustomAttributes.Add(new CodeAttributeDeclaration("Browsable", new CodeAttributeArgument( new CodePrimitiveExpression( false))));
            property.CustomAttributes.Add(new CodeAttributeDeclaration("DesignerSerializationVisibility", new CodeAttributeArgument(new CodeSnippetExpression("DesignerSerializationVisibility.Hidden"))));
            
            string name = element.name;
            if( name.StartsWith("Get") )
            {
                name= name.Substring(4);
            }

            if (name.StartsWith("AutoC"))
            {
                name = name.Replace("Get", "");
            }


            if (name.Equals("Anchor"))
                name = "Anchor_";
            if (name.Equals("Cursor"))
                name = "Cursor_";

            if (element.clrReturnType.Equals(typeof(bool)))
            {
                if (!(
                        element.name.StartsWith("Can") ||
                        element.name.StartsWith("Is")))
                {
                    name = "Is" + name;
                }
            }
            name = NameNormalizer.instance.FixName(name);
            property.Name = name;
            
            //property.Comments.Add(new CodeCommentStatement(new CodeComment(string.Format("<summary>\n {0}\n </summary>", element.comment.Replace("\n", " ")), true)));
			property.Comments.Add(new CodeCommentStatement(new CodeComment(string.Format("<include file='..\\Help\\GeneratedInclude.xml' path='root/Properties/Property[@name=\"{0}\"]/*' />", property.Name), true)));
            property.Attributes = MemberAttributes.Public;
            property.Type = new CodeTypeReference( element.clrReturnType );
            return property;
        }


        private void EmitFunctions(CodeTypeDeclaration co)
        {

            foreach (FunctionElement element in Functions)
                EmitMethod(co, element);

            foreach (FunctionElement e in Functions)
            {
                if (!e.emitted)
                {
                    co.Comments.Add(new CodeCommentStatement(string.Format("Function {0} {1}({2},{3}) skipped.", e.returnType, e.name, e.p1type, e.p2type)));
                }
            }


        }

        private void EmitMethod(CodeTypeDeclaration co, featureElement element)
        {
            CodeMemberMethod method = null;
            CodeMemberProperty property = null;
            if (element.emitted)
                return;
            switch (element.FunctionType)
            {
                // void(,)
                case FunctionTypes.ret_void | FunctionTypes.p1_void | FunctionTypes.p2_void:
                    method = NewMethod(element);
                    method.Statements.Add(Perform(element.index));
                    break;

                // void(int,)
                // void(bool,)
                // void(string,)
                case FunctionTypes.ret_void | FunctionTypes.p1_int | FunctionTypes.p2_void:
                case FunctionTypes.ret_void | FunctionTypes.p1_bool | FunctionTypes.p2_void:
                case FunctionTypes.ret_void | FunctionTypes.p1_string | FunctionTypes.p2_void:
                    method = NewMethod(element);
                    method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP1Type, element.p1name));
                    method.Statements.Add(Perform(element.index, element.p1name));
                    break;

                // void(int,int)
                // void(int,bool)
                // void(bool,int)
                // void(bool,bool)
                // void(string,string)
                // void(string,int)
                // void(string,bool)
                // void(bool,string)
                case FunctionTypes.ret_void | FunctionTypes.p1_int | FunctionTypes.p2_int:
                case FunctionTypes.ret_void | FunctionTypes.p1_bool | FunctionTypes.p2_bool:
                case FunctionTypes.ret_void | FunctionTypes.p1_int | FunctionTypes.p2_bool:
                case FunctionTypes.ret_void | FunctionTypes.p1_bool | FunctionTypes.p2_int:
                case FunctionTypes.ret_void | FunctionTypes.p1_string | FunctionTypes.p2_string:
                case FunctionTypes.ret_void | FunctionTypes.p1_string | FunctionTypes.p2_int:
                case FunctionTypes.ret_void | FunctionTypes.p1_bool | FunctionTypes.p2_string:
                case FunctionTypes.ret_void | FunctionTypes.p1_string | FunctionTypes.p2_bool:
                    method = NewMethod(element);
                    method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP1Type, element.p1name));
                    method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP2Type, element.p2name));
                    method.Statements.Add(Perform(element.index, element.p1name, element.p2name));
                    break;

                // void(int,string)
                case FunctionTypes.ret_void | FunctionTypes.p1_int | FunctionTypes.p2_string:
                    method = NewMethod(element);

                    if (element.p1name.Equals("length", StringComparison.CurrentCultureIgnoreCase))
                    {
                        method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP2Type, element.p2name));
                        method.Statements.Add(Perform(element.index, element.p2name + ".Length", element.p2name));
                    }
                    else
                    {
                        method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP1Type, element.p1name));
                        method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP2Type, element.p2name));
                        method.Statements.Add(Perform(element.index, element.p1name, element.p2name));
                    }
                    break;

                // void(,int)
                // void(,bool)
                // void(,string)
                case FunctionTypes.ret_void | FunctionTypes.p1_void | FunctionTypes.p2_int:
                case FunctionTypes.ret_void | FunctionTypes.p1_void | FunctionTypes.p2_bool:
                case FunctionTypes.ret_void | FunctionTypes.p1_void | FunctionTypes.p2_string:
                    method = NewMethod(element);
                    method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP2Type, element.p2name));
                    method.Statements.Add(PerformP1Void(element.index, element.p2name));
                    break;


                // int(,)
                case FunctionTypes.ret_int | FunctionTypes.p1_void | FunctionTypes.p2_void:
                    method = NewMethod(element);
                    method.ReturnType = new CodeTypeReference(typeof(int));
                    method.Statements.Add(new CodeMethodReturnStatement(Perform(element.index)));
                    break;

                // int(int,)
                // int(bool,)
                // int(string,)
                case FunctionTypes.ret_int | FunctionTypes.p1_int | FunctionTypes.p2_void:
                case FunctionTypes.ret_int | FunctionTypes.p1_bool | FunctionTypes.p2_void:
                case FunctionTypes.ret_int | FunctionTypes.p1_string | FunctionTypes.p2_void:
                    method = NewMethod(element);
                    method.ReturnType = new CodeTypeReference(typeof(int));
                    method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP1Type, element.p1name));
                    method.Statements.Add(new CodeMethodReturnStatement(Perform(element.index, element.p1name)));
                    break;
                    
                // bool(int,)
                // bool(bool,)
                case FunctionTypes.ret_bool | FunctionTypes.p1_int | FunctionTypes.p2_void:
                case FunctionTypes.ret_bool | FunctionTypes.p1_bool | FunctionTypes.p2_void:
                    method = NewMethod(element);
                    method.ReturnType = new CodeTypeReference(typeof(bool));
                    method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP1Type, element.p1name));
                    method.Statements.Add(new CodeMethodReturnStatement(AsBool(Perform(element.index, element.p1name))));
                    break;

                // int(int,int)
                // int(int,bool)
                // int(bool,int)
                // int(bool,bool)
                case FunctionTypes.ret_int | FunctionTypes.p1_int | FunctionTypes.p2_int:
                case FunctionTypes.ret_int | FunctionTypes.p1_bool | FunctionTypes.p2_bool:
                case FunctionTypes.ret_int | FunctionTypes.p1_int | FunctionTypes.p2_bool:
                case FunctionTypes.ret_int | FunctionTypes.p1_bool | FunctionTypes.p2_int:

                    method = NewMethod(element);
                    method.ReturnType = new CodeTypeReference(typeof(int));
                    method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP1Type, element.p1name));
                    method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP2Type, element.p2name));
                    method.Statements.Add(new CodeMethodReturnStatement(Perform(element.index, element.p1name, element.p2name)));
                    break;

                // int(int,string)
                case FunctionTypes.ret_int | FunctionTypes.p1_int | FunctionTypes.p2_string:
                    method = NewMethod(element);
                    method.ReturnType = new CodeTypeReference(typeof(int));

                    if (element.p1name.Equals("length", StringComparison.CurrentCultureIgnoreCase))
                    {
                        method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP2Type, element.p2name));
                        method.Statements.Add(new CodeMethodReturnStatement(Perform(element.index, element.p2name + ".Length", element.p2name)));
                    }
                    else
                    {
                        method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP1Type, element.p1name));
                        method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP2Type, element.p2name));
                        method.Statements.Add(new CodeMethodReturnStatement(Perform(element.index, element.p1name, element.p2name)));
                    }
                    break;
                // int(,int)
                // int(,bool)
                case FunctionTypes.ret_int | FunctionTypes.p1_void | FunctionTypes.p2_int:
                case FunctionTypes.ret_int | FunctionTypes.p1_void | FunctionTypes.p2_bool:
                    method = NewMethod(element);
                    method.ReturnType = new CodeTypeReference(typeof(int));
                    method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP2Type, element.p2name));
                    method.Statements.Add(new CodeMethodReturnStatement(PerformP1Void(element.index, element.p2name)));
                    break;
                    

                // bool(,)
                case FunctionTypes.ret_bool | FunctionTypes.p1_void | FunctionTypes.p2_void:
                    property = NewProperty(element);
                    property.HasGet = true;
                    property.GetStatements.Add(new CodeMethodReturnStatement(AsBool(Perform(element.index))));
                    break;

                // int(,stringresult)
                case FunctionTypes.ret_int | FunctionTypes.p1_void | FunctionTypes.p2_stringresult:
                    method = NewMethod(element);
                    method.ReturnType = new CodeTypeReference(typeof(string));
                    // method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP2Type, element.p2name));
                    method.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(string)), "result"));
                    method.Statements.Add(Perform(element.index, "out result"));
                    method.Statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("result")));
                    break;

                // int(int,stringresult)
                // int(string,stringresult)
                case FunctionTypes.ret_int | FunctionTypes.p1_int | FunctionTypes.p2_stringresult:
                case FunctionTypes.ret_int | FunctionTypes.p1_string | FunctionTypes.p2_stringresult:
                    method = NewMethod(element);
                    method.ReturnType = new CodeTypeReference(typeof(string));
                    if (element.p1name.Equals("length", StringComparison.CurrentCultureIgnoreCase))
                    {
                        method.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(string)), "result"));
                        method.Statements.Add(Perform(element.index, "out result"));
                        method.Statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("result")));
                    }
                    else
                    {
                        method.Parameters.Add(new CodeParameterDeclarationExpression(element.clrP1Type, element.p1name));
                        method.Statements.Add(new CodeVariableDeclarationStatement(new CodeTypeReference(typeof(string)), "result"));
                        method.Statements.Add(Perform(element.index, element.p1name, "out result"));
                        method.Statements.Add(new CodeMethodReturnStatement(new CodeSnippetExpression("result")));
                    }
                    break;


                /* case :
                    property = NewProperty(element);

                    property.HasGet = true;
                    property.GetStatements.Add(new CodeMethodReturnStatement(Perform(element.index)));
                    break;
                   */

                default:
                    break;
            }
            if (method != null)
            {
                co.Members.Add(method);
                element.emitted = true;
            }
            if (property != null)
            {
                co.Members.Add(property);
                element.emitted = true;
            }
        }

        public void EmitProperties(CodeTypeDeclaration co)
        {
            foreach (GetAccessorElement GetAccessor in Gets)
            {
                if (GetAccessor.emitted)
                    continue;

                SetAccessorElement SetAccessor = null; 
                // find setter
                foreach (SetAccessorElement s in Sets)
                {
                    string match = GetAccessor.name;
                    if (GetAccessor.name.StartsWith("AutoC"))
                        match = GetAccessor.name.Replace("Get", "Set");

                    if (s.name.Equals(match))
                    {
                        SetAccessor = s;
                        break;
                    }
                }

                CodeMemberProperty property = null;
                if (SetAccessor != null)
                {
                    switch (GetAccessor.FunctionType)
                    {
                        case FunctionTypes.ret_int | FunctionTypes.p1_void | FunctionTypes.p2_void:
                            property = NewProperty(GetAccessor);
                            property.HasGet = true;
                            property.HasSet = true;
                            property.GetStatements.Add(new CodeMethodReturnStatement(Perform(GetAccessor.index)));
                            property.SetStatements.Add(Perform(SetAccessor.index, "value"));
                            GetAccessor.emitted = true;
                            SetAccessor.emitted = true;
                            break;

                        case FunctionTypes.ret_bool | FunctionTypes.p1_void | FunctionTypes.p2_void:
                            property = NewProperty(GetAccessor);
                            property.HasGet = true;
                            property.HasSet = true;

                            property.GetStatements.Add(new CodeMethodReturnStatement(AsBool(Perform(GetAccessor.index))));
                            property.SetStatements.Add(Perform(SetAccessor.index, "value"));
                            GetAccessor.emitted = true;
                            SetAccessor.emitted = true;

                            break;
                    }

                }
                else
                {
                    switch (GetAccessor.FunctionType)
                    {
                        case FunctionTypes.ret_int | FunctionTypes.p1_void | FunctionTypes.p2_void:
                            property = NewProperty(GetAccessor);
                            property.HasGet = true;
                            property.GetStatements.Add(new CodeMethodReturnStatement(Perform(GetAccessor.index)));
                            GetAccessor.emitted = true;
                            break;

                        case FunctionTypes.ret_bool | FunctionTypes.p1_void | FunctionTypes.p2_void:
                            property = NewProperty(GetAccessor);
                            property.HasGet = true;
                            property.GetStatements.Add(new CodeMethodReturnStatement(AsBool(Perform(GetAccessor.index))));
                            GetAccessor.emitted = true;
                            break;
                    }
                }
                

                if (property != null)
                    co.Members.Add(property);

            }
            foreach (GetAccessorElement e in Gets)
            {
                if (!e.emitted)
                    EmitMethod(co, e);
            }
            foreach (SetAccessorElement e in Sets)
            {
                if (!e.emitted)
                    EmitMethod(co, e);
            }
            foreach (GetAccessorElement e in Gets)
            {
                if (!e.emitted)
                {
                    co.Comments.Add(new CodeCommentStatement(string.Format("Getter {0} {1}({2},{3}) skipped.", e.returnType, e.name, e.p1type, e.p2type)));
                }
            }
            foreach (SetAccessorElement e in Sets)
            {
                if (!e.emitted)
                {
                    co.Comments.Add(new CodeCommentStatement(string.Format("Setter {0} {1}({2},{3}) skipped.", e.returnType, e.name, e.p1type, e.p2type)));
                }
            }

        }

        public void EmitEvents(CodeTypeDeclaration co)
        {
            string membertext = "";            
            foreach (EventElement element in Events)
            {
                if (element.emitted)
                    continue;

                string name = NameNormalizer.instance.FixName(element.name);
                if (name == "Zoom")
                    name = "SCZoom";
                if (name == "DoubleClick")
                    name = "SCDoubleClick";
                if (name == "Key")
                    name = "SCKey";
                
                string text = string.Format(
@"
		/// <include file='..\\Help\\GeneratedInclude.xml' path='root/Events/Event[@name=""{0}""]/*' />
		private static readonly object {0}EventKey = new object();  
		public event EventHandler<{0}EventArgs> {0}
        {{
            add {{ Events.AddHandler({0}EventKey, value); }}
            remove {{ Events.RemoveHandler({0}EventKey, value); }}
        }}
", name);
                
                membertext +=string.Format(
@"                    case Scintilla.Enums.Events.{0}:
                    if (Events[{0}EventKey] != null)
                        ((EventHandler<{0}EventArgs>)Events[{0}EventKey])(this, new {0}EventArgs(notification));
                    break;

", name);
                
                
                CodeSnippetTypeMember cme = new CodeSnippetTypeMember(text);
                
                co.Members.Add(cme);
                element.emitted = true;
                    
            }

            CodeSnippetTypeMember cstm =
                new CodeSnippetTypeMember(
                    string.Format(
                        @"
        internal void DispatchScintillaEvent(SCNotification notification)
        {{
            switch (notification.nmhdr.code)
            {{
{0}

            }}
        }}
",
                        membertext));
            co.Members.Add(cstm);

            foreach (EventElement e in Events)
            {
                if (!e.emitted)
                {
                    co.Comments.Add(new CodeCommentStatement(string.Format("Event {0} {1}({2}) skipped.", e.type, e.name, e.parameters)));
                }
            }

        }

        public void EmitScintillaControl(string filename)
        {
            CSharpCodeProvider cg = new CSharpCodeProvider();

            CodeNamespace cnamespace = new CodeNamespace("Scintilla");
            cnamespace.Imports.Add(new CodeNamespaceImport("System"));
            cnamespace.Imports.Add(new CodeNamespaceImport("System.Collections"));
            cnamespace.Imports.Add(new CodeNamespaceImport("System.Windows.Forms"));
            cnamespace.Imports.Add(new CodeNamespaceImport("System.ComponentModel"));

            CodeTypeDeclaration co = new CodeTypeDeclaration("ScintillaControl");

            co.IsClass = true;
            co.IsPartial = true;
            // co.BaseTypes.Add(new CodeTypeReference("Control"));

            cnamespace.Types.Add(co);

            EmitFunctions(co);
            EmitProperties(co);
            EmitEvents(co);

			//	WorkItem 2378: Chris Rickard 2006-08-21 See writeOverReadOnlyFile() comments
			writeOverReadOnlyFile(filename, cg, cnamespace);
        }

		//	WorkItem 2378: Chris Rickard 2006-08-21
		/// <summary>
		///	TFS Makes all files under source control ReadOnly which
		///	makes the BindingGenerator 'splode. We need to un-readonly
		///	these files while overwriting them then change them back to
		///	make source control work properly. 
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="cg"></param>
		/// <param name="cnamespace"></param>
		private void writeOverReadOnlyFile(string filename, CSharpCodeProvider cg, CodeNamespace cnamespace)
		{			
			bool isReadOnly = false;
			FileAttributes att = FileAttributes.Normal;
			if(File.Exists(filename))
			{
				att = File.GetAttributes(filename);
				isReadOnly = (att & FileAttributes.ReadOnly) == FileAttributes.ReadOnly;
			}

			if(isReadOnly)
				File.SetAttributes(filename, att ^ FileAttributes.ReadOnly);

			using(TextWriter writer = new StreamWriter(filename))
				cg.GenerateCodeFromNamespace(cnamespace, writer, options);

			if(isReadOnly)
				File.SetAttributes(filename, att | FileAttributes.ReadOnly);
		}
    }
}


