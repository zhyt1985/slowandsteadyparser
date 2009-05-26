// ***************************************************************
//  Page   version:  1.0   Date: 12/17/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista, All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.IO;

using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Http;

namespace Winista.Text.HtmlParser.Lex
{
	/// <summary>
	/// Summary description for Page.
	/// </summary>
	/// 
	[Serializable]
	public class Page : IDisposable
	{
		#region Class Members
		/// <summary> The default charset.
		/// This should be <code>{@value}</code>,
		/// see RFC 2616 (http://www.ietf.org/rfc/rfc2616.txt?number=2616)
		/// section 3.7.1
		/// <p>Another alias is "8859_1".
		/// </summary>
		public const System.String DEFAULT_CHARSET = "ISO-8859-1";
		
		/// <summary> The default content type.
		/// In the absence of alternate information, assume html content ({@value}).
		/// </summary>
		public const System.String DEFAULT_CONTENT_TYPE = "text/html";
		
		/// <summary> Character value when the page is exhausted.
		/// Has a value of {@value}.
		/// </summary>
		public static readonly char EOF;
		
		/// <summary> The URL this page is coming from.
		/// Cached value of <code>getConnection().toExternalForm()</code> or
		/// <code>setUrl()</code>.
		/// </summary>
		protected internal System.String mUrl;
		
		/// <summary> The base URL for this page.</summary>
		protected internal System.String mBaseUrl;
		
		/// <summary> The source of characters.</summary>
		protected internal Source mSource;
		
		/// <summary> Character positions of the first character in each line.</summary>
		protected internal PageIndex mIndex;

		/// <summary> The connection this page is coming from or <code>null</code>.</summary>
		[NonSerialized]
		protected internal HttpProtocol mConnection;

		HttpProtocolOutput m_obProtocolOutput;

		private ContentProperties m_HttpContentProperties;
		private bool m_bHasContent;
		private bool m_bDisposed;

		#endregion
		
		#region Class Constructors

		/// <summary> Construct an empty page.</summary>
		public Page():this("")
		{
		}

		/// <summary> Construct a page reading from a URL connection.</summary>
		/// <param name="connection">A fully conditioned connection. The connect()
		/// method will be called so it need not be connected yet.
		/// </param>
		/// <exception cref="ParserException">An exception object wrapping a number of
		/// possible error conditions, some of which are outlined below.
		/// <li>IOException If an i/o exception occurs creating the
		/// source.</li>
		/// <li>UnsupportedEncodingException if the character set specified in the
		/// HTTP header is not supported.</li>
		/// </exception>
		public Page(HttpProtocol connection)
		{
			if (null == connection)
			{
				throw new System.ArgumentException("HttpProtocol cannot be null");
			}
			Connection = connection;
			mBaseUrl = null;
		}

		/// <summary> Construct a page from a stream encoded with the given charset.</summary>
		/// <param name="stream">The source of bytes.
		/// </param>
		/// <param name="charset">The encoding used.
		/// If null, defaults to the <code>DEFAULT_CHARSET</code>.
		/// </param>
		/// <exception cref="UnsupportedEncodingException">If the given charset
		/// is not supported.
		/// </exception>
		public Page(System.IO.Stream stream, System.String charset)
		{
			if (null == stream)
				throw new System.ArgumentException("stream cannot be null");
			if (null == charset)
				charset = DEFAULT_CHARSET;
			mSource = new InputStreamSource(stream, charset);
			mIndex = new PageIndex(this);
			mUrl = null;
			mBaseUrl = null;
		}

		/// <summary> Construct a page from the given string.</summary>
		/// <param name="text">The HTML text.
		/// </param>
		/// <param name="charset"><em>Optional</em>. The character set encoding that will
		/// be reported by {@link #getEncoding}. If charset is <code>null</code>
		/// the default character set is used.
		/// </param>
		public Page(System.String text, System.String charset)
		{
			if (null == text)
				throw new System.ArgumentException("text cannot be null");
			if (null == charset)
				charset = DEFAULT_CHARSET;
			mSource = new StringSource(text, charset);
			mIndex = new PageIndex(this);
			mUrl = null;
			mBaseUrl = null;
		}

		/// <summary> Construct a page from the given string.
		/// The page will report that it is using an encoding of
		/// {@link #DEFAULT_CHARSET}.
		/// </summary>
		/// <param name="text">The HTML text.
		/// </param>
		public Page(System.String text):this(text, null)
		{
		}
		
