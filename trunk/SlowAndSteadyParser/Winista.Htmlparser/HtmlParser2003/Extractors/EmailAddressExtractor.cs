// ***************************************************************
//  EmailAddressExtractor   version:  1.0   Date: 12/19/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Text.RegularExpressions;

using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Tags;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Extractors
{
	/// <summary>
	/// This class is a utility that helps in extracting all email
	/// addresses contained in the page.
	/// </summary>
	public class EmailAddressExtractor
	{
		#region Class Members
		private Parser m_obParser;
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obParser"></param>
		public EmailAddressExtractor(Parser obParser)
		{
			if (null == obParser)
			{
				throw new ArgumentNullException("Parser");
			}
			m_obParser = obParser;
		}

		public System.Collections.Specialized.StringCollection Extract()
		{
			String strPattern = "([A-Za-z0-9](([_\\.\\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\\.\\-]?[a-zA-Z0-9]+)*)\\.([A-Za-z]{2,}))";
			System.Collections.Specialized.StringCollection collAddresses = new System.Collections.Specialized.StringCollection();
			INodeFilter filter = new RegexFilter(strPattern, true);
			NodeList nodes = m_obParser.Parse(filter);
			if (null != nodes &&
				0 != nodes.Size())
			{
				RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Multiline;
				Regex obRegex = new Regex(strPattern, options);
				for(Int32 i = 0; i < nodes.Size(); i++)
				{
					INode obNode = nodes.ElementAt(i);
					String strText = obNode.GetText();

					Match m = obRegex.Match(strText);
					while(m.Success)
					{
						collAddresses.Add(m.Groups[0].Value);
						// Advance to the next match.
						m = m.NextMatch();
					}
				}
			}

			return collAddresses;
		}
	}
}
