// ***************************************************************
//  Lexer   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Nodes;
using Winista.Text.HtmlParser.Http;

namespace Winista.Text.HtmlParser.Lex
{
	/// <summary> This class parses the HTML stream into nodes.
	/// There are three major types of nodes (lexemes):
	/// <ul>
	/// <li>Remark</li>
	/// <li>Text</li>
	/// <li>Tag</li>
	/// </ul>
	/// Each time <code>nextNode()</code> is called, another node is returned until
	/// the stream is exhausted, and <code>null</code> is returned.
	/// </summary>
	[Serializable]
	public class Lexer : INodeFactory
	{
		#region Class Members
		/// <summary> The page lexemes are retrieved from.</summary>
		protected internal Page mPage;
		
		/// <summary> The current position on the page.</summary>
		protected internal Cursor mCursor;
		
		/// <summary> The factory for new nodes.</summary>
		protected internal INodeFactory mFactory;

		/// <summary> Line number to trigger on.
		/// This is tested on each <code>nextNode()</code> call, as a debugging aid.
		/// Alter this value and set a breakpoint on the guarded statement.
		/// Remember, these line numbers are zero based, while most editors are
		/// one based.
		/// </summary>
		protected internal static int mDebugLineTrigger = - 1;
		#endregion

		#region Class Constructors
		/// <summary> Creates a new instance of a Lexer.</summary>
		public Lexer():this(new Page(""))
		{
		}

		/// <summary> Creates a new instance of a Lexer.</summary>
		/// <param name="page">The page with HTML text.
		/// </param>
		public Lexer(Page page)
		{
			Page = page;
			Cursor = new Cursor(page, 0);
			NodeFactory = this;
		}

		/// <summary> Creates a new instance of a Lexer.</summary>
		/// <param name="text">The text to parse.
		/// </param>
		public Lexer(System.String text):this(new Page(text))
		{
		}

		/// <summary> Creates a new instance of a Lexer.</summary>
		/// <param name="connection">The url to parse.
		/// </param>
		/// <exception cref="ParserException">If an error occurs opening the connection.
		/// </exception>
		public Lexer(HttpProtocol connection):this(new Page(connection))
		{
		}
		#endregion
		
		/// <summary> Get the page this lexer is working on.</summary>
		/// <returns> The page that nodes are being read from.
		/// </returns>
		/// <summary> Set the page this lexer is working on.</summary>
		/// <param name="page">The page that nodes will be read from.
		/// </param>
		virtual public Page Page
		{
			get
			{
				return (mPage);
			}
			
			set
			{
				if (null == value)
				{
					throw new System.ArgumentException("page cannot be null");
				}
				// todo: sanity checks
				mPage = value;
			}
			
		}

		/// <summary> Get the current scanning position.</summary>
		/// <returns> The lexer's cursor position.
		/// </returns>
		/// <summary> Set the current scanning position.</summary>
		/// <param name="cursor">The lexer's new cursor position.
		/// </param>
		virtual public Cursor Cursor
		{
			get
			{
				return (mCursor);
			}
			
			set
			{
				if (null == value)
					throw new System.ArgumentException("cursor cannot be null");
				// todo: sanity checks
				mCursor = value;
			}
			
		}

		/// <summary> Get the current node factory.</summary>
		/// <returns> The lexer's node factory.
		/// </returns>
		/// <summary> Set the current node factory.</summary>
		/// <param name="factory">The node factory to be used by the lexer.
		/// </param>
		virtual public INodeFactory NodeFactory
		{
			get
			{
				return (mFactory);
			}
			
			set
			{
				if (null == value)
					throw new System.ArgumentException("node factory cannot be null");
				mFactory = value;
			}
			
		}

		/// <summary> Get the current cursor position.</summary>
		/// <returns> The current character offset into the source.
		/// </returns>
		/// <summary> Set the current cursor position.</summary>
		/// <param name="position">The new character offset into the source.
		/// </param>
		virtual public int Position
		{
			get
			{
				return (Cursor.Position);
			}
			
			set
			{
				// todo: sanity checks
				Cursor.Position = value;
			}
			
		}
		/// <summary> Get the current line number.</summary>
		/// <returns> The line number the lexer's working on.
		/// </returns>
		virtual public int CurrentLineNumber
		{
			get
			{
				return (Page.Row(Cursor));
			}
			
		}
		/// <summary> Get the current line.</summary>
		/// <returns> The string the lexer's working on.
		/// </returns>
		virtual public System.String CurrentLine
		{
			get
			{
				return (Page.GetLine(Cursor));
			}
		}

		/// <summary> Reset the lexer to start parsing from the beginning again.
		/// The underlying components are reset such that the next call to
		/// <code>nextNode()</code> will return the first lexeme on the page.
		/// </summary>
		public virtual void Reset()
		{
			Page.Reset();
			Cursor = new Cursor(Page, 0);
		}
		
