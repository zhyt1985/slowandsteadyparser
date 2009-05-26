// ***************************************************************
//  ATag   version:  1.0   Date: 02/24/2006
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2006 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Support;
using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> Identifies a "A" (href) tag.</summary>
	[Serializable]
	public class ATag:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"A"};
		
		/// <summary> The set of tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEnders = new System.String[]{"A", "P", "DIV", "TD", "TR", "FORM", "LI"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"P", "DIV", "TD", "TR", "FORM", "LI", "BODY", "HTML"};
		
		/// <summary> The URL where the link points to</summary>
		protected internal System.String mLink;
		
		/// <summary> Set to true when the link was a mailto: URL.</summary>
		private bool mailLink;

		/// <summary> Set to true when the link was a javascript: URL.</summary>
		private bool javascriptLink;

		/// <summary> Constructor creates an LinkTag object, which basically stores the location
		/// where the link points to, and the text it contains.
		/// <p>
		/// In order to get the contents of the link tag, use the method linkData(),
		/// which returns an enumeration of nodes encapsulated within the link.
		/// <p>
		/// The following code will get all the images inside a link tag.
		/// <pre>
		/// Node node ;
		/// ImageTag imageTag;
		/// for (Enumeration e=linkTag.linkData();e.hasMoreElements();) {
		/// node = (Node)e.nextElement();
		/// if (node is ImageTag) {
		/// imageTag = (ImageTag)node;
		/// // Process imageTag
		/// }
		/// }
		/// </pre>
		/// </summary>
		public ATag()
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

		/// <summary> Returns the url as a string, to which this link points.
		/// This string has had the "mailto:" and "javascript:" protocol stripped
		/// off the front (if those predicates return <code>true</code>) but not
		/// for other protocols. Don't ask me why, it's a legacy thing.
		/// </summary>
		/// <returns> The URL for this <code>A</code> tag.
		/// </returns>
		/// <summary> Set the <code>HREF</code> attribute.</summary>
		virtual public System.String Link
		{
			get
			{
				if (null == mLink)
				{
					mailLink = false;
					javascriptLink = false;
					mLink = ExtractLink();
					
					int mailto = mLink.IndexOf("mailto");
					if (mailto == 0)
					{
						// yes it is
						mailto = mLink.IndexOf(":");
						mLink = mLink.Substring(mailto + 1);
						mailLink = true;
					}
					int javascript = mLink.IndexOf("javascript:");
					if (javascript == 0)
					{
						mLink = mLink.Substring(11); // this magic number is "javascript:".length()
						javascriptLink = true;
					}
				}
				return (mLink);
			}
			
			set
			{
				mLink = value;
				SetAttribute("HREF", value);
			}
			
		}
		/// <summary> Returns the text contained inside this link tag.</summary>
		/// <returns> The textual contents between the {@.html <A></A>} pair.
		/// </returns>
		virtual public System.String LinkText
		{
			get
			{
				System.String ret;
				
				if (null != Children)
					ret = Children.AsString();
				else
					ret = "";
				
				return (ret);
			}
			
		}

		/// <summary> Is this a mail address</summary>
		/// <returns> boolean true/false
		/// </returns>
		/// <summary> Insert the method's description here.
		/// Creation date: (8/3/2001 1:49:31 AM)
		/// </summary>
		virtual public bool MailLink
		{
			get
			{
				System.String generatedAux = Link; // force an evaluation of the booleans
				return (mailLink);
			}
			
			set
			{
				mailLink = value;
			}
			
		}

		/// <summary> Tests if the link is javascript</summary>
		/// <returns> flag indicating if the link is a javascript code
		/// </returns>
		/// <summary> Set the link as a javascript link.
		/// 
		/// </summary>
		virtual public bool JavascriptLink
		{
			get
			{
				System.String generatedAux = Link; // force an evaluation of the booleans
				return (javascriptLink);
			}
			
			set
			{
				javascriptLink = value;
			}
			
		}
		/// <summary> Tests if the link is an FTP link.
		/// 
		/// </summary>
		/// <returns> flag indicating if this link is an FTP link
		/// </returns>
		virtual public bool FTPLink
		{
			get
			{
				return Link.IndexOf("ftp://") == 0;
			}
			
		}
		/// <summary> Tests if the link is an IRC link.</summary>
		/// <returns> flag indicating if this link is an IRC link
		/// </returns>
		virtual public bool IRCLink
		{
			get
			{
				return Link.IndexOf("irc://") == 0;
			}
			
		}
		/// <summary> Tests if the link is an HTTP link.
		/// 
		/// </summary>
		/// <returns> flag indicating if this link is an HTTP link
		/// </returns>
		virtual public bool HTTPLink
		{
			get
			{
				return (!FTPLink && !HTTPSLink && !JavascriptLink && !MailLink && !IRCLink);
			}
			
		}
		/// <summary> Tests if the link is an HTTPS link.
		/// 
		/// </summary>
		/// <returns> flag indicating if this link is an HTTPS link
		/// </returns>
		virtual public bool HTTPSLink
		{
			get
			{
				return Link.IndexOf("https://") == 0;
			}
			
		}
		/// <summary> Tests if the link is an HTTP link or one of its variations (HTTPS, etc.).
		/// 
		/// </summary>
		/// <returns> flag indicating if this link is an HTTP link or one of its variations (HTTPS, etc.)
		/// </returns>
		virtual public bool HTTPLikeLink
		{
			get
			{
				return HTTPLink || HTTPSLink;
			}
			
		}

		/// <summary> Return the contents of this link node as a string suitable for debugging.</summary>
		/// <returns> A string representation of this node.
		/// </returns>
		public override System.String ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("Link to : " + Link + "; titled : " + LinkText + "; begins at : " + StartPosition + "; ends at : " + EndPosition + ", AccessKey=");
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
				ret = Page.GetAbsoluteURL(ret);
			
			return (ret);
		}
	}
}