		/// <summary> Construct a page from a source.</summary>
		/// <param name="source">The source of characters.
		/// </param>
		public Page(Source source)
		{
			if (null == source)
				throw new System.ArgumentException("source cannot be null");
			mSource = source;
			mIndex = new PageIndex(this);
			mUrl = null;
			mBaseUrl = null;
		}

		#endregion

		/// <summary> Get the connection, if any.</summary>
		/// <returns> The connection object for this page, or null if this page
		/// is built from a stream or a string.
		/// </returns>
		/// <summary> Set the URLConnection to be used by this page.
		/// Starts reading from the given connection.
		/// This also resets the current url.
		/// </summary>
		/// <param name="connection">The connection to use.
		/// It will be connected by this method.
		/// </param>
		/// <exception cref="ParserException">If the <code>connect()</code> method fails,
		/// or an I/O error occurs opening the input stream or the character set
		/// designated in the HTTP header is unsupported.
		/// </exception>
		virtual public HttpProtocol Connection
		{
			get
			{
				return this.mConnection;
			}
			set
			{
				mConnection = value;
				//GetPageContent(value, false);
			}
		}

		/// <summary> Get the URL for this page.
		/// This is only available if the page has a connection
		/// (<code>getConnection()</code> returns non-null), or the document base has
		/// been set via a call to <code>setUrl()</code>.
		/// </summary>
		/// <returns> The url for the connection, or <code>null</code> if there is
		/// no connection or the document base has not been set.
		/// </returns>
		/// <summary> Set the URL for this page.
		/// This doesn't affect the contents of the page, just the interpretation
		/// of relative links from this point forward.
		/// </summary>
		/// <param name="url">The new URL.
		/// </param>
		virtual public System.String Url
		{
			get
			{
				return (mUrl);
			}
			
			set
			{
				mUrl = value;
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Gets the baseUrl.</summary>
		/// <returns> The base URL for this page, or <code>null</code> if not set.
		/// </returns>
		/// <summary> Sets the baseUrl.</summary>
		/// <param name="url">The base url for this page.
		/// </param>
		virtual public System.String BaseUrl
		{
			get
			{
				return (mBaseUrl);
			}
			
			set
			{
				mBaseUrl = value;
			}
			
		}
		/// <summary> Get the source this page is reading from.</summary>
		/// <returns> The current source.
		/// </returns>
		virtual public Source Source
		{
			get
			{
				return (mSource);
			}
			
		}
		/// <summary> Try and extract the content type from the HTTP header.</summary>
		/// <returns> The content type.
		/// </returns>
		virtual public System.String ContentType
		{
			get
			{
				if (mSource == null)
				{
					this.GetPageContent(this.mConnection, false);
				}
				System.String content;
				System.String ret;
				
				ret = DEFAULT_CONTENT_TYPE;

				if (null != this.m_HttpContentProperties)
				{
					string ContentType;
					try
					{
						ContentType = m_HttpContentProperties.GetProperty("Content-Type");
					}
					catch
					{
						ContentType = null;
					}
					content = ContentType;
					if (null != content &&
						String.Empty != content)
					{
						ret = content;
					}
				}
				
				return (ret);
			}
			
		}

		/// <summary> Get the current encoding being used.</summary>
		/// <returns> The encoding used to convert characters.
		/// </returns>
		/// <summary> Begins reading from the source with the given character set.
		/// If the current encoding is the same as the requested encoding,
		/// this method is a no-op. Otherwise any subsequent characters read from
		/// this page will have been decoded using the given character set.<p>
		/// Some magic happens here to obtain this result if characters have already
		/// been consumed from this page.
		/// Since a Reader cannot be dynamically altered to use a different character
		/// set, the underlying stream is reset, a new Source is constructed
		/// and a comparison made of the characters read so far with the newly
		/// read characters up to the current position.
		/// If a difference is encountered, or some other problem occurs,
		/// an exception is thrown.
		/// </summary>
		/// <param name="character_set">The character set to use to convert bytes into
		/// characters.
		/// </param>
		/// <exception cref="ParserException">If a character mismatch occurs between
		/// characters already provided and those that would have been returned
		/// had the new character set been in effect from the beginning. An
		/// exception is also thrown if the underlying stream won't put up with
		/// these shenanigans.
		/// </exception>
		virtual public System.String Encoding
		{
			get
			{
				return (Source.Encoding);
			}
			
			set
			{
				Source.Encoding = value;
			}
		}

		//
		// static methods
		//
		
		/// <summary> Get a CharacterSet name corresponding to a charset parameter.</summary>
		/// <param name="content">A text line of the form:
		/// <pre>
		/// text/html; charset=Shift_JIS
		/// </pre>
		/// which is applicable both to the HTTP header field Content-Type and
		/// the meta tag http-equiv="Content-Type".
		/// Note this method also handles non-compliant quoted charset directives
		/// such as:
		/// <pre>
		/// text/html; charset="UTF-8"
		/// </pre>
		/// and
		/// <pre>
		/// text/html; charset='UTF-8'
		/// </pre>
		/// </param>
		/// <returns> The character set name to use when reading the input stream.
		/// If the charset parameter is not found in the given string, the default
		/// character set is returned.
		/// </returns>
		/// <seealso cref="findCharset">
		/// </seealso>
		/// <seealso cref="DEFAULT_CHARSET">
		/// </seealso>
		public static System.String GetCharset(System.String content)
		{
			System.String CHARSET_STRING = "charset";
			int index;
			System.String ret;
			
			ret = DEFAULT_CHARSET;
			if (null != content)
			{
				index = content.IndexOf(CHARSET_STRING);
				
				if (index != - 1)
				{
					content = content.Substring(index + CHARSET_STRING.Length).Trim();
					if (content.StartsWith("="))
					{
						content = content.Substring(1).Trim();
						index = content.IndexOf(";");
						if (index != - 1)
							content = content.Substring(0, (index) - (0));
						
						//remove any double quotes from around charset string
						if (content.StartsWith("\"") && content.EndsWith("\"") && (1 < content.Length))
							content = content.Substring(1, (content.Length - 1) - (1));
						
						//remove any single quote from around charset string
						if (content.StartsWith("'") && content.EndsWith("'") && (1 < content.Length))
							content = content.Substring(1, (content.Length - 1) - (1));
						
						ret = FindCharset(content, ret);
					}
				}
			}
			
			return (ret);
		}
		
		/// <summary> Lookup a character set name.
		/// <em>Vacuous for JVM's without <code>java.nio.charset</code>.</em>
		/// This uses reflection so the code will still run under prior JDK's but
		/// in that case the default is always returned.
		/// </summary>
		/// <param name="name">The name to look up. One of the aliases for a character set.
		/// </param>
		/// <param name="fallback">The name to return if the lookup fails.
		/// </param>
		/// <returns> The character set name.
		/// </returns>
		public static System.String FindCharset(System.String name, System.String fallback)
		{
			System.String ret;
			
			try
			{
				System.Text.Encoding obEncoding = System.Text.Encoding.GetEncoding(name);
				ret = obEncoding.WebName;
			}
			catch (System.Exception cnfe)
			{
				ret = name;
			}
			
			return (ret);
		}
		
		//
		// Serialization support
		//
		
		/// <summary> Serialize the page.
		/// There are two modes to serializing a page based on the connected state.
		/// If connected, the URL and the current offset is saved, while if
		/// disconnected, the underling source is saved.
		/// </summary>
		/// <param name="out">The object stream to store this object in.
		/// </param>
		/// <exception cref="IOException">If there is a serialization problem.
		/// </exception>
		public virtual void  GetObjectData(System.Runtime.Serialization.SerializationInfo out_Renamed, System.Runtime.Serialization.StreamingContext context)
		{
			System.String href;
			Source source;
			PageIndex index;

			{
				out_Renamed.AddValue("Winista.Text.HtmlParser.Lex.Pagedata4", false);
				href = Url;
				out_Renamed.AddValue("Winista.Text.HtmlParser.Lex.Pagedata5", href);
				Url = null; // don't try and read a bogus URL
				Support.SupportMisc.DefaultWriteObject(out_Renamed, context, this);
				Url = href;
			}
		}
		
		/// <summary> Deserialize the page.
		/// For details see <code>writeObject()</code>.
		/// </summary>
		/// <param name="in">The object stream to decode.
		/// </param>
		/// <exception cref="IOException">If there is a deserialization problem with
		/// the stream.
		/// </exception>
		/// <exception cref="ClassNotFoundException">If the deserialized class can't be
		/// located with the current classpath and class loader.
		/// </exception>
		protected Page(System.Runtime.Serialization.SerializationInfo in_Renamed, System.Runtime.Serialization.StreamingContext context)
		{
			bool fromurl;
			int offset;
			System.String href;
			System.Uri url;
			Cursor cursor;
			
			fromurl = in_Renamed.GetBoolean("Winista.Text.Htmlparser.Lex.Pagedata1");
			if (fromurl)
			{
				offset = in_Renamed.GetInt32("Winista.Text.Htmlparser.Lex.Pagedata2");
				href = ((System.String) in_Renamed.GetValue("Winista.Text.Htmlparser.Lex.Pagedata3", typeof(System.String)));
				Support.SupportMisc.DefaultReadObject(in_Renamed, context, this);
				// open the URL
				if (null != Url)
				{
					url = new System.Uri(Url);
					try
					{
						//Connection = (System.Net.HttpWebRequest) System.Net.WebRequest.Create(url);
					}
					catch (ParserException pe)
					{
						throw new System.IO.IOException(pe.Message);
					}
				}
				cursor = new Cursor(this, 0);
				for (int i = 0; i < offset; i++)
					try
					{
						GetCharacter(cursor);
					}
					catch (ParserException pe)
					{
						throw new System.IO.IOException(pe.Message);
					}
				Url = href;
			}
			else
			{
				href = ((System.String) in_Renamed.GetValue("Winista.Text.Htmlparser.Lex.Pagedata4", typeof(System.String)));
				Support.SupportMisc.DefaultReadObject(in_Renamed, context, this);
				Url = href;
			}
		}
		
		/// <summary>
		/// Gets the value indicating if URL corresponding to this
		/// <see cref="Page"></see> is blocked by ROBOTS rules.
		/// </summary>
		/// <returns></returns>
		public bool IsBlockedByRobotRules()
		{
			this.GetPageContent(this.mConnection, false);
			return (m_obProtocolOutput.Status.Code == HttpProtocolStatus.ROBOTS_DENIED);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool DidPageFetchSucceed()
		{
			this.GetPageContent(this.mConnection, false);
			return (m_obProtocolOutput.Status.Code == HttpProtocolStatus.SUCCESS);
		}

		/// <summary> Reset the page by resetting the source of characters.</summary>
		public virtual void Reset()
		{
			Source.Reset();
			mIndex = new PageIndex(this); // todo: is this really necessary?
		}
		
		/// <summary>
		/// 
		/// </summary>
		public void RefreshPageContent()
		{
			this.GetPageContent(this.mConnection, true);
		}

		/// <summary> Close the page by destroying the source of characters.</summary>
		/// <exception cref="IOException">If destroying the source encounters an error.
		/// </exception>
		public virtual void Close()
		{
			if (null != Source)
			{
				Source.Destroy();
			}
		}

		/// <summary> Read the character at the given cursor position.
		/// The cursor position can be only behind or equal to the
		/// current source position.
		/// Returns end of lines (EOL) as \n, by converting \r and \r\n to \n,
		/// and updates the end-of-line index accordingly
		/// Advances the cursor position by one (or two in the \r\n case).
		/// </summary>
		/// <param name="cursor">The position to read at.
		/// </param>
		/// <returns> The character at that position, and modifies the cursor to
		/// prepare for the next read. If the source is exhausted a zero is returned.
		/// </returns>
		/// <exception cref="ParserException">If an IOException on the underlying source
		/// occurs, or an attemp is made to read characters in the future (the
		/// cursor position is ahead of the underlying stream)
		/// </exception>
		public virtual char GetCharacter(Cursor cursor)
		{
			int i;
			char ret;
			
			i = cursor.Position;
			if (mSource.Offset() < i)
				// hmmm, we could skip ahead, but then what about the EOL index
				throw new ParserException("Attempt to read future characters from source " + i + " > " + mSource.Offset());
			else if (mSource.Offset() == i)
				try
				{
					i = mSource.Read();
					if (Source.EOF == i)
						ret = EOF;
					else
					{
						ret = (char) i;
						cursor.Advance();
					}
				}
				catch (System.IO.IOException ioe)
				{
					throw new ParserException("Problem reading a character at position " + cursor.Position, ioe);
				}
			else
			{
				// historic read
				try
				{
					ret = mSource.GetCharacter(i);
				}
				catch (System.IO.IOException ioe)
				{
					throw new ParserException("Can't read a character at position " + i, ioe);
				}
				cursor.Advance();
			}
			
			// handle \r
			if ('\r' == ret)
			{
				// switch to single character EOL
				ret = '\n';
				
				// check for a \n in the next position
				if (mSource.Offset() == cursor.Position)
					try
					{
						i = mSource.Read();
						if (Source.EOF == i)
						{
							// do nothing
						}
						else if ('\n' == (char) i)
							cursor.Advance();
						else
							try
							{
								mSource.Unread();
							}
							catch (System.IO.IOException ioe)
							{
								throw new ParserException("Can't unread a character at position " + cursor.Position, ioe);
							}
					}
					catch (System.IO.IOException ioe)
					{
						throw new ParserException("Problem reading a character at position " + cursor.Position, ioe);
					}
				else
					try
					{
						if ('\n' == mSource.GetCharacter(cursor.Position))
							cursor.Advance();
					}
					catch (System.IO.IOException ioe)
					{
						throw new ParserException("can't read a character at position " + cursor.Position, ioe);
					}
			}
			if ('\n' == ret)
				// update the EOL index in any case
				mIndex.Add(cursor);
			
			return (ret);
		}
		
		/// <summary> Build a URL from the link and base provided.</summary>
		/// <returns> An absolute URL.
		/// </returns>
		/// <param name="link">The (relative) URI.
		/// </param>
		/// <param name="base">The base URL of the page, either from the &lt;BASE&gt; tag
		/// or, if none, the URL the page is being fetched from.
		/// </param>
		/// <exception cref="MalformedURLException">If creating the URL fails.
		/// </exception>
		public virtual System.Uri ConstructUrl(System.String link, System.String base_Renamed)
		{
			System.String path;
			bool modified;
			bool absolute;
			int index;
			System.Uri url; // constructed URL combining relative link and base
			
			url = new System.Uri(new System.Uri(base_Renamed), link);
			path = url.PathAndQuery;
			modified = false;
			absolute = link.StartsWith("/");
			if (!absolute)
			{
				// we prefer to fix incorrect relative links
				// this doesn't fix them all, just the ones at the start
				while (path.StartsWith("/."))
				{
					if (path.StartsWith("/../"))
					{
						path = path.Substring(3);
						modified = true;
					}
					else if (path.StartsWith("/./") || path.StartsWith("/."))
					{
						path = path.Substring(2);
						modified = true;
					}
					else
						break;
				}
			}
			// fix backslashes
			while (- 1 != (index = path.IndexOf("/\\")))
			{
				path = path.Substring(0, (index + 1) - (0)) + path.Substring(index + 2);
				modified = true;
			}
			if (modified)
			{
				//UPGRADE_TODO: Class 'java.net.URL' was converted to a 'System.Uri' which does not throw an exception if a URL specifies an unknown protocol. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1132'"
				url = new System.Uri(url, path);
			}
			
			return (url);
		}
		
		/// <summary> Create an absolute URL from a relative link.</summary>
		/// <param name="link">The reslative portion of a URL.
		/// </param>
		/// <returns> The fully qualified URL or the original link if it was absolute
		/// already or a failure occured.
		/// </returns>
		public virtual System.String GetAbsoluteURL(System.String link)
		{
			System.String base_Renamed;
			System.Uri url;
			System.String ret;
			
			if ((null == link) || ("".Equals(link)))
				ret = "";
			else
				try
				{
					base_Renamed = BaseUrl;
					if (null == base_Renamed)
						base_Renamed = Url;
					if (null == base_Renamed)
						ret = link;
					else
					{
						url = ConstructUrl(link, base_Renamed);
						ret = url.ToString();
					}
				}
				catch (System.UriFormatException murle)
				{
					ret = link;
				}
			
			return (ret);
		}
		
		/// <summary> Get the line number for a cursor.</summary>
		/// <param name="cursor">The character offset into the page.
		/// </param>
		/// <returns> The line number the character is in.
		/// </returns>
		public virtual int Row(Cursor cursor)
		{
			return (mIndex.Row(cursor));
		}
		
		/// <summary> Get the line number for a cursor.</summary>
		/// <param name="position">The character offset into the page.
		/// </param>
		/// <returns> The line number the character is in.
		/// </returns>
		public virtual int Row(int position)
		{
			return (mIndex.Row(position));
		}
		
		/// <summary> Get the column number for a cursor.</summary>
		/// <param name="cursor">The character offset into the page.
		/// </param>
		/// <returns> The character offset into the line this cursor is on.
		/// </returns>
		public virtual int Column(Cursor cursor)
		{
			return (mIndex.Column(cursor));
		}
		
		/// <summary> Get the column number for a cursor.</summary>
		/// <param name="position">The character offset into the page.
		/// </param>
		/// <returns> The character offset into the line this cursor is on.
		/// </returns>
		public virtual int Column(int position)
		{
			return (mIndex.Column(position));
		}
		
		/// <summary> Get the text identified by the given limits.</summary>
		/// <param name="start">The starting position, zero based.
		/// </param>
		/// <param name="end">The ending position
		/// (exclusive, i.e. the character at the ending position is not included),
		/// zero based.
		/// </param>
		/// <returns> The text from <code>start</code> to <code>end</code>.
		/// </returns>
		/// <seealso cref="getText(StringBuffer, int, int)">
		/// </seealso>
		/// <exception cref="IllegalArgumentException">If an attempt is made to get
		/// characters ahead of the current source offset (character position).
		/// </exception>
		public virtual System.String GetText(int start, int end)
		{
			System.String ret;
			
			try
			{
				if (mSource == null)
				{
					this.GetPageContent(this.mConnection, false);
				}
				ret = mSource.GetString(start, end - start);
			}
			catch (System.IO.IOException ioe)
			{
				throw new System.ArgumentException("can't get the " + (end - start) + "characters at position " + start + " - " + ioe.Message);
			}
			
			return (ret);
		}
		
		/// <summary> Put the text identified by the given limits into the given buffer.</summary>
		/// <param name="buffer">The accumulator for the characters.
		/// </param>
		/// <param name="start">The starting position, zero based.
		/// </param>
		/// <param name="end">The ending position
		/// (exclusive, i.e. the character at the ending position is not included),
		/// zero based.
		/// </param>
		/// <exception cref="IllegalArgumentException">If an attempt is made to get
		/// characters ahead of the current source offset (character position).
		/// </exception>
		public virtual void GetText(System.Text.StringBuilder buffer, int start, int end)
		{
			int length;
			
			if (mSource == null)
			{
				this.GetPageContent(this.mConnection, false);
			}

			if ((mSource.Offset() < start) || (mSource.Offset() < end))
				throw new System.ArgumentException("attempt to extract future characters from source" + start + "|" + end + " > " + mSource.Offset());
			if (end < start)
			{
				length = end;
				end = start;
				start = length;
			}
			length = end - start;
			try
			{
				mSource.GetCharacters(buffer, start, length);
			}
			catch (System.IO.IOException ioe)
			{
				throw new System.ArgumentException("can't get the " + (end - start) + "characters at position " + start + " - " + ioe.Message);
			}
		}
		
		/// <summary> Get all text read so far from the source.</summary>
		/// <returns> The text from the source.
		/// </returns>
		/// <seealso cref="getText(StringBuffer)">
		/// </seealso>
		public virtual System.String GetText()
		{
			return (GetText(0, mSource.Offset()));
		}
		
		/// <summary> Put all text read so far from the source into the given buffer.</summary>
		/// <param name="buffer">The accumulator for the characters.
		/// </param>
		/// <seealso cref="getText(StringBuffer,int,int)">
		/// </seealso>
		public virtual void GetText(System.Text.StringBuilder buffer)
		{
			GetText(buffer, 0, mSource.Offset());
		}
		
		/// <summary> Put the text identified by the given limits into the given array at the specified offset.</summary>
		/// <param name="array">The array of characters.
		/// </param>
		/// <param name="offset">The starting position in the array where characters are to be placed.
		/// </param>
		/// <param name="start">The starting position, zero based.
		/// </param>
		/// <param name="end">The ending position
		/// (exclusive, i.e. the character at the ending position is not included),
		/// zero based.
		/// </param>
		/// <exception cref="IllegalArgumentException">If an attempt is made to get
		/// characters ahead of the current source offset (character position).
		/// </exception>
		public virtual void GetText(char[] array, int offset, int start, int end)
		{
			int length;
			
			if (null == mSource)
			{
				this.GetPageContent(this.mConnection, false);
			}

			if ((mSource.Offset() < start) || (mSource.Offset() < end))
				throw new System.ArgumentException("attempt to extract future characters from source");
			if (end < start)
			{
				// swap
				length = end;
				end = start;
				start = length;
			}
			length = end - start;
			try
			{
				mSource.GetCharacters(array, offset, start, end);
			}
			catch (System.IO.IOException ioe)
			{
				throw new System.ArgumentException("can't get the " + (end - start) + "characters at position " + start + " - " + ioe.Message);
			}
		}
		
		/// <summary> Get the text line the position of the cursor lies on.</summary>
		/// <param name="cursor">The position to calculate for.
		/// </param>
		/// <returns> The contents of the URL or file corresponding to the line number
		/// containg the cursor position.
		/// </returns>
		public virtual System.String GetLine(Cursor cursor)
		{
			int line;
			int size;
			int start;
			int end;

			if (mSource == null)
			{
				this.GetPageContent(this.mConnection, false);
			}
			
			line = Row(cursor);
			size = mIndex.Size();
			if (line < size)
			{
				start = mIndex.ElementAt(line);
				line++;
				if (line <= size)
					end = mIndex.ElementAt(line);
				else
					end = mSource.Offset();
			}
				// current line
			else
			{
				start = mIndex.ElementAt(line - 1);
				end = mSource.Offset();
			}
			
			
			return (GetText(start, end));
		}
		
		/// <summary> Get the text line the position of the cursor lies on.</summary>
		/// <param name="position">The position to calculate for.
		/// </param>
		/// <returns> The contents of the URL or file corresponding to the line number
		/// containg the cursor position.
		/// </returns>
		public virtual System.String GetLine(int position)
		{
			return (GetLine(new Cursor(this, position)));
		}
		
		/// <summary> Display some of this page as a string.</summary>
		/// <returns> The last few characters the source read in.
		/// </returns>
		public override System.String ToString()
		{
			System.Text.StringBuilder buffer;
			int start;
			System.String ret;

			if (null == mSource)
			{
				this.GetPageContent(this.mConnection, false);
			}
			
			if (mSource.Offset() > 0)
			{
				buffer = new System.Text.StringBuilder(43);
				start = mSource.Offset() - 40;
				if (0 > start)
					start = 0;
				else
					buffer.Append("...");
				GetText(buffer, start, mSource.Offset());
				ret = buffer.ToString();
			}
			else
			{
				ret = base.ToString();
			}
			
			return (ret);
		}

		static Page()
		{
			EOF = (char) Support.SupportMisc.Identity(Source.EOF);
		}

		#region IDisposable Members

		/// <summary> Clean up this page, releasing resources.
		/// Calls <code>close()</code>.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing && !m_bDisposed)
			{
				Close();
				m_bDisposed = true;
			}
		}

		#endregion

		#region Helper Methods
		private void GetPageContent(HttpProtocol obProtocol, bool bIsRefresh)
		{
			if(m_bHasContent && !bIsRefresh)
			{
				return;
			}

			if(obProtocol == null)
			{
				throw new ArgumentNullException("obProtocol", "Null HttpProtocol object specified");
			}

			lock(this)
			{
				ParserStream stream = null;
				System.String type = String.Empty;
				System.String charset = String.Empty;
				try
				{
					m_obProtocolOutput = obProtocol.GetProtocolOutput();
					if (m_obProtocolOutput.Status.Code == HttpProtocolStatus.SUCCESS)
					{
						m_bHasContent = true;
						this.m_HttpContentProperties = m_obProtocolOutput.Content.ContentProperties;
						type = this.ContentType;
						charset = GetCharset(type);
						stream = new ParserStream(new System.IO.MemoryStream(m_obProtocolOutput.Content.ContentData));
					}

					if (null != stream)
					{
						mSource = new InputStreamSource(stream,charset,m_obProtocolOutput.Content.ContentData.Length);
					}
				}
				catch (System.Exception e)
				{
					throw new ParserException("Failed to get page content", e);
				}

				mUrl = obProtocol.URL.ToString();
				mIndex = new PageIndex(this);
			}
		}
		#endregion
	}
}
