// ***************************************************************
//  ChainedException   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary> Support for chained exceptions in code that predates Java 1.4.
	/// A chained exception can use a Throwable argument to reference
	/// a lower level exception. The chained exception provides a
	/// stack trace that includes the message and any throwable
	/// exception included as an argument in the chain.
	/// 
	/// Represents the output from two nested exceptions. The outside
	/// exception is a subclass of ChainedException called
	/// ApplicationException, which includes a throwable reference.
	/// The throwable reference is also a subclass of ChainedException,
	/// called ProcessException, which in turn includes a reference to
	/// a standard IOException. In each case, the message is increasingly
	/// specific about the nature of the problem. The end user may only
	/// see the application exception, but debugging is greatly
	/// enhanced by having more details in the stack trace.
	/// 
	/// </summary>
	public class ChainedException : System.Exception
	{
		protected internal System.Exception m_throwable;

		/// <summary>
		/// 
		/// </summary>
		public ChainedException()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ChainedException(System.String message):base(message)
		{
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="throwable"></param>
		public ChainedException(System.Exception throwable)
		{
			this.m_throwable = throwable;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="throwable"></param>
		public ChainedException(System.String message, System.Exception throwable):base(message)
		{
			this.m_throwable = throwable;
		}

		/// <summary>
		/// 
		/// </summary>
		virtual public System.Exception Throwable
		{
			get
			{
				return m_throwable;
			}
			
		}

		/// <summary>
		/// 
		/// </summary>
		virtual public System.String[] MessageChain
		{
			get
			{
				System.Collections.ArrayList list = MessageList;
				System.String[] chain = new System.String[list.Count];
				list.CopyTo(chain);
				return chain;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		virtual public System.Collections.ArrayList MessageList
		{
			get
			{
				System.Collections.ArrayList list = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
				list.Add(Message);
				if (m_throwable != null)
				{
					if (m_throwable is ChainedException)
					{
						ChainedException chain = (ChainedException) m_throwable;
						System.Collections.ArrayList sublist = chain.MessageList;
						for (int i = 0; i < sublist.Count; i++)
							list.Add(sublist[i]);
					}
					else
					{
						//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
						System.String message = m_throwable.Message;
						if (message != null && !message.Equals(""))
						{
							list.Add(message);
						}
					}
				}
				return list;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void  PrintStackTrace()
		{
			System.IO.StreamWriter temp_writer;
			temp_writer = new System.IO.StreamWriter(System.Console.OpenStandardError(), System.Console.Error.Encoding);
			temp_writer.AutoFlush = true;
			PrintStackTrace(temp_writer);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="out_Renamed"></param>
		public void  PrintStackTrace(System.IO.StreamWriter out_Renamed)
		{
			lock (out_Renamed)
			{
				if (m_throwable != null)
				{
					out_Renamed.WriteLine(GetType().FullName + ": " + Message + ";");
					if (m_throwable is ChainedException)
						((ChainedException) m_throwable).PrintStackTrace(out_Renamed);
					else
						Support.SupportMisc.WriteStackTrace(m_throwable, out_Renamed);
				}
				else
				{
					Support.SupportMisc.WriteStackTrace((System.Exception) this, out_Renamed);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="out_Renamed"></param>
		public void  printStackTrace(System.IO.StreamWriter out_Renamed)
		{
			lock (out_Renamed)
			{
				if (m_throwable != null)
				{
					out_Renamed.WriteLine(GetType().FullName + ": " + Message + ";");
					if (m_throwable is ChainedException)
						((ChainedException) m_throwable).PrintStackTrace(out_Renamed);
					else
						Support.SupportMisc.WriteStackTrace(m_throwable, out_Renamed);
				}
				else
				{
					Support.SupportMisc.WriteStackTrace((System.Exception) this, out_Renamed);
				}
			}
		}
	}
}
