// ***************************************************************
//  Parser   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Support;
using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Visitors;
using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Tags;
using Winista.Text.HtmlParser.Http;
using Winista.Text.HtmlParser.Data;
using Winista.Text.HtmlParser.Extractors;

namespace Winista.Text.HtmlParser
{
	/// <summary> The main parser class.
	/// This is the primary class of the HTML Parser library. It provides
	/// constructors that take a {@link #Parser(String) String},
	/// a {@link #Parser(URLConnection) URLConnection}, or a
	/// {@link #Parser(Lexer) Lexer}.  In the case of a String, an
	/// attempt is made to open it as a URL, and if that fails it assumes it is a
	/// local disk file. If you want to actually parse a String, use
	/// {@link #setInputHTML setInputHTML()} after using the
	/// {@link #Parser() no-args} constructor, or use {@link #createParser}.
	/// <p>The Parser provides access to the contents of the
	/// page, via a {@link #elements() NodeIterator}, a
	/// {@link #parse(NodeFilter) NodeList} or a
	/// {@link #visitAllNodesWith NodeVisitor}.
	/// <p>Typical usage of the parser is:
	/// <code>
	/// <pre>
	/// Parser parser = new Parser ("http://whatever");
	/// NodeList list = parser.parse ();
	/// // do something with your list of nodes.
	/// </pre>
	/// </code></p>
	/// <p>What types of nodes and what can be done with them is dependant on the
	/// setup, but in general a node can be converted back to HTML and it's
	/// children (enclosed nodes) and parent can be obtained, because nodes are
	/// nested. See the {@link Node} interface.</p>
	/// <p>For example, if the URL contains:<br>
	/// <code>
	/// {@.html
	/// <html>
	/// <head>
	/// <title>Mondays -- What a bad idea.</title>
	/// </head>
	/// <body BGCOLOR="#FFFFFF">
	/// Most people have a pathological hatred of Mondays...
	/// </body>
	/// </html>}
	/// </code><br>
	/// and the example code above is used, the list contain only one element, the
	/// {@.html <html>} node.  This node is a <see cref="ITag"></see>,
	/// which is an object of class
	/// <see cref="Html"></see> if the default <see cref="NodeFactory"></see>
	/// (a <see cref="PrototypicalNodeFactory"></see>) is used.</p>
	/// <p>To get at further content, the children of the top
	/// level nodes must be examined. When digging through a node list one must be
	/// conscious of the possibility of whitespace between nodes, e.g. in the example
	/// above:
	/// <code>
	/// <pre>
	/// Node node = list.elementAt (0);
	/// NodeList sublist = node.getChildren ();
	/// System.out.println (sublist.size ());
	/// </pre>
	/// </code>
	/// would print out 5, not 2, because there are newlines after {@.html <html>},
	/// {@.html </head>} and {@.html </body>} that are children of the HTML node
	/// besides the {@.html <head>} and {@.html} nodes.
	/// <p>Because processing nodes is so common, two interfaces are provided to
	/// ease this task, Filters and Visitors.
	/// </summary>
	[Serializable]
	public class Parser
	{
		#region Class Members
		// Please don't change the formatting of the version variables below.
		// This is done so as to facilitate ant script processing.
		
		/// <summary> The floating point version number.</summary>
		public const double VERSION_NUMBER = 1.6;
		
		/// <summary> The type of version ({@value}).</summary>
		public const System.String VERSION_TYPE = "Release Build";
		
		/// <summary> The date of the version ({@value}).</summary>
		public const System.String VERSION_DATE = "Jan 5, 2006";
		
		// End of formatting
		
		/// <summary> The display version ({@value}).</summary>
		public static readonly System.String VERSION_STRING = "" + VERSION_NUMBER + " (" + VERSION_TYPE + " " + VERSION_DATE + ")";
		
		/// <summary> Feedback object.</summary>
		protected internal IParserFeedBack mFeedback;
		
		/// <summary> The html lexer associated with this parser.</summary>
		protected internal Lexer mLexer;
		
		/// <summary> A quiet message sink.
		/// Use this for no feedback.
		/// </summary>
		public static readonly IParserFeedBack DEVNULL;
		
		/// <summary> A verbose message sink.
		/// Use this for output on <code>System.out</code>.
		/// </summary>
		public static readonly IParserFeedBack STDOUT = new DefaultParserFeedback();

		protected HttpProtocol m_obHttpProtocol;

		#endregion

