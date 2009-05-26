// ***************************************************************
//  IRemark   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser
{
	/// <summary> This interface represents a comment in the HTML document.</summary>
	public interface IRemark : INode
	{
		/// <summary> Returns the text contents of the comment tag.</summary>
		/// <returns> The contents of the text inside the comment delimiters.
		/// </returns>
		new System.String GetText();
		
		/// <summary> Sets the string contents of the node.
		/// If the text has the remark delimiters (&lt;!-- --&gt;),
		/// these are stripped off.
		/// </summary>
		/// <param name="text">The new text for the node.
		/// </param>
		new void  SetText(System.String text);
	}
}
