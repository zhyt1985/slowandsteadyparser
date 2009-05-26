// ***************************************************************
//  TagNode   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Scanners;
using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Visitors;

namespace Winista.Text.HtmlParser.Nodes
{
	/// <summary> TagNode represents a generic tag.
	/// If no scanner is registered for a given tag name, this is what you get.
	/// This is also the base class for all tags created by the parser.
	/// </summary>
	[Serializable]
	public class TagNode : AbstractNode, ITag
	{
		/// <summary> An empty set of tag names.</summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'NONE '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		private static readonly System.String[] NONE = new System.String[0];
		
		/// <summary> The scanner for this tag.</summary>
		private IScanner mScanner;
		
		/// <summary> The default scanner for non-composite tags.</summary>
		//UPGRADE_NOTE: Final was removed from the declaration of 'mDefaultScanner '. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1003'"
		protected internal static readonly IScanner mDefaultScanner = new TagScanner();
		
		/// <summary> The tag attributes.
		/// Objects of type {@link Attribute}.
		/// The first element is the tag name, subsequent elements being either
		/// whitespace or real attributes.
		/// </summary>
		protected internal System.Collections.ArrayList mAttributes;
		
		/// <summary> Set of tags that breaks the flow.</summary>
		protected internal static System.Collections.Hashtable breakTags;

