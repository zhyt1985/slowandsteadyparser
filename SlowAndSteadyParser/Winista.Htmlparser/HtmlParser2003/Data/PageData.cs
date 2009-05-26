// ***************************************************************
//  PageData   version:  1.0   Date: 12/22/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista, All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Net;
using System.Web;
using System.Text;

using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Data
{
	/// <summary>
	/// Summary description for PageData.
	/// </summary>
	public class PageData : AbstractData
	{
		#region Private Members
		internal String m_strUrl;
		internal String m_strBaseUrl = String.Empty;
		internal ContentProperties m_Headers;
		internal LinkDataCollection m_Outlinks;
		internal ImageDataCollection m_ImageLinks;
		internal TableDataCollection m_Tables;
		internal HeadData m_HeaderData;
		internal CookieCollection m_Cookies;
		internal Version m_ProtocolVersion;
		internal String m_strTextContent = String.Empty;
		internal Encoding m_ContentEncoding;
		#endregion

		/// <summary>
		/// Creates new instance of <see cref="PageData"></see> object.
		/// </summary>
		public PageData()
		{
			m_HeaderData = new HeadData();
			m_Outlinks = new LinkDataCollection();
			m_ImageLinks = new ImageDataCollection();
			m_Cookies = new CookieCollection();
			m_Tables = new TableDataCollection();
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
		/// Gets url for the page as specified in BASE header.
		/// </summary>
		public String BaseUrl
		{
			get
			{
				return this.m_strBaseUrl;
			}
		}

		/// <summary>
		/// Gets content's encoding type.
		/// </summary>
		public Encoding ContentType
		{
			get
			{
				return this.m_ContentEncoding;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ContentProperties Headers
		{
			get
			{
				return this.m_Headers;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public LinkDataCollection OutLinks
		{
			get
			{
				return this.m_Outlinks;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ImageDataCollection ImageLinks
		{
			get
			{
				return this.m_ImageLinks;
			}
		}

		/// <summary>
		/// Gets collection of tables in the page.
		/// </summary>
		public TableDataCollection Tables
		{
			get
			{
				return this.m_Tables;
			}
		}

		/// <summary>
		/// Gets head information.
		/// </summary>
		public HeadData HeadData
		{
			get
			{
				return this.m_HeaderData;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public String TextContent
		{
			get
			{
				return this.m_strTextContent;
			}
		}

		/// <summary>
		/// Gets cookies associated with page.
		/// </summary>
		public CookieCollection Cookies
		{
			get
			{
				return this.m_Cookies;
			}
		}

		/// <summary>
		/// Gets HTTP version for response.
		/// </summary>
		public Version ProtocolVersion
		{
			get
			{
				return this.m_ProtocolVersion;
			}
		}
		#endregion

		#region Public Methods
		public String GetTitle()
		{
			return m_HeaderData.Title;
		}
		#endregion

		#region Helper Methods
		private String GetHeaderValue()
		{
			return String.Empty;
		}
		#endregion
	}
}
