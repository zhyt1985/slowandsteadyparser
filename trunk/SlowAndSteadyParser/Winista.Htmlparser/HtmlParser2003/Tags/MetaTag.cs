// ***************************************************************
//  MetaTag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Nodes;
using Winista.Text.HtmlParser.Lex;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> A Meta Tag</summary>
	[Serializable]
	public class MetaTag:TagNode
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"META"};

		/// <summary>
		/// 
		/// </summary>
		public MetaTag()
		{
		}

		/// <summary> Return the set of names handled by this tag.</summary>
		/// <returns> The names to be matched that create tags of this type.
		/// </returns>
		override public System.String[] Ids
		{
			get
			{
				return (mIds);
			}
			
		}
		/// <summary> Get the <code>HTTP-EQUIV</code> attribute, if any.</summary>
		/// <returns> The value of the <code>HTTP-EQUIV</code> attribute,
		/// or <code>null</code> if the attribute doesn't exist.
		/// </returns>
		/// <summary> Set the <code>HTTP-EQUIV</code> attribute.</summary>
		virtual public System.String HttpEquiv
		{
			get
			{
				return (GetAttribute("HTTP-EQUIV"));
			}
			
			set
			{
				TagAttribute equiv;
				equiv = GetAttributeEx("HTTP-EQUIV");
				if (null != equiv)
					equiv.SetValue(value);
				else
					AttributesEx.Add(new TagAttribute("HTTP-EQUIV", value));
			}
			
		}
		/// <summary> Get the <code>CONTENT</code> attribute, if any.</summary>
		/// <returns> The value of the <code>CONTENT</code> attribute,
		/// or <code>null</code> if the attribute doesn't exist.
		/// </returns>
		virtual public System.String MetaContent
		{
			get
			{
				return (GetAttribute("CONTENT"));
			}
			
		}

		/// <summary> Get the <code>NAME</code> attribute, if any.</summary>
		/// <returns> The value of the <code>NAME</code> attribute,
		/// or <code>null</code> if the attribute doesn't exist.
		/// </returns>
		/// <summary> Set the <code>NAME</code> attribute.</summary>
		/// <param name="metaTagName">The new value of the <code>NAME</code> attribute.
		/// </param>
		virtual public System.String MetaTagName
		{
			get
			{
				return (GetAttribute("NAME"));
			}
			
			set
			{
				TagAttribute name;
				name = GetAttributeEx("NAME");
				if (null != name)
					name.SetValue(value);
				else
					AttributesEx.Add(new TagAttribute("NAME", value));
			}
			
		}
		/// <summary> Set the <code>CONTENT</code> attribute.</summary>
		/// <param name="metaTagContents">The new value of the <code>CONTENT</code> attribute.
		/// </param>
		virtual public System.String MetaTagContents
		{
			set
			{
				TagAttribute content;
				content = GetAttributeEx("CONTENT");
				if (null != content)
					content.SetValue(value);
				else
					AttributesEx.Add(new TagAttribute("CONTENT", value));
			}
			
		}

		/// <summary> Perform the META tag semantic action.
		/// Check for a charset directive, and if found, set the charset for the page.
		/// </summary>
		public override void  DoSemanticAction()
		{
			System.String httpEquiv;
			System.String charset;
			
			if (null != HttpEquiv)
			{
					httpEquiv = HttpEquiv;
				if ("Content-Type".ToUpper().Equals(httpEquiv.ToUpper()))
				{
					charset = Page.GetCharset(GetAttribute("CONTENT"));
					Page.Encoding = charset;
				}
			}
		}
	}
}