		//
		// Static methods
		//
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="strPath"></param>
		public static void SetConfigLocation(String strPath)
		{
			ParserConf.GetConfiguration().RootPath = strPath;
		}

		#region Class Constructors

		/// <summary> Creates the parser on an input string.</summary>
		/// <param name="html">The string containing HTML.
		/// </param>
		/// <param name="charset"><em>Optional</em>. The character set encoding that will
		/// be reported by {@link #getEncoding}. If charset is <code>null</code>
		/// the default character set is used.
		/// </param>
		/// <returns> A parser with the <code>html</code> string as input.
		/// </returns>
		public static Parser CreateParser(System.String html, System.String charset)
		{
			Parser ret;
			
			if (null == html)
			{
				throw new System.ArgumentException("Html cannot be null");
			}
			ret = new Parser(new Lexer(new Page(html, charset)));
			
			return (ret);
		}

		/// <summary> Zero argument constructor.
		/// The parser is in a safe but useless state parsing an empty string.
		/// Set the lexer or connection using {@link #setLexer}
		/// or {@link #setConnection}.
		/// </summary>
		/// <seealso cref="setLexer(Lexer)">
		/// </seealso>
		/// <seealso cref="setConnection(URLConnection)">
		/// </seealso>
		public Parser():this(new Lexer(new Page("")), DEVNULL)
		{
		}
		
		/// <summary> Construct a parser using the provided lexer and feedback object.
		/// This would be used to create a parser for special cases where the
		/// normal creation of a lexer on a URLConnection needs to be customized.
		/// </summary>
		/// <param name="lexer">The lexer to draw characters from.
		/// </param>
		/// <param name="fb">The object to use when information,
		/// warning and error messages are produced. If <em>null</em> no feedback
		/// is provided.
		/// </param>
		public Parser(Lexer lexer, IParserFeedBack fb)
		{
			Feedback = fb;
			if (null == lexer)
				throw new System.ArgumentException("lexer cannot be null");
			Lexer = lexer;
			NodeFactory = new PrototypicalNodeFactory();
		}
		
		/// <summary> Construct a parser using the provided lexer.
		/// A feedback object printing to {@link #STDOUT System.out} is used.
		/// This would be used to create a parser for special cases where the
		/// normal creation of a lexer on a URLConnection needs to be customized.
		/// </summary>
		/// <param name="lexer">The lexer to draw characters from.
		/// </param>
		public Parser(Lexer lexer):this(lexer, STDOUT)
		{
		}

		/// <summary> Creates a Parser object with the location of the resource (URL or file).
		/// A DefaultHTMLParserFeedback object is used for feedback.
		/// </summary>
		/// <throws>  ParserException If the resourceLocn argument does not resolve </throws>
		/// <param name="strUrl">Either the URL or the filename (autodetects).
		/// </param>
		public Parser(System.Uri strUrl):this(strUrl, STDOUT)
		{
		}

		/// <summary> Constructor for custom HTTP access.
		/// This would be used to create a parser for a URLConnection that needs
		/// a special setup or negotiation conditioning beyond what is available
		/// from the {@link #getConnectionManager ConnectionManager}.
		/// </summary>
		/// <param name="connection">A fully conditioned connection. The connect()
		/// method will be called so it need not be connected yet.
		/// </param>
		/// <param name="fb">The object to use for message communication.
		/// </param>
		/// <throws>  ParserException If the creation of the underlying Lexer </throws>
		/// <summary> cannot be performed.
		/// </summary>
		public Parser(HttpProtocol connection, IParserFeedBack fb):this(new Lexer(connection), fb)
		{
			this.m_obHttpProtocol = connection;
		}

		/// <summary> Creates a Parser object with the location of the resource (URL or file)
		/// You would typically create a DefaultHTMLParserFeedback object and pass
		/// it in.
		/// </summary>
		/// <param name="resourceLocn">Either the URL or the filename (autodetects).
		/// A standard HTTP GET is performed to read the content of the URL.
		/// </param>
		/// <param name="feedback">The HTMLParserFeedback object to use when information,
		/// warning and error messages are produced. If <em>null</em> no feedback
		/// is provided.
		/// </param>
		/// <throws>  ParserException If the URL is invalid. </throws>
		public Parser(System.Uri resourceLocn, IParserFeedBack feedback)
			:this(new HttpProtocol(resourceLocn), feedback)
		{
		}

