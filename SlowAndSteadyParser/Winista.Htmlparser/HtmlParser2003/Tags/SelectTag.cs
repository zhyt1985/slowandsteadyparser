// ***************************************************************
//  SelectTag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> A select tag within a form.</summary>
	[Serializable]
	public class SelectTag:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"SELECT"};
		
		/// <summary> The set of tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEnders = new System.String[]{"INPUT", "TEXTAREA", "SELECT"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"FORM", "BODY", "HTML"};

		public SelectTag()
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
		/// <summary> Return the set of tag names that cause this tag to finish.</summary>
		/// <returns> The names of following tags that stop further scanning.
		/// </returns>
		override public System.String[] Enders
		{
			get
			{
				return (mEnders);
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
		/// <summary> Get the list of options in this <code>SELECT</code> tag.</summary>
		/// <returns> The {@.html <OPTION>} tags contained by this tag.
		/// </returns>
		virtual public OptionTag[] OptionTags
		{
			get
			{
				NodeList list;
				OptionTag[] ret;
				
				list = SearchFor(typeof(OptionTag), true);
				ret = new OptionTag[list.Size()];
				list.CopyToNodeArray(ret);
				
				return (ret);
			}
			
		}
	}
}
