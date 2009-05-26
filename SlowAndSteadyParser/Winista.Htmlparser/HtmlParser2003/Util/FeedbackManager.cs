// ***************************************************************
//  FeedbackManager   version:  1.0   Date: 12/20/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary> Implementaiton of static methods that allow the parser to
	/// route various messages to any implementation of the
	/// HTMLParserFeedback interface. End users can use the default
	/// DefaultHTMLParserFeedback or may provide their own by calling
	/// the setParserFeedback method.
	/// 
	/// </summary>
	/// <seealso cref="IParserFeedBack">
	/// </seealso>
	/// <seealso cref="DefaultParserFeedback">
	/// 
	/// </seealso>
	public class FeedbackManager
	{
		/// <summary>
		/// 
		/// </summary>
		protected internal static IParserFeedBack g_Callback = new DefaultParserFeedback();

		/// <summary>
		/// 
		/// </summary>
		public static IParserFeedBack ParserFeedback
		{
			set
			{
				g_Callback = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public static void Info(System.String message)
		{
			g_Callback.Info(message);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public static void Warning(System.String message)
		{
			g_Callback.Warning(message);
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		public static void Error(System.String message, ParserException e)
		{
			g_Callback.Error(message, e);
		}
	}
}
