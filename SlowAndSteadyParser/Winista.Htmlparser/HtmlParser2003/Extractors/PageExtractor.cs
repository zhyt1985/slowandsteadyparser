// ***************************************************************
//  PageExtractor   version:  1.0   Date: 12/30/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Visitors;
using Winista.Text.HtmlParser.Data;
using Winista.Text.HtmlParser.Tags;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Http;

namespace Winista.Text.HtmlParser.Extractors
{
	/// <summary>
	/// Summary description for PageExtractor.
	/// </summary>
	public class PageExtractor : NodeVisitor
	{
		#region Class Members
		private Parser m_obParser;
		private StringExtractor m_obStringExtractor;
		private PageData m_obPageData;
		#endregion
		/// <summary>
		/// Creates new instance of <see cref="PageExtractor"></see> object.
		/// </summary>
		public PageExtractor(String strUrl) : base(true, true)
		{
			m_obParser = new Parser(new System.Uri(strUrl));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obParser"></param>
		public PageExtractor(Parser obParser) : base(true, true)
		{
			if (null == obParser)
			{
				throw new ArgumentNullException("obParser");
			}
			m_obParser = obParser;
		}

		#region Public Properties
		/// <summary>
		/// 
		/// </summary>
		public PageData PageData
		{
			get
			{
				return this.m_obPageData;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public PageData GetPageData()
		{
			if (null == m_obParser)
			{
				throw new ApplicationException("Parser has not been initialized");
			}
			m_obPageData = null;
			m_obPageData = new PageData();
			m_obPageData.m_ContentEncoding = m_obParser.Lexer.Page.Source.CurrentEncoding;
			if (this.m_obParser.Connection != null)
			{
				m_obPageData.m_Cookies = this.m_obParser.Connection.m_ProtocolOutput.Cookies;
				m_obPageData.m_ProtocolVersion = this.m_obParser.Connection.m_ProtocolOutput.ProtocolVersion;
			}

			m_obParser.VisitAllNodesWith(this);

			m_obStringExtractor = new StringExtractor(m_obParser);
			m_obPageData.m_strTextContent = m_obStringExtractor.GetStrings();
			return m_obPageData;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		public override void VisitTag(ITag tag)
		{
			System.Text.StringBuilder strBuffer = new System.Text.StringBuilder();
			if (tag is TitleTag)
			{
				m_obPageData.HeadData.m_Tag = tag;
				StringUtil.CollapseString(strBuffer, tag.ToPlainTextString());
				m_obPageData.HeadData.m_strTitle = strBuffer.ToString();
			}
			else if (tag is MetaTag)
			{
				ProcessMetaTag(tag as MetaTag);
			}
			else if (tag is BaseHrefTag)
			{
				m_obPageData.m_strBaseUrl = tag.ToPlainTextString();
			}
			else if (tag is ATag)
			{
				ProcessLinkTag(tag as ATag);
			}
			else if (tag is ImageTag)
			{
				ProcessImageTag(tag as ImageTag);
			}
			else if (tag is ScriptTag)
			{
				// TODO: Parse script to get more data out of it.
			}
			else if (tag is TableTag)
			{
				ProcessTableTag(tag as TableTag);
			}
		}

		#endregion

		#region Helper Methods
		private void ProcessMetaTag(MetaTag obTag)
		{
			System.Text.StringBuilder strBuffer = new System.Text.StringBuilder();
			String  strContent = obTag.Attributes["CONTENT"] as String;
			String strName = obTag.Attributes["NAME"] as String;
			String strHttpEquiv = obTag.Attributes["HTTP-EQUIV"] as String;
			m_obPageData.HeadData.MetaTags.Add(new MetaTagData(strName, strContent));

			if (null != strContent)
			{
				StringUtil.CollapseString(strBuffer, strContent);
			}
		
			Int32 idx = 0;

			if (strName != null && String.Empty != strName)
			{
				strName = strName.ToUpper();

				switch(strName)
				{
					case "DESCRIPTION":
						m_obPageData.HeadData.m_strDescription = strBuffer.ToString();
						break;
					case "KEYWORDS":
						m_obPageData.HeadData.m_collKeywords = strBuffer.ToString().Split(',');
						break;
					case "ROBOTS":
						if (null != strContent && String.Empty != strContent)
						{
							String strTemp = strContent.ToUpper();
							idx = strTemp.IndexOf("NONE");
							if (idx >= 0)
							{
								m_obPageData.HeadData.m_bNoIndex = true;
								m_obPageData.HeadData.m_bNoFollow = true;
							}
							
							idx = strTemp.IndexOf("ALL");
							if (idx >= 0)
							{
								// Pass through..
							}

							idx = strTemp.IndexOf("NOINDEX");
							if (idx >= 0)
							{
								m_obPageData.HeadData.m_bNoIndex = true;
							}

							idx = strTemp.IndexOf("NOFOLLOW");
							if (idx >= 0)
							{
								m_obPageData.HeadData.m_bNoFollow = true;
							}
						}
						break;
				}
			}
			else if (strHttpEquiv != null && String.Empty != strHttpEquiv)
			{
				idx = 0;
				String strTemp = strContent.ToUpper();
				strHttpEquiv = strHttpEquiv.ToUpper();
				switch(strHttpEquiv)
				{
					case "PRAGMA":
						idx = strTemp.IndexOf("NO-CACHE");
						if (idx >= 0)
						{
							m_obPageData.HeadData.m_bNoCache = true;
						}
						break;
					case "REFRESH":
						String strTime = String.Empty;
						String strRefreshUrl = String.Empty;
						idx = strTemp.IndexOf(";");
						if (idx == -1)
						{
							strTime = strTemp;
						}
						else
						{
							strRefreshUrl = strTemp.Substring(idx+1);
							strTime = strTemp.Substring(0, idx);
							idx = strTemp.IndexOf("URL=");
							if (idx != -1)
							{
								idx += 4;
								strRefreshUrl = strTemp.Substring(idx);
							}
							m_obPageData.HeadData.m_strRefreshUrl = strRefreshUrl;
						}
						try
						{
							m_obPageData.HeadData.m_iRefreshTime = Int32.Parse(strTime);
							m_obPageData.HeadData.m_bRefresh = true;
						}
						catch
						{
							m_obPageData.HeadData.m_iRefreshTime = -1;
							m_obPageData.HeadData.m_bRefresh = true;
						}
						break;
					case "EXPIRES":
						try
						{
							m_obPageData.HeadData.m_ContentExpirationDate = DateTime.Parse(strTemp);
						}
						catch
						{
							// Ignore exception....
						}
						break;
				}
			}
		}

		private void ProcessLinkTag(ATag obTag)
		{
			LinkData obLinkData = new LinkData(this.m_obPageData, obTag);
			m_obPageData.m_Outlinks.Add(obLinkData);
		}

		private void ProcessImageTag(ImageTag obTag)
		{
			ImageData obImageData = new ImageData(this.m_obPageData, obTag);
			m_obPageData.m_ImageLinks.Add(obImageData);
		}

		private void ProcessScriptTag(ScriptTag obTag)
		{

		}

		private void ProcessTableTag(TableTag obTag)
		{
			TableData obData = new TableData(obTag);
			this.m_obPageData.m_Tables.Add(obData);
		}
		#endregion
	}
}
