// ***************************************************************
//  HeadData   version:  1.0   Date: 12/30/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Data
{
	/// <summary>
	/// Summary description for HeadData.
	/// </summary>
	public class HeadData : AbstractData
	{
		#region Class Members
		internal String m_strTitle = String.Empty;
		internal String m_strDescription = String.Empty;
		internal String[] m_collKeywords = new String[]{};
		internal MetaTagDataCollection m_MetaTags;
		internal bool m_bNoCache;
		internal bool m_bNoIndex;
		internal bool m_bNoFollow;
		internal bool m_bRefresh;
		internal Int32 m_iRefreshTime  = -1;
		internal String m_strRefreshUrl = String.Empty;
		internal DateTime m_ContentExpirationDate;
		internal DateTime m_LastModifiedOn;
		#endregion
		/// <summary>
		/// Creates new instance of <see cref="HeadData"></see> object.
		/// </summary>
		public HeadData()
		{
			m_MetaTags = new MetaTagDataCollection();
		}

		#region Public Properties
		/// <summary>
		/// Gets page title.
		/// </summary>
		public String Title
		{
			get
			{
				return this.m_strTitle;
			}
		}

		/// <summary>
		/// Gets description meta tag.
		/// </summary>
		public String Description
		{
			get
			{
				return this.m_strDescription;
			}
		}
		/// <summary>
		/// Gets collection of Meta tags in header
		/// </summary>
		public MetaTagDataCollection MetaTags
		{
			get
			{
				return this.m_MetaTags;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public String[] Keywords
		{
			get
			{
				return this.m_collKeywords;
			}
		}

		/// <summary>
		/// Gets the value indicating of page is marked for no-caching.
		/// </summary>
		public bool NoCache
		{
			get
			{
				return this.m_bNoCache;
			}
		}

		/// <summary>
		/// Gets the value indicating if page is marked for no-follow.
		/// </summary>
		public bool NoFollow
		{
			get
			{
				return this.m_bNoFollow;
			}
		}

		/// <summary>
		/// Gets the value indicating if pages is marked for no indexing.
		/// </summary>
		public bool NoIndex
		{
			get
			{
				return this.m_bNoIndex;
			}
		}

		/// <summary>
		/// Gets value representing interval after which page will be refreshed.
		/// </summary>
		public Int32 RefreshTime
		{
			get
			{
				return this.m_iRefreshTime;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public bool Refresh
		{
			get
			{
				return this.m_bRefresh;
			}
		}

		/// <summary>
		/// Gets URL where refreshed content will be available.
		/// </summary>
		public String RefreshUrl
		{
			get
			{
				return this.m_strRefreshUrl;
			}
		}

		/// <summary>
		/// Gets date on which content will expire.
		/// </summary>
		public DateTime ContentExpirationDate
		{
			get
			{
				return this.m_ContentExpirationDate;
			}
		}

		/// <summary>
		/// Gets date on which content of this page was last modified.
		/// </summary>
		public DateTime LastModifiedOn
		{
			get
			{
				return this.m_LastModifiedOn;
			}
		}
		#endregion
	}
}
