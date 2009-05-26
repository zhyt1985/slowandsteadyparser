// ***************************************************************
//  StringExtractor   version:  1.0   Date: 12/19/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Visitors;
using Winista.Text.HtmlParser.Tags;
using Winista.Text.HtmlParser.Http;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Lex;

namespace Winista.Text.HtmlParser.Extractors
{
	/// <summary> Extract strings from a URL.
	/// <p>Text within &lt;SCRIPT&gt;&lt;/SCRIPT&gt; tags is removed.</p>
	/// <p>The text within &lt;PRE&gt;&lt;/PRE&gt; tags is not altered.</p>
	/// <p>The property <code>Strings</code>, which is the output property is null
	/// until a URL is set. So a typical usage is:</p>
	/// <pre>
	/// StringExtractor sb = new StringExtractor ();
	/// sb.SetLinks (false);
	/// sb.SetReplaceNonBreakingSpaces (true);
	/// sb.SetCollapse (true);
	/// sb.setURL ("http://www.apache.org"); // the HTTP is performed here
	/// String s = sb.GetStrings ();
	/// </pre>
	/// You can also use the StringExtractor as a NodeVisitor on your own parser,
	/// in which case you have to refetch your page if you change one of the
	/// properties because it resets the Strings property:</p>
	/// <pre>
	/// StringExtractor sb = new StringExtractor ();
	/// Parser parser = new Parser ("http://xyz.com");
	/// parser.VisitAllNodesWith (sb);
	/// String s = sb.GetStrings ();
	/// sb.SetLinks (true);
	/// parser.Reset ();
	/// parser.VisitAllNodesWith (sb);
	/// String sl = sb.GetStrings ();
	/// </pre>
	/// According to Nick Burch, who contributed the patch, this is handy if you
	/// don't want StringExtractor to wander off and get the content itself, either
	/// because you already have it, it's not on a website etc.
	/// </summary>
	public class StringExtractor : NodeVisitor
	{
		/// <summary> Property name in event where the URL contents changes.</summary>
		public const System.String PROP_STRINGS_PROPERTY = "strings";
		
		/// <summary> Property name in event where the 'embed links' state changes.</summary>
		public const System.String PROP_LINKS_PROPERTY = "links";
		
		/// <summary> Property name in event where the URL changes.</summary>
		public const System.String PROP_URL_PROPERTY = "URL";
		
		/// <summary> Property name in event where the 'replace non-breaking spaces'
		/// state changes.
		/// </summary>
		public const System.String PROP_REPLACE_SPACE_PROPERTY = "replaceNonBreakingSpaces";
		
		/// <summary> Property name in event where the 'collapse whitespace' state changes.</summary>
		public const System.String PROP_COLLAPSE_PROPERTY = "collapse";
		
		/// <summary> Property name in event where the connection changes.</summary>
		public const System.String PROP_CONNECTION_PROPERTY = "connection";
		
		/// <summary> The parser used to extract strings.</summary>
		protected internal Parser mParser;
		
		/// <summary> The strings extracted from the URL.</summary>
		protected internal System.String mStrings;
		
		/// <summary> If <code>true</code> the link URLs are embedded in the text output.</summary>
		protected internal bool mLinks;
		
		/// <summary> If <code>true</code> regular space characters are substituted for
		/// non-breaking spaces in the text output.
		/// </summary>
		protected internal bool mReplaceSpace;
		
		/// <summary> If <code>true</code> sequences of whitespace characters are replaced
		/// with a single space character.
		/// </summary>
		protected internal bool mCollapse;
		
		/// <summary> The buffer text is stored in while traversing the HTML.</summary>
		protected internal System.Text.StringBuilder mBuffer;
		
		/// <summary> Set <code>true</code> when traversing a SCRIPT tag.</summary>
		protected internal bool mIsScript;
		
		/// <summary> Set <code>true</code> when traversing a PRE tag.</summary>
		protected internal bool mIsPre;
		
		/// <summary> Set <code>true</code> when traversing a STYLE tag.</summary>
		protected internal bool mIsStyle;

