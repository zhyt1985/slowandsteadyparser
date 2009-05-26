// ***************************************************************
//  ParserException   version:  1.0   date: 12/18/2005
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
	/// <summary> Library-specific support for chained exceptions.
	/// 
	/// </summary>
	/// <seealso cref="ChainedException">
	/// 
	/// </seealso>
	[Serializable]
	public class ParserException:ChainedException
	{
		/// <summary>
		/// 
		/// </summary>
		public ParserException()
		{
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ParserException(System.String message):base(message)
		{
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="innerEx"></param>
		public ParserException(System.Exception innerEx):base(innerEx)
		{
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerEx"></param>
		public ParserException(System.String message, System.Exception innerEx):base(message, innerEx)
		{
		}
	}
}
