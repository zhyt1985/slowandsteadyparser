// ***************************************************************
//  StyleScanner   version:  1.0   date: 12/18/2005
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

namespace Winista.Text.HtmlParser.Scanners
{
	/// <summary> The StyleScanner handles style elements.
	/// It gathers all interior nodes into one undifferentiated string node.
	/// </summary>
	[Serializable]
	public class StyleScanner:CompositeTagScanner
	{
		public StyleScanner()
		{
		}

		/// <summary> Scan for style definitions.
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
			INode content;
			int position;
			INode node;
			TagAttribute attribute;
			System.Collections.ArrayList vector;
			
			content = lexer.ParseCDATA();
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
				attribute = new TagAttribute("/style", null);
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
