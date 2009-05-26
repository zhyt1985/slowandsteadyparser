// ***************************************************************
//  HttpProtocolOutput   version:  1.0   Date: 12/19/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Net;

namespace Winista.Text.HtmlParser.Http
{
	/// <summary>
	/// Summary description for HttpProtocolOutput.
	/// </summary>
	public sealed class HttpProtocolOutput
	{
		#region Class Memebers
		private Version m_ProtocolVersion;
		private HttpProtocolStatus m_status;
		private HttpProtocolContent m_content;
		private CookieCollection m_Cookies;
		#endregion

		#region Class Constructors
		public HttpProtocolOutput(HttpProtocolContent content)
		{
			this.m_content = content;
			this.m_status = HttpProtocolStatus.STATUS_SUCCESS;
			if (null == content)
			{
				this.m_status = HttpProtocolStatus.STATUS_NOTFOUND;
			}
		}

		public HttpProtocolOutput(HttpProtocolContent content, HttpProtocolStatus status)
		{
			this.m_content = content;
			this.m_status = status;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets or sets version of HTTP response.
		/// </summary>
		public Version ProtocolVersion
		{
			get
			{
				return this.m_ProtocolVersion;
			}
			set
			{
				this.m_ProtocolVersion = value;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public HttpProtocolContent Content
		{
			get
			{
				return m_content;
			}
			
			set
			{
				this.m_content = value;
			}
			
		}

		/// <summary>
		/// 
		/// </summary>
		public HttpProtocolStatus Status
		{
			get
			{
				return m_status;
			}
			
			set
			{
				this.m_status = value;
			}
		}

		/// <summary>
		/// Gets or sets cookies associated with response.
		/// </summary>
		public CookieCollection Cookies
		{
			get
			{
				return this.m_Cookies;
			}
			set
			{
				this.m_Cookies = value;
			}
		}
		#endregion
	}
}
