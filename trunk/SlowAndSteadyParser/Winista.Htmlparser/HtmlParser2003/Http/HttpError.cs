// ***************************************************************
//  HttpError   version:  1.0   date: 12/15/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Http
{
	/// <summary>
	/// Summary description for HttpError.
	/// </summary>
	public class HttpError : System.Web.HttpException
	{
		private int m_code;

		public HttpError(int code):base("HTTP Error: " + code)
		{
			this.m_code = code;
		}

		public virtual Int32 Code
		{
			get
			{
				return this.m_code;
			}
		}
	}
}
