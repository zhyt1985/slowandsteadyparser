// ***************************************************************
//  HttpProtocolStatus   version:  1.0   Date: 12/19/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Http
{
	/// <summary>
	/// Summary description for HttpProtocolStatus.
	/// </summary>
	public class HttpProtocolStatus
	{
		private const sbyte VERSION = 1;
		
		/// <summary>Content was retrieved without errors. </summary>
		public const int SUCCESS = 1;
		/// <summary>Content was not retrieved. Any further errors may be indicated in args. </summary>
		public const int FAILED = 2;
		
		/// <summary>This protocol was not found.  Application may attempt to retry later. </summary>
		public const int PROTO_NOT_FOUND = 10;
		/// <summary>Resource is gone. </summary>
		public const int GONE = 11;
		/// <summary>Resource has moved permanently. New url should be found in args. </summary>
		public const int MOVED = 12;
		/// <summary>Resource has moved temporarily. New url should be found in args. </summary>
		public const int TEMP_MOVED = 13;
		/// <summary>Resource was not found. </summary>
		public const int NOTFOUND = 14;
		/// <summary>Temporary failure. Application may retry immediately. </summary>
		public const int RETRY = 15;
		/// <summary>Unspecified exception occured. Further information may be provided in args. </summary>
		public const int EXCEPTION = 16;
		/// <summary>Access denied - authorization required, but missing/incorrect. </summary>
		public const int ACCESS_DENIED = 17;
		/// <summary>Access denied by robots.txt rules. </summary>
		public const int ROBOTS_DENIED = 18;
		/// <summary>Too many redirects. </summary>
		public const int REDIR_EXCEEDED = 19;
		/// <summary>Not fetching. </summary>
		public const int NOTFETCHING = 20;
		/// <summary>Unchanged since the last fetch. </summary>
		public const int NOTMODIFIED = 21;
		
		// Useful static instances for status codes that don't usually require any
		// additional arguments.
		public static readonly HttpProtocolStatus STATUS_SUCCESS = new HttpProtocolStatus(SUCCESS);
		public static readonly HttpProtocolStatus STATUS_FAILED = new HttpProtocolStatus(FAILED);
		public static readonly HttpProtocolStatus STATUS_GONE = new HttpProtocolStatus(GONE);
		public static readonly HttpProtocolStatus STATUS_NOTFOUND = new HttpProtocolStatus(NOTFOUND);
		public static readonly HttpProtocolStatus STATUS_RETRY = new HttpProtocolStatus(RETRY);
		public static readonly HttpProtocolStatus STATUS_ROBOTS_DENIED = new HttpProtocolStatus(ROBOTS_DENIED);
		public static readonly HttpProtocolStatus STATUS_REDIR_EXCEEDED = new HttpProtocolStatus(REDIR_EXCEEDED);
		public static readonly HttpProtocolStatus STATUS_NOTFETCHING = new HttpProtocolStatus(NOTFETCHING);
		public static readonly HttpProtocolStatus STATUS_NOTMODIFIED = new HttpProtocolStatus(NOTMODIFIED);

		private int m_code;
		private long m_lastModified;

		private static System.Collections.Hashtable m_codeToName = new System.Collections.Hashtable();

		public HttpProtocolStatus()
		{
		}

		public HttpProtocolStatus(int code):this(code, 0)
		{
		}

		public HttpProtocolStatus(int code, long lastModified)
		{
			this.m_code = code;
			this.m_lastModified = lastModified;
		}

		/// <summary>
		/// Gets the status code
		/// </summary>
		public Int32 Code
		{
			get
			{
				return this.m_code;
			}
		}

		/// <summary>
		/// Gets the date when the response was last modified.
		/// </summary>
		public Int64 LastModified
		{
			get
			{
				return this.m_lastModified;
			}
		}

		static HttpProtocolStatus()
		{
			{
				m_codeToName[(System.Int32) SUCCESS] = "success";
				m_codeToName[(System.Int32) FAILED] = "failed";
				m_codeToName[(System.Int32) PROTO_NOT_FOUND] = "proto_not_found";
				m_codeToName[(System.Int32) GONE] = "gone";
				m_codeToName[(System.Int32) MOVED] = "moved";
				m_codeToName[(System.Int32) TEMP_MOVED] = "temp_moved";
				m_codeToName[(System.Int32) NOTFOUND] = "notfound";
				m_codeToName[(System.Int32) RETRY] = "retry";
				m_codeToName[(System.Int32) EXCEPTION] = "exception";
				m_codeToName[(System.Int32) ACCESS_DENIED] = "access_denied";
				m_codeToName[(System.Int32) ROBOTS_DENIED] = "robots_denied";
				m_codeToName[(System.Int32) REDIR_EXCEEDED] = "redir_exceeded";
				m_codeToName[(System.Int32) NOTFETCHING] = "notfetching";
				m_codeToName[(System.Int32) NOTMODIFIED] = "notmodified";
			}
		}
	}
}