		/// <summary> Create an empty tag.</summary>
		public TagNode():this(null, - 1, - 1, System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10)))
		{
		}

		/// <summary> Create a tag with the location and attributes provided</summary>
		/// <param name="page">The page this tag was read from.
		/// </param>
		/// <param name="start">The starting offset of this node within the page.
		/// </param>
		/// <param name="end">The ending offset of this node within the page.
		/// </param>
		/// <param name="attributes">The list of attributes that were parsed in this tag.
		/// </param>
		/// <seealso cref="Attribute">
		/// </seealso>
		public TagNode(Page page, int start, int end, System.Collections.ArrayList attributes):base(page, start, end)
		{
			
			mScanner = mDefaultScanner;
			mAttributes = attributes;
			if ((null == mAttributes) || (0 == mAttributes.Count))
			{
				System.String[] names;
				
				names = Ids;
				if ((null != names) && (0 != names.Length))
					TagName = names[0];
				else
					TagName = ""; // make sure it's not null
			}
		}

		/// <summary> Create a tag like the one provided.</summary>
		/// <param name="tag">The tag to emulate.
		/// </param>
		/// <param name="scanner">The scanner for this tag.
		/// </param>
		public TagNode(TagNode tag, TagScanner scanner):this(tag.Page, tag.TagBegin, tag.TagEnd, tag.AttributesEx)
		{
			ThisScanner = scanner;
		}

		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Gets the attributes in the tag.</summary>
		/// <returns> Returns the list of {@link Attribute Attributes} in the tag.
		/// The first element is the tag name, subsequent elements being either
		/// whitespace or real attributes.
		/// </returns>
		/// <summary> Sets the attributes.
		/// NOTE: Values of the extended hashtable are two element arrays of String,
		/// with the first element being the original name (not uppercased),
		/// and the second element being the value.
		/// </summary>
		/// <param name="attribs">The attribute collection to set.
		/// </param>
		virtual public System.Collections.ArrayList AttributesEx
		{
			get
			{
				return (mAttributes);
			}
			
			set
			{
				mAttributes = value;
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Gets the attributes in the tag.
		/// This is not the preferred  method to get attributes, see {@link
		/// #getAttributesEx getAttributesEx} which returns a list of {@link
		/// Attribute} objects, which offer more information than the simple
		/// <code>String</code> objects available from this <code>Hashtable</code>.
		/// </summary>
		/// <returns> Returns a list of name/value pairs representing the attributes.
		/// These are not in order, the keys (names) are converted to uppercase and the values
		/// are not quoted, even if they need to be. The table <em>will</em> return
		/// <code>null</code> if there was no value for an attribute (no equals
		/// sign or nothing to the right of the equals sign). A special entry with
		/// a key of SpecialHashtable.TAGNAME ("$<TAGNAME>$") holds the tag name.
		/// The conversion to uppercase is performed with an ENGLISH locale.
		/// </returns>
		/// <summary> Sets the attributes.
		/// A special entry with a key of SpecialHashtable.TAGNAME ("$<TAGNAME>$")
		/// sets the tag name.
		/// </summary>
		/// <param name="attributes">The attribute collection to set.
		/// </param>
		virtual public System.Collections.Hashtable Attributes
		{
			get
			{
				System.Collections.ArrayList attributes;
				TagAttribute attribute;
				System.String value_Renamed;
				System.Collections.Hashtable ret;
				
				ret = new SpecialHashtable();
				attributes = AttributesEx;
				if (0 < attributes.Count)
				{
					// special handling for the node name
					attribute = (TagAttribute) attributes[0];
					ret[SpecialHashtable.TAGNAME] = attribute.GetName().ToUpper(new System.Globalization.CultureInfo("en"));
					// the rest
					for (int i = 1; i < attributes.Count; i++)
					{
						attribute = (TagAttribute) attributes[i];
						if (!attribute.Whitespace)
						{
							value_Renamed = attribute.GetValue();
							if (attribute.Empty)
								value_Renamed = SpecialHashtable.NOTHING;
							if (null == value_Renamed)
								value_Renamed = SpecialHashtable.NULLVALUE;
							ret[attribute.GetName().ToUpper(new System.Globalization.CultureInfo("en"))] = value_Renamed;
						}
					}
				}
				else
					ret[SpecialHashtable.TAGNAME] = "";
				
				return (ret);
			}
			
			set
			{
				System.Collections.ArrayList att;
				System.String key;
				System.String value_Renamed;
				char quote;
				TagAttribute attribute;
				
				att = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
				//UPGRADE_TODO: Method 'java.util.Enumeration.hasMoreElements' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilEnumerationhasMoreElements'"
				for (System.Collections.IEnumerator e = value.Keys.GetEnumerator(); e.MoveNext(); )
				{
					//UPGRADE_TODO: Method 'java.util.Enumeration.nextElement' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javautilEnumerationnextElement'"
					key = ((System.String) e.Current);
					value_Renamed = ((System.String) value[key]);
					if (value_Renamed.StartsWith("'") && value_Renamed.EndsWith("'") && (2 <= value_Renamed.Length))
					{
						quote = '\'';
						value_Renamed = value_Renamed.Substring(1, (value_Renamed.Length - 1) - (1));
					}
					else if (value_Renamed.StartsWith("\"") && value_Renamed.EndsWith("\"") && (2 <= value_Renamed.Length))
					{
						quote = '"';
						value_Renamed = value_Renamed.Substring(1, (value_Renamed.Length - 1) - (1));
					}
					else
						quote = (char) 0;
					if (key.Equals(SpecialHashtable.TAGNAME))
					{
						attribute = new TagAttribute(value_Renamed, null, quote);
						att.Insert(0, attribute);
					}
					else
					{
						// add whitespace between attributes
						attribute = new TagAttribute(" ");
						att.Add(attribute);
						attribute = new TagAttribute(key, value_Renamed, quote);
						att.Add(attribute);
					}
				}
				this.mAttributes = att;
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Return the name of this tag.
		/// <p>
		/// <em>
		/// Note: This value is converted to uppercase and does not
		/// begin with "/" if it is an end tag. Nor does it end with
		/// a slash in the case of an XML type tag.
		/// To get at the original text of the tag name use
		/// {@link #getRawTagName getRawTagName()}.
		/// The conversion to uppercase is performed with an ENGLISH locale.
		/// </em>
		/// </summary>
		/// <returns> The tag name.
		/// </returns>
		/// <summary> Set the name of this tag.
		/// This creates or replaces the first attribute of the tag (the
		/// zeroth element of the attribute vector).
		/// </summary>
		/// <param name="name">The tag name.
		/// </param>
		virtual public System.String TagName
		{
			get
			{
				System.String ret;
				
				ret = RawTagName;
				if (null != ret)
				{
					ret = ret.ToUpper(new System.Globalization.CultureInfo("en"));
					if (ret.StartsWith("/"))
						ret = ret.Substring(1);
					if (ret.EndsWith("/"))
						ret = ret.Substring(0, (ret.Length - 1) - (0));
				}
				
				return (ret);
			}
			
			set
			{
				TagAttribute attribute;
				System.Collections.ArrayList attributes;
				TagAttribute zeroth;
				
				attribute = new TagAttribute(value, null, (char) 0);
				attributes = AttributesEx;
				if (null == attributes)
				{
					attributes = System.Collections.ArrayList.Synchronized(new System.Collections.ArrayList(10));
					AttributesEx = attributes;
				}
				if (0 == attributes.Count)
					// nothing added yet
					attributes.Add(attribute);
				else
				{
					zeroth = (TagAttribute) attributes[0];
					// check for attribute that looks like a name
					if ((null == zeroth.GetValue()) && (0 == zeroth.GetQuote()))
						attributes[0] = attribute;
					else
						attributes.Insert(0, attribute);
				}
			}
			
		}
		/// <summary> Return the name of this tag.</summary>
		/// <returns> The tag name or null if this tag contains nothing or only
		/// whitespace.
		/// </returns>
		virtual public System.String RawTagName
		{
			get
			{
				System.Collections.ArrayList attributes;
				System.String ret;
				
				ret = null;
				
				attributes = AttributesEx;
				if (0 != attributes.Count)
					ret = ((TagAttribute) attributes[0]).GetName();
				
				return (ret);
			}
			
		}

		/// <summary> Gets the nodeBegin.</summary>
		/// <returns> The nodeBegin value.
		/// </returns>
		/// <summary> Sets the nodeBegin.</summary>
		virtual public int TagBegin
		{
			get
			{
				return (nodeBegin);
			}
			
			set
			{
				nodeBegin = value;
			}
			
		}

		/// <summary> Gets the nodeEnd.</summary>
		/// <returns> The nodeEnd value.
		/// </returns>
		/// <summary> Sets the nodeEnd.</summary>
		virtual public int TagEnd
		{
			get
			{
				return (nodeEnd);
			}
			
			set
			{
				nodeEnd = value;
			}
			
		}
		/// <summary> Returns table of attributes in the tag</summary>
		/// <returns> Hashtable
		/// </returns>
		/// <deprecated> This method is deprecated. Use getAttributes() instead.
		/// </deprecated>
		virtual public System.Collections.Hashtable Parsed
		{
			get
			{
				return Attributes;
			}
			
		}

		/// <summary> Is this an empty xml tag of the form &lt;tag/&gt;.</summary>
		/// <returns> true if the last character of the last attribute is a '/'.
		/// </returns>
		/// <summary> Set this tag to be an empty xml node, or not.
		/// Adds or removes an ending slash on the tag.
		/// </summary>
		/// <param name="emptyXmlTag">If true, ensures there is an ending slash in the node,
		/// i.e. &lt;tag/&gt;, otherwise removes it.
		/// </param>
		virtual public bool EmptyXmlTag
		{
			get
			{
				System.Collections.ArrayList attributes;
				int size;
				TagAttribute attribute;
				System.String name;
				int length;
				bool ret;
				
				ret = false;
				
				attributes = AttributesEx;
				size = attributes.Count;
				if (0 < size)
				{
					attribute = (TagAttribute) attributes[size - 1];
					name = attribute.GetName();
					if (null != name)
					{
						length = name.Length;
						ret = name[length - 1] == '/';
					}
				}
				
				return (ret);
			}
			
			set
			{
				System.Collections.ArrayList attributes;
				int size;
				TagAttribute attribute;
				System.String name;
				System.String value_Renamed;
				int length;
				
				attributes = AttributesEx;
				size = attributes.Count;
				if (0 < size)
				{
					attribute = (TagAttribute) attributes[size - 1];
					name = attribute.GetName();
					if (null != name)
					{
						length = name.Length;
						value_Renamed = attribute.GetValue();
						if (null == value_Renamed)
							if (name[length - 1] == '/')
							{
								// already exists, remove if requested
								if (!value)
									if (1 == length)
										attributes.RemoveAt(size - 1);
									else
									{
										// this shouldn't happen, but covers the case
										// where no whitespace separates the slash
										// from the previous attribute
										name = name.Substring(0, (length - 1) - (0));
										attribute = new TagAttribute(name, null);
										attributes.RemoveAt(size - 1);
										attributes.Add(attribute);
									}
							}
							else
							{
								// ends with attribute, add whitespace + slash if requested
								if (value)
								{
									attribute = new TagAttribute(" ");
									attributes.Add(attribute);
									attribute = new TagAttribute("/", null);
									attributes.Add(attribute);
								}
							}
						else
						{
							// some valued attribute, add whitespace + slash if requested
							if (value)
							{
								attribute = new TagAttribute(" ");
								attributes.Add(attribute);
								attribute = new TagAttribute("/", null);
								attributes.Add(attribute);
							}
						}
					}
					else
					{
						// ends with whitespace, add if requested
						if (value)
						{
							attribute = new TagAttribute("/", null);
							attributes.Add(attribute);
						}
					}
				}
					// nothing there, add if requested
				else if (value)
				{
					attribute = new TagAttribute("/", null);
					attributes.Add(attribute);
				}
			}
			
		}
		/// <summary> Get the line number where this tag starts.</summary>
		/// <returns> The (zero based) line number in the page where this tag starts.
		/// </returns>
		virtual public int StartingLineNumber
		{
			get
			{
				return (Page.Row(StartPosition));
			}
			
		}
		/// <summary> Get the line number where this tag ends.</summary>
		/// <returns> The (zero based) line number in the page where this tag ends.
		/// </returns>
		virtual public int EndingLineNumber
		{
			get
			{
				return (Page.Row(EndPosition));
			}
			
		}
		/// <summary> Return the set of names handled by this tag.
		/// Since this a a generic tag, it has no ids.
		/// </summary>
		/// <returns> The names to be matched that create tags of this type.
		/// </returns>
		virtual public System.String[] Ids
		{
			get
			{
				return (NONE);
			}
			
		}
		/// <summary> Return the set of tag names that cause this tag to finish.
		/// These are the normal (non end tags) that if encountered while
		/// scanning (a composite tag) will cause the generation of a virtual
		/// tag.
		/// Since this a a non-composite tag, the default is no enders.
		/// </summary>
		/// <returns> The names of following tags that stop further scanning.
		/// </returns>
		virtual public System.String[] Enders
		{
			get
			{
				return (NONE);
			}
			
		}
		/// <summary> Return the set of end tag names that cause this tag to finish.
		/// These are the end tags that if encountered while
		/// scanning (a composite tag) will cause the generation of a virtual
		/// tag.
		/// Since this a a non-composite tag, it has no end tag enders.
		/// </summary>
		/// <returns> The names of following end tags that stop further scanning.
		/// </returns>
		virtual public System.String[] EndTagEnders
		{
			get
			{
				return (NONE);
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Return the scanner associated with this tag.</summary>
		/// <returns> The scanner associated with this tag.
		/// </returns>
		/// <summary> Set the scanner associated with this tag.</summary>
		/// <param name="scanner">The scanner for this tag.
		/// </param>
		virtual public IScanner ThisScanner
		{
			get
			{
				return (mScanner);
			}
			
			set
			{
				mScanner = value;
			}
			
		}

		/// <summary> Returns the value of an attribute.</summary>
		/// <param name="name">Name of attribute, case insensitive.
		/// </param>
		/// <returns> The value associated with the attribute or null if it does
		/// not exist, or is a stand-alone or
		/// </returns>
		public virtual System.String GetAttribute(System.String name)
		{
			TagAttribute attribute;
			System.String ret;
			
			ret = null;
			
			if (name.ToUpper().Equals(SpecialHashtable.TAGNAME.ToUpper()))
				ret = ((TagAttribute) AttributesEx[0]).GetName();
			else
			{
				attribute = GetAttributeEx(name);
				if (null != attribute)
					ret = attribute.GetValue();
			}
			
			return (ret);
		}
		
		/// <summary> Set attribute with given key, value pair.
		/// Figures out a quote character to use if necessary.
		/// </summary>
		/// <param name="key">The name of the attribute.
		/// </param>
		/// <param name="value_Renamed">The value of the attribute.
		/// </param>
		public virtual void SetAttribute(System.String key, System.String value_Renamed)
		{
			char ch;
			bool needed;
			bool singleq;
			bool doubleq;
			System.String ref_Renamed;
			System.Text.StringBuilder buffer;
			char quote;
			TagAttribute attribute;
			
			// first determine if there's whitespace in the value
			// and while we'return at it find a suitable quote character
			needed = false;
			singleq = true;
			doubleq = true;
			if (null != value_Renamed)
				for (int i = 0; i < value_Renamed.Length; i++)
				{
					ch = value_Renamed[i];
					if (System.Char.IsWhiteSpace(ch))
						needed = true;
					else if ('\'' == ch)
						singleq = false;
					else if ('"' == ch)
						doubleq = false;
				}
			
			// now apply quoting
			if (needed)
			{
				if (doubleq)
					quote = '"';
				else if (singleq)
					quote = '\'';
				else
				{
					// uh-oh, we need to convert some quotes into character references
					// convert all double quotes into &#34;
					quote = '"';
					ref_Renamed = "&quot;"; // Translate.encode (quote);
					// JDK 1.4: value = value.replaceAll ("\"", ref);
					buffer = new System.Text.StringBuilder(value_Renamed.Length * 5);
					for (int i = 0; i < value_Renamed.Length; i++)
					{
						ch = value_Renamed[i];
						if (quote == ch)
							buffer.Append(ref_Renamed);
						else
							buffer.Append(ch);
					}
					value_Renamed = buffer.ToString();
				}
			}
			else
				quote = (char) (0);
			attribute = GetAttributeEx(key);
			if (null != attribute)
			{
				// see if we can splice it in rather than replace it
				attribute.SetValue(value_Renamed);
				if (0 != quote)
					attribute.SetQuote(quote);
			}
			else
				SetAttribute(key, value_Renamed, quote);
		}
		
		/// <summary> Remove the attribute with the given key, if it exists.</summary>
		/// <param name="key">The name of the attribute.
		/// </param>
		public virtual void RemoveAttribute(System.String key)
		{
			TagAttribute attribute;
			
			attribute = GetAttributeEx(key);
			if (null != attribute)
				AttributesEx.Remove(attribute);
		}
		
		/// <summary> Set attribute with given key, value pair where the value is quoted by quote.</summary>
		/// <param name="key">The name of the attribute.
		/// </param>
		/// <param name="value_Renamed">The value of the attribute.
		/// </param>
		/// <param name="quote">The quote character to be used around value.
		/// If zero, it is an unquoted value.
		/// </param>
		public virtual void SetAttribute(System.String key, System.String value_Renamed, char quote)
		{
			SetAttribute(new TagAttribute(key, value_Renamed, quote));
		}
		
		/// <summary> Returns the attribute with the given name.</summary>
		/// <param name="name">Name of attribute, case insensitive.
		/// </param>
		/// <returns> The attribute or null if it does
		/// not exist.
		/// </returns>
		public virtual TagAttribute GetAttributeEx(System.String name)
		{
			System.Collections.ArrayList attributes;
			int size;
			TagAttribute attribute;
			System.String string_Renamed;
			TagAttribute ret;
			
			ret = null;
			
			attributes = AttributesEx;
			if (null != attributes)
			{
				size = attributes.Count;
				for (int i = 0; i < size; i++)
				{
					attribute = (TagAttribute) attributes[i];
					string_Renamed = attribute.GetName();
					if ((null != string_Renamed) && name.ToUpper().Equals(string_Renamed.ToUpper()))
					{
						ret = attribute;
						i = size; // exit fast
					}
				}
			}
			
			return (ret);
		}
		
		/// <summary> Set an attribute.</summary>
		/// <param name="attribute">The attribute to set.
		/// </param>
		public virtual void SetAttributeEx(TagAttribute attribute)
		{
			SetAttribute(attribute);
		}
		
		/// <summary> Set an attribute.
		/// This replaces an attribute of the same name.
		/// To set the zeroth attribute (the tag name), use setTagName().
		/// </summary>
		/// <param name="attribute">The attribute to set.
		/// </param>
		public virtual void SetAttribute(TagAttribute attribute)
		{
			bool replaced;
			System.Collections.ArrayList attributes;
			int length;
			System.String name;
			TagAttribute test;
			System.String test_name;
			
			replaced = false;
			attributes = AttributesEx;
			length = attributes.Count;
			if (0 < length)
			{
				name = attribute.GetName();
				for (int i = 1; i < attributes.Count; i++)
				{
					test = (TagAttribute) attributes[i];
					test_name = test.GetName();
					if (null != test_name)
						if (test_name.ToUpper().Equals(name.ToUpper()))
						{
							attributes[i] = attribute;
							replaced = true;
						}
				}
			}
			if (!replaced)
			{
				// add whitespace between attributes
				if ((0 != length) && !((TagAttribute) attributes[length - 1]).Whitespace)
					attributes.Add(new TagAttribute(" "));
				attributes.Add(attribute);
			}
		}
		
		/// <summary> Return the text contained in this tag.</summary>
		/// <returns> The complete contents of the tag (within the angle brackets).
		/// </returns>
		public override System.String GetText()
		{
			System.String ret;
			
			ret = ToHtml();
			ret = ret.Substring(1, (ret.Length - 1) - (1));
			
			return (ret);
		}
		
		/// <summary> Parses the given text to create the tag contents.</summary>
		/// <param name="text">A string of the form &lt;TAGNAME xx="yy"&gt;.
		/// </param>
		public override void SetText(System.String text)
		{
			Lexer lexer;
			TagNode output;
			
			lexer = new Lexer(text);
			try
			{
				output = (TagNode) lexer.NextNode();
				mPage = output.Page;
				nodeBegin = output.StartPosition;
				nodeEnd = output.EndPosition;
				mAttributes = output.AttributesEx;
			}
			catch (ParserException pe)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
				throw new System.ArgumentException(pe.Message);
			}
		}
		
		/// <summary> Get the plain text from this node.</summary>
		/// <returns> An empty string (tag contents do not display in a browser).
		/// If you want this tags HTML equivalent, use {@link #toHtml toHtml()}.
		/// </returns>
		public override System.String ToPlainTextString()
		{
			return ("");
		}
		
		/// <summary> Render the tag as HTML.
		/// A call to a tag's <code>toHtml()</code> method will render it in HTML.
		/// </summary>
		/// <returns> The tag as an HTML fragment.
		/// </returns>
		/// <seealso cref="Node.ToHtml()">
		/// </seealso>
		public override System.String ToHtml()
		{
			int length;
			int size;
			System.Collections.ArrayList attributes;
			TagAttribute attribute;
			System.Text.StringBuilder ret;
			
			length = 2;
			attributes = AttributesEx;
			size = attributes.Count;
			for (int i = 0; i < size; i++)
			{
				attribute = (TagAttribute) attributes[i];
				length += attribute.Length;
			}
			ret = new System.Text.StringBuilder(length);
			ret.Append("<");
			for (int i = 0; i < size; i++)
			{
				attribute = (TagAttribute) attributes[i];
				attribute.ToString(ret);
			}
			ret.Append(">");
			
			return (ret.ToString());
		}
		
		/// <summary> Print the contents of the tag.</summary>
		/// <returns> An string describing the tag. For text that looks like HTML use #toHtml().
		/// </returns>
		public override System.String ToString()
		{
			System.String text;
			System.String type;
			Cursor start;
			Cursor end;
			System.Text.StringBuilder ret;
			
			text = GetText();
			ret = new System.Text.StringBuilder(20 + text.Length);
			if (IsEndTag())
				type = "End";
			else
				type = "Tag";
			start = new Cursor(Page, StartPosition);
			end = new Cursor(Page, EndPosition);
			ret.Append(type);
			ret.Append(" (");
			ret.Append(start);
			ret.Append(",");
			ret.Append(end);
			ret.Append("): ");
			if (80 < ret.Length + text.Length)
			{
				text = text.Substring(0, (77 - ret.Length) - (0));
				ret.Append(text);
				ret.Append("...");
			}
			else
				ret.Append(text);
			
			return (ret.ToString());
		}
		
		/// <summary> Determines if the given tag breaks the flow of text.</summary>
		/// <returns> <code>true</code> if following text would start on a new line,
		/// <code>false</code> otherwise.
		/// </returns>
		public virtual bool BreaksFlow()
		{
			return (breakTags.ContainsKey(TagName));
		}
		
		/// <summary> Default tag visiting code.
		/// Based on <code>isEndTag()</code>, calls either <code>visitTag()</code> or
		/// <code>visitEndTag()</code>.
		/// </summary>
		/// <param name="visitor">The visitor that is visiting this node.
		/// </param>
		public override void  Accept(NodeVisitor visitor)
		{
			if (IsEndTag())
				visitor.VisitEndTag(this as ITag);
			else
				visitor.VisitTag(this as ITag);
		}
		
		/// <summary> Predicate to determine if this tag is an end tag (i.e. &lt;/HTML&gt;).</summary>
		/// <returns> <code>true</code> if this tag is an end tag.
		/// </returns>
		public virtual bool IsEndTag()
		{
			System.String raw;
			
			raw = RawTagName;
			
			return ((null == raw)?false:((0 != raw.Length) && ('/' == raw[0])));
		}
		
		/// <summary> Get the end tag for this (composite) tag.
		/// For a non-composite tag this always returns <code>null</code>.
		/// </summary>
		/// <returns> The tag that terminates this composite tag, i.e. &lt;/HTML&gt;.
		/// </returns>
		public virtual ITag GetEndTag()
		{
			return (null);
		}
		
		/// <summary> Set the end tag for this (composite) tag.
		/// For a non-composite tag this is a no-op.
		/// </summary>
		/// <param name="end">The tag that terminates this composite tag, i.e. &lt;/HTML&gt;.
		/// </param>
		public virtual void SetEndTag(ITag end)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		override public System.Object Clone()
		{
			return base.Clone();
		}

		static TagNode()
		{
			{
				breakTags = System.Collections.Hashtable.Synchronized(new System.Collections.Hashtable(30));
				breakTags["BLOCKQUOTE"] = true;
				breakTags["BODY"] = true;
				breakTags["BR"] = true;
				breakTags["CENTER"] = true;
				breakTags["DD"] = true;
				breakTags["DIR"] = true;
				breakTags["DIV"] = true;
				breakTags["DL"] = true;
				breakTags["DT"] = true;
				breakTags["FORM"] = true;
				breakTags["H1"] = true;
				breakTags["H2"] = true;
				breakTags["H3"] = true;
				breakTags["H4"] = true;
				breakTags["H5"] = true;
				breakTags["H6"] = true;
				breakTags["HEAD"] = true;
				breakTags["HR"] = true;
				breakTags["HTML"] = true;
				breakTags["ISINDEX"] = true;
				breakTags["LI"] = true;
				breakTags["MENU"] = true;
				breakTags["NOFRAMES"] = true;
				breakTags["OL"] = true;
				breakTags["P"] = true;
				breakTags["PRE"] = true;
				breakTags["TD"] = true;
				breakTags["TH"] = true;
				breakTags["TITLE"] = true;
				breakTags["UL"] = true;
			}
		}
	}
}