		/// <summary> Get the next node from the source.</summary>
		/// <returns> A Remark, Text or Tag, or <code>null</code> if no
		/// more lexemes are present.
		/// </returns>
		/// <exception cref="ParserException">If there is a problem with the
		/// underlying page.
		/// </exception>
		public virtual INode NextNode()
		{
			return NextNode(false);
		}

		/// <summary> Get the next node from the source.</summary>
		/// <param name="quotesmart">If <code>true</code>, strings ignore quoted contents.
		/// </param>
		/// <returns> A Remark, Text or Tag, or <code>null</code> if no
		/// more lexemes are present.
		/// </returns>
		/// <exception cref="ParserException">If there is a problem with the
		/// underlying page.
		/// </exception>
		public virtual INode NextNode(bool quotesmart)
		{
			int start;
			char ch;
			INode ret;
			
			// debugging suppport
			if (- 1 != mDebugLineTrigger)
			{
				Page page = Page;
				int lineno = page.Row(mCursor);
				if (mDebugLineTrigger < lineno)
					mDebugLineTrigger = lineno + 1; // trigger on next line too
			}
			start = mCursor.Position;
			ch = mPage.GetCharacter(mCursor);
			switch (ch)
			{
				
				//case Page.EOF: 
				case unchecked((char)(-1)):
					ret = null;
					break;
				
				case '<': 
					ch = mPage.GetCharacter(mCursor);
					if (Page.EOF == ch)
						ret = MakeString(start, mCursor.Position);
					else if ('%' == ch)
					{
						mCursor.Retreat();
						ret = ParseJsp(start);
					}
					else if ('/' == ch || '%' == ch || System.Char.IsLetter(ch))
					{
						mCursor.Retreat();
						ret = ParseTag(start);
					}
					else if ('!' == ch)
					{
						ch = mPage.GetCharacter(mCursor);
						if (Page.EOF == ch)
							ret = MakeString(start, mCursor.Position);
						else
						{
							if ('>' == ch)
								// handle <!>
								ret = MakeRemark(start, mCursor.Position);
							else
							{
								mCursor.Retreat(); // remark/tag need this char
								if ('-' == ch)
									ret = ParseRemark(start, quotesmart);
								else
								{
									mCursor.Retreat(); // tag needs prior one too
									ret = ParseTag(start);
								}
							}
						}
					}
					else
						ret = ParseString(start, quotesmart);
					break;
				
				default: 
					mCursor.Retreat(); // string needs to see leading foreslash
					ret = ParseString(start, quotesmart);
					break;
				
			}
			
			return (ret);
		}

		/// <summary> Advance the cursor through a JIS escape sequence.</summary>
		/// <param name="cursor">A cursor positioned within the escape sequence.
		/// </param>
		/// <exception cref="ParserException">If a problem occurs reading from the source.
		/// </exception>
		protected internal virtual void ScanJIS(Cursor cursor)
		{
			bool done;
			char ch;
			int state;
			
			done = false;
			state = 0;
			while (!done)
			{
				ch = mPage.GetCharacter(cursor);
				if (Page.EOF == ch)
				{
					done = true;
				}
				else
				{
					switch (state)
					{
						
						case 0: 
							if (0x1b == ch)
								// escape
								state = 1;
							break;
						
						case 1: 
							if ('(' == ch)
								state = 2;
							else
								state = 0;
							break;
						
						case 2: 
							if ('J' == ch)
								done = true;
							else
								state = 0;
							break;
						
						default: 
							throw new System.SystemException("state " + state);
						
					}
				}
			}
		}

