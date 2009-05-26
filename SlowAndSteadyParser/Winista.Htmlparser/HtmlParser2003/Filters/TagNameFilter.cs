// ***************************************************************
//  TagNameFilter   version:  1.0   date: 12/18/2005
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
	/// <summary> This class accepts all tags matching the tag name.</summary>
	[Serializable]
	public class TagNameFilter : INodeFilter
	{
		/// <summary> The tag name to match.</summary>
		protected internal System.String mName;

		/// <summary> Creates a new instance of TagNameFilter.
		/// With no name, this would always return <code>false</code>
		/// from {@link #accept}.
		/// </summary>
		public TagNameFilter():this("")
		{
		}
		
		/// <summary> Creates a TagNameFilter that accepts tags with the given name.</summary>
		/// <param name="name">The tag name to match.
		/// </param>
		public TagNameFilter(System.String name)
		{
			mName = name.ToUpper(new System.Globalization.CultureInfo("en"));
		}

		/// <summary> Accept nodes that are tags and have a matching tag name.
		/// This discards non-tag nodes and end tags.
		/// The end tags are available on the enclosing non-end tag.
		/// </summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if the tag name matches,
		/// <code>false</code> otherwise.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			return ((node is ITag) && !((ITag) node).IsEndTag() && ((ITag) node).TagName.Equals(mName));
		}
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		virtual public System.Object Clone()
		{
			return null;
		}
	}
}
