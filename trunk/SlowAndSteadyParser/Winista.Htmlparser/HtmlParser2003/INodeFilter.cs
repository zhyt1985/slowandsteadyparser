// ***************************************************************
//  INodeFilter   version:  1.0   Date: 12/17/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 - Winista, All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser
{
	public interface INodeFilter
	{
		/// <summary> Predicate to determine whether or not to keep the given node.
		/// The behaviour based on this outcome is determined by the context
		/// in which it is called. It may lead to the node being added to a list
		/// or printed out. See the calling routine for details.
		/// </summary>
		/// <returns> <code>true</code> if the node is to be kept, <code>false</code>
		/// if it is to be discarded.
		/// </returns>
		/// <param name="node">The node to test.
		/// </param>
		bool Accept(INode node);
	}
}