		/// <summary> Parse a string node.
		/// Scan characters until "&lt;/", "&lt;%", "&lt;!" or &lt; followed by a
		/// letter is encountered, or the input stream is exhausted, in which
		/// case <code>null</code> is returned.
		/// </summary>
		/// <param name="start">The position at which to start scanning.
		/// </param>
		/// <param name="quotesmart">If <code>true</code>, strings ignore quoted contents.
		/// </param>
		/// <returns> The parsed node.
		/// </returns>
		/// <exception cref="ParserException">If a problem occurs reading from the source.
		/// </exception>
		protected internal virtual INode ParseString(int start, bool quotesmart)
		{
			bool done;
			char ch;
			char quote;
			
			done = false;
			quote = (char) (0);
			while (!done)
			{
				ch = mPage.GetCharacter(mCursor);
				if (Page.EOF == ch)
					done = true;
				else if (0x1b == ch)
					// escape
				{
					ch = mPage.GetCharacter(mCursor);
					if (Page.EOF == ch)
						done = true;
					else if ('$' == ch)
					{
						ch = mPage.GetCharacter(mCursor);
						if (Page.EOF == ch)
							done = true;
						else if ('B' == ch)
						{
							ScanJIS(mCursor);
						}
						else
						{
							mCursor.Retreat();
							mCursor.Retreat();
						}
					}
					else
						mCursor.Retreat();
				}
				else if (quotesmart && (0 == quote) && (('\'' == ch) || ('"' == ch)))
					quote = ch;
					// enter quoted state
					// patch from Gernot Fricke to handle escaped closing quote
				else if (quotesmart && (0 != quote) && ('\\' == ch))
				{
					ch = mPage.GetCharacter(mCursor); // try to consume escape
					if ((Page.EOF != ch) && ('\\' != ch) && (ch != quote))
						// escaped quote character
						// ( reflects ["] or [']  whichever opened the quotation)
						mCursor.Retreat(); // unconsume char if char not an escape
				}
				else if (quotesmart && (ch == quote))
					quote = (char) (0);
					// exit quoted state
				else if (quotesmart && (0 == quote) && (ch == '/'))
				{
					// handle multiline and double slash comments (with a quote)
					// in script like:
					// I can't handle single quotations.
					ch = mPage.GetCharacter(mCursor);
					if (Page.EOF == ch)
						done = true;
					else if ('/' == ch)
					{
						do 
							ch = mPage.GetCharacter(mCursor);
						while ((Page.EOF != ch) && ('\n' != ch));
					}
					else if ('*' == ch)
					{
						do 
						{
							do 
								ch = mPage.GetCharacter(mCursor);
							while ((Page.EOF != ch) && ('*' != ch));
							ch = mPage.GetCharacter(mCursor);
							if (ch == '*')
								mCursor.Retreat();
						}
						while ((Page.EOF != ch) && ('/' != ch));
					}
					else
						mCursor.Retreat();
				}
				else if ((0 == quote) && ('<' == ch))
				{
					ch = mPage.GetCharacter(mCursor);
					if (Page.EOF == ch)
						done = true;
						// the order of these tests might be optimized for speed:
					else if ('/' == ch || System.Char.IsLetter(ch) || '!' == ch || '%' == ch)
					{
						done = true;
						mCursor.Retreat();
						mCursor.Retreat();
					}
					else
					{
						// it's not a tag, so keep going, but check for quotes
						mCursor.Retreat();
					}
				}
			}
			
			return (MakeString(start, mCursor.Position));
		}

		/// <summary> Create a string node based on the current cursor and the one provided.</summary>
		/// <param name="start">The starting point of the node.
		/// </param>
		/// <param name="end">The ending point of the node.
		/// </param>
		/// <exception cref="ParserException">If the nodefactory creation of the text
		/// node fails.
		/// </exception>
		/// <returns> The new Text node.
		/// </returns>
		protected internal virtual INode MakeString(int start, int end)
		{
			int length;
			INode ret;
			
			length = end - start;
			if (0 != length)
				// got some characters
				ret = NodeFactory.CreateStringNode(this.Page, start, end);
			else
				ret = null;
			
			return (ret);
		}

		/// <summary> Generate a whitespace 'attribute',</summary>
		/// <param name="attributes">The list so far.
		/// </param>
		/// <param name="bookmarks">The array of positions.
		/// </param>
		private void Whitespace(System.Collections.ArrayList attributes, int[] bookmarks)
		{
			if (bookmarks[1] > bookmarks[0])
				attributes.Add(new PageAttribute(mPage, - 1, - 1, bookmarks[0], bookmarks[1], (char) 0));
		}

		/// <summary> Generate a standalone attribute -- font.</summary>
		/// <param name="attributes">The list so far.
		/// </param>
		/// <param name="bookmarks">The array of positions.
		/// </param>
		private void  Standalone(System.Collections.ArrayList attributes, int[] bookmarks)
		{
			attributes.Add(new PageAttribute(mPage, bookmarks[1], bookmarks[2], - 1, - 1, (char) 0));
		}
		
		/// <summary> Generate an empty attribute -- color=.</summary>
		/// <param name="attributes">The list so far.
		/// </param>
		/// <param name="bookmarks">The array of positions.
		/// </param>
		private void  Empty(System.Collections.ArrayList attributes, int[] bookmarks)
		{
			attributes.Add(new PageAttribute(mPage, bookmarks[1], bookmarks[2], bookmarks[2] + 1, - 1, (char) 0));
		}

		/// <summary> Generate an unquoted attribute -- size=1.</summary>
		/// <param name="attributes">The list so far.
		/// </param>
		/// <param name="bookmarks">The array of positions.
		/// </param>
		private void  Naked(System.Collections.ArrayList attributes, int[] bookmarks)
		{
			attributes.Add(new PageAttribute(mPage, bookmarks[1], bookmarks[2], bookmarks[3], bookmarks[4], (char) 0));
		}
		
		/// <summary> Generate an single quoted attribute -- width='100%'.</summary>
		/// <param name="attributes">The list so far.
		/// </param>
		/// <param name="bookmarks">The array of positions.
		/// </param>
		private void  Single_Quote(System.Collections.ArrayList attributes, int[] bookmarks)
		{
			attributes.Add(new PageAttribute(mPage, bookmarks[1], bookmarks[2], bookmarks[4] + 1, bookmarks[5], '\''));
		}
		
