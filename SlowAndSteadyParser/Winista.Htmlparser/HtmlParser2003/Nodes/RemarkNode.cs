// ***************************************************************
//  RemarkNode   version:  1.0   date: 12/18/2005
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
	/// <summary> The remark tag is identified and represented by this class.</summary>
	[Serializable]
	public class RemarkNode:AbstractNode, IRemark
	{
		/// <summary> The contents of the remark node, or override text.</summary>
		protected internal System.String mText;

		/// <summary> Constructor takes in the text string.</summary>
		/// <param name="text">The string node text. For correct generation of HTML, this
		/// should not contain representations of tags (unless they are balanced).
		/// </param>
		public RemarkNode(System.String text):base(null, 0, 0)
		{
			SetText(text);
		}
		
		/// <summary> Constructor takes in the page and beginning and ending posns.</summary>
		/// <param name="page">The page this remark is on.
		/// </param>
		/// <param name="start">The beginning position of the remark.
		/// </param>
		/// <param name="end">The ending positiong of the remark.
		/// </param>
		public RemarkNode(Page page, int start, int end):base(page, start, end)
		{
			mText = null;
		}

		/// <summary> Returns the text contents of the comment tag.</summary>
		/// <returns> The contents of the text inside the comment delimiters.
		/// </returns>
		public override System.String GetText()
		{
			int start;
			int end;
			System.String ret;
			
			if (null == mText)
			{
				start = StartPosition + 4; // <!--
				end = EndPosition - 3; // -->
				if (start >= end)
					ret = "";
				else
					ret = mPage.GetText(start, end);
			}
			else
				ret = mText;
			
			return (ret);
		}
		
		/// <summary> Sets the string contents of the node.
		/// If the text has the remark delimiters (&lt;!-- --&gt;), these are stripped off.
		/// </summary>
		/// <param name="text">The new text for the node.
		/// </param>
		public override void SetText(System.String text)
		{
			mText = text;
			if (text.StartsWith("<!--") && text.EndsWith("-->"))
				mText = text.Substring(4, (text.Length - 3) - (4));
			nodeBegin = 0;
			nodeEnd = mText.Length;
		}
		
		/// <summary> Return the remark text.</summary>
		/// <returns> The HTML comment.
		/// </returns>
		public override System.String ToPlainTextString()
		{
			return (GetText());
		}
		
		/// <summary> Return The full HTML remark.</summary>
		/// <returns> The comment, i.e. {@.html <!-- this is a comment -->}.
		/// </returns>
		public override System.String ToHtml()
		{
			System.Text.StringBuilder buffer;
			System.String ret;
			
			if (null == mText)
				ret = mPage.GetText(StartPosition, EndPosition);
			else
			{
				buffer = new System.Text.StringBuilder(mText.Length + 7);
				buffer.Append("<!--");
				buffer.Append(mText);
				buffer.Append("-->");
				ret = buffer.ToString();
			}
			
			return (ret);
		}
		
		/// <summary> Print the contents of the remark tag.
		/// This is suitable for display in a debugger or output to a printout.
		/// Control characters are replaced by their equivalent escape
		/// sequence and contents is truncated to 80 characters.
		/// </summary>
		/// <returns> A string representation of the remark node.
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
				ret.Append("Rem (");
				ret.Append(start);
				ret.Append(",");
				ret.Append(end);
				ret.Append("): ");
				start.Position = startpos + 4; // <!--
				endpos -= 3; // -->
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
				ret.Append("Rem (");
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
		
		/// <summary> Remark visiting code.</summary>
		/// <param name="visitor">The <code>NodeVisitor</code> object to invoke 
		/// <code>visitRemarkNode()</code> on.
		/// </param>
		public override void Accept(NodeVisitor visitor)
		{
			visitor.VisitRemarkNode(this);
		}

		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		override public System.Object Clone()
		{
			throw new NotImplementedException("Check why we are here!");
		}
	}
}