		/// <summary> Create a StringExtractor object.
		/// Default property values are set to 'do the right thing':
		/// <p><code>Links</code> is set <code>false</code> so text appears like a
		/// browser would display it, albeit without the colour or underline clues
		/// normally associated with a link.</p>
		/// <p><code>ReplaceNonBreakingSpaces</code> is set <code>true</code>, so
		/// that printing the text works, but the extra information regarding these
		/// formatting marks is available if you set it false.</p>
		/// <p><code>Collapse</code> is set <code>true</code>, so text appears
		/// compact like a browser would display it.</p>
		/// </summary>
		/// <param name="strUrl">Page URL</param>
		public StringExtractor(String strUrl):base(true, true)
		{
			mParser = new Parser(new System.Uri(strUrl));
			mStrings = null;
			mLinks = false;
			mReplaceSpace = true;
			mCollapse = true;
			mBuffer = new System.Text.StringBuilder(4096);
			mIsScript = false;
			mIsPre = false;
			mIsStyle = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obParser"></param>
		internal StringExtractor(Parser obParser)
			:base(true, true)
		{
			if (null == obParser)
			{
				throw new ArgumentNullException("obParser", "NULL Parser object specified");
			}
			mParser = obParser;
			obParser.Reset();
			mStrings = null;
			mLinks = false;
			mReplaceSpace = true;
			mCollapse = true;
			mBuffer = new System.Text.StringBuilder(4096);
			mIsScript = false;
			mIsPre = false;
			mIsStyle = false;
		}

		/// <summary> Get the current 'include links' state.</summary>
		/// <returns> <code>true</code> if link text is included in the text extracted
		/// from the URL, <code>false</code> otherwise.
		/// </returns>
		/// <summary> Set the 'include links' state.
		/// If the setting is changed after the URL has been set, the text from the
		/// URL will be reacquired, which is possibly expensive.
		/// </summary>
		/// <remarks>Use <code>true</code> if link text is to be included in the
		/// text extracted from the URL, <code>false</code> otherwise.
		/// </remarks>
		virtual public bool Links
		{
			get
			{
				return (mLinks);
			}
			
			set
			{
				bool oldValue = mLinks;
				if (oldValue != value)
				{
					mLinks = value;
					ResetStrings();
				}
			}
			
		}

		/// <summary> Get the current URL.</summary>
		/// <returns> The URL from which text has been extracted, or <code>null</code>
		/// if this property has not been set yet.
		/// </returns>
		/// <summary> Set the URL to extract strings from.
		/// The text from the URL will be fetched, which may be expensive, so this
		/// property should be set last.
		/// </summary>
		virtual public System.String URL
		{
			get
			{
				return ((null != mParser)?mParser.URL:null);
			}
			
			set
			{
				System.String old;
				old = URL;
				HttpProtocol conn = Connection;
				if (((null == old) && (null != value)) || ((null != old) && !old.Equals(value)))
				{
					try
					{
						if (null == mParser)
							mParser = new Parser(new System.Uri(value));
						else
							mParser.URL = value;
						SetStrings();
					}
					catch (ParserException pe)
					{
						UpdateStrings(pe.ToString());
					}
				}
			}
			
		}

		/// <summary> Get the current 'replace non breaking spaces' state.</summary>
		/// <returns> <code>true</code> if non-breaking spaces (character '&#92;u00a0',
		/// numeric character reference &amp;#160; or character entity
		/// reference &amp;nbsp;) are to be replaced with normal
		/// spaces (character '&#92;u0020').
		/// </returns>
		/// <summary> Set the 'replace non breaking spaces' state.
		/// If the setting is changed after the URL has been set, the text from the
		/// URL will be reacquired, which is possibly expensive.
		/// </summary>
		/// <remarks><code>true</code> if non-breaking spaces
		/// (character '&#92;u00a0', numeric character reference &amp;#160;
		/// or character entity reference &amp;nbsp;) are to be replaced with normal
		/// spaces (character '&#92;u0020').
		/// </remarks>
		virtual public bool ReplaceNonBreakingSpaces
		{
			get
			{
				return (mReplaceSpace);
			}
			
			set
			{
				bool oldValue = mReplaceSpace;
				if (oldValue != value)
				{
					mReplaceSpace = value;
					ResetStrings();
				}
			}
		}

		/// <summary> Get the current 'collapse whitespace' state.
		/// If set to <code>true</code> this emulates the operation of browsers
		/// in interpretting text where <quote>user agents should collapse input
		/// white space sequences when producing output inter-word space</quote>.
		/// See HTML specification section 9.1 White space
		/// <a href="http://www.w3.org/TR/html4/struct/text.html#h-9.1">
		/// http://www.w3.org/TR/html4/struct/text.html#h-9.1</a>.
		/// </summary>
		/// <returns> <code>true</code> if sequences of whitespace (space '&#92;u0020',
		/// tab '&#92;u0009', form feed '&#92;u000C', zero-width space '&#92;u200B',
		/// carriage-return '\r' and NEWLINE '\n') are to be replaced with a single
		/// space.
		/// </returns>
		/// <summary> Set the current 'collapse whitespace' state.
		/// If the setting is changed after the URL has been set, the text from the
		/// URL will be reacquired, which is possibly expensive.
		/// </summary>
		/// <remarks>If <code>true</code>, sequences of whitespace
		/// will be reduced to a single space.
		/// </remarks>
		virtual public bool Collapse
		{
			get
			{
				return (mCollapse);
			}
			
			set
			{
				bool oldValue = mCollapse;
				if (oldValue != value)
				{
					mCollapse = value;
					ResetStrings();
				}
			}
			
		}

		/// <summary> Get the current connection.</summary>
		/// <returns> The connection that the parser has or <code>null</code> if it
		/// hasn't been set or the parser hasn't been constructed yet.
		/// </returns>
		/// <summary> Set the parser's connection.
		/// The text from the URL will be fetched, which may be expensive, so this
		/// property should be set last.
		/// </summary>
		/// <param name="connection">New value of property Connection.
		/// </param>
		virtual public HttpProtocol Connection
		{
			get
			{
				return ((null != mParser)?mParser.Connection:null);
			}
			
			set
			{
				System.String url;
				HttpProtocol conn;
				
				url = URL;
				conn = Connection;
				if (((null == conn) && (null != value)) || ((null != conn) && !conn.Equals(value)))
				{
					try
					{
						if (null == mParser)
						{
							mParser = new Parser(value);
						}
						else
						{
							mParser.Connection = value;
						}
						SetStrings();
					}
					catch (ParserException pe)
					{
						UpdateStrings(pe.ToString());
					}
				}
			}
			
		}

		/// <summary> Appends a newline to the buffer if there isn't one there already.
		/// Except if the buffer is empty.
		/// </summary>
		protected internal virtual void CarriageReturn()
		{
			int length;
			
			length = mBuffer.Length;
			if ((0 != length) && ((StringUtil.NEWLINE_SIZE <= length) && (!mBuffer.ToString(length - StringUtil.NEWLINE_SIZE, StringUtil.NEWLINE_SIZE).Equals(StringUtil.NEWLINE))))
			{
				mBuffer.Append(StringUtil.NEWLINE);
			}
		}

		/// <summary> Extract the text from a page.</summary>
		/// <returns> The textual contents of the page.
		/// </returns>
		/// <exception cref=""> ParserException If a parse error occurs.
		/// </exception>
		protected internal virtual System.String ExtractStrings()
		{
			System.String ret;
			
			mParser.VisitAllNodesWith(this);
			ret = mBuffer.ToString();
			mBuffer = new System.Text.StringBuilder(4096);
			
			return (ret);
		}

		/// <summary> Assign the <code>Strings</code> property, firing the property change.</summary>
		/// <param name="strings">The new value of the <code>Strings</code> property.
		/// </param>
		protected internal virtual void  UpdateStrings(System.String strings)
		{
			System.String oldValue;
			
			if ((null == mStrings) || !mStrings.Equals(strings))
			{
				oldValue = mStrings;
				mStrings = strings;
			}
		}

		/// <summary> Appends the text to the output.</summary>
		/// <param name="string">The text node.
		/// </param>
		public override void VisitStringNode(IText string_Renamed)
		{
			if (!mIsScript && !mIsStyle)
			{
				System.String text = string_Renamed.GetText();
				if (!mIsPre)
				{
					text = Translate.Decode(text);
					if (ReplaceNonBreakingSpaces)
						text = text.Replace('\u00a0', ' ');
					if (Collapse)
						StringUtil.CollapseString(mBuffer, text);
					else
						mBuffer.Append(text);
				}
				else
					mBuffer.Append(text);
			}
		}

		/// <summary> Appends a NEWLINE to the output if the tag breaks flow, and
		/// possibly sets the state of the PRE and SCRIPT flags.
		/// </summary>
		/// <param name="tag">The tag to examine.
		/// </param>
		public override void VisitTag(ITag tag)
		{
			System.String name;
			
			if (tag is ATag)
				if (Links)
				{
					// appends the link as text between angle brackets to the output.
					mBuffer.Append("<");
					mBuffer.Append(((ATag) tag).Link);
					mBuffer.Append(">");
				}
			name = tag.TagName;
			if (name.ToUpper().Equals("PRE".ToUpper()))
				mIsPre = true;
			else if (name.ToUpper().Equals("SCRIPT".ToUpper()))
				mIsScript = true;
			else if (name.ToUpper().Equals("STYLE".ToUpper()))
				mIsStyle = true;
			if (tag.BreaksFlow())
				CarriageReturn();
		}

		/// <summary> Resets the state of the PRE and SCRIPT flags.</summary>
		/// <param name="tag">The end tag to process.
		/// </param>
		public override void VisitEndTag(ITag tag)
		{
			System.String name;
			
			name = tag.TagName;
			if (name.ToUpper().Equals("PRE".ToUpper()))
				mIsPre = false;
			else if (name.ToUpper().Equals("SCRIPT".ToUpper()))
				mIsScript = false;
			else if (name.ToUpper().Equals("STYLE".ToUpper()))
				mIsStyle = false;
		}

		/// <summary> Return the textual contents of the URL.
		/// This is the primary output of the object.
		/// </summary>
		/// <returns> The user visible (what would be seen in a browser) text.
		/// </returns>
		public virtual System.String GetStrings()
		{
			if (null == mStrings)
			{
				if (0 == mBuffer.Length)
				{
					SetStrings();
				}
				else
				{
					UpdateStrings(mBuffer.ToString());
				}
			}
			
			return (mStrings);
		}

		/// <summary> Fetch the URL contents.
		/// Only do work if there is a valid parser with it's URL set.
		/// </summary>
		protected internal virtual void  SetStrings()
		{
			if (null != URL)
			{
				try
				{
					try
					{
						mParser.VisitAllNodesWith(this);
						UpdateStrings(mBuffer.ToString());
					}
					finally
					{
						mBuffer = new System.Text.StringBuilder(4096);
					}
				}
				catch (EncodingChangeException ece)
				{
					mIsPre = false;
					mIsScript = false;
					mIsStyle = false;
					try
					{
						// try again with the encoding now in force
						mParser.Reset();
						mBuffer = new System.Text.StringBuilder(4096);
						mParser.VisitAllNodesWith(this);
						UpdateStrings(mBuffer.ToString());
					}
					catch (ParserException pe)
					{
						UpdateStrings(pe.ToString());
					}
					finally
					{
						mBuffer = new System.Text.StringBuilder(4096);
					}
				}
				catch (ParserException pe)
				{
					UpdateStrings(pe.ToString());
				}
			}
			else
			{
				// reset in case this StringBean is used as a visitor
				// on another parser, not it's own
				mStrings = null;
				mBuffer = new System.Text.StringBuilder(4096);
			}
		}

		/// <summary> Refetch the URL contents.
		/// Only need to worry if there is already a valid parser and it's
		/// been spent fetching the string contents.
		/// </summary>
		private void ResetStrings()
		{
			if (null != mStrings)
			{
				try
				{
					mParser.URL = URL;
					SetStrings();
				}
				catch (ParserException pe)
				{
					UpdateStrings(pe.ToString());
				}
			}
		}
	}
}
