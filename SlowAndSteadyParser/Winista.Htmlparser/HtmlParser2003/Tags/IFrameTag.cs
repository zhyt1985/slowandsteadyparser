// ***************************************************************
//  IFrameTag   version:  1.0   Date: 12/19/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Nodes;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> Identifies a IFrame tag</summary>
	[Serializable]
	public class IFrameTag:TagNode
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"IFRAME"};

		/// <summary>
		/// 
		/// </summary>
		public IFrameTag()
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

		/// <summary> Returns the location of the frame.</summary>
		/// <returns> The contents of the SRC attribute converted to an absolute URL.
		/// </returns>
		/// <summary> Sets the location of the frame.</summary>
		virtual public System.String FrameLocation
		{
			get
			{
				System.String ret;
				
				ret = GetAttribute("SRC");
				if (null == ret)
					ret = "";
				else if (null != Page)
					ret = Page.GetAbsoluteURL(ret);
				
				return (ret);
			}
			
			set
			{
				SetAttribute("SRC", value);
			}
		}

		/// <summary> Get the <code>NAME</code> attribute, if any.</summary>
		/// <returns> The value of the <code>NAME</code> attribute,
		/// or <code>null</code> if the attribute doesn't exist.
		/// </returns>
		virtual public System.String FrameName
		{
			get
			{
				return (GetAttribute("NAME"));
			}
		}

		/// <summary> Get the <code>NAME</code> attribute, if any.</summary>
		/// <returns> The value of the <code>NAME</code> attribute,
		/// or <code>null</code> if the attribute doesn't exist.
		/// </returns>
		virtual public System.String FrameId
		{
			get
			{
				return (GetAttribute("ID"));
			}
		}

		/// <summary> Return a string representation of the contents of this <code>FRAME</code> tag suitable for debugging.</summary>
		/// <returns> A string with this tag's contents.
		/// </returns>
		public override System.String ToString()
		{
			return "IFRAME TAG : IFrame " + FrameName + " at " + FrameLocation + "; begins at : " + StartPosition + "; ends at : " + EndPosition;
		}
	}
}
