// ***************************************************************
//  ResourceGoneException   version:  1.0   Date: 12/19/2005
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
	/// Summary description for ResourceGoneException.
	/// </summary>
	public class ResourceGoneException : System.ApplicationException
	{
		private System.Uri m_Url;

		public ResourceGoneException(System.Uri url)
			:base("Http resource gone")
		{
			m_Url = url;
		}

		public ResourceGoneException(System.Uri url, String strMsg)
			:base(strMsg)
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
