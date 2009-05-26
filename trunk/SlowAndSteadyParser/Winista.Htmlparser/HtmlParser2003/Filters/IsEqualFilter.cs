// ***************************************************************
//  IsEqualFilter   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Filters
{
	/// <summary> This class accepts only one specific node.</summary>
	[Serializable]
	public class IsEqualFilter : INodeFilter
	{
		/// <summary> The node to match.</summary>
		protected internal INode mNode;

		/// <summary> Creates a new IsEqualFilter that accepts only the node provided.</summary>
		/// <param name="node">The node to match.
		/// </param>
		public IsEqualFilter(INode node)
		{
			mNode = node;
		}

		/// <summary> Accept the node.</summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>false</code> unless <code>node</code> is the one and only.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			return (mNode == node);
		}
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		virtual public System.Object Clone()
		{
			return null;
		}
	}
}
