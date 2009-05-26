// ***************************************************************
//  IText   version:  1.0   date: 12/18/2005
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
	/// <summary> This interface represents a piece of the content of the HTML document.</summary>
	public interface IText : INode
	{
		/// <summary> Accesses the textual contents of the node.</summary>
		/// <returns> The text of the node.
		/// </returns>
		new System.String GetText();
		
		/// <summary> Sets the contents of the node.</summary>
		/// <param name="text">The new text for the node.
		/// </param>
		new void  SetText(System.String text);
	}
}
