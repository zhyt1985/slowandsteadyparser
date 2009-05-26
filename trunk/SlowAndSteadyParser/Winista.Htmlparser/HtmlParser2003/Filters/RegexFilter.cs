// ***************************************************************
//  RegexFilter   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using System.Text.RegularExpressions;

namespace Winista.Text.HtmlParser.Filters
{
	/// <summary> This filter accepts all string nodes matching a regular expression.
	/// Because this searches <see cref="IText"></see> nodes. it is
	/// only useful for finding small fragments of text, where it is
	/// unlikely to be broken up by a tag. To find large fragments of text
	/// you should convert the page to plain text and then apply
	/// the regular expression.
	/// <p>
	/// For example, to look for dates use:
	/// <pre>
	/// (19|20)\d\d([- \\/.](0[1-9]|1[012])[- \\/.](0[1-9]|[12][0-9]|3[01]))?
	/// </pre>
	/// as in:
	/// <pre>
	/// Parser parser = new Parser ("http://cbc.ca");
	/// RegexFilter filter = new RegexFilter ("(19|20)\\d\\d([- \\\\/.](0[1-9]|1[012])[- \\\\/.](0[1-9]|[12][0-9]|3[01]))?");
	/// NodeIterator iterator = parser.ExtractAllNodesThatMatch (filter).elements ();
	/// </pre>
	/// which matches a date in yyyy-mm-dd format between 1900-01-01 and 2099-12-31,
	/// with a choice of five separators, either a dash, a space, either kind of
	/// slash or a period.
	/// The year is matched by (19|20)\d\d which uses alternation to allow the
	/// either 19 or 20 as the first two digits. The round brackets are mandatory.
	/// The month is matched by 0[1-9]|1[012], again enclosed by round brackets
	/// to keep the two options together. By using character classes, the first
	/// option matches a number between 01 and 09, and the second
	/// matches 10, 11 or 12.
	/// The last part of the regex consists of three options. The first matches
	/// the numbers 01 through 09, the second 10 through 29, and the third matches 30 or 31.
	/// The day and month are optional, but must occur together because of the ()?
	/// bracketing after the year.
	/// </summary>
	[Serializable]
	public class RegexFilter : INodeFilter
	{
		private bool m_bIgnoreCase = true;

		/// <summary> The regular expression to search for.</summary>
		protected internal System.String mPatternString;
		
		/// <summary> The compiled regular expression to search for.</summary>
		protected internal Regex mPattern;

		/// <summary> The match strategy.</summary>
		/// <seealso cref="RegexFilter(String, int)">
		/// </seealso>
		protected internal int mStrategy;

		/// <summary> Creates a new instance of RegexFilter that accepts string nodes matching
		/// the regular expression ".*" using the FIND strategy.
		/// </summary>
		public RegexFilter():this(".*", true)
		{
		}
		
		/// <summary> Creates a new instance of RegexFilter that accepts string nodes matching
		/// a regular expression using the FIND strategy.
		/// </summary>
		/// <param name="pattern">The pattern to search for.
		/// </param>
		public RegexFilter(System.String pattern):this(pattern, true)
		{
		}

		/// <summary> Creates a new instance of RegexFilter that accepts string nodes matching
		/// a regular expression.
		/// </summary>
		/// <param name="pattern">The pattern to search for.
		/// </param>
		/// <param name="strategy">The type of match:
		/// <ol>
		/// <li>{@link #MATCH} use matches() method: attempts to match
		/// the entire input sequence against the pattern</li>
		/// <li>{@link #LOOKINGAT} use lookingAt() method: attempts to match
		/// the input sequence, starting at the beginning, against the pattern</li>
		/// <li>{@link #FIND} use find() method: scans the input sequence looking
		/// for the next subsequence that matches the pattern</li>
		/// </ol>
		/// </param>
		public RegexFilter(System.String pattern, bool bIgnoreCase)
		{
			Pattern = pattern;
			m_bIgnoreCase = bIgnoreCase;
		}

		/// <summary> Get the search pattern.</summary>
		/// <returns> Returns the pattern.
		/// </returns>
		/// <summary> Set the search pattern.</summary>
		/// <param name="pattern">The pattern to set.
		/// </param>
		virtual public System.String Pattern
		{
			get
			{
				return (mPatternString);
			}
			
			set
			{
				mPatternString = value;
			}
			
		}

		/// <summary>
		/// 
		/// </summary>
		virtual public bool IgnoreCase
		{
			get
			{
				return this.m_bIgnoreCase;
			}
			set
			{
				this.m_bIgnoreCase = value;
			}
		}

		/// <summary> Accept string nodes that match the regular expression.</summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if the regular expression matches the
		/// text of the node, <code>false</code> otherwise.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			System.String string_Renamed;
			bool ret;

			RegexOptions options = RegexOptions.Multiline;
			if (m_bIgnoreCase)
			{
				options |= RegexOptions.IgnoreCase;
			}
			if (mPattern == null)
			{
				mPattern = new Regex(mPatternString, options);
			}
			
			ret = false;
			if (node is IText)
			{
				string_Renamed = ((IText) node).GetText();
	
				ret = mPattern.IsMatch(string_Renamed);
				
			}
			
			return (ret);
		}
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		virtual public System.Object Clone()
		{
			return null;
		}
	}
}
