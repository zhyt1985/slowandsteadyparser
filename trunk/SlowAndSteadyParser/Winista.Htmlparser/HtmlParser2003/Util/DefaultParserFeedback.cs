// ***************************************************************
//  DefaultParserFeedback   version:  1.0   date: 12/18/2005
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
	/// <summary>
	/// Summary description for DefaultParserFeedback.
	/// </summary>
	public class DefaultParserFeedback : IParserFeedBack
	{
		/// <summary> Constructor argument for a quiet feedback.</summary>
		public const int QUIET = 0;
		
		/// <summary> Constructor argument for a normal feedback.</summary>
		public const int NORMAL = 1;
		
		/// <summary> Constructor argument for a debugging feedback.</summary>
		public const int DEBUG = 2;

		/// <summary> Verbosity level.
		/// Corresponds to constructor arguments:
		/// <pre>
		/// DEBUG = 2;
		/// NORMAL = 1;
		/// QUIET = 0;
		/// </pre>
		/// </summary>
		protected internal int mMode;

		/// <summary> Construct a feedback object of the given type.</summary>
		/// <param name="mode">The type of feedback:
		/// <pre>
		/// DEBUG - verbose debugging with stack traces
		/// NORMAL - normal messages
		/// QUIET - no messages
		/// </pre>
		/// </param>
		/// <exception cref="ArgumentException">if mode is not
		/// QUIET, NORMAL or DEBUG.
		/// </exception>
		public DefaultParserFeedback(int mode)
		{
			if (mode < QUIET || mode > DEBUG)
				throw new System.ArgumentException("illegal mode (" + mode + "), must be one of: QUIET, NORMAL, DEBUG");
			mMode = mode;
		}

		/// <summary> Construct a NORMAL feedback object.</summary>
		public DefaultParserFeedback():this(NORMAL)
		{
		}

		/// <summary> Print an info message.</summary>
		/// <param name="message">The message to print.
		/// </param>
		public virtual void Info(System.String message)
		{
			if (QUIET != mMode)
				System.Console.Out.WriteLine("INFO: " + message);
		}
		
		/// <summary> Print an warning message.</summary>
		/// <param name="message">The message to print.
		/// </param>
		public virtual void Warning(System.String message)
		{
			if (QUIET != mMode)
				System.Console.Out.WriteLine("WARNING: " + message);
		}
		
		/// <summary> Print an error message.</summary>
		/// <param name="message">The message to print.
		/// </param>
		/// <param name="exception">The exception for stack tracing.
		/// </param>
		public virtual void Error(System.String message, ParserException exception)
		{
			if (QUIET != mMode)
			{
				System.Console.Out.WriteLine("ERROR: " + message);
				if (DEBUG == mMode && (null != exception))
					exception.PrintStackTrace();
			}
		}
	}
}
