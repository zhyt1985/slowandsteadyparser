// ***************************************************************
//  LinkTag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Support;
using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> Identifies a link tag.</summary>
	[Serializable]
	public class LinkTag:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"LINK"};
		
		/// <summary> The set of tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEnders = new System.String[]{"HEAD"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"HTML", "HEAD"};
		
		/// <summary> The URL where the link points to</summary>
		protected internal System.String mLink;
		
		/// <summary>
		/// Forward link types 
		/// </summary>
		private String m_strRel = String.Empty;
		/// <summary>
		/// Reverse link types 
		/// </summary>
		private String m_strRev = String.Empty;

		/// <summary> Constructor creates an LinkTag object, which basically stores the location
		/// where the link points to, and the text it contains.
		/// <p>
		/// In order to get the contents of the link tag, use the method LinkData(),
		/// which returns an enumeration of nodes encapsulated within the link.
		/// <p>
		/// </summary>
		public LinkTag()
		{
		}

		/// <summary> Return the set of names handled by this tag.</summary>
		/// <returns> The names to be matched that create tags of this type.
		/// </returns>
		override public System.String[] Ids
		{
			get
			{
				return (mIds);
			}
			
		}
		/// <summary> Return the set of tag names that cause this tag to finish.</summary>
		/// <returns> The names of following tags that stop further scanning.
		/// </returns>
		override public System.String[] Enders
		{
			get
			{
				return (mEnders);
			}
			
		}
		/// <summary> Return the set of end tag names that cause this tag to finish.</summary>
		/// <returns> The names of following end tags that stop further scanning.
		/// </returns>
		override public System.String[] EndTagEnders
		{
			get
			{
				return (mEndTagEnders);
			}
			
		}
		/// <summary> Get the <code>ACCESSKEY</code> attribute, if any.</summary>
		/// <returns> The value of the <code>ACCESSKEY</code> attribute,
		/// or <code>null</code> if the attribute doesn't exist.
		/// </returns>
		virtual public System.String AccessKey
		{
			get
			{
				return (GetAttribute("ACCESSKEY"));
			}
			
		}

		virtual public System.String Link
		{
			get
			{
				return (mLink);
			}
			
			set
			{
				mLink = value;
			}
		}
		
		/// <summary> Return the contents of this link node as a string suitable for debugging.</summary>
		/// <returns> A string representation of this node.
		/// </returns>
		public override System.String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("Link to : " + Link + "begins at : " + StartPosition + "; ends at : " + EndPosition + ", AccessKey=");
			if (AccessKey == null)
				sb.Append("null\n");
			else
				sb.Append(AccessKey + "\n");
			if (null != Children)
			{
				sb.Append("  " + "LinkData\n");
				sb.Append("  " + "--------\n");
				
				INode node;
				int i = 0;
				for (ISimpleNodeIterator e = GetChildren(); e.HasMoreNodes(); )
				{
					node = e.NextNode();
					sb.Append("   " + (i++) + " ");
					sb.Append(node.ToString() + "\n");
				}
			}
			sb.Append("  " + "*** END of LinkData ***\n");
			return sb.ToString();
		}
		
		/// <summary> This method returns an enumeration of data that it contains</summary>
		/// <returns> Enumeration
		/// </returns>
		/// <deprecated> Use children() instead.
		/// </deprecated>
		public virtual ISimpleNodeIterator LinkData()
		{
			return GetChildren();
		}
		
		/// <summary> Extract the link from the HREF attribute.</summary>
		/// <returns> The URL from the HREF attibute. This is absolute if the tag has
		/// a valid page.
		/// </returns>
		public virtual System.String ExtractLink()
		{
			System.String ret;
			
			ret = GetAttribute("HREF");
			if (null != ret)
			{
				ret = ParserUtils.RemoveChars(ret, '\n');
				ret = ParserUtils.RemoveChars(ret, '\r');
			}
			if (null != Page)
			{
				ret = Page.GetAbsoluteURL(ret);
			}
			
			return (ret);
		}

		public virtual System.String ExtractForwardLinkType()
		{
			return String.Empty;
		}

		public virtual System.String ExtractReverseLinkType()
		{
			return String.Empty;
		}
	}
}
