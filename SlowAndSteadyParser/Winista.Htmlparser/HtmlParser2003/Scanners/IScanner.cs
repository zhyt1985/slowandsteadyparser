// ***************************************************************
//  IScanner   version:  1.0   date: 12/18/2005
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
	/// <summary> Generic interface for scanning.
	/// Tags needing specialized operations can provide an object that implements
	/// this interface via getThisScanner().
	/// By default non-composite tags simply perform the semantic action and
	/// return while composite tags will gather their children.
	/// </summary>
	public interface IScanner
	{
		/// <summary> Scan the tag.
		/// The Lexer is provided in order to do a lookahead operation.
		/// </summary>
		/// <param name="tag">HTML tag to be scanned for identification.
		/// </param>
		/// <param name="lexer">Provides html page access.
		/// </param>
		/// <param name="stack">The parse stack. May contain pending tags that enclose
		/// this tag. Nodes on the stack should be considered incomplete.
		/// </param>
		/// <returns> The resultant tag (may be unchanged).
		/// </returns>
		/// <exception cref="ParserException">if an unrecoverable problem occurs.
		/// </exception>
		ITag Scan(ITag tag, Lexer lexer, NodeList stack);
	}
}
