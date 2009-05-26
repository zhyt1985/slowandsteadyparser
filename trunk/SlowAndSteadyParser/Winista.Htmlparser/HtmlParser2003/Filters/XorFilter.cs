// ***************************************************************
//  XorFilter   version:  1.0 Date: 08/21/2006
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright (C) 2006 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************

using System;

namespace Winista.Text.HtmlParser.Filters
{
	
	/// <summary> Accepts nodes matching an odd number of its predicates filters (XOR operation).
	/// For example, where it has two filters, it accepts only if and only if one of the two filters accepts the Node, but does not accept if both filters accept the Node. 
	/// </summary>
	[Serializable]
	public class XorFilter : INodeFilter
	{
		/// <summary> Get the predicates used by this XorFilter.</summary>
		/// <returns> The predicates currently in use.
		/// </returns>
		/// <summary> Set the predicates for this XorFilter.</summary>
		/// <param name="predicates">The list of predidcates to use in {@link #accept}.
		/// </param>
		virtual public INodeFilter[] Predicates
		{
			get
			{
				return (mPredicates);
			}
			
			set
			{
				if (null == value)
					value = new INodeFilter[0];
				mPredicates = value;
			}
			
		}
		/// <summary> The predicates that are to be xor'ed together;</summary>
		protected internal INodeFilter[] mPredicates;
		
		/// <summary> Creates a new instance of an XorFilter.
		/// With no predicates, this would always answer <code>false</code>
		/// to {@link #accept}.
		/// </summary>
		/// <seealso cref="setPredicates">
		/// </seealso>
		public XorFilter()
		{
			Predicates = null;
		}
		
		/// <summary> Creates an XorFilter that accepts nodes acceptable to either filter, but not both.</summary>
		/// <param name="left">One filter.
		/// </param>
		/// <param name="right">The other filter.
		/// </param>
		public XorFilter(INodeFilter left, INodeFilter right)
		{
			INodeFilter[] predicates;
			
			predicates = new INodeFilter[2];
			predicates[0] = left;
			predicates[1] = right;
			Predicates = predicates;
		}
		
		/// <summary> Creates an XorFilter that accepts nodes acceptable an odd number of the given filters.</summary>
		/// <param name="predicates">The list of filters. 
		/// </param>
		public XorFilter(INodeFilter[] predicates)
		{
			Predicates = predicates;
		}
		
		//
		// NodeFilter interface
		//
		
		/// <summary> Accept nodes that are acceptable to an odd number of its predicate filters.</summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if an odd number of the predicate filters find the node
		/// is acceptable, <code>false</code> otherwise.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			int countTrue;
			
			countTrue = 0;
			
			for (int i = 0; i < mPredicates.Length; i++)
				if (mPredicates[i].Accept(node))
					++countTrue;
			
			return ((countTrue % 2) == 1);
		}

		virtual public System.Object Clone()
		{
			return null;
		}
	}
}