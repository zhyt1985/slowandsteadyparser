// ***************************************************************
//  IParseFeedBack   version:  1.0   date: 12/18/2005
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
	/// <summary> Interface for providing feedback without forcing the output
	/// destination to be predefined. A default implementation is
	/// provided to output events to the console but alternate
	/// implementations that log, watch for specific messages, etc.
	/// are also possible.
	/// 
	/// </summary>
	/// <seealso cref="DefaultParserFeedback">
	/// </seealso>
	/// <seealso cref="FeedbackManager">
	/// 
	/// </seealso>
	public interface IParserFeedBack
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		void  Info(System.String message);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		void  Warning(System.String message);
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="e"></param>
		void  Error(System.String message, ParserException e);
	}
}
