// ***************************************************************
//  TagScanner   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Scanners
{
	/// <summary>
	/// Summary description for TagScanner.
	/// </summary>
	public class TagScanner : IScanner
	{
		/// <summary> Create a (non-composite) tag scanner.</summary>
		public TagScanner()
		{
		}

		#region IScanner Members

		/// <summary> Scan the tag.
		/// For this implementation, the only operation is to perform the tag's
		/// semantic action.
		/// </summary>
		/// <param name="tag">The tag to scan.
		/// </param>
		/// <param name="lexer">Provides html page access.
		/// </param>
		/// <param name="stack">The parse stack. May contain pending tags that enclose
		/// this tag.
		/// </param>
		/// <returns> The resultant tag (may be unchanged).
		/// </returns>
		public virtual ITag Scan(ITag tag, Winista.Text.HtmlParser.Lex.Lexer lexer, NodeList stack)
		{
			tag.DoSemanticAction();
			
			return (tag);
		}

		#endregion
	}
}
