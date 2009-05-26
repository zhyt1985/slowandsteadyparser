// ***************************************************************
//  LinkData   version:  1.0   Date: 12/22/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista, All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Tags;

namespace Winista.Text.HtmlParser.Data
{
	/// <summary>
	/// 
	/// </summary>
	public enum LinkStatus
	{
		/// <summary>
		/// 
		/// </summary>
		Ok = 0,
		/// <summary>
		/// 
		/// </summary>
		Redirected = 1,
		/// <summary>
		/// 
		/// </summary>
		Broken = 2,
		/// <summary>
		/// 
		/// </summary>
		AuthenticationRequired = 3
	}

	/// <summary>
	/// 
	/// </summary>
	public enum LinkType
	{
		/// <summary>
		/// 
		/// </summary>
		Outlink = 0,
		/// <summary>
		/// 
		/// </summary>
		MailLink = 1,
		/// <summary>
		/// 
		/// </summary>
		SciptLink = 2,
		/// <summary>
		/// 
		/// </summary>
		Ftp = 3,
		/// <summary>
		/// 
		/// </summary>
		Other = 4
	}

	/// <summary>
	/// Summary description for LinkData.
	/// </summary>
	public class LinkData : AbstractData
	{
		#region Class Members
		private PageData m_Page;
		internal String m_strBaseUrl = String.Empty;
		internal String m_strUrl = String.Empty;
		internal String m_strText = String.Empty;
		internal LinkStatus m_Status;
		internal LinkType m_LinkType;
		#endregion

		/// <summary>
		/// Creates new instance of <see cref="LinkData"></see> object.
		/// </summary>
		/// <param name="obPage"></param>
		public LinkData(PageData obPage)
		{
			if (null == obPage)
			{
				throw new ArgumentNullException("obPage", "Null Page object specified");
			}
			this.m_Page = obPage;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obPage"></param>
		/// <param name="obTag"></param>
		public LinkData(PageData obPage, ATag obTag)
			:this(obPage)
		{
			this.ConvertFromLinkTag(obTag);
		}

		#region Public Properties
		/// <summary>
		/// 
		/// </summary>
		public String Url
		{
			get
			{
				return this.m_strUrl;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public String BaseUrl
		{
			get
			{
				return this.m_strBaseUrl;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public String Text
		{
			get
			{
				return this.m_strText;
			}
		}

		/// <summary>
		/// Gets or sets LinkStatus value.
		/// </summary>
		public LinkStatus LinkStatus
		{
			get
			{
				return this.m_Status;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public PageData PageData
		{
			get
			{
				return this.m_Page;
			}
		}

		/// <summary>
		/// Gets or sets link type.
		/// </summary>
		public LinkType LinkType
		{
			get
			{
				return this.m_LinkType;
			}
		}
		#endregion

		#region Helper Methods
		private void ConvertFromLinkTag(ATag obTag)
		{
			if (null == obTag)
			{
				throw new ArgumentNullException("obTag", "Null ATag object specified");
			}

			base.ConvertFromTag(obTag);
			this.m_strText = obTag.LinkText;
			this.m_strUrl = obTag.Link;
			this.m_LinkType = LinkType.Outlink;
			if (obTag.MailLink)
			{
				this.m_LinkType = LinkType.MailLink;
			}
			else if (obTag.FTPLink)
			{
				this.m_LinkType = LinkType.Ftp;
			}
			else if (obTag.IRCLink)
			{
				this.m_LinkType = LinkType.Other;
			}
		}
		#endregion
	}
}
