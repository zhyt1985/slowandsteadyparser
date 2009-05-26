// ***************************************************************
//  Tokenizer   version:  1.0   Date: 12/12/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Support
{
	/// <summary>
	/// Summary description for Tokenizer.
	/// </summary>
	public class Tokenizer : System.Collections.IEnumerator
	{
		/// Position over the string
		private Int64 m_iCurrentPos = 0;

		/// Include demiliters in the results.
		private bool m_bIncludeDelims = false;

		/// Char representation of the String to tokenize.
		private char[] m_collChars = null;
			
		//The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character and the form-feed character
		private string m_strDelimiters = " \t\n\r\f";

		/// <summary>
		/// Initializes a new class instance with a specified string to process
		/// </summary>
		/// <param name="source">String to tokenize</param>
		public Tokenizer(System.String source)
		{			
			this.m_collChars = source.ToCharArray();
		}

		/// <summary>
		/// Initializes a new class instance with a specified string to process
		/// and the specified token delimiters to use
		/// </summary>
		/// <param name="source">String to tokenize</param>
		/// <param name="delimiters">String containing the delimiters</param>
		public Tokenizer(System.String source, System.String delimiters)
			: this(source)
		{			
			this.m_strDelimiters = delimiters;
		}


		/// <summary>
		/// Initializes a new class instance with a specified string to process, the specified token 
		/// delimiters to use, and whether the delimiters must be included in the results.
		/// </summary>
		/// <param name="source">String to tokenize</param>
		/// <param name="delimiters">String containing the delimiters</param>
		/// <param name="includeDelims">Determines if delimiters are included in the results.</param>
		public Tokenizer(System.String source, System.String delimiters, bool includeDelims)
			: this(source,delimiters)
		{
			this.m_bIncludeDelims = includeDelims;
		}
	
		/// <summary>
		/// Returns the next token from the token list
		/// </summary>
		/// <returns>The string value of the token</returns>
		public System.String NextToken()
		{				
			return NextToken(this.m_strDelimiters);
		}

		/// <summary>
		/// Returns the next token from the source string, using the provided
		/// token delimiters
		/// </summary>
		/// <param name="delimiters">String containing the delimiters to use</param>
		/// <returns>The string value of the token</returns>
		public System.String NextToken(System.String delimiters)
		{
			//According to documentation, the usage of the received delimiters should be temporary (only for this call).
			//However, it seems it is not true, so the following line is necessary.
			this.m_strDelimiters = delimiters;

			//at the end 
			if (this.m_iCurrentPos == this.m_collChars.Length)
			{
				throw new System.ArgumentOutOfRangeException();
			}
				//if over a delimiter and delimiters must be returned
			else if ((System.Array.IndexOf(delimiters.ToCharArray(),m_collChars[this.m_iCurrentPos]) != -1)
				&& this.m_bIncludeDelims )   
			{
				return "" + this.m_collChars[this.m_iCurrentPos++];
			}
				//need to get the token wo delimiters.
			else
			{
				return NextToken(delimiters.ToCharArray());
			}
		}

		//Returns the nextToken wo delimiters
		private System.String NextToken(char[] delimiters)
		{
			string token="";
			long pos = this.m_iCurrentPos;

			//skip possible delimiters
			while (System.Array.IndexOf(delimiters,this.m_collChars[m_iCurrentPos]) != -1)
				//The last one is a delimiter (i.e there is no more tokens)
				if (++this.m_iCurrentPos == this.m_collChars.Length)
				{
					this.m_iCurrentPos = pos;
					throw new System.ArgumentOutOfRangeException();
				}
			
			//getting the token
			while (System.Array.IndexOf(delimiters,this.m_collChars[this.m_iCurrentPos]) == -1)
			{
				token+=this.m_collChars[this.m_iCurrentPos];
				//the last one is not a delimiter
				if (++this.m_iCurrentPos == this.m_collChars.Length)
				{
					break;
				}
			}
			return token;
		}

				
		/// <summary>
		/// Determines if there are more tokens to return from the source string
		/// </summary>
		/// <returns>True or false, depending if there are more tokens</returns>
		public bool HasMoreTokens()
		{
			//keeping the current pos
			long pos = this.m_iCurrentPos;
			
			try
			{
				this.NextToken();
			}
			catch (System.ArgumentOutOfRangeException)
			{				
				return false;
			}
			finally
			{
				this.m_iCurrentPos = pos;
			}
			return true;
		}

		/// <summary>
		/// Remaining tokens count
		/// </summary>
		public int Count
		{
			get
			{
				//keeping the current pos
				long pos = this.m_iCurrentPos;
				int i = 0;
			
				try
				{
					while (true)
					{
						this.NextToken();
						i++;
					}
				}
				catch (System.ArgumentOutOfRangeException)
				{				
					this.m_iCurrentPos = pos;
					return i;
				}
			}
		}

		/// <summary>
		///  Performs the same action as NextToken.
		/// </summary>
		public System.Object Current
		{
			get
			{
				return (Object) this.NextToken();
			}		
		}		
		
		/// <summary>
		/// Performs the same action as HasMoreTokens.
		/// </summary>
		/// <returns>True or false, depending if there are more tokens</returns>
		public bool MoveNext()
		{
			return this.HasMoreTokens();
		}
		
		/// <summary>
		/// Does nothing.
		/// </summary>
		public void  Reset()
		{
			;
		}
	}
}
