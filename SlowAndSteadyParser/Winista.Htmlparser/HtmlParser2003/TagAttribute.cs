// ***************************************************************
//  TagAttribute   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser
{
	/// <summary> An attribute within a tag.
	/// Holds the name, assignment string, value and quote character.
	/// <p>
	/// This class was made deliberately simple. Except for
	/// {@link #setRawValue RawValue}, the properties are completely orthogonal,
	/// that is: each property is independant of the others. This means you have
	/// enough rope here to hang yourself, and it's very easy to create
	/// malformed HTML. Where it's obvious, warnings and notes have been provided
	/// in the setters javadocs, but it is up to you -- the programmer --
	/// to ensure that the contents of the four fields will yield valid HTML
	/// (if that's what you want).
	/// <p>
	/// Be especially mindful of quotes and assignment strings. These are handled
	/// by the constructors where it's obvious, but in general, you need to set
	/// them explicitly when building an attribute. For example to construct
	/// the attribute <b><code>label="A multi word value."</code></b> you could use:
	/// <pre>
	/// attribute = new Attribute ();
	/// attribute.setName ("label");
	/// attribute.setAssignment ("=");
	/// attribute.setValue ("A multi word value.");
	/// attribute.setQuote ('"');
	/// </pre>
	/// or
	/// <pre>
	/// attribute = new Attribute ();
	/// attribute.setName ("label");
	/// attribute.setAssignment ("=");
	/// attribute.setRawValue ("A multi word value.");
	/// </pre>
	/// or
	/// <pre>
	/// attribute = new Attribute ("label", "A multi word value.");
	/// </pre>
	/// Note that the assignment value and quoting need to be set separately when
	/// building the attribute from scratch using the properties.
	/// <p>
	/// <table width="100.0%" align="Center" border="1">
	/// <caption>Valid States for Attributes.
	/// <tr>
	/// <th align="Center">Description</th>
	/// <th align="Center">toString()</th>
	/// <th align="Center">Name</th>
	/// <th align="Center">Assignment</th>
	/// <th align="Center">Value</th>
	/// <th align="Center">Quote</th>
	/// </tr>
	/// <tr>
	/// <td align="Center">whitespace attribute</td>
	/// <td align="Center">value</td>
	/// <td align="Center"><code>null</code></td>
	/// <td align="Center"><code>null</code></td>
	/// <td align="Center">"value"</td>
	/// <td align="Center"><code>0</code></td>
	/// </tr>
	/// <tr>
	/// <td align="Center">standalone attribute</td>
	/// <td align="Center">name</td>
	/// <td align="Center">"name"</td>
	/// <td align="Center"><code>null</code></td>
	/// <td align="Center"><code>null</code></td>
	/// <td align="Center"><code>0</code></td>
	/// </tr>
	/// <tr>
	/// <td align="Center">empty attribute</td>
	/// <td align="Center">name=</td>
	/// <td align="Center">"name"</td>
	/// <td align="Center">"="</td>
	/// <td align="Center"><code>null</code></td>
	/// <td align="Center"><code>0</code></td>
	/// </tr>
	/// <tr>
	/// <td align="Center">empty single quoted attribute</td>
	/// <td align="Center">name=''</td>
	/// <td align="Center">"name"</td>
	/// <td align="Center">"="</td>
	/// <td align="Center"><code>null</code></td>
	/// <td align="Center"><code>'</code></td>
	/// </tr>
	/// <tr>
	/// <td align="Center">empty double quoted attribute</td>
	/// <td align="Center">name=""</td>
	/// <td align="Center">"name"</td>
	/// <td align="Center">"="</td>
	/// <td align="Center"><code>null</code></td>
	/// <td align="Center"><code>"</code></td>
	/// </tr>
	/// <tr>
	/// <td align="Center">naked attribute</td>
	/// <td align="Center">name=value</td>
	/// <td align="Center">"name"</td>
	/// <td align="Center">"="</td>
	/// <td align="Center">"value"</td>
	/// <td align="Center"><code>0</code></td>
	/// </tr>
	/// <tr>
	/// <td align="Center">single quoted attribute</td>
	/// <td align="Center">name='value'</td>
	/// <td align="Center">"name"</td>
	/// <td align="Center">"="</td>
	/// <td align="Center">"value"</td>
	/// <td align="Center"><code>'</code></td>
	/// </tr>
	/// <tr>
	/// <td align="Center">double quoted attribute</td>
	/// <td align="Center">name="value"</td>
	/// <td align="Center">"name"</td>
	/// <td align="Center">"="</td>
	/// <td align="Center">"value"</td>
	/// <td align="Center"><code>"</code></td>
	/// </tr>
	/// </table>
	/// <br>In words:
	/// <br>If Name is null, and Assignment is null, and Quote is zero,
	/// it's whitepace and Value has the whitespace text -- value
	/// <br>If Name is not null, and both Assignment and Value are null
	/// it's a standalone attribute -- name
	/// <br>If Name is not null, and Assignment is an equals sign, and Quote is zero
	/// it's an empty attribute -- name=
	/// <br>If Name is not null, and Assignment is an equals sign,
	/// and Value is "" or null, and Quote is '
	/// it's an empty single quoted attribute -- name=''
	/// <br>If Name is not null, and Assignment is an equals sign,
	/// and Value is "" or null, and Quote is "
	/// it's an empty double quoted attribute -- name=""
	/// <br>If Name is not null, and Assignment is an equals sign,
	/// and Value is something, and Quote is zero
	/// it's a naked attribute -- name=value
	/// <br>If Name is not null, and Assignment is an equals sign,
	/// and Value is something, and Quote is '
	/// it's a single quoted attribute -- name='value'
	/// <br>If Name is not null, and Assignment is an equals sign,
	/// and Value is something, and Quote is "
	/// it's a double quoted attribute -- name="value"
	/// <br>All other states are invalid HTML.
	/// <p>
	/// From the <a href="http://www.w3.org/TR/html4/intro/sgmltut.html#h-3.2.2">
	/// HTML 4.01 Specification, W3C Recommendation 24 December 1999</a>
	/// http://www.w3.org/TR/html4/intro/sgmltut.html#h-3.2.2:<p>
	/// <cite>
	/// 3.2.2 Attributes<p>
	/// Elements may have associated properties, called attributes, which may
	/// have values (by default, or set by authors or scripts). Attribute/value
	/// pairs appear before the final ">" of an element's start tag. Any number
	/// of (legal) attribute value pairs, separated by spaces, may appear in an
	/// element's start tag. They may appear in any order.<p>
	/// In this example, the id attribute is set for an H1 element:
	/// <pre>
	/// <code>
	/// {@.html
	/// <H1 id="section1">
	/// This is an identified heading thanks to the id attribute
	/// </H1>}
	/// </code>
	/// </pre>
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
	/// <a href="http://www.w3.org/TR/html4/index/attributes.html">attribute
	/// index</a>.<p>
	/// </cite>
	/// <p>
	/// </summary>
	/// 
	[Serializable]
	public class TagAttribute
	{
		#region Class Members
		/// <summary> The name of this attribute.
		/// The part before the equals sign, or the stand-alone attribute.
		/// This will be <code>null</code> if the attribute is whitespace.
		/// </summary>
		protected internal System.String mName;
		
		/// <summary> The assignment string of the attribute.
		/// The equals sign.
		/// This will be <code>null</code> if the attribute is a
		/// stand-alone attribute.
		/// </summary>
		protected internal System.String mAssignment;
		
		/// <summary> The value of the attribute.
		/// The part after the equals sign.
		/// This will be <code>null</code> if the attribute is an empty or
		/// stand-alone attribute.
		/// </summary>
		protected internal System.String mValue;
		
		/// <summary> The quote, if any, surrounding the value of the attribute, if any.
		/// This will be zero if there are no quotes around the value.
		/// </summary>
		protected internal char mQuote;
		#endregion
		
		/// <summary> Create an attribute with the name, assignment, value and quote given.
		/// If the quote value is zero, assigns the value using {@link #setRawValue}
		/// which sets the quote character to a proper value if necessary.
		/// </summary>
		/// <param name="name">The name of this attribute.
		/// </param>
		/// <param name="assignment">The assignment string of this attribute.
		/// </param>
		/// <param name="value">The value of this attribute.
		/// </param>
		/// <param name="quote">The quote around the value of this attribute.
		/// </param>
		public TagAttribute(System.String name, System.String assignment, System.String value_Renamed, char quote)
		{
			SetName(name);
			SetAssignment(assignment);
			if (0 == quote)
			{
				SetRawValue(value_Renamed);
			}
			else
			{
				SetValue(value_Renamed);
				SetQuote(quote);
			}
		}
		
		/// <summary> Create an attribute with the name, value and quote given.
		/// Uses an equals sign as the assignment string if the value is not
		/// <code>null</code>, and calls {@link #setRawValue} to get the
		/// correct quoting if <code>quote</code> is zero.
		/// </summary>
		/// <param name="name">The name of this attribute.
		/// </param>
		/// <param name="value">The value of this attribute.
		/// </param>
		/// <param name="quote">The quote around the value of this attribute.
		/// </param>
		public TagAttribute(System.String name, System.String value_Renamed, char quote):this(name, (null == value_Renamed?"":"="), value_Renamed, quote)
		{
		}
		
		/// <summary> Create a whitespace attribute with the value given.</summary>
		/// <param name="value">The value of this attribute.
		/// </param>
		/// <exception cref="IllegalArgumentException">if the value contains other than
		/// whitespace. To set a real value use {@link #Attribute(String,String)}.
		/// </exception>
		public TagAttribute(System.String value_Renamed)
		{
			if (0 != value_Renamed.Trim().Length)
				throw new System.ArgumentException("non whitespace value");
			else
			{
				SetName(null);
				SetAssignment(null);
				SetValue(value_Renamed);
				SetQuote((char) 0);
			}
		}
		
		/// <summary> Create an attribute with the name and value given.
		/// Uses an equals sign as the assignment string if the value is not
		/// <code>null</code>, and calls {@link #setRawValue} to get the
		/// correct quoting.
		/// </summary>
		/// <param name="name">The name of this attribute.
		/// </param>
		/// <param name="value">The value of this attribute.
		/// </param>
		public TagAttribute(System.String name, System.String value_Renamed):this(name, (null == value_Renamed?"":"="), value_Renamed, (char) 0)
		{
		}
		
		/// <summary> Create an attribute with the name, assignment string and value given.
		/// Calls {@link #setRawValue} to get the correct quoting.
		/// </summary>
		/// <param name="name">The name of this attribute.
		/// </param>
		/// <param name="assignment">The assignment string of this attribute.
		/// </param>
		/// <param name="value">The value of this attribute.
		/// </param>
		public TagAttribute(System.String name, System.String assignment, System.String value_Renamed):this(name, assignment, value_Renamed, (char) 0)
		{
		}
		
		/// <summary> Create an empty attribute.
		/// This will provide "" from the {@link #toString} and
		/// {@link #toString(StringBuffer)} methods.
		/// </summary>
		public TagAttribute():this(null, null, null, (char) 0)
		{
		}

		/// <summary> Predicate to determine if this attribute is whitespace.</summary>
		/// <returns> <code>true</code> if this attribute is whitespace,
		/// <code>false</code> if it is a real attribute.
		/// </returns>
		virtual public bool Whitespace
		{
			get
			{
				return (null == GetName());
			}
			
		}
		/// <summary> Predicate to determine if this attribute has no equals sign (or value).</summary>
		/// <returns> <code>true</code> if this attribute is a standalone attribute.
		/// <code>false</code> if has an equals sign.
		/// </returns>
		virtual public bool StandAlone
		{
			get
			{
				return ((null != GetName()) && (null == GetAssignment()));
			}
			
		}
		/// <summary> Predicate to determine if this attribute has an equals sign but no value.</summary>
		/// <returns> <code>true</code> if this attribute is an empty attribute.
		/// <code>false</code> if has an equals sign and a value.
		/// </returns>
		virtual public bool Empty
		{
			get
			{
				return ((null != GetAssignment()) && (null == GetValue()));
			}
			
		}
		/// <summary> Predicate to determine if this attribute has a value.</summary>
		/// <returns> <code>true</code> if this attribute has a value.
		/// <code>false</code> if it is empty or standalone.
		/// </returns>
		virtual public bool Valued
		{
			get
			{
				return (null != GetValue());
			}
			
		}
		/// <summary> Get the length of the string value of this attribute.</summary>
		/// <returns> The number of characters required to express this attribute.
		/// </returns>
		virtual public int Length
		{
			get
			{
				System.String name;
				System.String assignment;
				System.String value_Renamed;
				char quote;
				int ret;
				
				ret = 0;
				name = GetName();
				if (null != name)
					ret += name.Length;
				assignment = GetAssignment();
				if (null != assignment)
					ret += assignment.Length;
				value_Renamed = GetValue();
				if (null != value_Renamed)
					ret += value_Renamed.Length;
				quote = GetQuote();
				if (0 != quote)
					ret += 2;
				
				return (ret);
			}
			
		}
		
		/// <summary> Get the name of this attribute.
		/// The part before the equals sign, or the contents of the
		/// stand-alone attribute.
		/// </summary>
		/// <returns> The name, or <code>null</code> if it's just a whitepace
		/// 'attribute'.
		/// </returns>
		public virtual System.String GetName()
		{
			return (mName);
		}
		
		/// <summary> Get the name of this attribute.</summary>
		/// <param name="buffer">The buffer to place the name in.
		/// </param>
		/// <seealso cref="getName()">
		/// </seealso>
		public virtual void GetName(System.Text.StringBuilder buffer)
		{
			if (null != mName)
				buffer.Append(mName);
		}
		
		/// <summary> Set the name of this attribute.
		/// Set the part before the equals sign, or the contents of the
		/// stand-alone attribute.
		/// <em>WARNING:</em> Setting this to <code>null</code> can result in
		/// malformed HTML if the assignment string is not <code>null</code>.
		/// </summary>
		/// <param name="name">The new name.
		/// </param>
		public virtual void  SetName(System.String name)
		{
			mName = name;
		}
		
		/// <summary> Get the assignment string of this attribute.
		/// This is usually just an equals sign, but in poorly formed attributes it
		/// can include whitespace on either or both sides of an equals sign.
		/// </summary>
		/// <returns> The assignment string.
		/// </returns>
		public virtual System.String GetAssignment()
		{
			return (mAssignment);
		}
		
		/// <summary> Get the assignment string of this attribute.</summary>
		/// <param name="buffer">The buffer to place the assignment string in.
		/// </param>
		/// <seealso cref="getAssignment()">
		/// </seealso>
		public virtual void  GetAssignment(System.Text.StringBuilder buffer)
		{
			if (null != mAssignment)
				buffer.Append(mAssignment);
		}
		
		/// <summary> Set the assignment string of this attribute.
		/// <em>WARNING:</em> Setting this property to other than an equals sign
		/// or <code>null</code> will result in malformed HTML. In the case of a
		/// <code>null</code>, the {@link  #setValue value} should also be set to
		/// <code>null</code>.
		/// </summary>
		/// <param name="assignment">The new assignment string.
		/// </param>
		public virtual void  SetAssignment(System.String assignment)
		{
			mAssignment = assignment;
		}
		
		/// <summary> Get the value of the attribute.
		/// The part after the equals sign, or the text if it's just a whitepace
		/// 'attribute'.
		/// <em>NOTE:</em> This does not include any quotes that may have enclosed
		/// the value when it was read. To get the un-stripped value use
		/// {@link  #getRawValue}.
		/// </summary>
		/// <returns> The value, or <code>null</code> if it's a stand-alone or
		/// empty attribute, or the text if it's just a whitepace 'attribute'.
		/// </returns>
		public virtual System.String GetValue()
		{
			return (mValue);
		}
		
		/// <summary> Get the value of the attribute.</summary>
		/// <param name="buffer">The buffer to place the value in.
		/// </param>
		/// <seealso cref="getValue()">
		/// </seealso>
		public virtual void  GetValue(System.Text.StringBuilder buffer)
		{
			if (null != mValue)
				buffer.Append(mValue);
		}
		
		/// <summary> Set the value of the attribute.
		/// The part after the equals sign, or the text if it's a whitepace
		/// 'attribute'.
		/// <em>WARNING:</em> Setting this property to a value that needs to be
		/// quoted without also setting the quote character will result in malformed
		/// HTML.
		/// </summary>
		/// <param name="value">The new value.
		/// </param>
		public virtual void SetValue(System.String value_Renamed)
		{
			mValue = value_Renamed;
		}
		
		/// <summary> Get the quote, if any, surrounding the value of the attribute, if any.</summary>
		/// <returns> Either ' or " if the attribute value was quoted, or zero
		/// if there are no quotes around it.
		/// </returns>
		public virtual char GetQuote()
		{
			return (mQuote);
		}
		
		/// <summary> Get the quote, if any, surrounding the value of the attribute, if any.</summary>
		/// <param name="buffer">The buffer to place the quote in.
		/// </param>
		/// <seealso cref="getQuote()">
		/// </seealso>
		public virtual void GetQuote(System.Text.StringBuilder buffer)
		{
			if (0 != mQuote)
				buffer.Append(mQuote);
		}
		
		/// <summary> Set the quote surrounding the value of the attribute.
		/// <em>WARNING:</em> Setting this property to zero will result in malformed
		/// HTML if the {@link  #getValue value} needs to be quoted (i.e. contains
		/// whitespace).
		/// </summary>
		/// <param name="quote">The new quote value.
		/// </param>
		public virtual void SetQuote(char quote)
		{
			mQuote = quote;
		}
		
		/// <summary> Get the raw value of the attribute.
		/// The part after the equals sign, or the text if it's just a whitepace
		/// 'attribute'. This includes the quotes around the value if any.
		/// </summary>
		/// <returns> The value, or <code>null</code> if it's a stand-alone attribute,
		/// or the text if it's just a whitepace 'attribute'.
		/// </returns>
		public virtual System.String GetRawValue()
		{
			char quote;
			System.Text.StringBuilder buffer;
			System.String ret;
			
			if (Valued)
			{
				quote = GetQuote();
				if (0 != quote)
				{
					buffer = new System.Text.StringBuilder(); // todo: what is the value length?
					buffer.Append(quote);
					GetValue(buffer);
					buffer.Append(quote);
					ret = buffer.ToString();
				}
				else
					ret = GetValue();
			}
			else
				ret = null;
			
			return (ret);
		}
		
		/// <summary> Get the raw value of the attribute.
		/// The part after the equals sign, or the text if it's just a whitepace
		/// 'attribute'. This includes the quotes around the value if any.
		/// </summary>
		/// <param name="buffer">The string buffer to append the attribute value to.
		/// </param>
		/// <seealso cref="getRawValue()">
		/// </seealso>
		public virtual void  GetRawValue(System.Text.StringBuilder buffer)
		{
			GetQuote(buffer);
			GetValue(buffer);
			GetQuote(buffer);
		}
		
		/// <summary> Set the value of the attribute and the quote character.
		/// If the value is pure whitespace, assign it 'as is' and reset the
		/// quote character. If not, check for leading and trailing double or
		/// single quotes, and if found use this as the quote character and
		/// the inner contents of <code>value</code> as the real value.
		/// Otherwise, examine the string to determine if quotes are needed
		/// and an appropriate quote character if so. This may involve changing
		/// double quotes within the string to character references.
		/// </summary>
		/// <param name="value">The new value.
		/// </param>
		public virtual void SetRawValue(System.String value_Renamed)
		{
			char ch;
			bool needed;
			bool singleq;
			bool doubleq;
			System.String ref_Renamed;
			System.Text.StringBuilder buffer;
			char quote;
			
			quote = (char) (0);
			if ((null != value_Renamed) && (0 != value_Renamed.Trim().Length))
			{
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
				{
					// first determine if there's whitespace in the value
					// and while we're at it find a suitable quote character
					needed = false;
					singleq = true;
					doubleq = true;
					for (int i = 0; i < value_Renamed.Length; i++)
					{
						ch = value_Renamed[i];
						if ('\'' == ch)
						{
							singleq = false;
							needed = true;
						}
						else if ('"' == ch)
						{
							doubleq = false;
							needed = true;
						}
						else if (!('-' == ch) && !('.' == ch) && !('_' == ch) && !(':' == ch) && !System.Char.IsLetterOrDigit(ch))
						{
							needed = true;
						}
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
							// uh-oh, we need to convert some quotes into character
							// references, so convert all double quotes into &#34;
							quote = '"';
							ref_Renamed = "&quot;"; // Translate.encode (quote);
							// JDK 1.4: value = value.replaceAll ("\"", ref);
							buffer = new System.Text.StringBuilder(value_Renamed.Length * (ref_Renamed.Length - 1));
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
				}
			}
			SetValue(value_Renamed);
			SetQuote(quote);
		}
		
		/// <summary> Get a text representation of this attribute.
		/// Suitable for insertion into a tag, the output is one of
		/// the forms:
		/// <code>
		/// <pre>
		/// value
		/// name
		/// name=
		/// name=value
		/// name='value'
		/// name="value"
		/// </pre>
		/// </code>
		/// </summary>
		/// <returns> A string that can be used within a tag.
		/// </returns>
		public override System.String ToString()
		{
			int length;
			System.Text.StringBuilder ret;
			
			// get the size to avoid extra StringBuffer allocations
			length = Length;
			ret = new System.Text.StringBuilder(length);
			ToString(ret);
			
			return (ret.ToString());
		}
		
		/// <summary> Get a text representation of this attribute.</summary>
		/// <param name="buffer">The accumulator for placing the text into.
		/// </param>
		/// <seealso cref="toString()">
		/// </seealso>
		public virtual void ToString(System.Text.StringBuilder buffer)
		{
			GetName(buffer);
			GetAssignment(buffer);
			GetRawValue(buffer);
		}
	}
}
