// ***************************************************************
//  StringUtil   version:  1.0   date: 12/30/2005
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
	/// Summary description for StringUtil.
	/// </summary>
	public class StringUtil
	{
		/// <summary> A newline.</summary>
		internal static readonly System.String NEWLINE = System.Environment.NewLine;
		
		/// <summary> The length of the NEWLINE.</summary>
		internal static readonly int NEWLINE_SIZE = NEWLINE.Length;

		private StringUtil()
		{
		}

		/// <summary> Add the given text collapsing whitespace.
		/// Use a little finite state machine:
		/// <pre>
		/// state 0: whitepace was last emitted character
		/// state 1: in whitespace
		/// state 2: in word
		/// A whitespace character moves us to state 1 and any other character
		/// moves us to state 2, except that state 0 stays in state 0 until
		/// a non-whitespace and going from whitespace to word we emit a space
		/// before the character:
		/// input:     whitespace   other-character
		/// state\next
		/// 0               0             2
		/// 1               1        space then 2
		/// 2               1             2
		/// </pre>
		/// </summary>
		/// <param name="buffer">The buffer to append to.
		/// </param>
		/// <param name="string">The string to append.
		/// </param>
		internal static void CollapseString(System.Text.StringBuilder buffer, System.String strIn)
		{
			Int32 chars;
			Int32 length;
			Int32 state;
			char character;
			
			chars = strIn.Length;
			if (0 != chars)
			{
				length = buffer.Length;
				state = ((0 == length) || (buffer[length - 1] == ' ') || ((NEWLINE_SIZE <= length) && buffer.ToString(length - NEWLINE_SIZE, NEWLINE_SIZE).Equals(NEWLINE)))?0:1;
				for (int i = 0; i < chars; i++)
				{
					character = strIn[i];
					switch (character)
					{
						// see HTML specification section 9.1 White space
						// http://www.w3.org/TR/html4/struct/text.html#h-9.1
						case '\u0020': 
						case '\u0009': 
						case '\u000C': 
						case '\u200B': 
						case '\r': 
						case '\n': 
							if (0 != state)
							{
								state = 1;
							}
							break;
						
						default: 
							if (1 == state)
							{
								buffer.Append(' ');
							}
							state = 2;
							buffer.Append(character);
							break;
						
					}
				}
			}
		}
	}
}
