// ***************************************************************
//  ITag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Scanners;

namespace Winista.Text.HtmlParser
{
	/// <summary> This interface represents a tag (&lt;xxx yyy="zzz"&gt;) in the HTML document.
	/// Adds capabilities to a Node that are specific to a tag.
	/// </summary>
	public interface ITag : INode
	{
		/// <summary> Gets the attributes in the tag.</summary>
		/// <returns> Returns the list of {@link Attribute Attributes} in the tag.
		/// </returns>
		/// <summary> Sets the attributes.
		/// NOTE: Values of the extended hashtable are two element arrays of String,
		/// with the first element being the original name (not uppercased),
		/// and the second element being the value.
		/// </summary>
		System.Collections.ArrayList AttributesEx
		{
			get;
			
			set;
			
		}

		/// <summary> Gets the attributes in the tag.
		/// This is not the preferred  method to get attributes, see {@link
		/// #getAttributesEx getAttributesEx} which returns a list of {@link
		/// Attribute} objects, which offer more information than the simple
		/// <code>String</code> objects available from this <code>Hashtable</code>.
		/// </summary>
		/// <returns> Returns a list of name/value pairs representing the attributes.
		/// These are not in order, the keys (names) are converted to uppercase
		/// and the values are not quoted, even if they need to be.
		/// The table <em>will</em> return <code>null</code> if there was no value
		/// for an attribute (either no equals sign or nothing to the right of the
		/// equals sign). A special entry with a key of
		/// SpecialHashtable.TAGNAME ("$<TAGNAME>$") holds the tag name.
		/// The conversion to uppercase is performed with an ENGLISH locale.
		/// </returns>
		/// <deprecated> Use getAttributesEx() instead.
		/// </deprecated>
		/// <summary> Sets the attributes.
		/// A special entry with a key of SpecialHashtable.TAGNAME ("$<TAGNAME>$")
		/// sets the tag name.
		/// </summary>
		/// <param name="attributes">The attribute collection to set.
		/// </param>
		/// <deprecated> Use setAttributesEx() instead.
		/// </deprecated>
		System.Collections.Hashtable Attributes
		{
			get;
			
			set;
			
		}

		/// <summary> Return the name of this tag.
		/// <p>
		/// <em>
		/// Note: This value is converted to uppercase and does not
		/// begin with "/" if it is an end tag. Nor does it end with
		/// a slash in the case of an XML type tag.
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
		System.String TagName
		{
			get;
			
			set;
			
		}
		/// <summary> Return the name of this tag.</summary>
		/// <returns> The tag name or null if this tag contains nothing or only
		/// whitespace.
		/// </returns>
		System.String RawTagName
		{
			get;
			
		}

