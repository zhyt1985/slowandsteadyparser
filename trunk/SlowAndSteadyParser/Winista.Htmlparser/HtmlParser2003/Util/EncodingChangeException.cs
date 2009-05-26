// ***************************************************************
//  EncodingChangeException   version:  1.0   date: 12/18/2005
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
	/// <summary> The encoding is changed invalidating already scanned characters.
	/// When the encoding is changed, as for example when encountering a &lt;META&gt;
	/// tag that includes a charset directive in the content attribute that
	/// disagrees with the encoding specified by the HTTP header (or the default
	/// encoding if none), the parser retraces the bytes it has interpreted so far
	/// comparing the characters produced under the new encoding. If the new
	/// characters differ from those it has already yielded to the application, it
	/// throws this exception to indicate that processing should be restarted under
	/// the new encoding.
	/// This exception is the object thrown so that applications may distinguish
	/// between an encoding change, which may be successfully cured by restarting
	/// the parse from the beginning, from more serious errors.
	/// </summary>
	/// <seealso cref="IteratorImpl">
	/// </seealso>
	/// <seealso cref="ParserException">
	/// 
	/// </seealso>
	[Serializable]
	public class EncodingChangeException:ParserException
	{
		/// <summary> Create an exception idicative of a problematic encoding change.</summary>
		/// <param name="message">The message describing the error condifion.
		/// </param>
		public EncodingChangeException(System.String message):base(message)
		{
		}
	}
}
