// ***************************************************************
//  DoctypeTag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Nodes;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> The HTML Document Declaration Tag can identify &lt;&#33;DOCTYPE&gt; tags.</summary>
	[Serializable]
	public class DoctypeTag:TagNode
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"!DOCTYPE"};

		/// <summary>
		/// 
		/// </summary>
		public DoctypeTag()
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

		/// <summary> Return a string representation of the contents of this <code>!DOCTYPE</code> tag suitable for debugging.</summary>
		/// <returns> The contents of the document declaration tag as a string.
		/// </returns>
		public override System.String ToString()
		{
			System.String guts = ToHtml();
			guts = guts.Substring(1, (guts.Length - 2) - (1));
			return "Doctype Tag : " + guts + "; begins at : " + StartPosition + "; ends at : " + EndPosition;
		}
	}
}