		/// <summary> Construct a parser using the provided HttpProtocol.
		/// A feedback object printing to System.Console is used.
		/// </summary>
		/// <param name="connection">HttpProtocol object.
		/// </param>
		/// <throws>  ParserException If the creation of the underlying Lexer </throws>
		/// <summary> cannot be performed.
		/// </summary>
		public Parser(HttpProtocol connection):this(connection, STDOUT)
		{
		}

		#endregion

		/// <summary> Return the version string of this parser.</summary>
		/// <returns> A string of the form:
		/// <pre>
		/// "[floating point number] ([build-type] [build-date])"
		/// </pre>
		/// </returns>
		public static System.String Version
		{
			get
			{
				return (VERSION_STRING);
			}
			
		}
		/// <summary> Return the version number of this parser.</summary>
		/// <returns> A floating point number, the whole number part is the major
		/// version, and the fractional part is the minor version.
		/// </returns>
		public static double VersionNumber
		{
			get
			{
				return (VERSION_NUMBER);
			}
			
		}
		
		/// <summary> Return the current URL being parsed.</summary>
		/// <returns> The current url. This is the URL for the current page.
		/// A string passed into the constructor or set via setURL may be altered,
		/// for example, a file name may be modified to be a URL.
		/// </returns>
		/// <summary> Set the URL for this parser.
		/// This method creates a new Lexer reading from the given URL.
		/// Trying to set the url to null or an empty string is a no-op.
		/// </summary>
		/// <throws>  ParserException If the url is invalid or creation of the </throws>
		/// <summary> underlying Lexer cannot be performed.
		/// </summary>
		virtual public System.String URL
		{
			get
			{
				return (Lexer.Page.Url);
			}
			
			set
			{
				Lexer.Page.Url = value;
			}
			
		}

		/// <summary> Get the encoding for the page this parser is reading from.
		/// This item is set from the HTTP header but may be overridden by meta
		/// tags in the head, so this may change after the head has been parsed.
		/// </summary>
		/// <returns> The encoding currently in force.
		/// </returns>
		/// <summary> Set the encoding for the page this parser is reading from.</summary>
		/// <throws>  ParserException If the encoding change causes characters that </throws>
		/// <summary> have already been consumed to differ from the characters that would
		/// have been seen had the new encoding been in force.
		/// </summary>
		/// <seealso cref="EncodingChangeException">
		/// </seealso>
		virtual public System.String Encoding
		{
			get
			{
				return (Lexer.Page.Encoding);
			}
			
			set
			{
				Lexer.Page.Encoding = value;
			}
		}

		/// <summary> Return the current connection.</summary>
		/// <returns> The connection either created by the parser or passed into this
		/// parser via <see cref="Connection"></see>.
		/// </returns>
		/// <summary> Set the connection for this parser.
		/// This method creates a new <code>Lexer</code> reading from the connection.
		/// </summary>
		/// <param name="Connection">A fully conditioned connection. The connect()
		/// method will be called so it need not be connected yet.
		/// </param>
		/// <exception cref="ParserException"> ParserException if the character set specified in the
		/// HTTP header is not supported, or an i/o exception occurs creating the
		/// lexer.
		/// </exception>
		virtual public HttpProtocol Connection
		{
			get
			{
				return (Lexer.Page.Connection);
			}
			
			set
			{
				if (null == value)
					throw new System.ArgumentException("connection cannot be null");
				Lexer = new Lexer(value);
			}
			
		}

		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Returns the lexer associated with the parser</summary>
		/// <returns> The current lexer.
		/// </returns>
		/// <summary> Set the lexer for this parser.
		/// The current NodeFactory is transferred to (set on) the given lexer,
		/// since the lexer owns the node factory object.
		/// It does not adjust the <code>feedback</code> object.
		/// Trying to set the lexer to <code>null</code> is a no-op.
		/// </summary>
		virtual public Lexer Lexer
		{
			get
			{
				return (mLexer);
			}
			
			set
			{
				INodeFactory factory;
				System.String type;
				
				if (null != value)
				{
					// move a node factory that's been set to the new lexer
					factory = null;
					if (null != Lexer)
						factory = Lexer.NodeFactory;
					if (null != factory)
						value.NodeFactory = factory;
					mLexer = value;
					// warn about content that's not likely text
					type = mLexer.Page.ContentType;
					if (type != null && !type.StartsWith("text"))
						Feedback.Warning("URL " + mLexer.Page.Url + " does not contain text");
				}
			}
			
		}

