// ***************************************************************
//  ImageLinkExtractor   version:  1.0   Date: 12/19/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Data;
using Winista.Text.HtmlParser.Tags;

namespace Winista.Text.HtmlParser.Extractors
{
	/// <summary>
	/// Summary description for ImageLinkExtractor.
	/// </summary>
	public class ImageLinkExtractor : AbstractExtractor
	{
		#region Class Members
		internal String m_strUrl = String.Empty;
		internal ImageDataCollection m_Links = null;
		internal Int32 m_iLevel = 0;
		#endregion

		/// <summary>
		/// 
		/// </summary>
		public ImageLinkExtractor(String strUrl)
		{
			if (null == strUrl ||
				String.Empty == strUrl)
			{
				throw new ArgumentException("No URL specified");
			}
			m_strUrl = strUrl;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strUrl"></param>
		/// <param name="iLevel"></param>
		public ImageLinkExtractor(String strUrl, Int32 iLevel)
			:this(strUrl)
		{
			this.m_iLevel = iLevel;
		}

		/// <summary>
		/// 
		/// </summary>
		public ImageDataCollection Links
		{
			get
			{
				return this.m_Links;
			}
		}

		public LinkStatus ExtractLinks()
		{
			if (String.Empty == m_strUrl)
			{
				throw new ArgumentException("No URL specified");
			}

			m_Links = new ImageDataCollection();
			CreateParser();
			if (m_obParser.Lexer.Page.mSource == null)
			{
				return LinkStatus.Broken;
			}

			INodeFilter obFilter = new NodeClassFilter(typeof(ImageTag));
			NodeList collNodes = m_obParser.Parse(obFilter);
			if (null != collNodes)
			{
				PageData obPageData = new PageData();
				obPageData.m_strUrl = m_obParser.URL;
				obPageData.m_iDepth = m_iLevel;
				for(Int32 i= 0; i < collNodes.Count; i++)
				{
					INode obNode = collNodes[i];
					ImageData obLinkData = new ImageData(obPageData, obNode as ImageTag);
					m_Links.Add(obLinkData);
				}
			}
			return LinkStatus.Ok;
		}

		#region Helper Methods
		private void CreateParser()
		{
			if (null == m_obParser)
			{
				m_obParser = new Parser(new System.Uri(m_strUrl));
			}
		}
		#endregion
	}
}
