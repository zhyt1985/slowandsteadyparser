// ***************************************************************
//  LinkStringFilter   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Tags;

namespace Winista.Text.HtmlParser.Filters
{
	/// <summary> This class accepts tags of class LinkTag that contain a link matching a given
	/// pattern string. Use this filter to extract LinkTag nodes with URLs containing
	/// the desired string.
	/// </summary>
	[Serializable]
	public class LinkStringFilter : INodeFilter
	{
		/// <summary> The pattern to search for in the link.</summary>
		protected internal System.String mPattern;
		
		/// <summary> Flag indicating case sensitive/insensitive search.</summary>
		protected internal bool mCaseSensitive;

		/// <summary> Creates a LinkStringFilter that accepts LinkTag nodes containing
		/// a URL that matches the supplied pattern.
		/// The match is case insensitive.
		/// </summary>
		/// <param name="pattern">The pattern to match.
		/// </param>
		public LinkStringFilter(System.String pattern):this(pattern, false)
		{
		}
		
		/// <summary> Creates a LinkStringFilter that accepts LinkTag nodes containing
		/// a URL that matches the supplied pattern.
		/// </summary>
		/// <param name="pattern">The pattern to match.
		/// </param>
		/// <param name="caseSensitive">Specifies case sensitivity for the matching process.
		/// </param>
		public LinkStringFilter(System.String pattern, bool caseSensitive)
		{
			mPattern = pattern;
			mCaseSensitive = caseSensitive;
		}
		
		/// <summary> Accept nodes that are a LinkTag and
		/// have a URL that matches the pattern supplied in the constructor.
		/// </summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if the node is a link with the pattern.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			bool ret;
			
			ret = false;
			if (typeof(LinkTag).IsAssignableFrom(node.GetType()))
			{
				System.String link = ((LinkTag) node).Link;
				if (mCaseSensitive)
				{
					if (link.IndexOf(mPattern) > - 1)
						ret = true;
				}
				else
				{
					if (link.ToUpper().IndexOf(mPattern.ToUpper()) > - 1)
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