		/// <summary> Generate an double quoted attribute -- CONTENT="Test Development".</summary>
		/// <param name="attributes">The list so far.
		/// </param>
		/// <param name="bookmarks">The array of positions.
		/// </param>
		private void  Double_Quote(System.Collections.ArrayList attributes, int[] bookmarks)
		{
			attributes.Add(new PageAttribute(mPage, bookmarks[1], bookmarks[2], bookmarks[5] + 1, bookmarks[6], '"'));
		}

		/// <summary> Parse a tag.
		/// Parse the name and attributes from a start tag.<p>
		/// From the <a href="http://www.w3.org/TR/html4/intro/sgmltut.html#h-3.2.2">
		/// HTML 4.01 Specification, W3C Recommendation 24 December 1999</a>
		/// http://www.w3.org/TR/html4/intro/sgmltut.html#h-3.2.2<p>
		/// <cite>
		/// 3.2.2 Attributes<p>
		/// Elements may have associated properties, called attributes, which may
		/// have values (by default, or set by authors or scripts). Attribute/value
		/// pairs appear before the final ">" of an element's start tag. Any number
		/// of (legal) attribute value pairs, separated by spaces, may appear in an
		/// element's start tag. They may appear in any order.<p>
		/// In this example, the id attribute is set for an H1 element:
		/// <code>
		/// &lt;H1 id="section1"&gt;
		/// </code>
		/// This is an identified heading thanks to the id attribute
		/// <code>
		/// &lt;/H1&gt;
		/// </code>
		/// By default, SGML requires that all attribute values be delimited using
		/// either double quotation marks (ASCII decimal 34) or single quotation
		/// marks (ASCII decimal 39). Single quote marks can be included within the
		/// attribute value when the value is delimited by double quote marks, and
		/// vice versa. Authors may also use numeric character references to
		/// represent double quotes (&amp;#34;) and single quotes (&amp;#39;).
		/// For doublequotes authors can also use the character entity reference
		/// &amp;quot;.<p>
		/// In certain cases, authors may specify the value of an attribute without
		/// any quotation marks. The attribute value may only contain letters
		/// (a-z and A-Z), digits (0-9), hyphens (ASCII decimal 45),
		/// periods (ASCII decimal 46), underscores (ASCII decimal 95),
		/// and colons (ASCII decimal 58). We recommend using quotation marks even
		/// when it is possible to eliminate them.<p>
		/// Attribute names are always case-insensitive.<p>
		/// Attribute values are generally case-insensitive. The definition of each
		/// attribute in the reference manual indicates whether its value is
		/// case-insensitive.<p>
		/// All the attributes defined by this specification are listed in the
		/// attribute index.<p>
		/// </cite>
		/// <p>
		/// This method uses a state machine with the following states:
		/// <ol>
		/// <li>state 0 - outside of any attribute</li>
		/// <li>state 1 - within attributre name</li>
		/// <li>state 2 - equals hit</li>
		/// <li>state 3 - within naked attribute value.</li>
		/// <li>state 4 - within single quoted attribute value</li>
		/// <li>state 5 - within double quoted attribute value</li>
		/// <li>state 6 - whitespaces after attribute name could lead to state 2 (=)or state 0</li>
		/// </ol>
		/// <p>
		/// The starting point for the various components is stored in an array
		/// of integers that match the initiation point for the states one-for-one,
		/// i.e. bookmarks[0] is where state 0 began, bookmarks[1] is where state 1
		/// began, etc.
		/// Attributes are stored in a <code>Vector</code> having
		/// one slot for each whitespace or attribute/value pair.
		/// The first slot is for attribute name (kind of like a standalone attribute).
		/// </summary>
		/// <param name="start">The position at which to start scanning.
		/// </param>
		/// <returns> The parsed tag.
		/// </returns>
		/// <exception cref="ParserException">If a problem occurs reading from the source.
		/// </exception>
		protected internal virtual INode ParseTag(int start)
		{
			bool done;
			char ch;
			int state;
			int[] bookmarks;
			System.Collections.ArrayList attributes;
			
			done = false;
			attributes = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			state = 0;
			bookmarks = new int[8];
			bookmarks[0] = mCursor.Position;
			while (!done)
			{
				bookmarks[state + 1] = mCursor.Position;
				ch = mPage.GetCharacter(mCursor);
				switch (state)
				{
					
					case 0:  // outside of any attribute
						if ((Page.EOF == ch) || ('>' == ch) || ('<' == ch))
						{
							if ('<' == ch)
							{
								// don't consume the opening angle
								mCursor.Retreat();
								bookmarks[state + 1] = mCursor.Position;
							}
							Whitespace(attributes, bookmarks);
							done = true;
						}
						else if (!System.Char.IsWhiteSpace(ch))
						{
							Whitespace(attributes, bookmarks);
							state = 1;
						}
						break;
					
					case 1:  // within attribute name
						if ((Page.EOF == ch) || ('>' == ch) || ('<' == ch))
						{
							if ('<' == ch)
							{
								// don't consume the opening angle
								mCursor.Retreat();
								bookmarks[state + 1] = mCursor.Position;
							}
							Standalone(attributes, bookmarks);
							done = true;
						}
						else if (System.Char.IsWhiteSpace(ch))
						{
							// whitespaces might be followed by next attribute or an equal sign
							// see Bug #891058 Bug in lexer.
							bookmarks[6] = bookmarks[2]; // setting the bookmark[0] is done in state 6 if applicable
							state = 6;
						}
						else if ('=' == ch)
							state = 2;
						break;
					
					case 2:  // equals hit
						if ((Page.EOF == ch) || ('>' == ch))
						{
							Empty(attributes, bookmarks);
							done = true;
						}
						else if ('\'' == ch)
						{
							state = 4;
							bookmarks[4] = bookmarks[3];
						}
						else if ('"' == ch)
						{
							state = 5;
							bookmarks[5] = bookmarks[3];
						}
						else if (System.Char.IsWhiteSpace(ch))
						{
							// collect white spaces after "=" into the assignment string;
							// do nothing
							// see Bug #891058 Bug in lexer.
						}
						else
							state = 3;
						break;
					
					case 3:  // within naked attribute value
						if ((Page.EOF == ch) || ('>' == ch))
						{
							Naked(attributes, bookmarks);
							done = true;
						}
						else if (System.Char.IsWhiteSpace(ch))
						{
							Naked(attributes, bookmarks);
							bookmarks[0] = bookmarks[4];
							state = 0;
						}
						break;
					
					case 4:  // within single quoted attribute value
						if (Page.EOF == ch)
						{
							Single_Quote(attributes, bookmarks);
							done = true; // complain?
						}
						else if ('\'' == ch)
						{
							Single_Quote(attributes, bookmarks);
							bookmarks[0] = bookmarks[5] + 1;
							state = 0;
						}
						break;
					
					case 5:  // within double quoted attribute value
						if (Page.EOF == ch)
						{
							Double_Quote(attributes, bookmarks);
							done = true; // complain?
						}
						else if ('"' == ch)
						{
							Double_Quote(attributes, bookmarks);
							bookmarks[0] = bookmarks[6] + 1;
							state = 0;
						}
						break;
						// patch for lexer state correction by
						// Gernot Fricke
						// See Bug # 891058 Bug in lexer.
					
					case 6:  // undecided for state 0 or 2
						// we have read white spaces after an attributte name
						if (Page.EOF == ch)
						{
							// same as last else clause
							Standalone(attributes, bookmarks);
							bookmarks[0] = bookmarks[6];
							mCursor.Retreat();
							state = 0;
						}
						else if (System.Char.IsWhiteSpace(ch))
						{
							// proceed
						}
						else if ('=' == ch)
							// yepp. the white spaces belonged to the equal.
						{
							bookmarks[2] = bookmarks[6];
							bookmarks[3] = bookmarks[7];
							state = 2;
						}
						else
						{
							// white spaces were not ended by equal
							// meaning the attribute was a stand alone attribute
							// now: create the stand alone attribute and rewind 
							// the cursor to the end of the white spaces
							// and restart scanning as whitespace attribute.
							Standalone(attributes, bookmarks);
							bookmarks[0] = bookmarks[6];
							mCursor.Retreat();
							state = 0;
						}
						break;
					
					default: 
						throw new System.SystemException("how the hell did we get in state " + state);
					
				}
			}
			
			return (MakeTag(start, mCursor.Position, attributes));
		}

