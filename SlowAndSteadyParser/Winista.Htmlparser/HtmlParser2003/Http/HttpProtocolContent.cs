// ***************************************************************
//  HttpProtocolContent   version:  1.0   Date: 12/20/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Http
{
	/// <summary>
	/// Summary description for HttpProtocolContent.
	/// </summary>
	public class HttpProtocolContent
	{
		#region Class Members
		private byte[] m_Data;
		private ContentProperties m_Properties;
		#endregion

		#region Class Constructor
		public HttpProtocolContent(byte[] data, ContentProperties props)
		{
			m_Data = data;
			m_Properties = props;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets data buffer associated with content.
		/// </summary>
		public byte[] ContentData
		{
			get
			{
				return this.m_Data;
			}
		}

		/// <summary>
		/// Gets headers or any other key-value pairs associated with the content.
		/// </summary>
		public ContentProperties ContentProperties
		{
			get
			{
				return this.m_Properties;
			}
		}
		#endregion
	}
}