		/// <summary> Get the current node factory.</summary>
		/// <returns> The current lexer's node factory.
		/// </returns>
		/// <summary> Set the current node factory.</summary>
		virtual public INodeFactory NodeFactory
		{
			get
			{
				return (Lexer.NodeFactory);
			}
			
			set
			{
				if (null == value)
					throw new System.ArgumentException("node factory cannot be null");
				Lexer.NodeFactory = value;
			}
			
		}

		/// <summary> Returns the current feedback object.</summary>
		/// <returns> The feedback object currently being used.
		/// </returns>
		/// <summary> Sets the feedback object used in scanning.</summary>
		virtual public IParserFeedBack Feedback
		{
			get
			{
				return (mFeedback);
			}
			
			set
			{
				if (null == value)
					mFeedback = DEVNULL;
				else
					mFeedback = value;
			}
			
		}
		/// <summary> Initializes the parser with the given input HTML String.</summary>
		/// <throws>  ParserException If a error occurs in setting up the </throws>
		/// <summary> underlying Lexer.
		/// </summary>
		virtual public System.String InputHTML
		{
			set
			{
				if (null == value)
					throw new System.ArgumentException("html cannot be null");
				if (!"".Equals(value))
					Lexer = new Lexer(new Page(value));
			}
		}

		/// <summary> Reset the parser to start from the beginning again.
		/// This assumes support for a reset from the underlying
		/// <see cref="Source"></see> object.
		/// <p>This is cheaper (in terms of time) than resetting the URL, i.e.
		/// <pre>
		/// parser.setURL (parser.getURL ());
		/// </pre>
		/// because the page is not refetched from the internet.
		/// <em>Note: the nodes returned on the second parse are new
		/// nodes and not the same nodes returned on the first parse. If you
		/// want the same nodes for re-use, collect them in a NodeList with
		/// Parse(null)} and operate on the NodeList.</em>
		/// </summary>
		public virtual void  Reset()
		{
			Lexer.Reset();
		}
		
		/// <summary> Returns an iterator (enumeration) over the html nodes.
		/// Nodes can be of three main types:
		/// <ul>
		/// <li><see cref="TagNode"></see></li>
		/// <li><see cref="TextNode"></see></li>
		/// <li><see cref="RemarkNode"></see></li>
		/// </ul>
		/// In general, when parsing with an iterator or processing a NodeList,
		/// you will need to use recursion. For example:
		/// <code>
		/// <pre>
		/// void processMyNodes (Node node)
		/// {
		/// if (node instanceof TextNode)
		/// {
		/// // downcast to TextNode
		/// TextNode text = (TextNode)node;
		/// // do whatever processing you want with the text
		/// System.out.println (text.getText ());
		/// }
		/// if (node instanceof RemarkNode)
		/// {
		/// // downcast to RemarkNode
		/// RemarkNode remark = (RemarkNode)node;
		/// // do whatever processing you want with the comment
		/// }
		/// else if (node instanceof TagNode)
		/// {
		/// // downcast to TagNode
		/// TagNode tag = (TagNode)node;
		/// // do whatever processing you want with the tag itself
		/// // ...
		/// // process recursively (nodes within nodes) via getChildren()
		/// NodeList nl = tag.GetChildren ();
		/// if (null != nl)
		/// for (NodeIterator i = nl.elements (); i.hasMoreElements (); )
		/// processMyNodes (i.nextNode ());
		/// }
		/// }
		/// 
		/// Parser parser = new Parser ("http://www.yahoo.com");
		/// for (NodeIterator i = parser.elements (); i.hasMoreElements (); )
		/// processMyNodes (i.nextNode ());
		/// </pre>
		/// </code>
		/// </summary>
		/// <throws>  ParserException If a parsing error occurs. </throws>
		/// <returns> An iterator over the top level nodes (usually {@.html <html>}).
		/// </returns>
		public virtual INodeIterator Elements()
		{
			return (new IteratorImpl(Lexer, Feedback));
		}
		
