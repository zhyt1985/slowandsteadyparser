// ***************************************************************
//  HttpResponseMgr   version:  1.0   Date: 12/15/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.IO;
using System.Web;
using System.Net;
using System.Diagnostics;

using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Http
{
	/// <summary>
	/// Summary description for HttpResponseMgr.
	/// </summary>
	internal sealed class HttpResponseMgr
	{
		#region Class Members
		private System.String m_strOrig;
		private System.String m_strBase;
		private byte[] m_content;
		private int m_code;
		private CookieCollection m_Cookies;
		private Version m_ProtocolVersion;
		private ContentProperties m_headers = new ContentProperties();
		#endregion

		#region Class Constructors
		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		public HttpResponseMgr(System.Uri url):this(url.ToString(), url)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orig"></param>
		/// <param name="url"></param>
		public HttpResponseMgr(System.String orig, System.Uri url)
			: this(orig, url, false)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="orig"></param>
		/// <param name="url"></param>
		/// <param name="bHeaderOnly"></param>
		public HttpResponseMgr(System.String orig, System.Uri url, bool bHeaderOnly)
		{
			ProcessRequest(orig, url, bHeaderOnly);
		}
		#endregion

		#region Internal Properties

		/// <summary>Returns the response code. </summary>
		internal int Code
		{
			get
			{
				return m_code;
			}
			
		}

		internal byte[] Content
		{
			get
			{
				return m_content;
			}
		}

		/// <summary>
		/// Gets headers for the page.
		/// </summary>
		internal ContentProperties Headers
		{
			get
			{
				return this.m_headers;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal CookieCollection Cookies
		{
			get
			{
				return this.m_Cookies;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		internal Version ProtocolVersion
		{
			get
			{
				return this.m_ProtocolVersion;
			}
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		internal System.String GetHeader(System.String name)
		{
			return (System.String) m_headers.GetProperty(name);
		}

		internal void ProcessRequest(System.String orig, System.Uri url, bool bHeadersOnly)
		{
			this.m_strOrig = orig;
			this.m_strBase = url.ToString();
			
			if (!"http".Equals(url.Scheme))
			{
				throw new HttpException("Not an HTTP url:" + url);
			}

			Trace.WriteLine("Fetching - " + url);

			System.String path = "".Equals(url.PathAndQuery)?"/":url.PathAndQuery;

			System.String host = url.Host;
			int port;
			System.String portString;
			if (url.Port == - 1)
			{
				port = 80;
				portString = "";
			}
			else
			{
				port = url.Port;
				portString = ":" + port;
			}

			System.Net.HttpWebRequest req = System.Net.WebRequest.Create(url) as System.Net.HttpWebRequest;
			if (HttpProtocol.PROXY)
			{
				String strProxyUrl = String.Empty;
				strProxyUrl += HttpProtocol.PROXY_HOST;
				if (HttpProtocol.PROXY_PORT != 0)
				{
					strProxyUrl += String.Format(":{0}", HttpProtocol.PROXY_PORT);
				}
				System.Net.WebProxy obProxy = new System.Net.WebProxy(strProxyUrl) ;
				req.Proxy = obProxy;
			}

			//
			// Set the number of redirections allowed.
			//
			if (HttpProtocol.MAX_REDIRECTS >= 0)
			{
				req.MaximumAutomaticRedirections = HttpProtocol.MAX_REDIRECTS;
			}

			req.Timeout = HttpProtocol.TIMEOUT;
			if (bHeadersOnly)
			{
				req.Method = "HEADER";
			}
			else
			{

				req.Method = "GET";
			}
			

			req.Headers.Add("Accept-Encoding", "x-gzip, gzip");
			if (HttpProtocol.AGENT_STRING != null &&
				String.Empty != HttpProtocol.AGENT_STRING)
			{
				req.UserAgent = HttpProtocol.AGENT_STRING;
			}

			System.Net.HttpWebResponse resp = null;
			//
			// Now send the request.
			//
			try
			{
				resp = req.GetResponse() as System.Net.HttpWebResponse;
				
				this.m_ProtocolVersion = resp.ProtocolVersion;
				ProcessStatus(resp);
				ProcessHeaders(resp);
				ProcessCookies(resp);

				//
				// If this is header only call, then there is no need to go
				// through processing of content stream.
				//
				if(!bHeadersOnly)
				{
					ProcessContent(resp);

					System.String contentEncoding = resp.Headers["Content-Encoding"];
					if ((String.Compare("gzip", contentEncoding, true) == 0) ||
						(String.Compare("x-gzip", contentEncoding) == 0))
					{
						System.IO.MemoryStream memStream = null;
						//
						// Need to decompress the response.
						//
						Trace.WriteLine("Uncompressing response ...");
						Int32 compressesLength = m_content.Length;
						try
						{
							memStream = new System.IO.MemoryStream(m_content);
							m_content = GZIPUtils.GUnzipBestEffort(memStream);
							if (null == m_content)
							{
								System.Diagnostics.Trace.WriteLine("GUnzip failed");
							}
							else
							{
								System.Diagnostics.Trace.WriteLine("fetched " + compressesLength + " bytes of compressed content (expanded to " + m_content.Length + " bytes) from " + url);
							}
						}
						catch (System.Exception e)
						{
							Trace.WriteLine(e.Message);
							if (null != memStream)
							{
								memStream.Close();
							}
						}
					}
					else if(String.Compare("deflate", contentEncoding, true) == 0)
					{
						System.IO.MemoryStream memStream = null;
						//
						// Need to decompress the response.
						//
						Trace.WriteLine("Deflating response ...");
						Int32 compressesLength = m_content.Length;
						try
						{
							memStream = new System.IO.MemoryStream(m_content);
							m_content = GZIPUtils.Unzip(memStream);
							if (null == m_content)
							{
								System.Diagnostics.Trace.WriteLine("Unzip failed");
							}
							else
							{
								System.Diagnostics.Trace.WriteLine("fetched " + compressesLength + " bytes of compressed content (expanded to " + m_content.Length + " bytes) from " + url);
							}
						}
						catch (System.Exception e)
						{
							Trace.WriteLine(e.Message);
							if (null != memStream)
							{
								memStream.Close();
							}
						}
					}
					else
					{
						Trace.WriteLine("fetched " + resp.ContentLength + " bytes from " + url);
					}
				}
			}
			catch(ProtocolViolationException prEx)
			{
				Trace.WriteLine("Protocol violation - " + prEx.Message);
				throw new ParserException("Protocol violation", prEx);
			}
			catch(WebException webEx)
			{
				Trace.WriteLine(HttpUtil.GetWebExceptionDescription(webEx) + webEx.Message);
				if (webEx.Status == WebExceptionStatus.ProtocolError)
				{
					m_code = (Int32)webEx.Status;
					ProcessContent(webEx.Response as HttpWebResponse);
				}
			}
			catch(InvalidOperationException opEx)
			{
				Trace.WriteLine("Invalid operation performed on request - " + opEx.Message);
				throw new ParserException("Invalid operation performed on request", opEx);
			}
			catch (System.Exception e)
			{
				throw new ApplicationException(String.Format("ProcessRequest for Url failed - {0}", e.Message), e);
			}
			finally
			{
				if (null != resp)
				{
					resp.Close();
				}
			}
		}

		private void ProcessContent(System.Net.HttpWebResponse resp)
		{
			Int64 iContentLength = resp.ContentLength;
			if (HttpProtocol.MAX_CONTENT >= 0 &&
				iContentLength > HttpProtocol.MAX_CONTENT)
			{
				iContentLength = HttpProtocol.MAX_CONTENT;
			}

			MemoryStream memStream = new MemoryStream();
			const int BUFFER_SIZE = 4096;
			int iRead = 0;
			int idx = 0;
			Int64 iSize = 0;
			memStream.SetLength(BUFFER_SIZE);
			while(true)
			{
				byte [] respBuffer = new byte[BUFFER_SIZE];
				try
				{
					iRead = resp.GetResponseStream().Read(respBuffer, 0, BUFFER_SIZE);
				}
				catch (System.Exception e)
				{
					Trace.WriteLine(e.Message);
				}
				
				if (iRead == 0)
				{
					break;
				}

				iSize += iRead;
				memStream.SetLength(iSize);
				memStream.Write(respBuffer, 0, iRead);
				idx += iRead;
			}

			m_content = memStream.ToArray();
			memStream.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="resp"></param>
		private void ProcessStatus(System.Net.HttpWebResponse resp)
		{
			m_code = (Int32)resp.StatusCode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="resp"></param>
		private void ProcessHeaders(System.Net.HttpWebResponse resp)
		{
			if (resp.Headers.Count != 0)
			{
				m_headers = new ContentProperties(resp.Headers);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="resp"></param>
		private void ProcessCookies(System.Net.HttpWebResponse resp)
		{
			this.m_Cookies = resp.Cookies;
		}
	}
}
