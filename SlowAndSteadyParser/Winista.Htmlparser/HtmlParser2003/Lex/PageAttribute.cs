// ***************************************************************
//  PageAttribute   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Lex
{
	/// <summary> An attribute within a tag on a page.
	/// This attribute is similar to Attribute but 'lazy loaded' from the
	/// <code>Page</code> by providing the page and cursor offsets
	/// into the page for the name and value. This is done for speed, since
	/// if the name and value are not needed we can avoid the cost and memory
	/// overhead of creating the strings.
	/// <p>
	/// Thus the property getters, defer to the base class unless the property
	/// is null, in which case an attempt is made to read it from the underlying
	/// page. Optimizations in the predicates and length calculation defer the
	/// actual instantiation of strings until absolutely needed.
	/// </summary>
	[Serializable]
	public class PageAttribute : TagAttribute
	{
		/// <summary> The page this attribute is extracted from.</summary>
		protected internal Page mPage;
		
		/// <summary> The starting offset of the name within the page.
		/// If negative, the name is considered <code>null</code>.
		/// </summary>
		protected internal int mNameStart;
		
		/// <summary> The ending offset of the name within the page.</summary>
		protected internal int mNameEnd;
		
		/// <summary> The starting offset of the value within the page.
		/// If negative, the value is considered <code>null</code>.
		/// </summary>
		protected internal int mValueStart;
		
		/// <summary> The ending offset of the name within the page.</summary>
		protected internal int mValueEnd;

		/// <summary> Create an attribute.</summary>
		/// <param name="page">The page containing the attribute.
		/// </param>
		/// <param name="name_start">The starting offset of the name within the page.
		/// If this is negative, the name is considered null.
		/// </param>
		/// <param name="name_end">The ending offset of the name within the page.
		/// </param>
		/// <param name="value_start">he starting offset of the value within the page.
		/// If this is negative, the value is considered null.
		/// </param>
		/// <param name="value_end">The ending offset of the value within the page.
		/// </param>
		/// <param name="quote">The quote, if any, surrounding the value of the attribute,
		/// (i.e. ' or "), or zero if none.
		/// </param>
		public PageAttribute(Page page, int name_start, int name_end, int value_start, int value_end, char quote)
		{
			mPage = page;
			mNameStart = name_start;
			mNameEnd = name_end;
			mValueStart = value_start;
			mValueEnd = value_end;
			SetName(null);
			SetAssignment(null);
			SetValue(null);
			SetQuote(quote);
		}

		//
		// provide same constructors as super class
		//
		
		private void Init()
		{
			mPage = null;
			mNameStart = - 1;
			mNameEnd = - 1;
			mValueStart = - 1;
			mValueEnd = - 1;
		}

		/// <summary> Create an attribute with the name, assignment string, value and quote given.
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
		public PageAttribute(System.String name, System.String assignment, System.String value_Renamed, char quote):base(name, assignment, value_Renamed, quote)
		{
			Init();
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
		public PageAttribute(System.String name, System.String value_Renamed, char quote):base(name, value_Renamed, quote)
		{
			Init();
		}
		
		/// <summary> Create a whitespace attribute with the value given.</summary>
		/// <param name="value">The value of this attribute.
		/// </param>
		/// <exception cref="IllegalArgumentException">if the value contains other than
		/// whitespace. To set a real value use {@link #PageAttribute(String,String)}.
		/// </exception>
		public PageAttribute(System.String value_Renamed):base(value_Renamed)
		{
			Init();
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
		public PageAttribute(System.String name, System.String value_Renamed):base(name, value_Renamed)
		{
			Init();
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
		public PageAttribute(System.String name, System.String assignment, System.String value_Renamed):base(name, assignment, value_Renamed)
		{
			Init();
		}
		
		/// <summary> Create an empty attribute.
		/// This will provide "" from the {@link #toString} and 
		/// {@link #toString(StringBuffer)} methods.
		/// </summary>
		public PageAttribute():base()
		{
			Init();
		}

		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the page this attribute is anchored to, if any.</summary>
		/// <returns> The page used to construct this attribute, or null if this
		/// is just a regular attribute.
		/// </returns>
		/// <summary> Set the page this attribute is anchored to.</summary>
		/// <param name="page">The page to be used to construct this attribute.
		/// Note: If you set this you probably also want to uncache the property
		/// values by setting them to null.
		/// </param>
		virtual public Page Page
		{
			get
			{
				return (mPage);
			}
			
			set
			{
				mPage = value;
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the starting position of the attribute name.</summary>
		/// <returns> The offset into the page at which the name begins.
		/// </returns>
		/// <summary> Set the starting position of the attribute name.</summary>
		/// <param name="start">The new offset into the page at which the name begins.
		/// </param>
		virtual public int NameStartPosition
		{
			get
			{
				return (mNameStart);
			}
			
			set
			{
				mNameStart = value;
				SetName(null); // uncache value
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the ending position of the attribute name.</summary>
		/// <returns> The offset into the page at which the name ends.
		/// </returns>
		/// <summary> Set the ending position of the attribute name.</summary>
		/// <param name="end">The new offset into the page at which the name ends.
		/// </param>
		virtual public int NameEndPosition
		{
			get
			{
				return (mNameEnd);
			}
			
			set
			{
				mNameEnd = value;
				SetName(null); // uncache value
				SetAssignment(null); // uncache value
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the starting position of the attribute value.</summary>
		/// <returns> The offset into the page at which the value begins.
		/// </returns>
		/// <summary> Set the starting position of the attribute value.</summary>
		/// <param name="start">The new offset into the page at which the value begins.
		/// </param>
		virtual public int ValueStartPosition
		{
			get
			{
				return (mValueStart);
			}
			
			set
			{
				mValueStart = value;
				SetAssignment(null); // uncache value
				SetValue(null); // uncache value
			}
			
		}
		//UPGRADE_NOTE: Respective javadoc comments were merged.  It should be changed in order to comply with .NET documentation conventions. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1199'"
		/// <summary> Get the ending position of the attribute value.</summary>
		/// <returns> The offset into the page at which the value ends.
		/// </returns>
		/// <summary> Set the ending position of the attribute value.</summary>
		/// <param name="end">The new offset into the page at which the value ends.
		/// </param>
		virtual public int ValueEndPosition
		{
			get
			{
				return (mValueEnd);
			}
			
			set
			{
				mValueEnd = value;
				SetValue(null); // uncache value
			}
			
		}
		/// <summary> Predicate to determine if this attribute is whitespace.</summary>
		/// <returns> <code>true</code> if this attribute is whitespace,
		/// <code>false</code> if it is a real attribute.
		/// </returns>
		override public bool Whitespace
		{
			get
			{
				return (((null == base.GetName()) && (null == mPage)) || ((null != mPage) && (0 > mNameStart)));
			}
			
		}
		/// <summary> Predicate to determine if this attribute has no equals sign (or value).</summary>
		/// <returns> <code>true</code> if this attribute is a standalone attribute.
		/// <code>false</code> if has an equals sign.
		/// </returns>
		override public bool StandAlone
		{
			get
			{
				return (!Whitespace && (null == base.GetAssignment()) && !Valued && ((null == mPage) || ((null != mPage) && (0 <= mNameEnd) && (0 > mValueStart))));
			}
			
		}
		/// <summary> Predicate to determine if this attribute has an equals sign but no value.</summary>
		/// <returns> <code>true</code> if this attribute is an empty attribute.
		/// <code>false</code> if has an equals sign and a value.
		/// </returns>
		override public bool Empty
		{
			get
			{
				return (!Whitespace && !StandAlone && (null == base.GetValue()) && ((null == mPage) || ((null != mPage) && (0 > mValueEnd))));
			}
			
		}
		/// <summary> Predicate to determine if this attribute has a value.</summary>
		/// <returns> <code>true</code> if this attribute has a value.
		/// <code>false</code> if it is empty or standalone.
		/// </returns>
		override public bool Valued
		{
			get
			{
				return ((null != base.GetValue()) || ((null != mPage) && ((0 <= mValueStart) && (0 <= mValueEnd)) && (mValueStart != mValueEnd)));
			}
			
		}
		/// <summary> Get the length of the string value of this attribute.</summary>
		/// <returns> The number of characters required to express this attribute.
		/// </returns>
		override public int Length
		{
			get
			{
				System.String name;
				System.String assignment;
				System.String value_Renamed;
				char quote;
				int ret;
				
				ret = 0;
				name = base.GetName();
				if (null != name)
					ret += name.Length;
				else if ((null != mPage) && (0 <= mNameStart) && (0 <= mNameEnd))
					ret += mNameEnd - mNameStart;
				assignment = base.GetAssignment();
				if (null != assignment)
					ret += assignment.Length;
				else if ((null != mPage) && (0 <= mNameEnd) && (0 <= mValueStart))
					ret += mValueStart - mNameEnd;
				value_Renamed = base.GetValue();
				if (null != value_Renamed)
					ret += value_Renamed.Length;
				else if ((null != mPage) && (0 <= mValueStart) && (0 <= mValueEnd))
					ret += mValueEnd - mValueStart;
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
		public override System.String GetName()
		{
			System.String ret;
			
			ret = base.GetName();
			if (null == ret)
			{
				if ((null != mPage) && (0 <= mNameStart))
				{
					ret = mPage.GetText(mNameStart, mNameEnd);
					SetName(ret); // cache the value
				}
			}
			
			return (ret);
		}
		
		/// <summary> Get the name of this attribute.</summary>
		/// <param name="buffer">The buffer to place the name in.
		/// </param>
		/// <seealso cref="getName()">
		/// </seealso>
		public override void GetName(System.Text.StringBuilder buffer)
		{
			System.String name;
			
			name = base.GetName();
			if (null == name)
			{
				if ((null != mPage) && (0 <= mNameStart))
					mPage.GetText(buffer, mNameStart, mNameEnd);
			}
			else
				buffer.Append(name);
		}
		
		/// <summary> Get the assignment string of this attribute.
		/// This is usually just an equals sign, but in poorly formed attributes it
		/// can include whitespace on either or both sides of an equals sign.
		/// </summary>
		/// <returns> The assignment string.
		/// </returns>
		public override System.String GetAssignment()
		{
			System.String ret;
			
			ret = base.GetAssignment();
			if (null == ret)
			{
				if ((null != mPage) && (0 <= mNameEnd) && (0 <= mValueStart))
				{
					ret = mPage.GetText(mNameEnd, mValueStart);
					// remove a possible quote included in the assignment
					// since mValueStart points at the real start of the value
					if (ret.EndsWith("\"") || ret.EndsWith("'"))
						ret = ret.Substring(0, (ret.Length - 1) - (0));
					SetAssignment(ret); // cache the value
				}
			}
			
			return (ret);
		}
		
		/// <summary> Get the assignment string of this attribute.</summary>
		/// <param name="buffer">The buffer to place the assignment string in.
		/// </param>
		/// <seealso cref="getAssignment()">
		/// </seealso>
		public override void GetAssignment(System.Text.StringBuilder buffer)
		{
			int length;
			char ch;
			System.String assignment;
			
			assignment = base.GetAssignment();
			if (null == assignment)
			{
				if ((null != mPage) && (0 <= mNameEnd) && (0 <= mValueStart))
				{
					mPage.GetText(buffer, mNameEnd, mValueStart);
					// remove a possible quote included in the assignment
					// since mValueStart points at the real start of the value
					length = buffer.Length - 1;
					ch = buffer[length];
					if (('\'' == ch) || ('"' == ch))
						buffer.Length = length;
				}
			}
			else
				buffer.Append(assignment);
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
		public override System.String GetValue()
		{
			System.String ret;
			
			ret = base.GetValue();
			if (null == ret)
			{
				if ((null != mPage) && (0 <= mValueEnd))
				{
					ret = mPage.GetText(mValueStart, mValueEnd);
					SetValue(ret); // cache the value
				}
			}
			
			return (ret);
		}
		
		/// <summary> Get the value of the attribute.</summary>
		/// <param name="buffer">The buffer to place the value in.
		/// </param>
		/// <seealso cref="getValue()">
		/// </seealso>
		public override void GetValue(System.Text.StringBuilder buffer)
		{
			System.String value_Renamed;
			
			value_Renamed = base.GetValue();
			if (null == value_Renamed)
			{
				if ((null != mPage) && (0 <= mValueEnd))
					mPage.GetText(buffer, mNameStart, mNameEnd);
			}
			else
				buffer.Append(value_Renamed);
		}
		
		/// <summary> Get the raw value of the attribute.
		/// The part after the equals sign, or the text if it's just a whitepace
		/// 'attribute'. This includes the quotes around the value if any.
		/// </summary>
		/// <returns> The value, or <code>null</code> if it's a stand-alone attribute,
		/// or the text if it's just a whitepace 'attribute'.
		/// </returns>
		public override System.String GetRawValue()
		{
			char quote;
			System.Text.StringBuilder buffer;
			System.String ret;
			
			ret = GetValue();
			if (null != ret && (0 != (quote = GetQuote())))
			{
				buffer = new System.Text.StringBuilder(ret.Length + 2);
				buffer.Append(quote);
				buffer.Append(ret);
				buffer.Append(quote);
				ret = buffer.ToString();
			}
			
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
		public override void GetRawValue(System.Text.StringBuilder buffer)
		{
			char quote;
			
			if (null == mValue)
			{
				if (0 <= mValueEnd)
				{
					if (0 != (quote = GetQuote()))
						buffer.Append(quote);
					if (mValueStart != mValueEnd)
						mPage.GetText(buffer, mValueStart, mValueEnd);
					if (0 != quote)
						buffer.Append(quote);
				}
			}
			else
			{
				if (0 != (quote = GetQuote()))
					buffer.Append(quote);
				buffer.Append(mValue);
				if (0 != quote)
					buffer.Append(quote);
			}
		}
	}
}