		/// <summary> Create a tag node based on the current cursor and the one provided.</summary>
		/// <param name="start">The starting point of the node.
		/// </param>
		/// <param name="end">The ending point of the node.
		/// </param>
		/// <param name="attributes">The attributes parsed from the tag.
		/// </param>
		/// <exception cref="ParserException">If the nodefactory creation of the tag node fails.
		/// </exception>
		/// <returns> The new Tag node.
		/// </returns>
		protected internal virtual INode MakeTag(int start, int end, System.Collections.ArrayList attributes)
		{
			int length;
			INode ret;
			
			length = end - start;
			if (0 != length)
			{
				// return tag based on second character, '/', '%', Letter (ch), '!'
				if (2 > length)
					// this is an error
					return (MakeString(start, end));
				ret = NodeFactory.CreateTagNode(this.Page, start, end, attributes);
			}
			else
				ret = null;
			
			return (ret);
		}

		/// <summary> Parse a comment.
		/// Parse a remark markup.<p>
		/// From the <a href="http://www.w3.org/TR/html4/intro/sgmltut.html#h-3.2.4">
		/// HTML 4.01 Specification, W3C Recommendation 24 December 1999</a>
		/// http://www.w3.org/TR/html4/intro/sgmltut.html#h-3.2.4<p>
		/// <cite>
		/// 3.2.4 Comments<p>
		/// HTML comments have the following syntax:<p>
		/// <code>
		/// &lt;!-- this is a comment --&gt;<p>
		/// &lt;!-- and so is this one,<p>
		/// which occupies more than one line --&gt;<p>
		/// </code>
		/// White space is not permitted between the markup declaration
		/// open delimiter("&lt;!") and the comment open delimiter ("--"),
		/// but is permitted between the comment close delimiter ("--") and
		/// the markup declaration close delimiter ("&gt;").
		/// A common error is to include a string of hyphens ("---") within a comment.
		/// Authors should avoid putting two or more adjacent hyphens inside comments.
		/// Information that appears between comments has no special meaning
		/// (e.g., character references are not interpreted as such).
		/// Note that comments are markup.<p>
		/// </cite>
		/// <p>
		/// This method uses a state machine with the following states:
		/// <ol>
		/// <li>state 0 - prior to the first open delimiter</li>
		/// <li>state 1 - prior to the second open delimiter</li>
		/// <li>state 2 - prior to the first closing delimiter</li>
		/// <li>state 3 - prior to the second closing delimiter</li>
		/// <li>state 4 - prior to the terminating &gt;</li>
		/// </ol>
		/// <p>
		/// All comment text (everything excluding the &lt; and &gt;), is included
		/// in the remark text.
		/// We allow terminators like --!&gt; even though this isn't part of the spec.
		/// </summary>
		/// <param name="start">The position at which to start scanning.
		/// </param>
		/// <param name="quotesmart">If <code>true</code>, strings ignore quoted contents.
		/// </param>
		/// <returns> The parsed node.
		/// </returns>
		/// <exception cref="ParserException">If a problem occurs reading from the source.
		/// </exception>
		protected internal virtual INode ParseRemark(int start, bool quotesmart)
		{
			bool done;
			char ch;
			int state;
			
			done = false;
			state = 0;
			while (!done)
			{
				ch = mPage.GetCharacter(mCursor);
				if (Page.EOF == ch)
					done = true;
				else
					switch (state)
					{
						
						case 0:  // prior to the first open delimiter
							if ('>' == ch)
								done = true;
							if ('-' == ch)
								state = 1;
							else
								return (ParseString(start, quotesmart));
							break;
						
						case 1:  // prior to the second open delimiter
							if ('-' == ch)
							{
								// handle <!--> because netscape does
								ch = mPage.GetCharacter(mCursor);
								if (Page.EOF == ch)
									done = true;
								else if ('>' == ch)
									done = true;
								else
								{
									mCursor.Retreat();
									state = 2;
								}
							}
							else
								return (ParseString(start, quotesmart));
							break;
						
						case 2:  // prior to the first closing delimiter
							if ('-' == ch)
								state = 3;
							else if (Page.EOF == ch)
								return (ParseString(start, quotesmart)); // no terminator
							break;
						
						case 3:  // prior to the second closing delimiter
							if ('-' == ch)
								state = 4;
							else
								state = 2;
							break;
						
						case 4:  // prior to the terminating >
							if ('>' == ch)
								done = true;
							else if (('!' == ch) || ('-' == ch) || System.Char.IsWhiteSpace(ch))
							{
								// stay in state 4
							}
							else
								state = 2;
							break;
						
						default: 
							throw new System.SystemException("how the hell did we get in state " + state);
						
					}
			}
			
			return (MakeRemark(start, mCursor.Position));
		}

