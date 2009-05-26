// ***************************************************************
//  StyleTag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Scanners;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> A StyleTag represents a &lt;style&gt; tag.</summary>
	[Serializable]
	public class StyleTag:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"STYLE"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"BODY", "HTML"};

		public StyleTag()
		{
			ThisScanner = new StyleScanner();
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
		/// <summary> Return the set of end tag names that cause this tag to finish.</summary>
		/// <returns> The names of following end tags that stop further scanning.
		/// </returns>
		override public System.String[] EndTagEnders
		{
			get
			{
				return (mEndTagEnders);
			}
			
		}
		/// <summary> Get the style data in this tag.</summary>
		/// <returns> The HTML of the children of this tag.
		/// </returns>
		virtual public System.String StyleCode
		{
			get
			{
				return (ChildrenHTML);
			}
			
		}

		/// <summary> Print the contents of the style node.</summary>
		/// <returns> A string suitable for debugging or a printout.
		/// </returns>
		public override System.String ToString()
		{
			System.String guts;
			System.Text.StringBuilder ret;
			
			ret = new System.Text.StringBuilder();
			
			guts = ToHtml();
			guts = guts.Substring(1, (guts.Length - 1) - (1));
			ret.Append("Style node :\n");
			ret.Append(guts);
			ret.Append("\n");
			
			return (ret.ToString());
		}
	}
}
