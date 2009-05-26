// ***************************************************************
//  HasParentFilter   version:  1.0   date: 12/18/2005
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
	/// <summary> This class accepts all tags that have a parent acceptable to another filter.
	/// It can be set to operate recursively, that is perform a scan up the node
	/// heirarchy looking for any ancestor that matches the predicate filter.
	/// End tags are not considered to be children of any tag.
	/// </summary>
	[Serializable]
	public class HasParentFilter : INodeFilter
	{
		/// <summary> The filter to apply to the parent.</summary>
		protected internal INodeFilter mParentFilter;
		
		/// <summary> Performs a recursive search up the node heirarchy if <code>true</code>.</summary>
		protected internal bool mRecursive;

		/// <summary> Creates a new instance of HasParentFilter.
		/// With no parent filter, this would always return <code>false</code>
		/// from {@link #accept}.
		/// </summary>
		public HasParentFilter():this(null)
		{
		}
		
		/// <summary> Creates a new instance of HasParentFilter that accepts nodes with
		/// the direct parent acceptable to the filter.
		/// </summary>
		/// <param name="filter">The filter to apply to the parent.
		/// </param>
		public HasParentFilter(INodeFilter filter):this(filter, false)
		{
		}
		
		/// <summary> Creates a new instance of HasParentFilter that accepts nodes with
		/// a parent acceptable to the filter.
		/// </summary>
		/// <param name="filter">The filter to apply to the parent.
		/// </param>
		/// <param name="recursive">If <code>true</code>, any enclosing node acceptable
		/// to the given filter causes the node being tested to be accepted
		/// (i.e. a recursive scan through the parent nodes up the node
		/// heirarchy is performed).
		/// </param>
		public HasParentFilter(INodeFilter filter, bool recursive)
		{
			ParentFilter = filter;
			Recursive = recursive;
		}

		/// <summary> Get the filter used by this HasParentFilter.</summary>
		/// <returns> The filter to apply to parents.
		/// </returns>
		/// <summary> Set the filter for this HasParentFilter.</summary>
		/// <param name="filter">The filter to apply to parents in {@link #accept}.
		/// </param>
		virtual public INodeFilter ParentFilter
		{
			get
			{
				return (mParentFilter);
			}
			
			set
			{
				mParentFilter = value;
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

		/// <summary> Accept tags with parent acceptable to the filter.
		/// If recursion is enabled, each parent in turn up to
		/// the topmost enclosing node is checked.
		/// Recursion only proceeds while no parent satisfies the
		/// filter.
		/// </summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if the node has an acceptable parent,
		/// <code>false</code> otherwise.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			INode parent;
			bool ret;
			
			ret = false;
			if (!(node is ITag) || !((ITag) node).IsEndTag())
			{
				parent = node.Parent;
				if ((null != parent) && (null != ParentFilter))
				{
					ret = ParentFilter.Accept(parent);
					if (!ret && Recursive)
						ret = Accept(parent);
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