		/// <summary> Create a remark node based on the current cursor and the one provided.</summary>
		/// <param name="start">The starting point of the node.
		/// </param>
		/// <param name="end">The ending point of the node.
		/// </param>
		/// <exception cref="ParserException">If the nodefactory creation of the remark node fails.
		/// </exception>
		/// <returns> The new Remark node.
		/// </returns>
		protected internal virtual INode MakeRemark(int start, int end)
		{
			int length;
			INode ret;
			
			length = end - start;
			if (0 != length)
			{
				// return tag based on second character, '/', '%', Letter (ch), '!'
				if (2 > length)
					// this is an error
					return (MakeString(start, end));
				ret = NodeFactory.CreateRemarkNode(this.Page, start, end);
			}
			else
				ret = null;
			
			return (ret);
		}

		/// <summary> Parse a java server page node.
		/// Scan characters until "%&gt;" is encountered, or the input stream is
		/// exhausted, in which case <code>null</code> is returned.
		/// </summary>
		/// <param name="start">The position at which to start scanning.
		/// </param>
		/// <returns> The parsed node.
		/// </returns>
		/// <exception cref="ParserException">If a problem occurs reading from the source.
		/// </exception>
		protected internal virtual INode ParseJsp(int start)
		{
			bool done;
			char ch;
			int state;
			System.Collections.ArrayList attributes;
			int code;
			
			done = false;
			state = 0;
			code = 0;
			attributes = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
			// <%xyz%>
			// 012223d
			// <%=xyz%>
			// 0122223d
			// <%@xyz%d
			// 0122223d
			while (!done)
			{
				ch = mPage.GetCharacter(mCursor);
				switch (state)
				{
					
					case 0:  // prior to the percent
					switch (ch)
					{
							
						case '%':  // <%
							state = 1;
							break;
							// case Page.EOF: // <\0
							// case '>': // <>
							
						default: 
							done = true;
							break;
							
					}
						break;
					
					case 1:  // prior to the optional qualifier
					switch (ch)
					{
							
						//case Page.EOF: 
						case unchecked((char)(-1)):
							// <%\0
						case '>':  // <%>
							done = true;
							break;
							
						case '=': 
							// <%=
						case '@':  // <%@
							code = mCursor.Position;
							attributes.Add(new PageAttribute(mPage, start + 1, code, - 1, - 1, (char) 0));
							state = 2;
							break;
							
						default:  // <%x
							code = mCursor.Position - 1;
							attributes.Add(new PageAttribute(mPage, start + 1, code, - 1, - 1, (char) 0));
							state = 2;
							break;
							
					}
						break;
					
					case 2:  // prior to the closing percent
					switch (ch)
					{
							
						//case Page.EOF: 
						case unchecked((char)(-1)):
							// <%x\0
						case '>':  // <%x>
							done = true;
							break;
							
						case '\'': 
						case '"':  // <%???"
							state = ch;
							break;
							
						case '%':  // <%???%
							state = 3;
							break;
							
						default:  // <%???x
							break;
							
					}
						break;
					
					case 3: 
					switch (ch)
					{
							
						//case Page.EOF:  // <%x??%\0
						case unchecked((char)(-1)):
							done = true;
							break;
							
						case '>': 
							state = 4;
							done = true;
							break;
							
						default:  // <%???%x
							state = 2;
							break;
							
					}
						break;
					
					case '"': 
					switch (ch)
					{
							
						//case Page.EOF:  // <%x??"\0
						case unchecked((char)(-1)):
							done = true;
							break;
							
						case '"': 
							state = 2;
							break;
							
						default:  // <%???'??x
							break;
							
					}
						break;
					
					case '\'': 
					switch (ch)
					{
							
						//case Page.EOF:  // <%x??'\0
						case unchecked((char)(-1)):
							done = true;
							break;
							
						case '\'': 
							state = 2;
							break;
							
						default:  // <%???"??x
							break;
							
					}
						break;
					
					default: 
						throw new System.SystemException("how the hell did we get in state " + state);
					
				}
			}
			
			if (4 == state)
				// normal exit
			{
				if (0 != code)
				{
					state = mCursor.Position - 2; // reuse state
					attributes.Add(new PageAttribute(mPage, code, state, - 1, - 1, (char) 0));
					attributes.Add(new PageAttribute(mPage, state, state + 1, - 1, - 1, (char) 0));
				}
				else
					throw new System.SystemException("jsp with no code!");
			}
			else
				return (ParseString(start, true)); // hmmm, true?
			
			return (MakeTag(start, mCursor.Position, attributes));
		}

