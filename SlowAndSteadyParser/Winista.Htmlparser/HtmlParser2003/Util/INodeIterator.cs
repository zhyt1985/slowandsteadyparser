// ***************************************************************
//  INodeIterator   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary>
	/// Summary description for INodeIterator.
	/// </summary>
	public interface INodeIterator
	{
		/// <summary> Check if more nodes are available.</summary>
		/// <returns> <code>true</code> if a call to <code>nextHTMLNode()</code> will succeed.
		/// </returns>
		bool HasMoreNodes();
		
		/// <summary> Get the next node.</summary>
		/// <returns> The next node in the HTML stream, or null if there are no more nodes.
		/// </returns>
		INode NextNode();
	}
}
