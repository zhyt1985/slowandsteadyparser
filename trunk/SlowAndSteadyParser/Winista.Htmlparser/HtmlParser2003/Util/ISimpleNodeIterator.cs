// ***************************************************************
//  ISimpleNodeIterator   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary> The HTMLSimpleEnumeration interface is similar to NodeIterator,
	/// except that it does not throw exceptions. This interface is useful
	/// when using HTMLVector, to enumerate through its elements in a simple
	/// manner, without needing to do class casts for Node.
	/// </summary>
	/// <author>  Somik Raha
	/// </author>
	public interface ISimpleNodeIterator : INodeIterator
	{
		/// <summary> Check if more nodes are available.</summary>
		/// <returns> <code>true</code> if a call to <code>nextHTMLNode()</code> will
		/// succeed.
		/// </returns>
		new bool HasMoreNodes();
		
		/// <summary> Get the next node.</summary>
		/// <returns> The next node in the HTML stream, or null if there are no more
		/// nodes.
		/// </returns>
		new INode NextNode();
	}
}
