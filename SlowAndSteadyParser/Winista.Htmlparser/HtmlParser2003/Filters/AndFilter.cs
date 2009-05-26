// ***************************************************************
//  AndFilter   version:  1.0   date: 12/18/2005
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
	/// <summary> Accepts nodes matching all of it's predicate filters (AND operation).</summary>
	[Serializable]
	public class AndFilter : INodeFilter
	{
		/// <summary> The predicates that are to be and'ed together;</summary>
		protected internal INodeFilter[] mPredicates;

		/// <summary> Creates a new instance of an AndFilter.
		/// With no predicates, this would always answer <code>true</code>
		/// to {@link #accept}.
		/// </summary>
		/// <seealso cref="setPredicates">
		/// </seealso>
		public AndFilter()
		{
			Predicates = null;
		}

		/// <summary> Creates an AndFilter that accepts nodes acceptable to both filters.</summary>
		/// <param name="left">One filter.
		/// </param>
		/// <param name="right">The other filter.
		/// </param>
		public AndFilter(INodeFilter left, INodeFilter right)
		{
			INodeFilter[] predicates;
			
			predicates = new INodeFilter[2];
			predicates[0] = left;
			predicates[1] = right;
			Predicates = predicates;
		}

		/// <summary> Get the predicates used by this AndFilter.</summary>
		/// <returns> The predicates currently in use.
		/// </returns>
		/// <summary> Set the predicates for this AndFilter.</summary>
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

		/// <summary> Accept nodes that are acceptable to all of it's predicate filters.</summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if all the predicate filters find the node
		/// is acceptable, <code>false</code> otherwise.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			bool ret;
			
			ret = true;
			
			for (int i = 0; ret && (i < mPredicates.Length); i++)
				if (!mPredicates[i].Accept(node))
					ret = false;
			
			return (ret);
		}
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		virtual public System.Object Clone()
		{
			return null;
		}

        private static AndFilter ms_TrueFilter = new AndFilter();

        public static AndFilter TrueFilter
        {
            get { return ms_TrueFilter; }
        }
	}
}
