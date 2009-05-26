// ***************************************************************
//  NotFilter   version:  1.0   date: 12/18/2005
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
	/// <summary> Accepts all nodes not acceptable to it's predicate filter.</summary>
	[Serializable]
	public class NotFilter : INodeFilter
	{
		/// <summary> The filter to gainsay.</summary>
		protected internal INodeFilter mPredicate;

		/// <summary> Creates a new instance of a NotFilter.
		/// With no predicates, this would always return <code>false</code>
		/// from {@link #accept}.
		/// </summary>
		/// <seealso cref="SetPredicate">
		/// </seealso>
		public NotFilter()
		{
			Predicate = null;
		}
		
		/// <summary> Creates a NotFilter that accepts nodes not acceptable to the predicate.</summary>
		/// <param name="predicate">The filter to consult.
		/// </param>
		public NotFilter(INodeFilter predicate)
		{
			Predicate = predicate;
		}

		/// <summary> Get the predicate used by this NotFilter.</summary>
		/// <returns> The predicate currently in use.
		/// </returns>
		/// <summary> Set the predicate for this NotFilter.</summary>
		/// <param name="predicate">The predidcate to use in {@link #accept}.
		/// </param>
		virtual public INodeFilter Predicate
		{
			get
			{
				return (mPredicate);
			}
			
			set
			{
				mPredicate = value;
			}
		}

		/// <summary> Accept nodes that are not acceptable to the predicate filter.</summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if the node is not acceptable to the
		/// predicate filter, <code>false</code> otherwise.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			return ((null != mPredicate) && !mPredicate.Accept(node));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		virtual public System.Object Clone()
		{
			return null;
		}
	}
}
