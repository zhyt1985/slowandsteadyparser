// ***************************************************************
//  TagFindingVisitor   version:  1.0   date: 12/25/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Visitors
{
	/// <summary>
	/// Summary description for TagFindingVisitor.
	/// </summary>
	public class TagFindingVisitor:NodeVisitor
	{
		#region Class Members

		private System.String[] tagsToBeFound;
		private int[] count;
		private int[] endTagCount;
		private NodeList[] tags;
		private NodeList[] endTags;
		private bool endTagCheck;
		
		#endregion
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tagsToBeFound"></param>
		public TagFindingVisitor(System.String[] tagsToBeFound):this(tagsToBeFound, false)
		{
		}

		public TagFindingVisitor(System.String[] tagsToBeFound, bool endTagCheck)
		{
			this.tagsToBeFound = tagsToBeFound;
			this.tags = new NodeList[tagsToBeFound.Length];
			if (endTagCheck)
			{
				endTags = new NodeList[tagsToBeFound.Length];
				endTagCount = new int[tagsToBeFound.Length];
			}
			for (int i = 0; i < tagsToBeFound.Length; i++)
			{
				tags[i] = new NodeList();
				if (endTagCheck)
				{
					endTags[i] = new NodeList();
				}
			}
			this.count = new int[tagsToBeFound.Length];
			this.endTagCheck = endTagCheck;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public virtual int GetTagCount(int index)
		{
			return count[index];
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		public override void VisitTag(ITag tag)
		{
			for (int i = 0; i < tagsToBeFound.Length; i++)
			{
				if (tag.TagName.ToUpper().Equals(tagsToBeFound[i].ToUpper()))
				{
					count[i]++;
					tags[i].Add(tag);
				}
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		public override void VisitEndTag(ITag tag)
		{
			if (!endTagCheck)
			{
				return;
			}

			for (int i = 0; i < tagsToBeFound.Length; i++)
			{
				if (tag.TagName.ToUpper().Equals(tagsToBeFound[i].ToUpper()))
				{
					endTagCount[i]++;
					endTags[i].Add(tag);
				}
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public virtual INode[] GetTags(int index)
		{
			return tags[index].ToNodeArray();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public virtual int GetEndTagCount(int index)
		{
			return endTagCount[index];
		}
	}
}
