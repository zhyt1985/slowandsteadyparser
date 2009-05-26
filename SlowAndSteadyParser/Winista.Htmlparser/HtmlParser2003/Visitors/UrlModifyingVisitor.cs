// ***************************************************************
//  UrlModifyingVisitor   version:  1.0   date: 12/25/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Tags;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Visitors
{
	/// <summary>
	/// Summary description for UrlModifyingVisitor.
	/// </summary>
	public class UrlModifyingVisitor:NodeVisitor
	{
		#region Class Members
		private System.String linkPrefix;
		private System.Text.StringBuilder modifiedResult;
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="linkPrefix"></param>
		public UrlModifyingVisitor(System.String linkPrefix):base(true, true)
		{
			this.linkPrefix = linkPrefix;
			modifiedResult = new System.Text.StringBuilder();
		}

		/// <summary>
		/// 
		/// </summary>
		virtual public System.String ModifiedResult
		{
			get
			{
				return modifiedResult.ToString();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="remarkNode"></param>
		public override void VisitRemarkNode(IRemark remarkNode)
		{
			modifiedResult.Append(remarkNode.ToHtml());
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="stringNode"></param>
		public override void VisitStringNode(IText stringNode)
		{
			modifiedResult.Append(stringNode.ToHtml());
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		public override void  VisitTag(ITag tag)
		{
			if (tag is LinkTag)
			{
				((LinkTag) tag).Link = linkPrefix + ((LinkTag) tag).Link;
			}
			else if (tag is ImageTag)
			{
				((ImageTag) tag).ImageURL = linkPrefix + ((ImageTag) tag).ImageURL;
			}
			// process only those nodes that won't be processed by an end tag,
			// nodes without parents or parents without an end tag, since
			// the complete processing of all children should happen before
			// we turn this node back into html text
			if (null == tag.Parent && (!(tag is CompositeTag) || null == ((CompositeTag) tag).GetEndTag()))
			{
				modifiedResult.Append(tag.ToHtml());
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		public override void VisitEndTag(ITag tag)
		{
			INode parent;
			
			parent = tag.Parent;
			// process only those nodes not processed by a parent
			if (null == parent)
				// an orphan end tag
				modifiedResult.Append(tag.ToHtml());
			else if (null == parent.Parent)
				// a top level tag with no parents
				modifiedResult.Append(parent.ToHtml());
		}
	}
}
