// ***************************************************************
//  BodyTag   version:  1.0   date: 12/18/2005
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
	/// <summary> A Body Tag.
	/// Primarily a container for child tags.
	/// </summary>
	[Serializable]
	public class BodyTag : CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"BODY"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"HTML"};

		/// <summary>
		/// 
		/// </summary>
		public BodyTag()
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
		/// <summary> Returns the textual contents of this <code>BODY</code> tag.
		/// Equivalent to <code>toPlainTextString()</code>.
		/// </summary>
		/// <returns> The 'browser' text in this tag.
		/// </returns>
		virtual public System.String Body
		{
			get
			{
				return ToPlainTextString();
			}
		}

		/// <summary> Return a string representation of this <code>BODY</code> tag suitable for debugging.</summary>
		/// <returns> A string representing this <code>BODY</code> tag.
		/// </returns>
		public override System.String ToString()
		{
			return "BODY: " + Body;
		}
	}
}
