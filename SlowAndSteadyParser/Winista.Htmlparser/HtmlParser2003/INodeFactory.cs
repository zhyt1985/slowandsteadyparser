// ***************************************************************
//  NodeFactory   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Lex;

namespace Winista.Text.HtmlParser
{
	/// <summary> This interface defines the methods needed to create new nodes.
	/// <p>The factory is used when lexing to generate the nodes passed
	/// back to the caller. By implementing this interface, and setting
	/// that concrete object as the node factory for the
	/// LexerSetNodeFactory (perhaps via the
	/// Parser.SetNodeFactory), the way that nodes are generated
	/// can be customized.</p>
	/// <p>In general, replacing the factory with a custom factory is not required
	/// because of the flexibility of the <see cref="PrototypicalNodeFactory"></see>.</p>
	/// <p>Creation of Text and Remark nodes is straight forward, because essentially
	/// they are just sequences of characters extracted from the page. Creation of a
	/// Tag node requires that the attributes from the tag be remembered as well.
	/// </summary>
	/// <seealso cref="PrototypicalNodeFactory">
	/// </seealso>
	public interface INodeFactory
	{
		/// <summary> Create a new text node.</summary>
		/// <param name="page">The page the node is on.
		/// </param>
		/// <param name="start">The beginning position of the string.
		/// </param>
		/// <param name="end">The ending positiong of the string.
		/// </param>
		/// <throws>  ParserException If there is a problem encountered </throws>
		/// <summary> when creating the node.
		/// </summary>
		/// <returns> A text node comprising the indicated characters from the page.
		/// </returns>
		IText CreateStringNode(Page page, int start, int end);
		
		/// <summary> Create a new remark node.</summary>
		/// <param name="page">The page the node is on.
		/// </param>
		/// <param name="start">The beginning position of the remark.
		/// </param>
		/// <param name="end">The ending positiong of the remark.
		/// </param>
		/// <throws>  ParserException If there is a problem encountered </throws>
		/// <summary> when creating the node.
		/// </summary>
		/// <returns> A remark node comprising the indicated characters from the page.
		/// </returns>
		IRemark CreateRemarkNode(Page page, int start, int end);
		
		/// <summary> Create a new tag node.
		/// Note that the attributes vector contains at least one element,
		/// which is the tag name (standalone attribute) at position zero.
		/// This can be used to decide which type of node to create, or
		/// gate other processing that may be appropriate.
		/// </summary>
		/// <param name="page">The page the node is on.
		/// </param>
		/// <param name="start">The beginning position of the tag.
		/// </param>
		/// <param name="end">The ending positiong of the tag.
		/// </param>
		/// <param name="attributes">The attributes contained in this tag.
		/// </param>
		/// <throws>  ParserException If there is a problem encountered </throws>
		/// <summary> when creating the node.
		/// </summary>
		/// <returns> A tag node comprising the indicated characters from the page.
		/// </returns>
		ITag CreateTagNode(Page page, int start, int end, System.Collections.ArrayList attributes);
	}
}