		/// <summary> Is this an empty xml tag of the form &lt;tag/&gt;.</summary>
		/// <returns> true if the last character of the last attribute is a '/'.
		/// </returns>
		/// <summary> Set this tag to be an empty xml node, or not.
		/// Adds or removes an ending slash on the tag.
		/// </summary>
		/// <remarks>If true, ensures there is an ending slash in the node,
		/// i.e. &lt;tag/&gt;, otherwise removes it.
		/// </remarks>
		bool EmptyXmlTag
		{
			get;
			
			set;
			
		}
		/// <summary> Return the set of names handled by this tag.
		/// Since this a a generic tag, it has no ids.
		/// </summary>
		/// <returns> The names to be matched that create tags of this type.
		/// </returns>
		System.String[] Ids
		{
			get;
			
		}
		/// <summary> Return the set of tag names that cause this tag to finish.
		/// These are the normal (non end tags) that if encountered while
		/// scanning (a composite tag) will cause the generation of a virtual
		/// tag.
		/// Since this a a non-composite tag, the default is no enders.
		/// </summary>
		/// <returns> The names of following tags that stop further scanning.
		/// </returns>
		System.String[] Enders
		{
			get;
			
		}
		/// <summary> Return the set of end tag names that cause this tag to finish.
		/// These are the end tags that if encountered while
		/// scanning (a composite tag) will cause the generation of a virtual
		/// tag.
		/// Since this a a non-composite tag, it has no end tag enders.
		/// </summary>
		/// <returns> The names of following end tags that stop further scanning.
		/// </returns>
		System.String[] EndTagEnders
		{
			get;
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Return the scanner associated with this tag.</summary>
		/// <returns> The scanner associated with this tag.
		/// </returns>
		/// <summary> Set the scanner associated with this tag.</summary>
		IScanner ThisScanner
		{
			get;
			
			set;
			
		}
		/// <summary> Get the line number where this tag starts.</summary>
		/// <returns> The (zero based) line number in the page where this tag starts.
		/// </returns>
		int StartingLineNumber
		{
			get;
			
		}
		/// <summary> Get the line number where this tag ends.</summary>
		/// <returns> The (zero based) line number in the page where this tag ends.
		/// </returns>
		int EndingLineNumber
		{
			get;
			
		}
		/// <summary> Returns the value of an attribute.</summary>
		/// <param name="name">Name of attribute, case insensitive.
		/// </param>
		/// <returns> The value associated with the attribute or null if it does
		/// not exist, or is a stand-alone or
		/// </returns>
		System.String GetAttribute(System.String name);
		
		/// <summary> Set attribute with given key, value pair.
		/// Figures out a quote character to use if necessary.
		/// </summary>
		/// <param name="key">The name of the attribute.
		/// </param>
		/// <param name="value">The value of the attribute.
		/// </param>
		void  SetAttribute(System.String key, System.String value_Renamed);
		
		/// <summary> Set attribute with given key/value pair, the value is quoted by quote.</summary>
		/// <param name="key">The name of the attribute.
		/// </param>
		/// <param name="val">The value of the attribute.
		/// </param>
		/// <param name="quote">The quote character to be used around value.
		/// If zero, it is an unquoted value.
		/// </param>
		void  SetAttribute(System.String key, System.String val, char quote);
		
		/// <summary> Remove the attribute with the given key, if it exists.</summary>
		/// <param name="key">The name of the attribute.
		/// </param>
		void  RemoveAttribute(System.String key);
		
		/// <summary> Returns the attribute with the given name.</summary>
		/// <param name="name">Name of attribute, case insensitive.
		/// </param>
		/// <returns> The attribute or null if it does
		/// not exist.
		/// </returns>
		TagAttribute GetAttributeEx(System.String name);
		
		/// <summary> Set an attribute.
		/// This replaces an attribute of the same name.
		/// To set the zeroth attribute (the tag name), use setTagName().
		/// </summary>
		/// <param name="attribute">The attribute to set.
		/// </param>
		void  SetAttributeEx(TagAttribute attribute);
		
		/// <summary> Determines if the given tag breaks the flow of text.</summary>
		/// <returns> <code>true</code> if following text would start on a new line,
		/// <code>false</code> otherwise.
		/// </returns>
		bool BreaksFlow();
		
		/// <summary> Predicate to determine if this tag is an end tag (i.e. &lt;/HTML&gt;).</summary>
		/// <returns> <code>true</code> if this tag is an end tag.
		/// </returns>
		bool IsEndTag();
		
		/// <summary> Get the end tag for this (composite) tag.
		/// For a non-composite tag this always returns <code>null</code>.
		/// </summary>
		/// <returns> The tag that terminates this composite tag, i.e. &lt;/HTML&gt;.
		/// </returns>
		ITag GetEndTag();
		
		/// <summary> Set the end tag for this (composite) tag.
		/// For a non-composite tag this is a no-op.
		/// </summary>
		/// <param name="tag">The tag that closes this composite tag, i.e. &lt;/HTML&gt;.
		/// </param>
		void SetEndTag(ITag tag);
	}
}
