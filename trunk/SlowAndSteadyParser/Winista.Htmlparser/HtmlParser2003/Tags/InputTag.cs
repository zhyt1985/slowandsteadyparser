// ***************************************************************
//  InputTag   version:  1.0   date: 12/18/2005
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
	/// <summary>
	/// Represents types of input tags.
	/// </summary>
	public enum InputTagType
	{
		/// <summary>
		/// 
		/// </summary>
		BUTTON = 0,
		/// <summary>
		/// 
		/// </summary>
		CHECKBOX = 1,
		/// <summary>
		/// 
		/// </summary>
		FILE = 2,
		/// <summary>
		/// 
		/// </summary>
		HIDDEN = 3,
		/// <summary>
		/// 
		/// </summary>
		IMAGE = 4,
		/// <summary>
		/// 
		/// </summary>
		PASSWORD = 5,
		/// <summary>
		/// 
		/// </summary>
		RADIO = 6,
		/// <summary>
		/// 
		/// </summary>
		RESET = 7,
		/// <summary>
		/// 
		/// </summary>
		SUBMIT = 8,
		/// <summary>
		/// 
		/// </summary>
		TEXT = 9
	}

	/// <summary> An input tag in a form.</summary>
	[Serializable]
	public class InputTag:TagNode
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"INPUT"};

		private InputTagType m_TagType = InputTagType.TEXT;

		public InputTag()
		{
			m_TagType = InputTagType.TEXT;
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
