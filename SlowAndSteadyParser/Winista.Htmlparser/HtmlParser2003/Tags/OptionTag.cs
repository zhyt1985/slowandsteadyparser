// ***************************************************************
//  OptionsTag   version:  1.0   date: 12/18/2005
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
	/// <summary> An option tag within a form.</summary>
	[Serializable]
	public class OptionTag:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"OPTION"};
		
		/// <summary> The set of tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEnders = new System.String[]{"INPUT", "TEXTAREA", "SELECT", "OPTION"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"SELECT", "FORM", "BODY", "HTML"};

		/// <summary> Create a new option tag.</summary>
		public OptionTag()
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

		/// <summary> Get the <code>VALUE</code> attribute, if any.</summary>
		/// <returns> The value of the <code>VALUE</code> attribute,
		/// or <code>null</code> if the attribute doesn't exist.
		/// </returns>
		/// <summary> Set the value of the value attribute.</summary>
		virtual public System.String Value
		{
			get
			{
				return (GetAttribute("VALUE"));
			}
			
			set
			{
				this.SetAttribute("VALUE", value);
			}
			
		}
		/// <summary> Get the text of this option.</summary>
		/// <returns> The textual contents of this <code>OPTION</code> tag.
		/// </returns>
		virtual public System.String OptionText
		{
			get
			{
				return ToPlainTextString();
			}
		}

		/// <summary> Return a string representation of this node suitable for debugging.</summary>
		/// <returns> The value and text of this tag in a string.
		/// </returns>
		public override System.String ToString()
		{
			System.String output = "OPTION VALUE: " + Value + " TEXT: " + OptionText + "\n";
			return output;
		}
	}
}
