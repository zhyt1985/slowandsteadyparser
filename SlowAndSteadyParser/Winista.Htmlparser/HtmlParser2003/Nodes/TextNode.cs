// ***************************************************************
//  TextNode   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Visitors;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Nodes
{
	/// <summary> Normal text in the HTML document is represented by this class.</summary>
	[Serializable]
	public class TextNode:AbstractNode, IText
	{
		/// <summary> The contents of the string node, or override text.</summary>
		protected internal System.String mText;

		/// <summary> Constructor takes in the text string.</summary>
		/// <param name="text">The string node text. For correct generation of HTML, this
		/// should not contain representations of tags (unless they are balanced).
		/// </param>
		public TextNode(System.String text):base(null, 0, 0)
		{
			SetText(text);
		}
		
		/// <summary> Constructor takes in the page and beginning and ending posns.</summary>
		/// <param name="page">The page this string is on.
		/// </param>
		/// <param name="start">The beginning position of the string.
		/// </param>
		/// <param name="end">The ending positiong of the string.
		/// </param>
		public TextNode(Page page, int start, int end):base(page, start, end)
		{
			mText = null;
		}

		/// <summary> Returns if the node consists of only white space.
		/// White space can be spaces, new lines, etc.
		/// </summary>
		virtual public bool WhiteSpace
		{
			get
			{
				if (mText == null || mText.Trim().Equals(""))
					return true;
				return false;
			}
			
		}

		/// <summary> Returns the text of the node.
		/// This is the same as {@link #toHtml} for this type of node.
		/// </summary>
		/// <returns> The contents of this text node.
		/// </returns>
		public override System.String GetText()
		{
			return (ToHtml());
		}
		
		/// <summary> Sets the string contents of the node.</summary>
		/// <param name="text">The new text for the node.
		/// </param>
		public override void  SetText(System.String text)
		{
			mText = text;
			nodeBegin = 0;
			nodeEnd = mText.Length;
		}
		
		/// <summary> Returns the text of the node.
		/// This is the same as {@link #toHtml} for this type of node.
		/// </summary>
		/// <returns> The contents of this text node.
		/// </returns>
		public override System.String ToPlainTextString()
		{
			return (ToHtml());
		}
		
		/// <summary> Returns the text of the node.</summary>
		/// <returns> The contents of this text node.
		/// </returns>
		public override System.String ToHtml()
		{
			System.String ret;
			
			ret = mText;
			if (null == ret)
				ret = mPage.GetText(StartPosition, EndPosition);
			
			return (ret);
		}
		
		/// <summary> Express this string node as a printable string
		/// This is suitable for display in a debugger or output to a printout.
		/// Control characters are replaced by their equivalent escape
		/// sequence and contents is truncated to 80 characters.
		/// </summary>
		/// <returns> A string representation of the string node.
		/// </returns>
		public override System.String ToString()
		{
			int startpos;
			int endpos;
			Cursor start;
			Cursor end;
			char c;
			System.Text.StringBuilder ret;
			
			startpos = StartPosition;
			endpos = EndPosition;
			ret = new System.Text.StringBuilder(endpos - startpos + 20);
			if (null == mText)
			{
				start = new Cursor(Page, startpos);
				end = new Cursor(Page, endpos);
				ret.Append("Txt (");
				ret.Append(start);
				ret.Append(",");
				ret.Append(end);
				ret.Append("): ");
				while (start.Position < endpos)
				{
					try
					{
						c = mPage.GetCharacter(start);
						switch (c)
						{
							
							case '\t': 
								ret.Append("\\t");
								break;
							
							case '\n': 
								ret.Append("\\n");
								break;
							
							case '\r': 
								ret.Append("\\r");
								break;
							
							default: 
								ret.Append(c);
								break;
							
						}
					}
					catch (ParserException pe)
					{
						// not really expected, but we're only doing toString, so ignore
					}
					if (77 <= ret.Length)
					{
						ret.Append("...");
						break;
					}
				}
			}
			else
			{
				ret.Append("Txt (");
				ret.Append(startpos);
				ret.Append(",");
				ret.Append(endpos);
				ret.Append("): ");
				for (int i = 0; i < mText.Length; i++)
				{
					c = mText[i];
					switch (c)
					{
						
						case '\t': 
							ret.Append("\\t");
							break;
						
						case '\n': 
							ret.Append("\\n");
							break;
						
						case '\r': 
							ret.Append("\\r");
							break;
						
						default: 
							ret.Append(c);
							break;
						
					}
					if (77 <= ret.Length)
					{
						ret.Append("...");
						break;
					}
				}
			}
			
			return (ret.ToString());
		}
		
		/// <summary> String visiting code.</summary>
		/// <param name="visitor">The <code>NodeVisitor</code> object to invoke 
		/// <code>visitStringNode()</code> on.
		/// </param>
		public override void Accept(NodeVisitor visitor)
		{
			visitor.VisitStringNode(this);
		}

		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		override public System.Object Clone()
		{
			throw new NotImplementedException("Check why we are here");
		}
	}
}
