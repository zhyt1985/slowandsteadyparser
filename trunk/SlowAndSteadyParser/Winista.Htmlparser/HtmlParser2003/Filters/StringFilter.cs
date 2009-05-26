// ***************************************************************
//  StringFilter   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Filters
{
	/// <summary> This class accepts all string nodes containing the given string.
	/// This is a fairly simplistic filter, so for more sophisticated
	/// string matching, for example newline and whitespace handling,
	/// use a {@link RegexFilter} instead.
	/// </summary>
	[Serializable]
	public class StringFilter : INodeFilter
	{
		/// <summary> The string to search for.</summary>
		protected internal System.String mPattern;
		
		/// <summary> The string to really search for (converted to uppercase if necessary).</summary>
		protected internal System.String mUpperPattern;
		
		/// <summary> Case sensitive toggle.
		/// If <code>true</code> strings are compared with case sensitivity.
		/// </summary>
		protected internal bool mCaseSensitive;
		
		/// <summary> The locale to use converting to uppercase in case insensitive searches.</summary>
		protected internal System.Globalization.CultureInfo mLocale;

		/// <summary> Creates a new instance of StringFilter that accepts all string nodes.</summary>
		public StringFilter():this("", false)
		{
		}
		
		/// <summary> Creates a StringFilter that accepts text nodes containing a string.
		/// The comparison is case insensitive, with conversions done using
		/// the default <code>Locale</code>.
		/// </summary>
		/// <param name="pattern">The pattern to search for.
		/// </param>
		public StringFilter(System.String pattern):this(pattern, false)
		{
		}
		
		/// <summary> Creates a StringFilter that accepts text nodes containing a string.</summary>
		/// <param name="pattern">The pattern to search for.
		/// </param>
		/// <param name="sensitive">If <code>true</code>, comparisons are performed
		/// respecting case, with conversions done using the default
		/// <code>Locale</code>.
		/// </param>
		public StringFilter(System.String pattern, bool sensitive):this(pattern, sensitive, null)
		{
		}
		
		/// <summary> Creates a StringFilter that accepts text nodes containing a string.</summary>
		/// <param name="pattern">The pattern to search for.
		/// </param>
		/// <param name="sensitive">If <code>true</code>, comparisons are performed
		/// respecting case.
		/// </param>
		/// <param name="locale">The locale to use when converting to uppercase.
		/// If <code>null</code>, the default <code>Locale</code> is used.
		/// </param>
		public StringFilter(System.String pattern, bool sensitive, System.Globalization.CultureInfo locale)
		{
			mPattern = pattern;
			mCaseSensitive = sensitive;
			//UPGRADE_TODO: Method 'java.util.Locale.getDefault' was converted to 'System.Threading.Thread.CurrentThread.CurrentCulture' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073'"
			mLocale = (null == locale)?System.Threading.Thread.CurrentThread.CurrentCulture:locale;
			SetUpperPattern();
		}

		/// <summary> Get the case sensitivity.</summary>
		/// <returns> Returns the case sensitivity.
		/// </returns>
		/// <summary> Set case sensitivity on or off.</summary>
		/// <param name="sensitive">If <code>false</code> searches for the
		/// string are case insensitive.
		/// </param>
		virtual public bool CaseSensitive
		{
			get
			{
				return (mCaseSensitive);
			}
			
			set
			{
				mCaseSensitive = value;
				SetUpperPattern();
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the locale for uppercase conversion.</summary>
		/// <returns> Returns the locale.
		/// </returns>
		/// <summary> Set the locale for uppercase conversion.</summary>
		/// <param name="locale">The locale to set.
		/// </param>
		virtual public System.Globalization.CultureInfo Locale
		{
			get
			{
				return (mLocale);
			}
			
			set
			{
				mLocale = value;
				SetUpperPattern();
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
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
				return (mPattern);
			}
			
			set
			{
				mPattern = value;
				SetUpperPattern();
			}
		}

		//
		// protected methods
		//
		
		/// <summary> Set the real (upper case) comparison string.</summary>
		protected internal virtual void SetUpperPattern()
		{
			if (CaseSensitive)
				mUpperPattern = Pattern;
			else
				mUpperPattern = Pattern.ToUpper(Locale);
		}
		
		//
		// NodeFilter interface
		//
		
		/// <summary> Accept string nodes that contain the string.</summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if <code>node</code> is a {@link Text} node
		/// and contains the pattern string, <code>false</code> otherwise.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			System.String string_Renamed;
			bool ret;
			
			ret = false;
			if (node is IText)
			{
				string_Renamed = ((IText) node).GetText();
				if (!CaseSensitive)
					string_Renamed = string_Renamed.ToUpper(Locale);
				ret = (- 1 != string_Renamed.IndexOf(mUpperPattern));
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
