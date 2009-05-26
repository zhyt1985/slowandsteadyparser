// ***************************************************************
//  HtmlPage   version:  1.0   date: 12/19/2005
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
	/// Summary description for HtmlPage.
	/// </summary>
	public class HtmlPage:NodeVisitor
	{
		#region Class Members
		private System.String title;
		private NodeList nodesInBody;
		private NodeList tables;
		private NodeList m_Images;
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parser"></param>
		public HtmlPage(Parser parser):base(true)
		{
			title = "";
			nodesInBody = new NodeList();
			tables = new NodeList();
			m_Images = new NodeList();
		}

		/// <summary>
		/// Gets or sets title of the page.
		/// </summary>
		virtual public System.String Title
		{
			get
			{
				return title;
			}
			
			set
			{
				this.title = value;
			}
			
		}
		virtual public NodeList Body
		{
			get
			{
				return nodesInBody;
			}
			
		}

		/// <summary>
		/// Gets all the tables contained in the page.
		/// </summary>
		virtual public TableTag[] Tables
		{
			get
			{
				TableTag[] tableArr = new TableTag[tables.Size()];
				tables.CopyToNodeArray(tableArr);
				return tableArr;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tag"></param>
		public override void  VisitTag(ITag tag)
		{
			if (IsTable(tag))
			{
				tables.Add(tag);
			}
			else if (IsImageTag(tag))
			{
				m_Images.Add(tag);
			}
			else if (IsBodyTag(tag))
			{
				nodesInBody = tag.Children;
			}
			else if (IsTitleTag(tag))
			{
				title = ((TitleTag) tag).Title;
			}
		}

		#region Helper Methods
		private bool IsTable(ITag tag)
		{
			return (tag is TableTag);
		}
		
		private bool IsBodyTag(ITag tag)
		{
			return (tag is BodyTag);
		}
		
		private bool IsTitleTag(ITag tag)
		{
			return (tag is TitleTag);
		}

		private bool IsImageTag(ITag tag)
		{
			return (tag is ImageTag);
		}
		#endregion
	}
}
