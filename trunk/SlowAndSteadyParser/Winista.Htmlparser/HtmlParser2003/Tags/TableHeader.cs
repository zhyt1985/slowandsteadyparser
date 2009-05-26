// ***************************************************************
//  TableHeader   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> A table header tag.</summary>
	[Serializable]
	public class TableHeader:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"TH"};
		
		/// <summary> The set of tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEnders = new System.String[]{"TH", "TR"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"TR", "TABLE"};

		/// <summary>
		/// 
		/// </summary>
		public TableHeader()
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
	}
}
