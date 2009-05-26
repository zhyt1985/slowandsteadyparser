// ***************************************************************
//  HttpProtocol   version:  1.0   Date: 12/15/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Diagnostics;
using System.Web;

using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Http
{
	/// <summary>
	/// Summary description for HttpProtocol.
	/// </summary>
	public sealed class HttpProtocol
	{
		internal const int BUFFER_SIZE = 8 * 1024;
		
		internal static readonly int MAX_REDIRECTS = ParserConf.GetConfiguration().GetInt("http.redirect.max", 3);
		
		internal static System.String PROXY_HOST = ParserConf.GetConfiguration().GetPoperty("http.proxy.host");
		internal static int PROXY_PORT = ParserConf.GetConfiguration().GetInt("http.proxy.port", 8080);
		internal static bool PROXY = (PROXY_HOST != null && PROXY_HOST.Length > 0);
		
		internal static int TIMEOUT = ParserConf.GetConfiguration().GetInt("http.timeout", 10000);
		internal static int MAX_CONTENT = ParserConf.GetConfiguration().GetInt("http.content.limit", 64 * 1024);
		
		internal static int MAX_DELAYS = ParserConf.GetConfiguration().GetInt("http.max.delays", 3);
		internal static int MAX_THREADS_PER_HOST = ParserConf.GetConfiguration().GetInt("fetcher.threads.per.host", 1);
		
		internal static System.String AGENT_STRING = AgentString;
		
		internal static long SERVER_DELAY = (long) (ParserConf.GetConfiguration().GetFloat("fetcher.server.delay", 1.0f) * 1000);
		internal static bool HONOR_ROBOTSTEXT = ParserConf.GetConfiguration().GetBoolean("http.robots.honor", true);

		internal static bool CONTENTTYPE_CHANGE_STRICT = ParserConf.GetConfiguration().GetBoolean("http.contenttype.check.strict", false);
		
		/// <summary>Maps from InetAddress to a Long naming the time it should be unblocked.
		/// The Long is zero while the address is in use, then set to now+wait when
		/// a request finishes.  This way only one thread at a time accesses an
		/// address. 
		/// </summary>
		private static System.Collections.Hashtable BLOCKED_ADDR_TO_TIME = new System.Collections.Hashtable();
		
		/// <summary>Maps an address to the number of threads accessing that address. </summary>
		private static System.Collections.Hashtable THREADS_PER_HOST_COUNT = new System.Collections.Hashtable();
		
		/// <summary>Queue of blocked InetAddress.  This contains all of the non-zero entries
		/// from BLOCKED_ADDR_TO_TIME, ordered by increasing time. 
		/// </summary>
		private static System.Collections.ArrayList BLOCKED_ADDR_QUEUE = new System.Collections.ArrayList();
		
		private RobotRulesParser m_robotRules = new RobotRulesParser();

		private System.Uri m_Url;
		internal HttpProtocolOutput m_ProtocolOutput;

		/// <summary>
		/// 
		/// </summary>
		public HttpProtocol()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="url"></param>
		public HttpProtocol(System.Uri url)
		{
			m_Url = url;
		}

		/// <summary>
		/// Gets URL of page that needs to be parsed.
		/// </summary>
		public Uri URL
		{
			get
			{
				return this.m_Url;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public HttpProtocolOutput GetProtocolOutput()
		{
			return GetProtocolOutput(m_Url);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pageUri"></param>
		/// <returns></returns>
		public HttpProtocolOutput GetProtocolOutput(System.Uri pageUri)
		{
			this.m_ProtocolOutput = null;
			HttpProtocolStatus obStatus = null;
			System.String urlString = pageUri.ToString();
			try
			{
				System.Uri url = new System.Uri(urlString);
				int redirects = 0;
				while(true)
				{
					if (!RobotRulesParser.IsAllowed(url))
					{
						if (HttpProtocol.HONOR_ROBOTSTEXT)
						{
							throw new RobotBlockedException(url);
						}
					}

					System.Net.IPAddress addr = BlockAddr(url);

					HttpResponseMgr response;

					try
					{
						response = new HttpResponseMgr(urlString, url); // make a request
					}
					finally
					{
						UnblockAddr(addr);
					}

					int code = response.Code;

					if (code == 200)
					{
						// got a good response
						obStatus = HttpProtocolStatus.STATUS_SUCCESS;
						m_ProtocolOutput = new HttpProtocolOutput(new HttpProtocolContent(response.Content, response.Headers),obStatus); // return it
						m_ProtocolOutput.Cookies = response.Cookies;
						m_ProtocolOutput.ProtocolVersion = response.ProtocolVersion;
						return m_ProtocolOutput;
					}
					else if (code == 410)
					{
						// page is gone
						throw new ResourceGoneException(url, "Http: " + code);
					}
					else if (code >= 300 && code < 400)
					{
						// handle redirect
						if (redirects == MAX_REDIRECTS)
						{
							throw new System.Web.HttpException("Too many redirects: " + urlString);
						}
						url = new System.Uri(url, response.GetHeader("Location"));
						redirects++;
						System.Diagnostics.Trace.WriteLine("redirect to " + url);
					}
					else
					{
						// convert to exception
						throw new HttpError(code);
					}
				}
			}
			catch(RobotBlockedException ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.Message);
				m_ProtocolOutput = new HttpProtocolOutput(null, HttpProtocolStatus.STATUS_ROBOTS_DENIED);
			}
			catch(HttpError ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.Message);
				obStatus = new HttpProtocolStatus(ex.Code);
				m_ProtocolOutput = new HttpProtocolOutput(null, obStatus);
			}
			catch (System.Exception e)
			{
				System.Diagnostics.Trace.WriteLine(e.Message);
				m_ProtocolOutput = new HttpProtocolOutput(null, HttpProtocolStatus.STATUS_FAILED);
			}
			return m_ProtocolOutput;
		}

		private static System.Net.IPAddress BlockAddr(System.Uri url)
		{
			System.Net.IPAddress addr;
			try
			{
				addr = System.Net.Dns.Resolve(url.Host).AddressList[0];
			}
			catch (System.Exception e)
			{
				throw new System.Web.HttpException("Failed to resolve address", e);
			}
			
			int delays = 0;
			while (true)
			{
				CleanExpiredServerBlocks(); // free held addresses
				
				System.Int64 time;
				lock (BLOCKED_ADDR_TO_TIME.SyncRoot)
				{
					object obTime = BLOCKED_ADDR_TO_TIME[addr];
					if (obTime == null)
					{
						// address is free
						
						// get # of threads already accessing this addr
						object obCounter = THREADS_PER_HOST_COUNT[addr];
						int count = (obCounter == null) ? 0:(Int32)obCounter;
						
						count++; // increment & store
						THREADS_PER_HOST_COUNT[addr] = (System.Int32) count;
						
						if (count >= MAX_THREADS_PER_HOST)
						{
							BLOCKED_ADDR_TO_TIME[addr] = 0; // block it
						}
						return addr;
					}
					else
					{
						time = (Int64)obTime;
					}
				}
								
				if (delays == MAX_DELAYS)
				{
					throw new RetryLaterException(url);
				}
				
				long done = time;
				long now = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
				long sleep = 0;
				if (done == 0)
				{
					// address is still in use
					sleep = SERVER_DELAY; // wait at least delay
				}
				else if (now < done)
				{
					// address is on hold
					sleep = done - now; // wait until its free
				}
				
				try
				{
					System.Threading.Thread.Sleep(new System.TimeSpan((System.Int64) 10000 * sleep));
				}
				catch (System.Threading.ThreadInterruptedException e)
				{
				}
				delays++;
			}
		}

		private static void CleanExpiredServerBlocks()
		{
			lock (BLOCKED_ADDR_TO_TIME.SyncRoot)
			{
				while (!(BLOCKED_ADDR_QUEUE.Count == 0))
				{
					System.Net.IPAddress addr = (System.Net.IPAddress) BLOCKED_ADDR_QUEUE[BLOCKED_ADDR_QUEUE.Count - 1];
					long time = (long) ((System.Int64) BLOCKED_ADDR_TO_TIME[addr]);
					if (time <= (System.DateTime.Now.Ticks - 621355968000000000) / 10000)
					{
						BLOCKED_ADDR_TO_TIME.Remove(addr);
						BLOCKED_ADDR_QUEUE.RemoveAt(BLOCKED_ADDR_QUEUE.Count - 1);
					}
					else
					{
						break;
					}
				}
			}
		}

		private static void UnblockAddr(System.Net.IPAddress addr)
		{
			lock (BLOCKED_ADDR_TO_TIME.SyncRoot)
			{
				int addrCount = ((System.Int32) THREADS_PER_HOST_COUNT[addr]);
				if (addrCount == 1)
				{
					THREADS_PER_HOST_COUNT.Remove(addr);
					BLOCKED_ADDR_QUEUE.Insert(0, addr);
					BLOCKED_ADDR_TO_TIME[addr] = (long) ((System.DateTime.Now.Ticks - 621355968000000000) / 10000 + SERVER_DELAY);
				}
				else
				{
					THREADS_PER_HOST_COUNT[addr] = (System.Int32) (addrCount - 1);
				}
			}
		}

		private static System.String AgentString
		{
			get
			{
				System.String agentName = ParserConf.GetConfiguration().GetPoperty("http.agent.name");
				System.String agentVersion = ParserConf.GetConfiguration().GetPoperty("http.agent.version");
				System.String agentDesc = ParserConf.GetConfiguration().GetPoperty("http.agent.description");
				System.String agentURL = ParserConf.GetConfiguration().GetPoperty("http.agent.url");
				System.String agentEmail = ParserConf.GetConfiguration().GetPoperty("http.agent.email");
				
				if ((agentName == null) || (agentName.Trim().Length == 0))
				{
					Trace.WriteLine("No User-Agent string set (http.agent.name)!");
				}
				
				System.Text.StringBuilder buf = new System.Text.StringBuilder();
				
				buf.Append(agentName);
				if (agentVersion != null)
				{
					buf.Append("/");
					buf.Append(agentVersion);
				}
				if (((agentDesc != null) && (agentDesc.Length != 0)) || ((agentEmail != null) && (agentEmail.Length != 0)) || ((agentURL != null) && (agentURL.Length != 0)))
				{
					buf.Append(" (");
					
					if ((agentDesc != null) && (agentDesc.Length != 0))
					{
						buf.Append(agentDesc);
						if ((agentURL != null) || (agentEmail != null))
							buf.Append("; ");
					}
					
					if ((agentURL != null) && (agentURL.Length != 0))
					{
						buf.Append(agentURL);
						if (agentEmail != null)
							buf.Append("; ");
					}
					
					if ((agentEmail != null) && (agentEmail.Length != 0))
						buf.Append(agentEmail);
					
					buf.Append(")");
				}
				return buf.ToString();
			}
			
		}
	}
}