		/// <summary> Parse the given resource, using the filter provided.
		/// This can be used to extract information from specific nodes.
		/// When used with a <code>null</code> filter it returns an
		/// entire page which can then be modified and converted back to HTML
		/// (Note: the synthesis use-case is not handled very well; the parser
		/// is more often used to extract information from a web page).
		/// <p>For example, to replace the entire contents of the HEAD with a
		/// single TITLE tag you could do this:
		/// <pre>
		/// NodeList nl = parser.parse (null); // here is your two node list
		/// NodeList heads = nl.extractAllNodesThatMatch (new TagNameFilter ("HEAD"))
		/// if (heads.size () > 0) // there may not be a HEAD tag
		/// {
		/// Head head = heads.elementAt (0); // there should be only one
		/// head.removeAll (); // clean out the contents
		/// Tag title = new TitleTag ();
		/// title.setTagName ("title");
		/// title.setChildren (new NodeList (new TextNode ("The New Title")));
		/// Tag title_end = new TitleTag ();
		/// title_end.setTagName ("/title");
		/// title.setEndTag (title_end);
		/// head.add (title);
		/// }
		/// System.out.println (nl.toHtml ()); // output the modified HTML
		/// </pre>
		/// </p>
		/// </summary>
		/// <returns> The list of matching nodes (for a <code>null</code>
		/// filter this is all the top level nodes).
		/// </returns>
		/// <param name="filter">The filter to apply to the parsed nodes,
		/// or <code>null</code> to retrieve all the top level nodes.
		/// </param>
		/// <throws>  ParserException If a parsing error occurs. </throws>
		public virtual NodeList Parse(INodeFilter filter)
		{
			INodeIterator e;
			INode node;
			NodeList ret;
			
			ret = new NodeList();
			for (e = Elements(); e.HasMoreNodes(); )
			{
				node = e.NextNode();
				if (null != filter)
					node.CollectInto(ret, filter);
				else
					ret.Add(node);
			}
			
			return (ret);
		}
		
		/// <summary> Apply the given visitor to the current page.
		/// The visitor is passed to the <code>accept()</code> method of each node
		/// in the page in a depth first traversal. The visitor
		/// <code>beginParsing()</code> method is called prior to processing the
		/// page and <code>finishedParsing()</code> is called after the processing.
		/// </summary>
		/// <param name="visitor">The visitor to visit all nodes with.
		/// </param>
		/// <throws>  ParserException If a parse error occurs while traversing </throws>
		/// <summary> the page with the visitor.
		/// </summary>
		public virtual void VisitAllNodesWith(NodeVisitor visitor)
		{
			INode node;
			visitor.BeginParsing();
			for (INodeIterator e = Elements(); e.HasMoreNodes(); )
			{
				node = e.NextNode();
				node.Accept(visitor);
			}
			visitor.FinishedParsing();
		}
		
		/// <summary> Extract all nodes matching the given filter.</summary>
		/// <param name="filter">The filter to be applied to the nodes.
		/// </param>
		/// <throws>  ParserException If a parse error occurs. </throws>
		/// <returns> A list of nodes matching the filter criteria,
		/// i.e. for which the filter's accept method
		/// returned <code>true</code>.
		/// </returns>
		public virtual NodeList ExtractAllNodesThatMatch(INodeFilter filter)
		{
			INodeIterator e;
			NodeList ret;
			
			ret = new NodeList();
			for (e = Elements(); e.HasMoreNodes(); )
				e.NextNode().CollectInto(ret, filter);			
			return (ret);
		}
		
		/// <summary> Convenience method to extract all nodes of a given class type.
		/// Equivalent to
		/// <code>extractAllNodesThatMatch (new NodeClassFilter (nodeType))</code>.
		/// </summary>
		/// <param name="nodeType">The class of the nodes to collect.
		/// </param>
		/// <throws>  ParserException If a parse error occurs. </throws>
		/// <returns> A list of nodes which have the class specified.
		/// </returns>
		/// <deprecated> Use extractAllNodesThatMatch (new NodeClassFilter (cls)).
		/// </deprecated>
		/// <seealso cref="ExtractAllNodesThatAre">
		/// </seealso>
		public virtual INode[] ExtractAllNodesThatAre(System.Type nodeType)
		{
			NodeList ret;
			
			ret = ExtractAllNodesThatMatch(new NodeClassFilter(nodeType));
			
			return (ret.ToNodeArray());
		}