		/// <summary> Return CDATA as a text node.
		/// According to appendix <a href="http://www.w3.org/TR/html4/appendix/notes.html#notes-specifying-data">
		/// B.3.2 Specifying non-HTML data</a> of the
		/// <a href="http://www.w3.org/TR/html4/">HTML 4.01 Specification</a>:<br>
		/// <quote>
		/// <b>Element content</b><br>
		/// When script or style data is the content of an element (SCRIPT and STYLE),
		/// the data begins immediately after the element start tag and ends at the
		/// first ETAGO ("&lt;/") delimiter followed by a name start character ([a-zA-Z]);
		/// note that this may not be the element's end tag.
		/// Authors should therefore escape "&lt;/" within the content. Escape mechanisms
		/// are specific to each scripting or style sheet language.
		/// </quote>
		/// </summary>
		/// <returns> The <code>TextNode</code> of the CDATA or <code>null</code> if none.
		/// </returns>
		/// <exception cref="ParserException">If a problem occurs reading from the source.
		/// </exception>
		public virtual INode ParseCDATA()
		{
			return (ParseCDATA(false));
		}

		/// <summary> Return CDATA as a text node.
		/// Slightly less rigid than {@link #parseCDATA()} this method provides for
		/// parsing CDATA that may contain quoted strings that have embedded
		/// ETAGO ("&lt;/") delimiters and skips single and multiline comments.
		/// </summary>
		/// <param name="quotesmart">If <code>true</code> the strict definition of CDATA is
		/// extended to allow for single or double quoted ETAGO ("&lt;/") sequences.
		/// </param>
		/// <returns> The <code>TextNode</code> of the CDATA or <code>null</code> if none.
		/// </returns>
		/// <seealso cref="parseCDATA()">
		/// </seealso>
		/// <exception cref="ParserException">If a problem occurs reading from the source.
		/// </exception>
		public virtual INode ParseCDATA(bool quotesmart)
		{
			int start;
			int state;
			bool done;
			char quote;
			char ch;
			int end;
			
			start = mCursor.Position;
			state = 0;
			done = false;
			quote = (char) (0);
			while (!done)
			{
				ch = mPage.GetCharacter(mCursor);
				switch (state)
				{
					
					case 0:  // prior to ETAGO
					switch (ch)
					{
							
						//case Page.EOF: 
						case unchecked((char)(-1)):
							done = true;
							break;
							
						case '\'': 
							if (quotesmart)
								if (0 == quote)
									quote = '\'';
									// enter quoted state
								else if ('\'' == quote)
									quote = (char) (0); // exit quoted state
							break;
							
						case '"': 
							if (quotesmart)
								if (0 == quote)
									quote = '"';
									// enter quoted state
								else if ('"' == quote)
									quote = (char) (0); // exit quoted state
							break;
							
						case '\\': 
							if (quotesmart)
								if (0 != quote)
								{
									ch = mPage.GetCharacter(mCursor); // try to consume escaped character
									if (Page.EOF == ch)
										done = true;
									else if ((ch != '\\') && (ch != quote))
										mCursor.Retreat(); // unconsume char if character was not an escapable char.
								}
							break;
							
						case '/': 
							if (quotesmart)
								if (0 == quote)
								{
									// handle multiline and double slash comments (with a quote)
									ch = mPage.GetCharacter(mCursor);
									if (Page.EOF == ch)
										done = true;
									else if ('/' == ch)
									{
										do 
											ch = mPage.GetCharacter(mCursor);
										while ((Page.EOF != ch) && ('\n' != ch));
									}
									else if ('*' == ch)
									{
										do 
										{
											do 
												ch = mPage.GetCharacter(mCursor);
											while ((Page.EOF != ch) && ('*' != ch));
											ch = mPage.GetCharacter(mCursor);
											if (ch == '*')
												mCursor.Retreat();
										}
										while ((Page.EOF != ch) && ('/' != ch));
									}
									else
										mCursor.Retreat();
								}
							break;
							
						case '<': 
							if (quotesmart)
							{
								if (0 == quote)
									state = 1;
							}
							else
								state = 1;
							break;
							
						default: 
							break;
							
					}
						break;
					
					case 1:  // <
					switch (ch)
					{
							
						//case Page.EOF: 
						case unchecked((char)(-1)):
							done = true;
							break;
							
						case '/': 
							state = 2;
							break;
							
						default: 
							state = 0;
							break;
							
					}
						break;
					
					case 2:  // </
						if (Page.EOF == ch)
							done = true;
						else if (System.Char.IsLetter(ch))
						{
							done = true;
							// back up to the start of ETAGO
							mCursor.Retreat();
							mCursor.Retreat();
							mCursor.Retreat();
						}
						else
							state = 0;
						break;
					
					default: 
						throw new System.SystemException("how the fuck did we get in state " + state);
					
				}
			}
			end = mCursor.Position;
			
			return (MakeString(start, end));
		}

