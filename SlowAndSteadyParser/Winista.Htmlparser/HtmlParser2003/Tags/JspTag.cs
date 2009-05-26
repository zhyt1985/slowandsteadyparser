// ***************************************************************
//  JspTag   version:  1.0   date: 12/18/2005
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
	/// <summary> The JSP/ASP tags like &lt;%&#46;&#46;&#46;%&gt; can be identified by this class.</summary>
	[Serializable]
	public class JspTag:TagNode
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"%", "%=", "%@"};

		public JspTag()
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

		/// <summary> Returns a string representation of this jsp tag suitable for debugging.</summary>
		/// <returns> A string representing this tag.
		/// </returns>
		public override System.String ToString()
		{
			System.String guts = ToHtml();
			guts = guts.Substring(1, (guts.Length - 2) - (1));
			return "JSP/ASP Tag : " + guts + "; begins at : " + StartPosition + "; ends at : " + EndPosition;
		}
	}
}