		/// <summary>
		/// Gets all links contained in the page
		/// </summary>
		/// <returns></returns>
		public NodeList GetAllOutLinks()
		{
			INodeFilter filter = new NodeClassFilter(typeof(LinkTag));
			return this.Parse(filter);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="iDepth"></param>
		/// <param name="bFlatHierarchy"></param>
		/// <returns></returns>
		public PageData GetAllOutLinks(Int32 iDepth, bool bFlatHierarchy)
		{
			if (iDepth <= 0)
			{
				iDepth = 1;
			}

			PageData obPage = new PageData();
			obPage.m_strUrl = this.URL;
			ProcessPageForLinks(obPage, 0, iDepth-1, bFlatHierarchy);
			
			return obPage;
		}

		/// <summary>
		/// Returns all image links contained in the page.
		/// </summary>
		/// <param name="iDepth"></param>
		/// <param name="bFlatHierarchy"></param>
		/// <returns></returns>
		public PageData GetAllImageLinks(Int32 iDepth, bool bFlatHierarchy)
		{
			if (iDepth <= 0)
			{
				iDepth = 1;
			}

			PageData obPage = new PageData();
			obPage.m_strUrl = this.URL;
			ProcessPageForImageLinks(obPage, 0, iDepth-1, bFlatHierarchy);
			
			return obPage;
		}

		/// <summary>
		/// Gets all mail links in the page
		/// </summary>
		/// <returns></returns>
		public virtual NodeList GetAllMailLinks()
		{
			INodeFilter filter = new AndFilter(new NodeClassFilter(typeof(LinkTag)), new MailLinkFilter());
			return this.Parse(filter);
		}

		/// <summary>
		/// Fetches content for the specified URL and analyze it.
		/// </summary>
		/// <returns><see cref="PageData"></see> object</returns>
		/// <remarks>
		/// <p>
		/// This method fetches content from the specified URL and then analyzes it to put
		/// data into respective components like <see cref="HeaderData"></see>, <see cref="LinkDataCollection"></see> etc.
		/// inside <see cref="PageData"></see> object.
		/// </p>
		/// </remarks>
		public PageData AnalyzePage()
		{
			PageExtractor obPageExtractor = new PageExtractor(this);
			PageData obPageData = obPageExtractor.GetPageData();
			return obPageData;
		}

		#region Helper Methods
		private void ProcessPageForLinks(PageData obPageData, Int32 iLevel, Int32 iStopLevel, bool bFlatHierarchy)
		{
			LinkDataCollection obLinks = new LinkDataCollection();
			LinkExtractor obLinkExtractor = new LinkExtractor(obPageData.Url, iLevel);
			do
			{
				LinkStatus status = obLinkExtractor.ExtractLinks();
				LinkDataCollection tempColl = obLinkExtractor.Links;

				ICollectionSupport.AddAll(obPageData.m_Outlinks, tempColl);

				if (iLevel >= iStopLevel)
				{
					break;
				}
				iLevel++;
				tempColl = new LinkDataCollection();
				foreach(LinkData obLinkData in obPageData.m_Outlinks)
				{
					if (obLinkData.LinkType == LinkType.Outlink)
					{
						PageData obPage = new PageData();
						obPage.m_strUrl = obLinkData.Url;
						ProcessPageForLinks(obPage, iLevel, iStopLevel, bFlatHierarchy);
						if (bFlatHierarchy)
						{
							ICollectionSupport.AddAll(tempColl, obPage.OutLinks);
						}
					}
				}
				if (bFlatHierarchy)
				{
					ICollectionSupport.AddAll(obPageData.m_Outlinks, tempColl);
				}
			}while(true);
		}

		private void ProcessPageForImageLinks(PageData obPageData, Int32 iLevel, Int32 iStopLevel, bool bFlatHierarchy)
		{
			LinkDataCollection obLinks = new LinkDataCollection();
			LinkExtractor obLinkExtractor = new LinkExtractor(obPageData.Url, iLevel);
			do
			{
				LinkStatus status = obLinkExtractor.ExtractLinks();
				LinkDataCollection tempColl = obLinkExtractor.Links;

				ICollectionSupport.AddAll(obPageData.m_Outlinks, tempColl);

				if (iLevel >= iStopLevel)
				{
					break;
				}
				iLevel++;
				tempColl = new LinkDataCollection();
				foreach(LinkData obLinkData in obPageData.m_Outlinks)
				{
					if (obLinkData.LinkType == LinkType.Outlink)
					{
						PageData obPage = new PageData();
						obPage.m_strUrl = obLinkData.Url;
						ProcessPageForLinks(obPage, iLevel, iStopLevel, bFlatHierarchy);
						if (bFlatHierarchy)
						{
							ICollectionSupport.AddAll(tempColl, obPage.OutLinks);
						}
					}
				}
				if (bFlatHierarchy)
				{
					ICollectionSupport.AddAll(obPageData.m_Outlinks, tempColl);
				}
			}while(true);
		}
		#endregion

		static Parser()
		{
			DEVNULL = new DefaultParserFeedback(DefaultParserFeedback.QUIET);
		}
	}
}