		//
		// NodeFactory interface
		//
		
		/// <summary> Create a new string node.</summary>
		/// <param name="page">The page the node is on.
		/// </param>
		/// <param name="start">The beginning position of the string.
		/// </param>
		/// <param name="end">The ending positiong of the string.
		/// </param>
		/// <returns> The created Text node.
		/// </returns>
		public virtual IText CreateStringNode(Page page, int start, int end)
		{
			return (new TextNode(page, start, end));
		}
		
		/// <summary> Create a new remark node.</summary>
		/// <param name="page">The page the node is on.
		/// </param>
		/// <param name="start">The beginning position of the remark.
		/// </param>
		/// <param name="end">The ending positiong of the remark.
		/// </param>
		/// <returns> The created Remark node.
		/// </returns>
		public virtual IRemark CreateRemarkNode(Page page, int start, int end)
		{
			return (new RemarkNode(page, start, end));
		}
		
		/// <summary> Create a new tag node.
		/// Note that the attributes vector contains at least one element,
		/// which is the tag name (standalone attribute) at position zero.
		/// This can be used to decide which type of node to create, or
		/// gate other processing that may be appropriate.
		/// </summary>
		/// <param name="page">The page the node is on.
		/// </param>
		/// <param name="start">The beginning position of the tag.
		/// </param>
		/// <param name="end">The ending positiong of the tag.
		/// </param>
		/// <param name="attributes">The attributes contained in this tag.
		/// </param>
		/// <returns> The created Tag node.
		/// </returns>
		public virtual ITag CreateTagNode(Page page, int start, int end, System.Collections.ArrayList attributes)
		{
			return (new TagNode(page, start, end, attributes));
		}
	}
}
