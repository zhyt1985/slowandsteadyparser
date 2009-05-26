// ***************************************************************
//  ScriptScanner   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Tags;

namespace Winista.Text.HtmlParser.Scanners
{
	/// <summary> The ScriptScanner handles script CDATA.</summary>
	[Serializable]
	public class ScriptScanner:CompositeTagScanner
	{
		/// <summary> Strict parsing of CDATA flag.
		/// If this flag is set true, the parsing of script is performed without
		/// regard to quotes. This means that erroneous script such as:
		/// <pre>
		/// document.write("&lt;/script&gt");
		/// </pre>
		/// will be parsed in strict accordance with appendix
		/// <a href="http://www.w3.org/TR/html4/appendix/notes.html#notes-specifying-data">
		/// B.3.2 Specifying non-HTML data</a> of the
		/// <a href="http://www.w3.org/TR/html4/">HTML 4.01 Specification</a> and
		/// hence will be split into two or more nodes. Correct javascript would
		/// escape the ETAGO:
		/// <pre>
		/// document.write("&lt;\/script&gt");
		/// </pre>
		/// If true, CDATA parsing will stop at the first ETAGO ("&lt;/") no matter
		/// whether it is quoted or not. If false, balanced quotes (either single or
		/// double) will shield an ETAGO. Beacuse of the possibility of quotes within
		/// single or multiline comments, these are also parsed. In most cases,
		/// users prefer non-strict handling since there is so much broken script
		/// out in the wild.
		/// </summary>
		public static bool STRICT = false;

		/// <summary> Create a script scanner.</summary>
		public ScriptScanner()
		{
		}

		/// <summary> Scan for script.
		/// Accumulates text from the page, until &lt;/[a-zA-Z] is encountered.
		/// </summary>
		/// <param name="tag">The tag this scanner is responsible for.
		/// </param>
		/// <param name="lexer">The source of CDATA.
		/// </param>
		/// <param name="stack">The parse stack, <em>not used</em>.
		/// </param>
		public override ITag Scan(ITag tag, Lexer lexer, NodeList stack)
		{
			System.String language;
			System.String code;
			INode content;
			int position;
			INode node;
			TagAttribute attribute;
			System.Collections.ArrayList vector;
			
			if (tag is ScriptTag)
			{
				language = ((ScriptTag) tag).Language;
				if ((null != language) && (language.ToUpper().Equals("JScript.Encode".ToUpper()) || language.ToUpper().Equals("VBScript.Encode".ToUpper())))
				{
					code = ScriptDecoder.Decode(lexer.Page, lexer.Cursor);
					((ScriptTag) tag).ScriptCode = code;
				}
			}
			content = lexer.ParseCDATA(!STRICT);
			position = lexer.Position;
			node = lexer.NextNode(false);
			if (null != node)
				if (!(node is ITag) || !(((ITag) node).IsEndTag() && ((ITag) node).TagName.Equals(tag.Ids[0])))
				{
					lexer.Position = position;
					node = null;
				}
			
			// build new end tag if required
			if (null == node)
			{
				attribute = new TagAttribute("/script", null);
				vector = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
				vector.Add(attribute);
				node = lexer.NodeFactory.CreateTagNode(lexer.Page, position, position, vector);
			}
			tag.SetEndTag((ITag) node);
			if (null != content)
			{
				tag.Children = new NodeList(content);
				content.Parent = tag;
			}
			node.Parent = tag;
			tag.DoSemanticAction();
			
			return (tag);
		}
	}
}
