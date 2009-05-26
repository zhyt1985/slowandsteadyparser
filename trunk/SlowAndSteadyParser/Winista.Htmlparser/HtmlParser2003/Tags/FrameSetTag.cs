// ***************************************************************
//  FrameSetTag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Nodes;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> Identifies an frame set tag.</summary>
	[Serializable]
	public class FrameSetTag:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"FRAMESET"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"HTML"};

		/// <summary> Create a new frame set tag.</summary>
		public FrameSetTag()
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

		/// <summary> Returns the frames.</summary>
		/// <returns> The children of this tag.
		/// </returns>
		/// <summary> Sets the frames (children of this tag).</summary>
		virtual public NodeList Frames
		{
			get
			{
				return (Children);
			}
			
			set
			{
				Children = value;
			}
		}

		/// <summary> Return a string representation of the contents of this <code>FRAMESET</code> tag suitable for debugging.</summary>
		/// <returns> A string with this tag's contents.
		/// </returns>
		public override System.String ToString()
		{
			return "FRAMESET TAG : begins at : " + StartPosition + "; ends at : " + EndPosition;
		}
		
		/// <summary> Gets a frame by name.
		/// Names are checked without case sensitivity and conversion to uppercase
		/// is performed with an English locale.
		/// </summary>
		/// <param name="name">The name of the frame to retrieve.
		/// </param>
		/// <returns> The specified frame or <code>null</code> if it wasn't found.
		/// </returns>
		public virtual FrameTag GetFrame(System.String name)
		{
			return (GetFrame(name, new System.Globalization.CultureInfo("en")));
		}
		
		/// <summary> Gets a frame by name.
		/// Names are checked without case sensitivity and conversion to uppercase
		/// is performed with the locale provided.
		/// </summary>
		/// <param name="name">The name of the frame to retrieve.
		/// </param>
		/// <param name="locale">The locale to use when converting to uppercase.
		/// </param>
		/// <returns> The specified frame or <code>null</code> if it wasn't found.
		/// </returns>
		public virtual FrameTag GetFrame(System.String name, System.Globalization.CultureInfo locale)
		{
			INode node;
			FrameTag ret;
			
			ret = null;
			
			name = name.ToUpper(locale);
			for (ISimpleNodeIterator e = Frames.Elements(); e.HasMoreNodes() && (null == ret); )
			{
				node = e.NextNode();
				if (node is FrameTag)
				{
					ret = (FrameTag) node;
					if (!ret.FrameName.ToUpper(locale).Equals(name))
						ret = null;
				}
			}
			
			return (ret);
		}
	}
}
