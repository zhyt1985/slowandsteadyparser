// ***************************************************************
//  BaseHrefTag   version:  1.0   date: 12/18/2005
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
	/// <summary> BaseHrefTag represents an &lt;Base&gt; tag.
	/// It extends a basic tag by providing an accessor to the HREF attribute.
	/// </summary>
	[Serializable]
	public class BaseHrefTag : TagNode
	{
		private static readonly System.String[] mIds = new System.String[]{"BASE"};

		/// <summary>
		/// 
		/// </summary>
		public BaseHrefTag()
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

		/// <summary> Get the value of the <code>HREF</code> attribute, if any.</summary>
		/// <returns> The <code>HREF</code> value, with the leading and trailing whitespace removed, if any.
		/// </returns>
		/// <summary> Set the value of the <code>HREF</code> attribute.</summary>
		virtual public System.String BaseUrl
		{
			get
			{
				System.String base_Renamed;
				
				base_Renamed = GetAttribute("HREF");
				if (base_Renamed != null && base_Renamed.Length > 0)
					base_Renamed = base_Renamed.Trim();
				base_Renamed = (null == base_Renamed)?"":base_Renamed;
				
				return (base_Renamed);
			}
			
			set
			{
				SetAttribute("HREF", value);
			}
		}

		/// <summary> Perform the meaning of this tag.
		/// This sets the base URL to use for the rest of the page.
		/// </summary>
		public override void DoSemanticAction()
		{
			Page page;
			
			page = Page;
			if (null != page)
			{
				page.BaseUrl = BaseUrl;
			}
		}
	}
}
