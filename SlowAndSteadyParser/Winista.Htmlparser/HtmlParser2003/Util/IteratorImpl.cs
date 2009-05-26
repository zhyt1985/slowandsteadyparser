// ***************************************************************
//  IteratorImpl   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Scanners;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary>
	/// Summary description for IteratorImpl.
	/// </summary>
	public class IteratorImpl : INodeIterator
	{
		internal Lexer mLexer;
		internal IParserFeedBack mFeedback;
		internal Cursor mCursor;

		public IteratorImpl(Lexer lexer, IParserFeedBack fb)
		{
			mLexer = lexer;
			mFeedback = fb;
			mCursor = new Cursor(mLexer.Page, 0);
		}

		/// <summary> Check if more nodes are available.</summary>
		/// <returns> <code>true</code> if a call to <code>nextNode()</code> will succeed.
		/// </returns>
		public virtual bool HasMoreNodes()
		{
			bool ret;
			
			mCursor.Position = mLexer.Position;
			ret = Page.EOF != mLexer.Page.GetCharacter(mCursor); // more characters?
			
			return (ret);
		}
		
		/// <summary> Get the next node.</summary>
		/// <returns> The next node in the HTML stream, or null if there are no more nodes.
		/// </returns>
		/// <exception cref="ParserException">If an unrecoverable error occurs.
		/// </exception>
		public virtual INode NextNode()
		{
			ITag tag;
			IScanner scanner;
			NodeList stack;
			INode ret;
			
			try
			{
				ret = mLexer.NextNode();
				if (null != ret)
				{
					// kick off recursion for the top level node
					if (ret is ITag)
					{
						tag = (ITag) ret;
						if (!tag.IsEndTag())
						{
							// now recurse if there is a scanner for this type of tag
							scanner = tag.ThisScanner;
							if (null != scanner)
							{
								stack = new NodeList();
								ret = scanner.Scan(tag, mLexer, stack);
							}
						}
					}
				}
			}
			catch (ParserException pe)
			{
				throw pe; // no need to wrap an existing ParserException
			}
			catch (System.Exception e)
			{
				System.Text.StringBuilder msgBuffer = new System.Text.StringBuilder();
				msgBuffer.Append("Unexpected Exception occurred while reading ");
				msgBuffer.Append(mLexer.Page.Url);
				msgBuffer.Append(", in nextNode");
				// TODO: appendLineDetails (msgBuffer);
				ParserException ex = new ParserException(msgBuffer.ToString(), e);
				mFeedback.Error(msgBuffer.ToString(), ex);
				throw ex;
			}
			
			return (ret);
		}
	}
}
