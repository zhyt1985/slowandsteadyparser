// ***************************************************************
//  RetryLaterException   version:  1.0   Date: 12/19/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Http
{
	/// <summary>
	/// Summary description for RetryLaterException.
	/// </summary>
	public class RetryLaterException : ApplicationException
	{
		private System.Uri m_Url;

		public RetryLaterException(System.Uri url)
			:base("Exceeded http.max.delays: retry later.")
		{
			m_Url = url;
		}

		public System.Uri Url
		{
			get
			{
				return this.m_Url;
			}
		}
	}
}
