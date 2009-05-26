// ***************************************************************
//  RobotBlockedException   version:  1.0   Date: 12/19/2005
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
	/// Summary description for RobotBlockedException.
	/// </summary>
	public class RobotBlockedException : ApplicationException
	{
		private System.Uri m_strUrl;

		public RobotBlockedException(System.Uri strUrl)
			:base("Blocked by robots.txt")
		{
			m_strUrl = strUrl;
		}

		public System.Uri Url
		{
			get
			{
				return this.m_strUrl;
			}
		}
	}
}
