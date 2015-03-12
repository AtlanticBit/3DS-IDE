// ------------------------------------------------------------
// Writer, WYSIWYG editor for HTML
// Copyright (c) 2002-2003 Lutz Roeder. All rights reserved.
// http://www.aisto.com/roeder
// ------------------------------------------------------------
// Added by Husein Choroomi (http://www.ManagedComponents.com)
// ------------------------------------------------------------
// Based on HTML editor control code
// Copyright (c) 2002-2003 Nikhil Kothari. All rights reserved.
// http://www.nikhilk.net
// ------------------------------------------------------------
namespace HTMLEditor 
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// </summary>
    /// TODO: Have the formatter take a TextBuffer and format that buffer and output a string
    /// TODO: Handle TD tags correctly
    public class HtmlFormatter 
    {
        private static IDictionary tagTable;

        private delegate bool CanContainTag(TagInfo info);

        private static TagInfo commentTag;
        private static TagInfo directiveTag;
        private static TagInfo otherServerSideScriptTag;
        private static TagInfo nestedXmlTag;
        private static TagInfo unknownXmlTag;
        private static TagInfo unknownHtmlTag;

        private HtmlFormatterCase _elementCasing = HtmlFormatterCase.LowerCase;
        private HtmlFormatterCase _attributeCasing = HtmlFormatterCase.LowerCase;
        private char _indentChar = '\t';
        private int _indentSize = 1;
        private int _maxLineLength = 80;

        static HtmlFormatter() 
        {
            commentTag = new TagInfo("", FormattingFlags.Comment | FormattingFlags.NoEndTag, WhiteSpaceType.CarryThrough, WhiteSpaceType.CarryThrough, ElementType.Any);
            directiveTag = new TagInfo("", FormattingFlags.NoEndTag, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Any);
            otherServerSideScriptTag = new TagInfo("", FormattingFlags.NoEndTag | FormattingFlags.Inline, ElementType.Any);
            unknownXmlTag = new TagInfo("", FormattingFlags.Xml, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Any);
            nestedXmlTag = new TagInfo("", FormattingFlags.AllowPartialTags, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Any);
            unknownHtmlTag = new TagInfo("", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Any);

            tagTable = new HybridDictionary(true);
            tagTable["a"] =  new TagInfo("a", FormattingFlags.Inline, ElementType.Inline);
            tagTable["acronym"] = new TagInfo("acronym", FormattingFlags.Inline, ElementType.Inline);
            tagTable["address"] = new TagInfo("address", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["applet"] = new TagInfo("applet", FormattingFlags.Inline, WhiteSpaceType.CarryThrough, WhiteSpaceType.Significant, ElementType.Inline);
            tagTable["area"] = new TagInfo("area", FormattingFlags.NoEndTag);
            tagTable["b"] = new TagInfo("b", FormattingFlags.Inline, ElementType.Inline);
            tagTable["base"] = new TagInfo("base", FormattingFlags.NoEndTag);
            tagTable["basefont"] = new TagInfo("basefont", FormattingFlags.NoEndTag, ElementType.Block);
            tagTable["bdo"] = new TagInfo("bdo", FormattingFlags.Inline, ElementType.Inline);
            tagTable["bgsound"] = new TagInfo("bgsound", FormattingFlags.NoEndTag);
            tagTable["big"] = new TagInfo("big", FormattingFlags.Inline, ElementType.Inline);
            tagTable["blink"] = new TagInfo("blink", FormattingFlags.Inline);
            tagTable["blockquote"] = new TagInfo("blockquote", FormattingFlags.Inline, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["body"] = new TagInfo("body", FormattingFlags.None);
            tagTable["br"] = new TagInfo("br", FormattingFlags.NoEndTag, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Inline);
            tagTable["button"] = new TagInfo("button", FormattingFlags.Inline, ElementType.Inline);
            tagTable["caption"] = new TagInfo("caption", FormattingFlags.None);
            tagTable["cite"] = new TagInfo("cite", FormattingFlags.Inline, ElementType.Inline);
            tagTable["center"] = new TagInfo("center", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["code"] = new TagInfo("code", FormattingFlags.Inline, ElementType.Inline);
            tagTable["col"] = new TagInfo("col", FormattingFlags.NoEndTag);
            tagTable["colgroup"] = new TagInfo("colgroup", FormattingFlags.None);
            tagTable["dd"] = new TagInfo("dd", FormattingFlags.None);
            tagTable["del"] = new TagInfo("del", FormattingFlags.None);
            tagTable["dfn"] = new TagInfo("dfn", FormattingFlags.Inline, ElementType.Inline);
            tagTable["dir"] = new TagInfo("dir", FormattingFlags.None, ElementType.Block);
            tagTable["div"] = new TagInfo("div", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["dl"] = new TagInfo("dl", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["dt"] = new TagInfo("dt", FormattingFlags.Inline);
            tagTable["em"] = new TagInfo("em", FormattingFlags.Inline, ElementType.Inline);
            tagTable["embed"] = new TagInfo("embed", FormattingFlags.Inline, WhiteSpaceType.Significant, WhiteSpaceType.CarryThrough, ElementType.Inline);
            tagTable["fieldset"] = new TagInfo("fieldset", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["font"] = new TagInfo("font", FormattingFlags.Inline, ElementType.Inline);
            tagTable["form"] = new TagInfo("form", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["frame"] = new TagInfo("frame", FormattingFlags.NoEndTag);
            tagTable["frameset"] = new TagInfo("frameset", FormattingFlags.None);
            tagTable["head"] = new TagInfo("head", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant);
            tagTable["h1"] = new TagInfo("h1", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["h2"] = new TagInfo("h2", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["h3"] = new TagInfo("h3", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["h4"] = new TagInfo("h4", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["h5"] = new TagInfo("h5", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["h6"] = new TagInfo("h6", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            // REVIEW: <hr> was changed to be an Block element b/c IE appears to allow it.
            tagTable["hr"] = new TagInfo("hr", FormattingFlags.NoEndTag, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["html"] = new TagInfo("html", FormattingFlags.NoIndent, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant);
            tagTable["i"] = new TagInfo("i", FormattingFlags.Inline, ElementType.Inline);
            tagTable["iframe"] = new TagInfo("iframe", FormattingFlags.None, WhiteSpaceType.CarryThrough, WhiteSpaceType.NotSignificant, ElementType.Inline);
            tagTable["img"] = new TagInfo("img", FormattingFlags.Inline | FormattingFlags.NoEndTag, WhiteSpaceType.Significant, WhiteSpaceType.Significant, ElementType.Inline);
            tagTable["input"] = new TagInfo("input", FormattingFlags.NoEndTag, WhiteSpaceType.Significant, WhiteSpaceType.Significant, ElementType.Inline);
            tagTable["ins"] = new TagInfo("ins", FormattingFlags.None);
            tagTable["isindex"] = new TagInfo("isindex", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.CarryThrough, ElementType.Block);
            tagTable["kbd"] = new TagInfo("kbd", FormattingFlags.Inline, ElementType.Inline);
            tagTable["label"] = new TagInfo("label", FormattingFlags.Inline, ElementType.Inline);
            tagTable["legend"] = new TagInfo("legend", FormattingFlags.None);
            tagTable["li"] = new LITagInfo();
            tagTable["link"] = new TagInfo("link", FormattingFlags.NoEndTag);
            tagTable["listing"] = new TagInfo("listing", FormattingFlags.None, WhiteSpaceType.CarryThrough, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["map"] = new TagInfo("map", FormattingFlags.Inline, ElementType.Inline);
            tagTable["marquee"] = new TagInfo("marquee", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["menu"] = new TagInfo("menu", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["meta"] = new TagInfo("meta", FormattingFlags.NoEndTag);
            tagTable["nobr"] = new TagInfo("nobr", FormattingFlags.Inline | FormattingFlags.NoEndTag, ElementType.Inline);
            tagTable["noembed"] = new TagInfo("noembed", FormattingFlags.None, ElementType.Block);
            tagTable["noframes"] = new TagInfo("noframes", FormattingFlags.None, ElementType.Block);
            tagTable["noscript"] = new TagInfo("noscript", FormattingFlags.None, ElementType.Block);
            tagTable["object"] = new TagInfo("object", FormattingFlags.None, ElementType.Inline);
            tagTable["ol"] = new OLTagInfo();
            tagTable["option"] = new TagInfo("option", FormattingFlags.None, WhiteSpaceType.Significant, WhiteSpaceType.CarryThrough);
            tagTable["p"] = new PTagInfo();
            tagTable["param"] = new TagInfo("param", FormattingFlags.NoEndTag);
            tagTable["pre"] = new TagInfo("pre", FormattingFlags.PreserveContent, WhiteSpaceType.CarryThrough, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["q"] = new TagInfo("q", FormattingFlags.Inline, ElementType.Inline);
            tagTable["rt"] = new TagInfo("rt", FormattingFlags.None);
            tagTable["ruby"] = new TagInfo("ruby", FormattingFlags.None, ElementType.Inline);
            tagTable["s"] = new TagInfo("s", FormattingFlags.Inline, ElementType.Inline);
            tagTable["samp"] = new TagInfo("samp", FormattingFlags.None, ElementType.Inline);
            tagTable["script"] = new TagInfo("script", FormattingFlags.PreserveContent, WhiteSpaceType.CarryThrough, WhiteSpaceType.CarryThrough, ElementType.Inline);
            tagTable["select"] = new TagInfo("select", FormattingFlags.None, WhiteSpaceType.CarryThrough, WhiteSpaceType.Significant, ElementType.Block);
            tagTable["small"] = new TagInfo("small", FormattingFlags.Inline, ElementType.Inline);
            tagTable["span"] = new TagInfo("span", FormattingFlags.Inline, ElementType.Inline);
            tagTable["strike"] = new TagInfo("strike", FormattingFlags.Inline, ElementType.Inline);
            tagTable["strong"] = new TagInfo("strong", FormattingFlags.Inline, ElementType.Inline);
            tagTable["style"] = new TagInfo("style", FormattingFlags.PreserveContent, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Any);
            tagTable["sub"] = new TagInfo("sub", FormattingFlags.Inline, ElementType.Inline);
            tagTable["sup"] = new TagInfo("sup", FormattingFlags.Inline, ElementType.Inline);
            tagTable["table"] = new TagInfo("table", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["tbody"] = new TagInfo("tbody", FormattingFlags.None);
            tagTable["td"] = new TDTagInfo();
            tagTable["textarea"] = new TagInfo("textarea", FormattingFlags.Inline, WhiteSpaceType.CarryThrough, WhiteSpaceType.Significant, ElementType.Inline);
            tagTable["tfoot"] = new TagInfo("tfoot", FormattingFlags.None);
            tagTable["th"] = new TagInfo("th", FormattingFlags.None);
            tagTable["thead"] = new TagInfo("thead", FormattingFlags.None);
            tagTable["title"] = new TagInfo("title", FormattingFlags.Inline);
            tagTable["tr"] = new TRTagInfo();
            tagTable["tt"] = new TagInfo("tt", FormattingFlags.Inline, ElementType.Inline);
            tagTable["u"] = new TagInfo("u", FormattingFlags.Inline, ElementType.Inline);
            tagTable["ul"] = new TagInfo("ul", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["xml"] = new TagInfo("xml", FormattingFlags.Xml, WhiteSpaceType.Significant, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["xmp"] = new TagInfo("xmp", FormattingFlags.PreserveContent, WhiteSpaceType.CarryThrough, WhiteSpaceType.NotSignificant, ElementType.Block);
            tagTable["var"] = new TagInfo("var", FormattingFlags.Inline, ElementType.Inline);
            tagTable["wbr"] = new TagInfo("wbr", FormattingFlags.Inline | FormattingFlags.NoEndTag, ElementType.Inline);
        }

        public char IndentChar 
        {
        	set
        	{
        		this._indentChar = value;	
        	}
        	
            get 
            {
                return _indentChar;
            }
        }

        public int IndentSize 
        {
        	set
        	{
        		this._indentSize = value;	
        	}
        	
            get 
            {
                return _indentSize;
            }
        }
    
        public int MaxLineLength 
        {
        	set
        	{
        		this._maxLineLength = value;	
        	}
        	
            get 
            {
                return _maxLineLength;
            }
        }

        public void Format(string input, TextWriter output) 
        {
            // Determine if we are outputting xhtml
            bool makeXhtml = true;

            // Save the max line length
            int maxLineLength = this._maxLineLength;

            // Make the indent string
            string indentString = new String(this._indentChar, this._indentSize);

            char[] chars = input.ToCharArray();
            Stack tagStack = new Stack();
            Stack writerStack = new Stack();

            // The previous begin or end tag that was seen
            FormatInfo previousTag = null;

            // The current begin or end tag that was seen
            FormatInfo currentTag = null;

            // The text between previousTag and currentTag
            string text = String.Empty;

            // True if we've seen whitespace at the end of the last text outputted
            bool sawWhiteSpace = false;

            // True if the last tag seen was self-terminated with a '/>'
            bool sawSelfTerminatingTag = false;

            // True if we saw a tag that we decided not to render
            bool ignoredLastTag = false;

            // Put the initial writer on the stack
            HtmlWriter writer = new HtmlWriter(output, indentString, maxLineLength);
            writerStack.Push(writer);

            Token t = HtmlTokenizer.GetFirstToken(chars);
            Token lastToken = t;

            while (t != null) 
            {
                writer = (HtmlWriter)writerStack.Peek();
                switch (t.Type) 
                {
                    case Token.AttrName:
                        if (makeXhtml) 
                        {
                            string attrName = String.Empty;
                            if (!previousTag.tagInfo.IsXml) 
                            {
                                // Need to lowercase the HTML attribute names for XHTML
                                attrName = t.Text.ToLower();
                            }
                            else 
                            {
                                attrName = t.Text;
                            }
                            writer.Write(attrName);

                            // If we are trying to be compliant XHTML, don't allow attribute minimization
                            Token nextToken = HtmlTokenizer.GetNextToken(t);
                            if (nextToken.Type != Token.EqualsChar) 
                            {
                                writer.Write("=\"" + attrName + "\"");
                            }
                        }
                        else 
                        {
                            // Convert the case of the attribute if the tag isn't xml
                            if (!previousTag.tagInfo.IsXml) 
                            {
                                if (this._attributeCasing == HtmlFormatterCase.UpperCase) 
                                {
                                    writer.Write(t.Text.ToUpper());
                                }
                                else if (this._attributeCasing == HtmlFormatterCase.LowerCase) 
                                {
                                    writer.Write(t.Text.ToLower());
                                }
                                else 
                                {
                                    writer.Write(t.Text);
                                }
                            }
                            else 
                            {
                                writer.Write(t.Text);
                            }
                        }
                        break;
                    case Token.AttrVal:
                        if (makeXhtml && (lastToken.Type != Token.DoubleQuote) && (lastToken.Type != Token.SingleQuote)) 
                        {
                            // If the attribute value isn't quoted, double quote it, replacing the inner double quotes
                            writer.Write('\"');
                            writer.Write(t.Text.Replace("\"", "&quot;"));
                            writer.Write('\"');
                        }
                        else 
                        {
                            writer.Write(t.Text);
                        }
                        break;
                    case Token.CloseBracket:
                        if (makeXhtml) 
                        {
                            if (ignoredLastTag) 
                            {
                                // Don't render the close bracket if we ignored the last tag
                                ignoredLastTag = false;
                            }
                            else 
                            {
                                if (sawSelfTerminatingTag && (!previousTag.tagInfo.IsComment)) 
                                {
                                    // If we saw a self terminating tag, that doesn't have the forward slash, put it in (except for comments)
                                    writer.Write(" />");
                                }
                                else 
                                {
                                    // If we are just closing a normal tag, just put in a normal close bracket
                                    writer.Write('>');
                                }
                            }
                        }
                        else 
                        {
                            // If there's no XHTML to be made, just put in a normal close bracket
                            writer.Write('>');
                        }
                        break;
                    case Token.DoubleQuote:
                        writer.Write('\"');
                        break;
                    case Token.Empty:
                        break;
                    case Token.EqualsChar:
                        writer.Write('=');
                        break;
                    case Token.Error:
                        if (lastToken.Type == Token.OpenBracket) 
                        {
                            // Since we aren't outputting open brackets right away, we might have to output one now
                            writer.Write('<');
                        }
                        writer.Write(t.Text);
                        break;
                    case Token.ForwardSlash:
                    case Token.OpenBracket:
                        // Just push these symbols on the stack for now... output them when we write the tag
                        break;
                    case Token.SelfTerminating:
                        previousTag.isEndTag = true;
                        if (!previousTag.tagInfo.NoEndTag) 
                        {
                            // If the tag that is self-terminating is normally not a self-closed tag
                            // then we've placed an entry on the stack for it.  Since it's self terminating, we now need
                            // to pop that item off of the tag stack
                            tagStack.Pop();

                            // If it was a self-closed Xml tag, then we also need to clean up the writerStack
                            if (previousTag.tagInfo.IsXml) 
                            {
                                HtmlWriter oldWriter = (HtmlWriter)writerStack.Pop();
                                writer = (HtmlWriter)writerStack.Peek();

                                // Since a self-closed xml tag can't have any text content, we can just write out the formatted contents
                                writer.Write(oldWriter.Content);
                            }
                        }
                        if ((lastToken.Type == Token.Whitespace) && (lastToken.Text.Length > 0)) 
                        {
                            writer.Write("/>");
                        }
                        else 
                        {
                            writer.Write(" />");
                        }
                        break;
                    case Token.SingleQuote:
                        writer.Write('\'');
                        break;
                    case Token.XmlDirective:
                        writer.WriteLineIfNotOnNewLine();
                        writer.Write('<');
                        writer.Write(t.Text);
                        writer.Write('>');
                        writer.WriteLineIfNotOnNewLine();
                        ignoredLastTag = true;
                        break;
                    case Token.TagName:
                    case Token.Comment:
                    case Token.InlineServerScript:
                        string tagName;

                        // Reset the self terminating tag flag
                        sawSelfTerminatingTag = false;

                        // Create or get the proper tagInfo, depending on the type of token
                        TagInfo info;
                        if (t.Type == Token.Comment) 
                        {
                            // Handle comment tags
                            tagName = t.Text;
                            info = new TagInfo(t.Text, commentTag);
                        }
                        else if (t.Type == Token.InlineServerScript) 
                        {
                            // Handle server-side script tags
                            string script = t.Text.Trim();
                            script = script.Substring(1);
                            tagName = script;
                            if (script.StartsWith("%@")) 
                            {
                                // Directives are block tags
                                info = new TagInfo(script, directiveTag);
                            }
                            else 
                            {
                                // Other server side script tags aren't
                                info = new TagInfo(script, otherServerSideScriptTag);
                            }
                        }
                        else 
                        {
                            // Otherwise, this is a normal tag, and try to get a TagInfo for it
                            tagName = t.Text;
                            info = tagTable[tagName] as TagInfo;
                            if (info == null) 
                            {
                                // if we couldn't find one, create a copy of the unknownTag with a new tagname
                                if (tagName.IndexOf(':') > -1) 
                                {
                                    // If it is a prefixed tag, it's probably unknown XML
                                    info = new TagInfo(tagName, unknownXmlTag);
                                }
                                else if (writer is XmlWriter) 
                                {
                                    info = new TagInfo(tagName, nestedXmlTag);
                                }
                                else 
                                {
                                    // If it is a not prefixed, it's probably an unknown HTML tag
                                    info = new TagInfo(tagName, unknownHtmlTag);
                                }
                            }
                            else 
                            {
                                // If it's not an unknown tag, converting to the desired case (and leave as is for PreserveCase)
                                if ((this._elementCasing == HtmlFormatterCase.LowerCase) || makeXhtml) 
                                {
                                    tagName = info.TagName;
                                }
                                else if (this._elementCasing == HtmlFormatterCase.UpperCase) 
                                {
                                    tagName = info.TagName.ToUpper();
                                }
                            }
                        }

                        if (previousTag == null) 
                        {
                            // Special case for the first tag seen
                            previousTag = new FormatInfo(info, false);
                            // Since this is the first tag, set it's indent to 0
                            previousTag.indent = 0;

                            // Push it on the stack
                            tagStack.Push(previousTag);
                            // And output the preceeding text
                            writer.Write(text);

                            if (info.IsXml) 
                            {
                                // When we encounter an xml block, create a new writer to contain the inner content of the xml
                                HtmlWriter newWriter = new XmlWriter(writer.Indent, info.TagName, indentString, maxLineLength);
                                writerStack.Push(newWriter);
                                writer = newWriter;
                            }

                            if (lastToken.Type == Token.ForwardSlash) 
                            {
                                // If this is an end tag, output the proper prefix
                                writer.Write("</");
                            }
                            else 
                            {
                                writer.Write('<');
                            }
                            // Write the name
                            writer.Write(tagName);
                            // Indicate that we've written out the last text block
                            text = String.Empty;
                        }
                        else 
                        {
                            // Put the new tag in the next spot
                            currentTag = new FormatInfo(info, (lastToken.Type == Token.ForwardSlash));

                            WhiteSpaceType whiteSpaceType;
                            if (previousTag.isEndTag) 
                            {
                                // If the previous tag is an end tag, we need to check the following whitespace
                                whiteSpaceType = previousTag.tagInfo.FollowingWhiteSpaceType;
                            }
                            else 
                            {
                                // otherwise check the initial inner whitespace
                                whiteSpaceType = previousTag.tagInfo.InnerWhiteSpaceType;
                            }

                            // Flag that indicates if the previous tag (before the text) is an inline tag
                            bool inline = previousTag.tagInfo.IsInline;
                            bool emptyXml = false;
                            bool firstOrLastUnknownXmlText = false;

                            if (writer is XmlWriter) 
                            {
                                // if we're in an xml block
                                XmlWriter xmlWriter = (XmlWriter)writer;

                                if (xmlWriter.IsUnknownXml) 
                                {
                                    // Special case for unknown XML tags
                                    // Determine if this is the first or last xml text in an unknown xml tag, so we know to preserve the text content here
                                    firstOrLastUnknownXmlText = (((previousTag.isBeginTag) && (previousTag.tagInfo.TagName.ToLower() == xmlWriter.TagName.ToLower())) ||
                                        ((currentTag.isEndTag) && (currentTag.tagInfo.TagName.ToLower() == xmlWriter.TagName.ToLower()))) &&
                                        (!FormattedTextWriter.IsWhiteSpace(text));
                                }

                                if (previousTag.isBeginTag) 
                                {
                                    if (FormattedTextWriter.IsWhiteSpace(text)) 
                                    {
                                        if ((xmlWriter.IsUnknownXml) && (currentTag.isEndTag) &&
                                            (previousTag.tagInfo.TagName.ToLower() == currentTag.tagInfo.TagName.ToLower())) 
                                        {
                                            // Special case for unknown XML tags:
                                            // If the previous tag is an open tag and the next tag is the corresponding close tag and the text is only whitespace, also
                                            // treat the tag as inline, so the begin and end tag appear on the same line
                                            inline = true;
                                            emptyXml = true;
                                            // Empty the text since we want the open and close tag to be touching
                                            text = "";
                                        }
                                    }
                                    else 
                                    {
                                        if (!xmlWriter.IsUnknownXml) 
                                        {
                                            // If there is non-whitespace text and we're in a normal Xml block, then remember that there was text
                                            xmlWriter.ContainsText = true;
                                        }
                                    }
                                }
                            }

                            // Flag that indicates if we want to preserve whitespace in the front of the text
                            bool frontWhitespace = true;

                            if ((previousTag.isBeginTag) && (previousTag.tagInfo.PreserveContent)) 
                            {
                                // If the previous tag is a begin tag and we're preserving the content as-is, just write out the text
                                writer.Write(text);
                            }
                            else 
                            {
                                if (whiteSpaceType == WhiteSpaceType.NotSignificant) 
                                {
                                    // If the whitespace is not significant in this location
                                    if (!inline && !firstOrLastUnknownXmlText) 
                                    {
                                        // If the previous tag is not an inline tag, write out a new line
                                        writer.WriteLineIfNotOnNewLine();
                                        // Since we've written out a newline, we no longer need to preserve front whitespace
                                        frontWhitespace = false;
                                    }
                                }
                                else if (whiteSpaceType == WhiteSpaceType.Significant) 
                                {
                                    // If the whitespace in this location is significant
                                    if (FormattedTextWriter.HasFrontWhiteSpace(text)) 
                                    {
                                        // If there is whitespace in the front, that means we can insert more whitespace without
                                        // changing rendering behavior
                                        if (!inline && !firstOrLastUnknownXmlText) 
                                        {
                                            // Only insert a new line if the tag isn't inline
                                            writer.WriteLineIfNotOnNewLine();
                                            frontWhitespace = false;
                                        }
                                    }
                                }
                                else if (whiteSpaceType == WhiteSpaceType.CarryThrough) 
                                {
                                    // If the whitespace in this location is carry through (meaning whitespace at the end of the previous
                                    // text block eats up any whitespace in this location
                                    if ((sawWhiteSpace) || (FormattedTextWriter.HasFrontWhiteSpace(text))) 
                                    {
                                        // If the last text block ended in whitspace or if there is already whitespace in this location
                                        // we can add a new line
                                        if (!inline && !firstOrLastUnknownXmlText) 
                                        {
                                            // Only add it if the previous tag isn't inline
                                            writer.WriteLineIfNotOnNewLine();
                                            frontWhitespace = false;
                                        }
                                    }
                                }

                                if (previousTag.isBeginTag) 
                                {
                                    // If the previous tag is a begin tag
                                    if (!previousTag.tagInfo.NoIndent && !inline) 
                                    {
                                        // Indent if desired
                                        writer.Indent++;
                                    }
                                }

                                // Special case for unknown XML tags:
                                if (firstOrLastUnknownXmlText) 
                                {
                                    writer.Write(text);
                                }
                                else 
                                {
                                    writer.WriteLiteral(text, frontWhitespace);
                                }
                            }

                            if (currentTag.isEndTag) 
                            {
                                // If the currentTag is an end tag
                                if (!currentTag.tagInfo.NoEndTag) 
                                {
                                    // Figure out where the corresponding begin tag is
                                    ArrayList popped = new ArrayList();
                                    FormatInfo formatInfo = null;

                                    bool foundOpenTag = false;

                                    bool allowPartial = false;
                                    if ((currentTag.tagInfo.Flags & FormattingFlags.AllowPartialTags) != 0) 
                                    {
                                        // Once we've exited a tag that allows partial tags, clear the flag
                                        allowPartial = true;
                                    }

                                    // Start popping off the tag stack if there are tags on the stack
                                    if (tagStack.Count > 0) 
                                    {
                                        // Keep popping until we find the right tag, remember what we've popped off
                                        formatInfo = (FormatInfo)tagStack.Pop();
                                        popped.Add(formatInfo);
                                        while ((tagStack.Count > 0) && (formatInfo.tagInfo.TagName.ToLower() != currentTag.tagInfo.TagName.ToLower())) 
                                        {
                                            if ((formatInfo.tagInfo.Flags & FormattingFlags.AllowPartialTags) != 0) 
                                            {
                                                // Special case for tags that allow partial tags inside of them.
                                                allowPartial = true;
                                                break;
                                            }
                                            formatInfo = (FormatInfo)tagStack.Pop();
                                            popped.Add(formatInfo);
                                        }

                                        if (formatInfo.tagInfo.TagName.ToLower() != currentTag.tagInfo.TagName.ToLower()) 
                                        {
                                            // If we didn't find the corresponding open tag, push everything back on
                                            for (int i = popped.Count - 1; i >= 0; i--) 
                                            {
                                                tagStack.Push(popped[i]);
                                            }
                                        }
                                        else 
                                        {
                                            foundOpenTag = true;
                                            for (int i = 0; i < popped.Count - 1; i++) 
                                            {
                                                FormatInfo fInfo = (FormatInfo)popped[i];
                                                if (fInfo.tagInfo.IsXml) 
                                                {
                                                    // If we have an xml tag that was unclosed, we need to clean up the xml stack
                                                    if (writerStack.Count > 1) 
                                                    {
                                                        HtmlWriter oldWriter = (HtmlWriter)writerStack.Pop();
                                                        writer = (HtmlWriter)writerStack.Peek();
                                                        // Write out the contents of the old writer
                                                        writer.Write(oldWriter.Content);
                                                    }
                                                }

                                                if (!fInfo.tagInfo.NoEndTag) 
                                                {
                                                    writer.WriteLineIfNotOnNewLine();
                                                    writer.Indent = fInfo.indent;
                                                    if ((makeXhtml) && (!allowPartial)) 
                                                    {
                                                        // If we're trying to be XHTML compliant, close unclosed child tags
                                                        // Don't close if we are under a tag that allows partial tags
                                                        writer.Write("</"+fInfo.tagInfo.TagName+">");
                                                    }
                                                }
                                            }

                                            // Set the indent to the indent of the corresponding open tag
                                            writer.Indent = formatInfo.indent;
                                        }
                                    }
                                    if (foundOpenTag || allowPartial) 
                                    {
                                        // Only write out the close tag if there was a corresponding open tag or we are under
                                        // a tag that allows partial tags
                                        if ((!emptyXml) && 
                                            (!firstOrLastUnknownXmlText) && 
                                            (!currentTag.tagInfo.IsInline) &&
                                            (!currentTag.tagInfo.PreserveContent) &&
                                            (  FormattedTextWriter.IsWhiteSpace(text) || 
                                            FormattedTextWriter.HasBackWhiteSpace(text) ||
                                            (currentTag.tagInfo.FollowingWhiteSpaceType == WhiteSpaceType.NotSignificant)
                                            ) &&
                                            (  !(currentTag.tagInfo is TDTagInfo) ||
                                            FormattedTextWriter.HasBackWhiteSpace(text)
                                            )
                                            ) 
                                        {                                           
                                            // Insert a newline before the next tag, if allowed
                                            writer.WriteLineIfNotOnNewLine();
                                        }
                                        // Write out the end tag prefix
                                        writer.Write("</");
                                        // Finally, write out the tag name
                                        writer.Write(tagName);
                                    }
                                    else 
                                    {
                                        ignoredLastTag = true;
                                    }

                                    if (currentTag.tagInfo.IsXml) 
                                    {
                                        // If we have an xml tag that was unclosed, we need to clean up the xml stack
                                        if (writerStack.Count > 1) 
                                        {
                                            HtmlWriter oldWriter = (HtmlWriter)writerStack.Pop();
                                            writer = (HtmlWriter)writerStack.Peek();
                                            // Write out the contents of the old writer
                                            writer.Write(oldWriter.Content);
                                        }
                                    }
                                }
                                else 
                                {
                                    ignoredLastTag = true;
                                }
                            }
                            else 
                            {
                                // If the currentTag is a begin tag
                                bool done = false;
                                // Close implicitClosure tags
                                while (!done && (tagStack.Count > 0)) 
                                {
                                    // Peek at the top of the stack to see the last unclosed tag
                                    FormatInfo fInfo = (FormatInfo)tagStack.Peek();
                                    // If the currentTag can't be a child of that tag, then we need to close that tag
                                    done = fInfo.tagInfo.CanContainTag(currentTag.tagInfo);
                                    if (!done) 
                                    {
                                        // Pop it off and write a close tag for it
                                        // REVIEW: Will XML tags always be able to contained in any tag?  If not we should be cleaning up the writerStack as well...
                                        tagStack.Pop();
                                        writer.Indent = fInfo.indent;
                                        // If we're trying to be XHTML compliant, write in the end tags
                                        if (makeXhtml) 
                                        {
                                            if (!fInfo.tagInfo.IsInline) 
                                            {
                                                // Only insert a newline if we are allowed to
                                                writer.WriteLineIfNotOnNewLine();
                                            }
                                            writer.Write("</"+fInfo.tagInfo.TagName+">");
                                        }
                                    }
                                }

                                // Remember the indent so we can properly indent the corresponding close tag for this open tag
                                currentTag.indent = writer.Indent;

                                if ((!firstOrLastUnknownXmlText) && 
                                    (!currentTag.tagInfo.IsInline) && 
                                    (!currentTag.tagInfo.PreserveContent) &&
                                    ( (FormattedTextWriter.IsWhiteSpace(text) || FormattedTextWriter.HasBackWhiteSpace(text)) ||
                                    (  (text.Length == 0) &&
                                    (  ((previousTag.isBeginTag) && (previousTag.tagInfo.InnerWhiteSpaceType == WhiteSpaceType.NotSignificant)) ||
                                    ((previousTag.isEndTag) && (previousTag.tagInfo.FollowingWhiteSpaceType == WhiteSpaceType.NotSignificant))
                                    )
                                    )
                                    ) 
                                    ) 
                                {
                                    // Insert a newline before the currentTag if we are allowed to
                                    writer.WriteLineIfNotOnNewLine();
                                }

                                if (!currentTag.tagInfo.NoEndTag) 
                                {
                                    // Only push tags with close tags onto the stack
                                    tagStack.Push(currentTag);
                                }
                                else 
                                {
                                    // If this tag doesn't have a close tag, remember that it is self terminating
                                    sawSelfTerminatingTag = true;
                                }

                                if (currentTag.tagInfo.IsXml) 
                                {
                                    // When we encounter an xml block, create a new writer to contain the inner content of the xml
                                    HtmlWriter newWriter = new XmlWriter(writer.Indent, currentTag.tagInfo.TagName, indentString, maxLineLength);
                                    writerStack.Push(newWriter);
                                    writer = newWriter;
                                }

                                writer.Write('<');
                                // Finally, write out the tag name
                                writer.Write(tagName);
                            }

                            // Remember if the text ended in whitespace
                            sawWhiteSpace = FormattedTextWriter.HasBackWhiteSpace(text);

                            // Clear out the text, since we have already outputted it
                            text = String.Empty;

                            previousTag = currentTag;
                        }
                        break;
                    case Token.ServerScriptBlock:
                    case Token.ClientScriptBlock:
                    case Token.Style:
                    case Token.TextToken:
                        // Remember all these types of tokens as text so we can output them between the tags
                        if (makeXhtml)
                        {
                            // UNDONE: Need to implement this in the tokenizer, etc... 
                            text += t.Text.Replace("&nbsp;", "&#160;");
                        }
                        else
                        {
                            text += t.Text;
                        }
                        break;
                    case Token.Whitespace:
                        if (t.Text.Length > 0) 
                        {
                            writer.Write(' ');
                        }
                        break;
                    default:
                        Debug.Fail("Invalid token type!");
                        break;
                }
                // Remember what the last token was
                lastToken = t;

                // Get the next token
                t = HtmlTokenizer.GetNextToken(t);
            }

            if (text.Length > 0) 
            {
                // Write out the last text if there is any
                writer.Write(text);
            }

            while (writerStack.Count > 1) 
            {
                // If we haven't cleared out the writer stack, do it
                HtmlWriter oldWriter = (HtmlWriter)writerStack.Pop();
                writer = (HtmlWriter)writerStack.Peek();
                writer.Write(oldWriter.Content);
            }

            // Flush the writer original
            writer.Flush();
        }

        private class FormatInfo 
        {
            public TagInfo tagInfo;
            public bool isEndTag;
            public int indent;

            public FormatInfo(TagInfo info, bool isEnd) 
            {
                tagInfo = info;
                isEndTag = isEnd;
            }

            public bool isBeginTag 
            {
                get 
                {
                    return !isEndTag;
                }
            }
        }

	    private class Token 
	    {
	        public const int Whitespace = 0;
	        public const int TagName = 1;
	        public const int AttrName = 2;
	        public const int AttrVal = 3;
	        public const int TextToken = 4;
	        public const int SelfTerminating = 5;
	        public const int Empty = 6;
	        public const int Comment = 7;
	        public const int Error = 8;
	
	        public const int OpenBracket = 10;
	        public const int CloseBracket = 11;
	        public const int ForwardSlash = 12;
	        public const int DoubleQuote = 13;
	        public const int SingleQuote = 14;
	        public const int EqualsChar = 15;
	
	        public const int ClientScriptBlock = 20;
	        public const int Style = 21;
	        public const int InlineServerScript = 22;
	        public const int ServerScriptBlock = 23;
	        public const int XmlDirective = 24;
	
	        private int _type;
	        private char[] _chars;
	        private int _charsLength;
	        private string _text;
	        private int _startIndex;
	        private int _endIndex;
	        private int _endState;
	
	        public Token(int type, int endState, int startIndex, int endIndex, char[] chars, int charsLength) 
	        {
	            _type = type;
	            _chars = chars;
	            _charsLength = charsLength;
	            _startIndex = startIndex;
	            _endIndex = endIndex;
	            _endState = endState;
	        }
	
	        internal char[] Chars 
	        {
	            get 
	            {
	                return _chars;
	            }
	        }
	
	        internal int CharsLength 
	        {
	            get 
	            {
	                return _charsLength;
	            }
	        }
	
	        public int EndIndex 
	        {
	            get 
	            {
	                return _endIndex;
	            }
	        }
	
	        public int EndState 
	        {
	            get 
	            {
	                return _endState;
	            }
	        }
	
	        public int Length 
	        {
	            get 
	            {
	                return _endIndex - _startIndex;
	            }
	        }
	
	        public int StartIndex 
	        {
	            get 
	            {
	                return _startIndex;
	            }
	        }
	
	        public string Text 
	        {
	            get 
	            {
	                if (_text == null) 
	                {
	                    _text = new String(_chars, StartIndex, EndIndex - StartIndex);
	                }
	                return _text;
	            }
	        }
	
	        public int Type 
	        {
	            get 
	            {
	                return _type;
	            }
	        }
	
			#if DEBUG
	        public override string ToString() 
	        {
	            string s = "\'" + Text + "\'";
	            switch (Type) 
	            {
	                case Whitespace:
	                    s += "(Whitespace)";
	                    break;
	                case ForwardSlash:
	                    s += "(ForwardSlash)";
	                    break;
	                case DoubleQuote:
	                    s += "(DoubleQuote)";
	                    break;
	                case SingleQuote:
	                    s += "(SingleQuote)";
	                    break;
	                case OpenBracket:
	                    s += "(OpenBracket)";
	                    break;
	                case CloseBracket:
	                    s += "(CloseBracket)";
	                    break;
	                case EqualsChar:
	                    s += "(Equals)";
	                    break;
	                case TagName:
	                    s += "(Tag)";
	                    break;
	                case AttrName:
	                    s += "(AttrName)";
	                    break;
	                case AttrVal:
	                    s += "(AttrVal)";
	                    break;
	                case TextToken:
	                    s += "(Text)";
	                    break;
	                case SelfTerminating:
	                    s += "(SelfTerm)";
	                    break;
	                case Empty:
	                    s += "(Empty)";
	                    break;
	                case Comment:
	                    s += "(Comment)";
	                    break;
	                case Error:
	                    s += "(Error)";
	                    break;
	                case ClientScriptBlock:
	                    s += "(ClientScriptBlock)";
	                    break;
	                case Style:
	                    s += "(Style)";
	                    break;
	                case InlineServerScript:
	                    s += "(InlineServerScript)";
	                    break;
	                case ServerScriptBlock:
	                    s += "(ServerScriptBlock)";
	                    break;
	            }
	
	            return s;
	        }
			#endif //DEBUG
	    }
	
	    private class HtmlTokenizer 
	    {
	        public static Token GetFirstToken(char[] chars) 
	        {
	            if (chars == null) 
	            {
	                throw new ArgumentNullException("chars");
	            }
	
	            return GetNextToken(chars, chars.Length, 0, 0);
	        }
	
	        public static Token GetFirstToken(char[] chars, int length, int initialState) 
	        {
	            return GetNextToken(chars, length, 0, initialState);
	        }
	
	        public static Token GetNextToken(Token token) 
	        {
	            if (token == null) 
	            {
	                throw new ArgumentNullException("token");
	            }
	            return GetNextToken(token.Chars, token.CharsLength, token.EndIndex, token.EndState);
	        }
	
	        public static Token GetNextToken(char[] chars, int length, int startIndex, int startState) 
	        {
	            if (chars == null) 
	            {
	                throw new ArgumentNullException("chars");
	            }
	
	            if (startIndex >= length) 
	            {
	                return null;
	            }
	
	            int state = startState;
	
	            bool inScript = ((startState & HtmlTokenizerStates.ScriptState) != 0);
	            int scriptState = (inScript ? HtmlTokenizerStates.ScriptState : 0);
	
	            bool inStyle = ((startState & HtmlTokenizerStates.StyleState) != 0);
	            int styleState = (inStyle ? HtmlTokenizerStates.StyleState : 0);
	
	            bool hasRunAt = ((startState & HtmlTokenizerStates.RunAtState) != 0);
	            int runAtState = (hasRunAt ? HtmlTokenizerStates.RunAtState : 0);
	
	            bool hasRunAtServer = ((startState & HtmlTokenizerStates.RunAtServerState) != 0);
	            int runAtServerState = (hasRunAtServer ? HtmlTokenizerStates.RunAtServerState : 0);
	
	            int index = startIndex;
	            int tokenStart = startIndex; // inclusive
	            int tokenEnd = startIndex; // exclusive
	            Token token = null;
	
	            while ((token == null) && (index < length)) 
	            {
	                char c = chars[index];
	                switch (state & 0xFF) 
	                {
	                    case HtmlTokenizerStates.Text:
	                        if (c == '<') 
	                        {
	                            state = HtmlTokenizerStates.StartTag;
	                            tokenEnd = index;
	                            token = new Token(Token.TextToken, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        break;
	                    case HtmlTokenizerStates.StartTag:
	                        if (c == '<') 
	                        {
	                            if ((index + 1 < length) && (chars[index + 1] == '%')) 
	                            {
	                                // Include the open bracket in a server-side script token
	                                state = HtmlTokenizerStates.ServerSideScript | scriptState | styleState;
	                                tokenStart = index;
	                            }
	                            else 
	                            {
	                                state = HtmlTokenizerStates.ExpTag | scriptState | styleState;
	                                tokenEnd = index + 1;
	                                token = new Token(Token.OpenBracket, state, tokenStart, tokenEnd, chars, length);
	                            }
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.ExpTag:
	                        if (c == '/') 
	                        {
	                            state = HtmlTokenizerStates.ForwardSlash | scriptState | styleState;
	                            tokenEnd = index;
	                            token = new Token(Token.Empty, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '!') 
	                        {
	                            state = HtmlTokenizerStates.BeginCommentTag1 | scriptState | styleState;
	                            tokenStart = index;
	                        }
	                        else if (c == '%') 
	                        {
	                            state = HtmlTokenizerStates.ServerSideScript;
	                            tokenStart = index;
	                        }
	                        else if (IsWordChar(c)) 
	                        {
	                            // If we get a word char, go to the in tag state
	                            state = HtmlTokenizerStates.InTagName | scriptState | styleState;
	                            tokenStart = index;
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.ServerSideScript:
	                        int endServerSideScriptIndex = IndexOf(chars, index, length, "%>");
	                        if (endServerSideScriptIndex > -1) 
	                        {
	                            state = HtmlTokenizerStates.Text;
	                            // Include the percent and close bracket in the server side script
	                            tokenEnd = endServerSideScriptIndex + 2;
	                            token = new Token(Token.InlineServerScript, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            index = length;
	                            tokenEnd = index;
	                        }
	                        break;
	                    case HtmlTokenizerStates.ForwardSlash:
	                        if (c == '/') 
	                        {
	                            state = HtmlTokenizerStates.ExpTagAfterSlash | scriptState | styleState;
	                            tokenEnd = index + 1;
	                            token = new Token(Token.ForwardSlash, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.ExpTagAfterSlash:
	                        if (IsWordChar(c)) 
	                        {
	                            // If we get a word char, go to the in tag state
	                            state = HtmlTokenizerStates.InTagName | scriptState | styleState;
	                            tokenStart = index;
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.InTagName:
	                        if (IsWhitespace(c)) 
	                        {
	                            // If we hit whitespace, return a token
	                            state = HtmlTokenizerStates.ExpAttr;
	                            tokenEnd = index;
	                            string tagName = new String(chars, tokenStart, tokenEnd - tokenStart);
	                            if (tagName.ToLower().Equals("script")) 
	                            {
	                                if (!inScript) 
	                                {
	                                    state |= HtmlTokenizerStates.ScriptState;
	                                }
	                            }
	                            else if (tagName.ToLower().Equals("style")) 
	                            {
	                                if (!inStyle) 
	                                {
	                                    state |= HtmlTokenizerStates.StyleState;
	                                }
	                            }
	                            token = new Token(Token.TagName, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '>') 
	                        {
	                            state = HtmlTokenizerStates.EndTag;
	                            tokenEnd = index;
	                            string tagName = new String(chars, tokenStart, tokenEnd - tokenStart);
	                            if (tagName.ToLower().Equals("script")) 
	                            {
	                                if (!inScript) 
	                                {
	                                    state |= HtmlTokenizerStates.ScriptState;
	                                }
	                            }
	                            else if (tagName.ToLower().Equals("style")) 
	                            {
	                                if (!inStyle) 
	                                {
	                                    state |= HtmlTokenizerStates.StyleState;
	                                }
	                            }
	                            token = new Token(Token.TagName, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (IsWordChar(c)) 
	                        {
	                            // Keep traversing if we get a word char
	                        }
	                        else if (c == '/') 
	                        {
	                            state = HtmlTokenizerStates.SelfTerminating;
	                            tokenEnd = index;
	                            string tagName = new String(chars, tokenStart, tokenEnd - tokenStart);
	                            if (tagName.ToLower().Equals("script")) 
	                            {
	                                if (!inScript) 
	                                {
	                                    state |= HtmlTokenizerStates.ScriptState;
	                                }
	                            }
	                            else if (tagName.ToLower().Equals("style")) 
	                            {
	                                if (!inStyle) 
	                                {
	                                    state |= HtmlTokenizerStates.StyleState;
	                                }
	                            }
	                            token = new Token(Token.TagName, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.BeginCommentTag1:
	                        if (c == '-') 
	                        {
	                            state = HtmlTokenizerStates.BeginCommentTag2;
	                        }
	                        else if (IsWordChar(c)) 
	                        {
	                            // This will allow the tokenizer to recognize xml directives as normal tags
	                            state = HtmlTokenizerStates.XmlDirective;
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.BeginCommentTag2:
	                        if (c == '-') 
	                        {
	                            state = HtmlTokenizerStates.InCommentTag;
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.InCommentTag:
	                        if (c == '-') 
	                        {
	                            state = HtmlTokenizerStates.EndCommentTag1;
	                        }
	                        break;
	                    case HtmlTokenizerStates.EndCommentTag1:
	                        if (c == '-') 
	                        {
	                            state = HtmlTokenizerStates.EndCommentTag2;
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.InCommentTag;
	                        }
	                        break;
	                    case HtmlTokenizerStates.EndCommentTag2:
	                        if (Char.IsWhiteSpace(c)) 
	                        {
	                        }
	                        else if (c == '>') 
	                        {
	                            state = HtmlTokenizerStates.EndTag;
	                            tokenEnd = index;
	                            token = new Token(Token.Comment, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.InCommentTag;
	                        }
	                        break;
	                    case HtmlTokenizerStates.XmlDirective:
	                        if (c == '>') 
	                        {
	                            state = HtmlTokenizerStates.EndTag;
	                            tokenEnd = index;
	                            token = new Token(Token.XmlDirective, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        break;
	                    case HtmlTokenizerStates.ExpAttr:
	                        if (IsWordChar(c)) 
	                        {
	                            state = HtmlTokenizerStates.InAttr | scriptState | styleState | runAtServerState;
	                            tokenEnd = index;
	                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '>') 
	                        {
	                            state = HtmlTokenizerStates.EndTag | scriptState | styleState | runAtServerState;
	                            tokenEnd = index;
	                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '/') 
	                        {
	                            state = HtmlTokenizerStates.SelfTerminating | scriptState | styleState;
	                            tokenEnd = index;
	                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (IsWhitespace(c)) 
	                        {
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.InAttr:
	                        if (IsWhitespace(c)) 
	                        {
	                            // If we hit whitespace, return a token
	                            state = HtmlTokenizerStates.ExpEquals | scriptState | styleState | runAtServerState;
	                            tokenEnd = index;
	
	                            if (inScript) 
	                            {
	                                // Check if this is a runat="server" script block
	                                if (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "runat") 
	                                {
	                                    state |= HtmlTokenizerStates.RunAtState;
	                                }
	                            }
	
	                            token = new Token(Token.AttrName, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '=') 
	                        {
	                            state = HtmlTokenizerStates.ExpEquals | scriptState | styleState | runAtServerState;
	                            tokenEnd = index;
	
	                            if (inScript) 
	                            {
	                                // Check if this is a runat="server" script block
	                                if (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "runat") 
	                                {
	                                    state |= HtmlTokenizerStates.RunAtState;
	                                }
	                            }
	
	                            token = new Token(Token.AttrName, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '>') 
	                        {
	                            state = HtmlTokenizerStates.EndTag | scriptState | styleState | runAtServerState;
	                            tokenEnd = index;
	                            token = new Token(Token.AttrName, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '/') 
	                        {
	                            state = HtmlTokenizerStates.SelfTerminating | scriptState | styleState;
	                            tokenEnd = index;
	                            token = new Token(Token.AttrName, state, tokenStart, tokenEnd, chars, length);
	                        }                        
	                        else if (IsWordChar(c)) 
	                        {
	                            // Keep traversing if we get a word char
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	
	                        break;
	                    case HtmlTokenizerStates.ExpEquals:
	                        if (c == '=') 
	                        {
	                            state = HtmlTokenizerStates.ExpAttrVal | scriptState | styleState | runAtState | runAtServerState;
	                            tokenStart = index;
	                            tokenEnd = index + 1;
	                            token = new Token(Token.EqualsChar, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '>') 
	                        {
	                            state = HtmlTokenizerStates.EndTag | scriptState | styleState | runAtServerState;
	                            tokenEnd = index;
	                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '/') 
	                        {
	                            state = HtmlTokenizerStates.SelfTerminating;
	                            tokenEnd = index;
	                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (IsWordChar(c)) 
	                        {
	                            state = HtmlTokenizerStates.InAttr | scriptState | styleState | runAtServerState;
	                            tokenEnd = index;
	                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (IsWhitespace(c)) 
	                        {
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	
	                        break;
	                    case HtmlTokenizerStates.EqualsChar:
	                        if (c == '=') 
	                        {
	                            state = HtmlTokenizerStates.ExpAttrVal | scriptState | styleState | runAtState | runAtServerState;
	                            tokenEnd = index + 1;
	                            token = new Token(Token.EqualsChar, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.ExpAttrVal:
	                        if (c == '\'') 
	                        {
	                            state = HtmlTokenizerStates.BeginSingleQuote | scriptState | styleState | runAtState | runAtServerState;
	                            tokenEnd = index;
	                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '\"') 
	                        {
	                            state = HtmlTokenizerStates.BeginDoubleQuote | scriptState | styleState | runAtState | runAtServerState;
	                            tokenEnd = index;
	                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (IsWordChar(c)) 
	                        {
	                            state = HtmlTokenizerStates.InAttrVal | scriptState | styleState | runAtState | runAtServerState;
	                            tokenEnd = index;
	                            token = new Token(Token.Whitespace, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (IsWhitespace(c)) 
	                        {
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.BeginDoubleQuote:
	                        if (c == '\"') 
	                        {
	                            state = HtmlTokenizerStates.InDoubleQuoteAttrVal | scriptState | styleState | runAtState | runAtServerState;
	                            tokenEnd = index + 1;
	                            token = new Token(Token.DoubleQuote, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.InDoubleQuoteAttrVal:
	                        if (c == '\"') 
	                        {
	                            state = HtmlTokenizerStates.EndDoubleQuote | scriptState | styleState | runAtServerState;
	                            tokenEnd = index;
	
	                            if ((hasRunAt) && (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "server")) 
	                            {
	                                state |= HtmlTokenizerStates.RunAtServerState;
	                            }
	
	                            token = new Token(Token.AttrVal, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        break;
	                    case HtmlTokenizerStates.EndDoubleQuote:
	                        if (c == '\"') 
	                        {
	                            state = HtmlTokenizerStates.ExpAttr | scriptState | styleState | runAtServerState;
	                            tokenEnd = index + 1;
	                            token = new Token(Token.DoubleQuote, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.BeginSingleQuote:
	                        if (c == '\'') 
	                        {
	                            state = HtmlTokenizerStates.InSingleQuoteAttrVal | scriptState | styleState | runAtState | runAtServerState;
	                            tokenEnd = index + 1;
	                            token = new Token(Token.SingleQuote, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.InSingleQuoteAttrVal:
	                        if (c == '\'') 
	                        {
	                            state = HtmlTokenizerStates.EndSingleQuote | scriptState | styleState | runAtServerState;
	                            tokenEnd = index;
	
	                            if ((hasRunAt) && (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "server")) 
	                            {
	                                state |= HtmlTokenizerStates.RunAtServerState;
	                            }
	
	                            token = new Token(Token.AttrVal, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        break;
	                    case HtmlTokenizerStates.EndSingleQuote:
	                        if (c == '\'') 
	                        {
	                            state = HtmlTokenizerStates.ExpAttr | scriptState | styleState | runAtServerState;
	                            tokenEnd = index + 1;
	                            token = new Token(Token.SingleQuote, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.InAttrVal:
	                        if (IsWhitespace(c)) 
	                        {
	                            state = HtmlTokenizerStates.ExpAttr | scriptState | styleState | runAtServerState;
	                            tokenEnd = index;
	
	                            if ((hasRunAt) && (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "server")) 
	                            {
	                                state |= HtmlTokenizerStates.RunAtServerState;
	                            }
	
	                            token = new Token(Token.AttrVal, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '>') 
	                        {
	                            state = HtmlTokenizerStates.EndTag | scriptState | styleState | runAtServerState;
	                            tokenEnd = index;
	
	                            if ((hasRunAt) && (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "server")) 
	                            {
	                                state |= HtmlTokenizerStates.RunAtServerState;
	                            }
	
	                            token = new Token(Token.AttrVal, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else if (c == '/') 
	                        {
	                            // This check fixes a bug when there's a forward slash in an attrval (since Trident likes to remove
	                            // double quotes from our attrvals
	                            if (((index + 1) < length) && (chars[index + 1] == '>')) 
	                            {
	                                state = HtmlTokenizerStates.SelfTerminating | scriptState | styleState | runAtServerState;
	                                tokenEnd = index;
	
	                                if ((hasRunAt) && (new String(chars, tokenStart, tokenEnd - tokenStart).ToLower() == "server")) 
	                                {
	                                    state |= HtmlTokenizerStates.RunAtServerState;
	                                }
	
	                                token = new Token(Token.AttrVal, state, tokenStart, tokenEnd, chars, length);
	                            }
	                        }
	                        break;
	                    case HtmlTokenizerStates.SelfTerminating:
	                        if ((c == '/') && (index + 1 < length) && (chars[index + 1] == '>')) 
	                        {
	                            state = HtmlTokenizerStates.Text;
	                            tokenEnd = index + 2;
	                            token = new Token(Token.SelfTerminating, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.EndTag:
	                        if (c == '>') 
	                        {
	                            if (inScript) 
	                            {
	                                state = HtmlTokenizerStates.Script | scriptState | styleState | runAtServerState;
	                            }
	                            else if (inStyle) 
	                            {
	                                state = HtmlTokenizerStates.Style | scriptState | styleState;
	                            }
	                            else 
	                            {
	                                state = HtmlTokenizerStates.Text;
	                            }
	                            tokenEnd = index + 1;
	                            token = new Token(Token.CloseBracket, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            state = HtmlTokenizerStates.Error;
	                        }
	                        break;
	                    case HtmlTokenizerStates.Script:
	                        int endScriptIndex = IndexOf(chars, index, length, "</script>");
	                        if (endScriptIndex > -1) 
	                        {
	                            state = HtmlTokenizerStates.StartTag | scriptState | styleState | runAtServerState;
	                            tokenEnd = endScriptIndex;
	                            if (hasRunAtServer) 
	                            {
	                                token = new Token(Token.ServerScriptBlock, state, tokenStart, tokenEnd, chars, length);
	                            }
	                            else 
	                            {
	                                token = new Token(Token.ClientScriptBlock, state, tokenStart, tokenEnd, chars, length);
	                            }
	                        }
	                        else 
	                        {
	                            index = length - 1;
	                            tokenEnd = index;
	                        }
	                        break;
	                    case HtmlTokenizerStates.Style:
	                        int endStyleIndex = IndexOf(chars, index, length, "</style>");
	                        if (endStyleIndex > -1) 
	                        {
	                            state = HtmlTokenizerStates.StartTag | scriptState | styleState;
	                            tokenEnd = endStyleIndex;
	                            token = new Token(Token.Style, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        else 
	                        {
	                            index = length - 1;
	                            tokenEnd = index;
	                        }
	                        break;
	                    case HtmlTokenizerStates.Error:
	                        if (c == '>') 
	                        {
	                            state = HtmlTokenizerStates.EndTag;
	                            tokenEnd = index;
	                            token = new Token(Token.Error, state, tokenStart, tokenEnd, chars, length);
	                        }
	                        break;
	                }
	                
	                index++;
	            }
	
	            if ((index >= length) && (token == null)) 
	            {
	                int tokenType;
	                // Some tokens can span multiple lines, so return a token if we haven't found one yet
	                switch (state & 0xFF) 
	                {
	                    case HtmlTokenizerStates.Text:
	                        tokenType = Token.TextToken;
	                        break;
	                    case HtmlTokenizerStates.Script:
	                        if (hasRunAtServer) 
	                        {
	                            tokenType = Token.ServerScriptBlock;
	                        }
	                        else 
	                        {
	                            tokenType = Token.ClientScriptBlock;
	                        }
	                        break;
	                    case HtmlTokenizerStates.Style:
	                        tokenType = Token.Style;
	                        break;
	                    case HtmlTokenizerStates.ServerSideScript:
	                        tokenType = Token.InlineServerScript;
	                        break;
	                    case HtmlTokenizerStates.BeginCommentTag1:
	                    case HtmlTokenizerStates.BeginCommentTag2:
	                    case HtmlTokenizerStates.InCommentTag:
	                    case HtmlTokenizerStates.EndCommentTag1:
	                    case HtmlTokenizerStates.EndCommentTag2:
	                        tokenType = Token.Comment;
	                        break;
	                    default:
	                        tokenType = Token.Error;
	                        state = HtmlTokenizerStates.Error;
	                        break;
	                }
	                tokenEnd = index;
	                token = new Token(tokenType, state, tokenStart, tokenEnd, chars, length);
	            }
	            return token;
	        }
	
	        private static bool IsWhitespace(char c) 
	        {
	            return Char.IsWhiteSpace(c);
	        }
	
	        private static bool IsWordChar(char c) 
	        {
	            return (Char.IsLetterOrDigit(c) || (c == '_') || (c == ':') || (c == '#') || (c == '-') || (c == '.'));
	        }
	
	        private static int IndexOf(char[] chars, int startIndex, int endColumnNumber, string s) 
	        {
	            int stringLength = s.Length;
	            int end = endColumnNumber - stringLength + 1;
	            for (int i = startIndex; i < end; i++) 
	            {
	                bool success = true;
	                for (int j = 0; j < stringLength; j++) 
	                {
	                    if (Char.ToUpper(chars[i + j]) != Char.ToUpper(s[j])) 
	                    {
	                        success = false;
	                        break;
	                    }
	                }
	                if (success) 
	                {
	                    return i;
	                }
	            }
	            return -1;
	        }
	    }

        private class HtmlTokenizerStates 
        {
            public const int Text = 0;
            public const int StartTag = 1;
            public const int ExpTag = 2;
            public const int ForwardSlash = 3;
            public const int ExpTagAfterSlash = 4;
            public const int InTagName = 5;
            public const int ExpAttr = 6;
            public const int InAttr = 7;
            public const int ExpEquals = 8;
            public const int ExpAttrVal = 9;
            public const int InDoubleQuoteAttrVal = 10;
            public const int EndDoubleQuote = 11;
            public const int InSingleQuoteAttrVal = 12;
            public const int EndSingleQuote = 13;
            public const int InAttrVal = 14;
            public const int SelfTerminating = 15;
            public const int Error = 16;
            public const int EndTag = 17;

            public const int EqualsChar = 18;
            public const int BeginDoubleQuote = 19;
            public const int BeginSingleQuote = 20;

            public const int ServerSideScript = 30;

            public const int Script = 40;

            public const int Style = 50;

            public const int XmlDirective = 60;

            public const int BeginCommentTag1 = 100;
            public const int BeginCommentTag2 = 101;
            public const int InCommentTag = 102;
            public const int EndCommentTag1 = 103;
            public const int EndCommentTag2 = 104;

            public const int ScriptState = 0x0100;
            public const int StyleState = 0x0200;
            public const int RunAtState = 0x0400;
            public const int RunAtServerState = 0x800;
        }

	    internal enum ElementType 
	    {
	        Other = 0,
	        Block = 1,
	        Inline = 2,
	        Any = 3
	    }

	    internal class FormattedTextWriter : TextWriter 
	    {
	        private TextWriter baseWriter;
	        private string indentString;
	        private int currentColumn;
	        private int indentLevel;
	        private bool indentPending;
	        private bool onNewLine;
	
	        public FormattedTextWriter(TextWriter writer, string indentString) 
	        {
	            this.baseWriter = writer;
	            this.indentString = indentString;
	            this.onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override Encoding Encoding 
	        {
	            get 
	            {
	                return baseWriter.Encoding;
	            }
	        }
	
	        public override string NewLine 
	        {
	            get 
	            {
	                return baseWriter.NewLine;
	            }
	            set 
	            {
	                baseWriter.NewLine = value;
	            }
	        }
	
	        public int Indent 
	        {
	            get 
	            {
	                return indentLevel;
	            }
	            set 
	            {
	                if (value < 0) 
	                {
	                    value = 0;
	                }
	                indentLevel = value;
	                Debug.Assert(value >= 0, "Invalid IndentLevel");
	            }
	        }
	
	        public override void Close() 
	        {
	            baseWriter.Close();
	        }
	
	        public override void Flush() 
	        {
	            baseWriter.Flush();
	        }
	
	        public static bool HasBackWhiteSpace(string s) 
	        {
	            if ((s == null) || (s.Length == 0)) 
	            {
	                return false;
	            }
	            return (Char.IsWhiteSpace(s[s.Length - 1]));
	        }
	
	        public static bool HasFrontWhiteSpace(string s) 
	        {
	            if ((s == null) || (s.Length == 0)) 
	            {
	                return false;
	            }
	            return (Char.IsWhiteSpace(s[0]));
	        }
	
	        public static bool IsWhiteSpace(string s) 
	        {
	            for (int i = 0; i < s.Length; i++) 
	            {
	                if (!Char.IsWhiteSpace(s[i])) 
	                {
	                    return false;
	                }
	            }
	
	            return true;
	        }
	
	        /// <summary>
	        /// Converts the string into a single line seperated by single spaces
	        /// </summary>
	        /// <param name="s"></param>
	        /// <returns></returns>
	        /// TODO: Rename to CollapseWhitespace
	        private string MakeSingleLine(string s) 
	        {
	            StringBuilder builder = new StringBuilder();
	            int i = 0;
	            while (i < s.Length) 
	            {
	                char c = s[i];
	                if (Char.IsWhiteSpace(c)) 
	                {
	                    builder.Append(' ');
	                    while ((i < s.Length) && (Char.IsWhiteSpace(s[i]))) 
	                    {
	                        i++;
	                    }
	                }
	                else 
	                {
	                    builder.Append(c);
	                    i++;
	                }
	            }
	            return builder.ToString();
	        }
	
	        public static string Trim(string text, bool frontWhiteSpace) 
	        {
	            if (text.Length == 0) 
	            {
	                // If there is no text, return the empty string
	                return String.Empty;
	            }
	
	            if (IsWhiteSpace(text)) 
	            {
	                // If the text is all whitespace
	                if (frontWhiteSpace) 
	                {
	                    // If the caller wanted to preserve front whitespace, then return just one space
	                    return " ";
	                }
	                else 
	                {
	                    // If the caller isn't trying to preserve anything, return the empty string
	                    return String.Empty;
	                }
	            }
	
	            // Trim off all whitespace
	            string t = text.Trim();
	            if (frontWhiteSpace && HasFrontWhiteSpace(text)) 
	            {
	                // Add front whitespace if there was some and we're trying to preserve it
	                t = ' ' + t;
	            }
	            if (HasBackWhiteSpace(text)) 
	            {
	                // Add back whitespace if there was some
	                t = t + ' ';
	            }
	            return t;
	        }
	
	        private void OutputIndent() 
	        {
	            if (indentPending) 
	            {
	                for (int i = 0; i < indentLevel; i++) 
	                {
	                    baseWriter.Write(indentString);
	                }
	                indentPending = false;
	            }
	        }
	
	        public void WriteLiteral(string s) 
	        {
	            if (s.Length != 0) 
	            {
	                StringReader reader = new StringReader(s);
	                // We want to output the first line of the string trimming the back whitespace
	                // the middle lines trimming all whitespace
	                // and the last line trimming the leading whitespace (which requires a one string lookahead)
	                string lastString = reader.ReadLine();
	                string nextString = reader.ReadLine();
	                while (lastString != null) 
	                {
	                    Write(lastString);
	                    lastString = nextString;
	                    nextString = reader.ReadLine();
	
	                    if (lastString != null) 
	                    {
	                        WriteLine();
	                    }
	                    if (nextString != null) 
	                    {
	                        lastString = lastString.Trim();
	                    }
	                    else if (lastString != null) 
	                    {                            
	                        lastString = Trim(lastString, false);
	                    }
	                }
	            }
	        }
	
	        public void WriteLiteralWrapped(string s, int maxLength) 
	        {
	            if (s.Length != 0) 
	            {
	                // First make the string a single line space-delimited string and split on the strings
	                string[] tokens = MakeSingleLine(s).Split(null);
	                // Preserve the initial whitespace
	                if (HasFrontWhiteSpace(s)) 
	                {
	                    Write(' ');
	                }
	
	                // Write out all tokens, wrapping when the length exceeds the specified length
	                for (int i = 0; i < tokens.Length; i++) 
	                {
	                    if (tokens[i].Length > 0) 
	                    {
	                        Write(tokens[i]);
	                        if ((i < (tokens.Length - 1)) && (tokens[i + 1].Length > 0)) 
	                        {
	                            if (currentColumn > maxLength) 
	                            {
	                                WriteLine();
	                            }
	                            else 
	                            {
	                                Write(' ');
	                            }
	                        }
	                    }
	                }
	
	                if (HasBackWhiteSpace(s) && !IsWhiteSpace(s)) 
	                {
	                    Write(' ');
	                }
	            }
	        }
	
	        public void WriteLineIfNotOnNewLine() 
	        {
	            if (onNewLine == false) 
	            {
	                baseWriter.WriteLine();
	                onNewLine = true;
	                currentColumn = 0;
	                indentPending = true;
	            }
	        }
	                
	        public override void Write(string s) 
	        {
	            OutputIndent();
	            baseWriter.Write(s);
	            onNewLine = false;
	            currentColumn += s.Length;
	        }
	
	        public override void Write(bool value) 
	        {
	            OutputIndent();
	            baseWriter.Write(value);
	            onNewLine = false;
	            currentColumn += value.ToString().Length;
	        }
	
	        public override void Write(char value) 
	        {
	            OutputIndent();
	            baseWriter.Write(value);
	            onNewLine = false;
	            currentColumn ++;
	        }
	
	        public override void Write(char[] buffer) 
	        {
	            OutputIndent();
	            baseWriter.Write(buffer);
	            onNewLine = false;
	            currentColumn += buffer.Length;
	        }
	
	        public override void Write(char[] buffer, int index, int count) 
	        {
	            OutputIndent();
	            baseWriter.Write(buffer, index, count);
	            onNewLine = false;
	            currentColumn += count;
	        }
	
	        public override void Write(double value) 
	        {
	            OutputIndent();
	            baseWriter.Write(value);
	            onNewLine = false;
	            currentColumn += value.ToString().Length;
	        }
	
	        public override void Write(float value) 
	        {
	            OutputIndent();
	            baseWriter.Write(value);
	            onNewLine = false;
	            currentColumn += value.ToString().Length;
	        }
	
	        public override void Write(int value) 
	        {
	            OutputIndent();
	            baseWriter.Write(value);
	            onNewLine = false;
	            currentColumn += value.ToString().Length;
	        }
	
	        public override void Write(long value) 
	        {
	            OutputIndent();
	            baseWriter.Write(value);
	            onNewLine = false;
	            currentColumn += value.ToString().Length;
	        }
	
	        public override void Write(object value) 
	        {
	            OutputIndent();
	            baseWriter.Write(value);
	            onNewLine = false;
	            currentColumn += value.ToString().Length;
	        }
	
	        public override void Write(string format, object arg0) 
	        {
	            OutputIndent();
	            string s = String.Format(format, arg0);
	            baseWriter.Write(s);
	            onNewLine = false;
	            currentColumn += s.Length;
	        }
	
	        public override void Write(string format, object arg0, object arg1) 
	        {
	            OutputIndent();
	            string s = String.Format(format, arg0, arg1);
	            baseWriter.Write(s);
	            onNewLine = false;
	            currentColumn += s.Length;
	        }
	
	        public override void Write(string format, params object[] arg) 
	        {
	            OutputIndent();
	            string s = String.Format(format, arg);
	            baseWriter.Write(s);
	            onNewLine = false;
	            currentColumn += s.Length;
	        }
	
	        public override void WriteLine(string s) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(s);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine() 
	        {
	            OutputIndent();
	            baseWriter.WriteLine();
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(bool value) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(value);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(char value) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(value);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(char[] buffer) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(buffer);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(char[] buffer, int index, int count) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(buffer, index, count);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(double value) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(value);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(float value) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(value);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(int value) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(value);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(long value) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(value);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(object value) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(value);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(string format, object arg0) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(format, arg0);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(string format, object arg0, object arg1) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(format, arg0, arg1);
	            indentPending = true;
	            onNewLine = true;
	            currentColumn = 0;
	        }
	
	        public override void WriteLine(string format, params object[] arg) 
	        {
	            OutputIndent();
	            baseWriter.WriteLine(format, arg);
	            indentPending = true;
	            currentColumn = 0;
	            onNewLine = true;
	        }
	    }

	    [Flags]
	    internal enum FormattingFlags 
	    {
	        None = 0,
	        Inline = 0x1,
	        NoIndent = 0x2,
	        NoEndTag =  0x4,
	        PreserveContent = 0x8,
	        Xml = 0x10,
	        Comment = 0x20,
	        AllowPartialTags = 0x40
	    }

	    internal class HtmlWriter 
	    {
	        private FormattedTextWriter _writer;
	        private TextWriter _baseWriter;
	        private int _maxLineLength;
	
	        public HtmlWriter(TextWriter writer, string indentString, int maxLineLength) 
	        {
	            _baseWriter = writer;
	            _maxLineLength = maxLineLength;
	            _writer = new FormattedTextWriter(_baseWriter, indentString);
	        }
	
	        protected TextWriter BaseWriter 
	        {
	            get 
	            {
	                return _baseWriter;
	            }
	        }
	
	        public virtual string Content 
	        {
	            get 
	            {
	                _writer.Flush();
	                return _baseWriter.ToString();
	            }
	        }
	
	        public int Indent 
	        {
	            get 
	            {
	                return _writer.Indent;
	            }
	            set 
	            {
	                _writer.Indent = value;
	            }
	        }
	
	        public void Flush() 
	        {
	            _writer.Flush();
	        }
	
	        public FormattedTextWriter Writer 
	        {
	            get 
	            {
	                return _writer;
	            }
	        }
	
	        public virtual void Write(char c) 
	        {
	            _writer.Write(c);
	        }
	
	        public virtual void Write(string s) 
	        {
	            _writer.Write(s);
	        }
	
	        public virtual void WriteLiteral(string s, bool frontWhiteSpace) 
	        {
	            _writer.WriteLiteralWrapped(FormattedTextWriter.Trim(s, frontWhiteSpace), _maxLineLength);
	        }
	
	        public virtual void WriteLineIfNotOnNewLine() 
	        {
	            _writer.WriteLineIfNotOnNewLine();
	        }
	    }



	    internal class LITagInfo : TagInfo 
	    {
	        public LITagInfo() : base("li", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.CarryThrough) 
	        {
	        }
	
	        public override bool CanContainTag(TagInfo info) 
	        {
	            if (info.Type == ElementType.Any) 
	            {
	                return true;
	            }
	
	            if ((info.Type == ElementType.Inline) |
	                (info.Type == ElementType.Block)) 
	            {
	                return true;
	            }
	
	            return false;
	        }
	    }

	    internal class OLTagInfo : TagInfo 
	    {
	        public OLTagInfo() : base("ol", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block) 
	        {
	        }
	
	        public override bool CanContainTag(TagInfo info) 
	        {
	            if (info.Type == ElementType.Any) 
	            {
	                return true;
	            }
	
	            if (info.TagName.ToLower().Equals("li")) 
	            {
	                return true;
	            }
	
	            return false;
	        }
	    }

	    internal class PTagInfo : TagInfo 
	    {
	        public PTagInfo() : base("p", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block) 
	        {
	        }
	
	        public override bool CanContainTag(TagInfo info) 
	        {
	            if (info.Type == ElementType.Any) 
	            {
	                return true;
	            }
	
	            if ((info.Type == ElementType.Inline) |
	                (info.TagName.ToLower().Equals("table")) |
	                (info.TagName.ToLower().Equals("hr"))) 
	            {
	                return true;
	            }
	
	            return false;
	        }
	    }
	
	    internal class TagInfo 
	    {
	        private string _tagName;
	        private WhiteSpaceType _inner;
	        private WhiteSpaceType _following;
	        private FormattingFlags _flags;
	        private ElementType _type;
	
	        public TagInfo(string tagName, FormattingFlags flags) : this(tagName, flags, WhiteSpaceType.CarryThrough, WhiteSpaceType.CarryThrough, ElementType.Other) 
	        {
	        }
	
	        public TagInfo(string tagName, FormattingFlags flags, ElementType type) : this(tagName, flags, WhiteSpaceType.CarryThrough, WhiteSpaceType.CarryThrough, type) 
	        {
	        }
	
	        public TagInfo(string tagName, FormattingFlags flags, WhiteSpaceType innerWhiteSpace, WhiteSpaceType followingWhiteSpace) : this(tagName, flags, innerWhiteSpace, followingWhiteSpace, ElementType.Other) 
	        {
	        }
	
	        public TagInfo(string tagName, FormattingFlags flags, WhiteSpaceType innerWhiteSpace, WhiteSpaceType followingWhiteSpace, ElementType type) 
	        {
	            Debug.Assert((innerWhiteSpace == WhiteSpaceType.NotSignificant) ||
	                (innerWhiteSpace == WhiteSpaceType.Significant) ||
	                (innerWhiteSpace == WhiteSpaceType.CarryThrough), "Invalid whitespace type");
	
	            Debug.Assert((followingWhiteSpace == WhiteSpaceType.NotSignificant) ||
	                (followingWhiteSpace == WhiteSpaceType.Significant) ||
	                (followingWhiteSpace == WhiteSpaceType.CarryThrough), "Invalid whitespace type");
	
	            _tagName = tagName;
	            _inner = innerWhiteSpace;
	            _following = followingWhiteSpace;
	            _flags = flags;
	            _type = type;
	        }
	
	        public TagInfo(string newTagName, TagInfo info) 
	        {
	            _tagName = newTagName;
	            _inner = info.InnerWhiteSpaceType;
	            _following = info.FollowingWhiteSpaceType;
	            _flags = info.Flags;
	            _type = info.Type;
	        }
	
	        public ElementType Type 
	        {
	            get 
	            {
	                return _type;
	            }
	        }
	
	        public FormattingFlags Flags 
	        {
	            get 
	            {
	                return _flags;
	            }
	        }
	
	        public WhiteSpaceType FollowingWhiteSpaceType 
	        {
	            get 
	            {
	                return _following;
	            }
	        }
	
	        public WhiteSpaceType InnerWhiteSpaceType 
	        {
	            get 
	            {
	                return _inner;
	            }
	        }
	
	        public bool IsComment 
	        {
	            get 
	            {
	                return ((_flags & FormattingFlags.Comment) != 0);
	            }
	        }
	
	        public bool IsInline 
	        {
	            get 
	            {
	                return ((_flags & FormattingFlags.Inline) != 0);
	            }
	        }
	
	        public bool IsXml 
	        {
	            get 
	            {
	                return ((_flags & FormattingFlags.Xml) != 0);
	            }
	        }
	
	        public bool NoEndTag 
	        {
	            get 
	            {
	                return ((_flags & FormattingFlags.NoEndTag) != 0);
	            }
	        }
	
	        public bool NoIndent 
	        {
	            get 
	            {
	                return (((_flags & FormattingFlags.NoIndent) != 0) || NoEndTag);
	            }
	        }
	
	        public bool PreserveContent 
	        {
	            get 
	            {
	                return ((_flags & FormattingFlags.PreserveContent) != 0);
	            }
	        }
	
	        public string TagName 
	        {
	            get 
	            {
	                return _tagName;
	            }
	        }
	
	        public virtual bool CanContainTag(TagInfo info) 
	        {
	            return true;
	        }
	    }

	    internal class TDTagInfo : TagInfo 
	    {
	        public TDTagInfo() : base("td", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Other) 
	        {
	        }
	
	        public override bool CanContainTag(TagInfo info) 
	        {
	            if (info.Type == ElementType.Any) 
	            {
	                return true;
	            }
	
	            if ((info.Type == ElementType.Inline) |
	                (info.Type == ElementType.Block)) 
	            {
	                return true;
	            }
	
	            return false;
	        }
	    }

	    internal class TRTagInfo : TagInfo 
	    {
	        public TRTagInfo() : base("tr", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Other) 
	        {
	        }
	
	        public override bool CanContainTag(TagInfo info) 
	        {
	            if (info.Type == ElementType.Any) 
	            {
	                return true;
	            }
	
	            if ((info.TagName.ToLower().Equals("th")) |
	                (info.TagName.ToLower().Equals("td"))) 
	            {
	                return true;
	            }
	
	            return false;
	        }
	    }

	    internal enum WhiteSpaceType 
	    {
	        Significant = 0,
	        NotSignificant = 1,
	        CarryThrough = 2
	    }

	    internal class XmlWriter : HtmlWriter 
	    {
	        private bool _containsText;
	        private StringBuilder _unformatted;
	        private string _tagName;
	        private bool _isUnknownXml;
	
	        public XmlWriter(int initialIndent, string tagName, string indentString, int maxLineLength) : base(new StringWriter(), indentString, maxLineLength) 
	        {
	            Writer.Indent = initialIndent;
	            _unformatted = new StringBuilder();
	            _tagName = tagName;
	            _isUnknownXml = (_tagName.IndexOf(':') > -1);
	        }
	
	        public bool ContainsText 
	        {
	            get 
	            {
	                return _containsText;
	            }
	            set 
	            {
	                _containsText = value;
	            }
	        }
	
	        public override string Content 
	        {
	            get 
	            {
	                if (ContainsText) 
	                {
	                    return _unformatted.ToString();
	                }
	                else 
	                {
	                    return base.Content;                    
	                }
	            }
	        }
	
	        public string TagName 
	        {
	            get 
	            {
	                return _tagName;
	            }
	        }
	
	        public bool IsUnknownXml 
	        {
	            get 
	            {
	                return _isUnknownXml;
	            }
	        }
	
	        public override void Write(char c) 
	        {
	            base.Write(c);
	            // Keep an unformatted copy around
	            _unformatted.Append(c);
	        }
	
	        public override void Write(string s) 
	        {
	            base.Write(s);
	            // Keep an unformatted copy around
	            _unformatted.Append(s);
	        }
	
	        public override void WriteLiteral(string s, bool frontWhiteSpace) 
	        {
	            base.WriteLiteral(s, frontWhiteSpace);
	            // Keep an unformatted copy around
	            _unformatted.Append(s);
	        }
	    }

	    private enum HtmlFormatterCase 
	    {
	        PreserveCase = 0,
	        UpperCase = 1,
	        LowerCase = 2
	    }
    }
}
