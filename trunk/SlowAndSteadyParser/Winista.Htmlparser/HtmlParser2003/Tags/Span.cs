// ***************************************************************
//  Span   version:  1.0   date: 12/18/2005
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
	/// <summary> A span tag.</summary>
	[Serializable]
	public class Span:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"SPAN"};
		
		/// <summary>
		/// 
		/// </summary>
		public Span()
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
	}
}
