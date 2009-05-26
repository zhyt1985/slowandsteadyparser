// ***************************************************************
//  LabelTag   version:  1.0   date: 12/18/2005
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
	/// <summary> A label tag.</summary>
	[Serializable]
	public class LabelTag:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"LABEL"};
		
		/// <summary> Create a new label tag.</summary>
		public LabelTag()
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
		/// <summary> Returns the text contained inside this label tag.</summary>
		/// <returns> The textual contents between the {@.html <LABEL></LABEL>} pair.
		/// </returns>
		virtual public System.String Label
		{
			get
			{
				return ToPlainTextString();
			}
		}

		/// <summary> Returns a string representation of this label tag suitable for debugging.</summary>
		/// <returns> A string representing this label.
		/// </returns>
		public override System.String ToString()
		{
			return "LABEL: " + Label;
		}
	}
}
