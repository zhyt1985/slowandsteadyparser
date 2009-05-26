// ***************************************************************
//  LinkRegexFilter   version:  1.0   Date: 12/19/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Text.RegularExpressions;

using Winista.Text.HtmlParser.Tags;

namespace Winista.Text.HtmlParser.Filters
{
	/// <summary> This class accepts tags of class ATag that contain a link matching a given
	/// regex pattern. Use this filter to extract ATag nodes with URLs that match
	/// the desired regex pattern.
	/// </summary>
	[Serializable]
	public class LinkRegexFilter : INodeFilter
	{
		#region Class Members
		private String m_strRegexPattern = ".*";
		private bool m_bCaseSensitive = false;
		private Regex m_obRegex;
		#endregion

		/// <summary> Creates a LinkRegexFilter that accepts ATag nodes containing
		/// a URL that matches the supplied regex pattern.
		/// The match is case insensitive.
		/// </summary>
		/// <param name="regexPattern">The pattern to match.
		/// </param>
		public LinkRegexFilter(System.String regexPattern):this(regexPattern, true)
		{
		}

		/// <summary> Creates a LinkRegexFilter that accepts ATag nodes containing
		/// a URL that matches the supplied regex pattern.
		/// </summary>
		/// <param name="regexPattern">The regex pattern to match.
		/// </param>
		/// <param name="caseSensitive">Specifies case sensitivity for the matching process.
		/// </param>
		public LinkRegexFilter(System.String regexPattern, bool caseSensitive)
		{
			m_strRegexPattern = regexPattern;
			m_bCaseSensitive = caseSensitive;
		}

		/// <summary> Accept nodes that are a ATag and have a URL
		/// that matches the regex pattern supplied in the constructor.
		/// </summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if the node is a link with the pattern.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			bool ret;
			ret = false;
			if (typeof(ATag).IsAssignableFrom(node.GetType()))
			{
				System.String link = ((ATag) node).Link;
				CreateMatcher();
				return m_obRegex.IsMatch(link);
			}
			
			return (ret);
		}

		virtual public System.Object Clone()
		{
			return null;
		}

		#region Helper Methods
		private void CreateMatcher()
		{
			if (null == m_obRegex)
			{
				System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.Singleline;
				if (!m_bCaseSensitive)
				{
					options |= System.Text.RegularExpressions.RegexOptions.IgnoreCase;
				}

				m_obRegex = new Regex(m_strRegexPattern, options);
			}
		}
		#endregion
	}
}
