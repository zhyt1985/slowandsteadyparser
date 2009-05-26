// ***************************************************************
//  HasChildFilter   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Tags;

namespace Winista.Text.HtmlParser.Filters
{
	/// <summary> This class accepts all tags that have a child acceptable to the filter.
	/// It can be set to operate recursively, that is perform a scan down
	/// through the node hierarchy in a breadth first traversal looking for any
	/// descendant that matches the predicate filter (which stops the search).
	/// </summary>
	[Serializable]
	public class HasChildFilter : INodeFilter
	{
		/// <summary> The filter to apply to children.</summary>
		protected internal INodeFilter mChildFilter;
		
		/// <summary> Performs a recursive search down the node hierarchy if <code>true</code>.</summary>
		protected internal bool mRecursive;

		/// <summary> Creates a new instance of a HasChildFilter.
		/// With no child filter, this would always return <code>false</code>
		/// from {@link #accept}.
		/// </summary>
		public HasChildFilter():this(null)
		{
		}
		
		/// <summary> Creates a new instance of HasChildFilter that accepts nodes
		/// with a direct child acceptable to the filter.
		/// </summary>
		/// <param name="filter">The filter to apply to the children.
		/// </param>
		public HasChildFilter(INodeFilter filter):this(filter, false)
		{
		}
		
		/// <summary> Creates a new instance of HasChildFilter that accepts nodes
		/// with a child acceptable to the filter.
		/// Of necessity, this applies only to composite tags, i.e. those that can
		/// contain other nodes, for example &lt;HTML&gt;&lt;/HTML&gt;.
		/// </summary>
		/// <param name="filter">The filter to apply to children.
		/// </param>
		/// <param name="recursive">If <code>true</code>, any enclosed node acceptable
		/// to the given filter causes the node being tested to be accepted
		/// (i.e. a recursive scan through the child nodes down the node
		/// heirarchy is performed).
		/// </param>
		public HasChildFilter(INodeFilter filter, bool recursive)
		{
			ChildFilter = filter;
			Recursive = recursive;
		}

		/// <summary> Get the filter used by this HasParentFilter.</summary>
		/// <returns> The filter to apply to parents.
		/// </returns>
		/// <summary> Set the filter for this HasParentFilter.</summary>
		/// <param name="filter">The filter to apply to parents in {@link #accept}.
		/// </param>
		virtual public INodeFilter ChildFilter
		{
			get
			{
				return (mChildFilter);
			}
			
			set
			{
				mChildFilter = value;
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the recusion setting for the filter.</summary>
		/// <returns> Returns <code>true</code> if the filter is recursive
		/// up the node heirarchy.
		/// </returns>
		/// <summary> Sets whether the filter is recursive or not.</summary>
		/// <param name="recursive">The recursion setting for the filter.
		/// </param>
		virtual public bool Recursive
		{
			get
			{
				return (mRecursive);
			}
			
			set
			{
				mRecursive = value;
			}
			
		}

		/// <summary> Accept tags with children acceptable to the filter.</summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if the node has an acceptable child,
		/// <code>false</code> otherwise.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			CompositeTag tag;
			NodeList children;
			bool ret;
			
			ret = false;
			if (node is CompositeTag)
			{
				tag = (CompositeTag) node;
				children = tag.Children;
				if (null != children)
				{
					for (int i = 0; !ret && i < children.Size(); i++)
						if (ChildFilter.Accept(children.ElementAt(i)))
							ret = true;
					// do recursion after all children are checked
					// to get breadth first traversal
					if (!ret && Recursive)
						for (int i = 0; !ret && i < children.Size(); i++)
							if (Accept(children.ElementAt(i)))
								ret = true;
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
