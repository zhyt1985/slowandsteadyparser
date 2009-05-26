// ***************************************************************
//  HasSiblingFilter   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Filters
{
	/// <summary> This class accepts all tags that have a sibling acceptable to another filter.
	/// End tags are not considered to be siblings of any tag.
	/// </summary>
	[Serializable]
	public class HasSiblingFilter : INodeFilter
	{
		/// <summary> The filter to apply to the sibling.</summary>
		protected internal INodeFilter mSiblingFilter;

		/// <summary> Creates a new instance of HasSiblingFilter.
		/// With no sibling filter, this would always return <code>false</code>
		/// from {@link #accept}.
		/// </summary>
		public HasSiblingFilter():this(null)
		{
		}
		
		/// <summary> Creates a new instance of HasSiblingFilter that accepts nodes
		/// with sibling acceptable to the filter.
		/// </summary>
		/// <param name="filter">The filter to apply to the sibling.
		/// </param>
		public HasSiblingFilter(INodeFilter filter)
		{
			SiblingFilter = filter;
		}

		/// <summary> Get the filter used by this HasSiblingFilter.</summary>
		/// <returns> The filter to apply to siblings.
		/// </returns>
		/// <summary> Set the filter for this HasSiblingFilter.</summary>
		/// <param name="filter">The filter to apply to siblings in {@link #accept}.
		/// </param>
		virtual public INodeFilter SiblingFilter
		{
			get
			{
				return (mSiblingFilter);
			}
			
			set
			{
				mSiblingFilter = value;
			}
		}

		/// <summary> Accept tags with a sibling acceptable to the filter.</summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if the node has an acceptable sibling,
		/// <code>false</code> otherwise.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			INode parent;
			NodeList siblings;
			int count;
			bool ret;
			
			ret = false;
			if (!(node is ITag) || !((ITag) node).IsEndTag())
			{
				parent = node.Parent;
				if (null != parent)
				{
					siblings = parent.Children;
					if (null != siblings)
					{
						count = siblings.Size();
						for (int i = 0; !ret && (i < count); i++)
							if (SiblingFilter.Accept(siblings.ElementAt(i)))
								ret = true;
					}
				}
			}
			
			return (ret);
		}
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		virtual public System.Object Clone()
		{
			return null;
		}
	}
}
