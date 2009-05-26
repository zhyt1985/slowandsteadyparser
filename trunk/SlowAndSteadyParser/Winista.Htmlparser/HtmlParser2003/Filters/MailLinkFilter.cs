// ***************************************************************
//  MailLinkFilter   version:  1.0   date: 12/19/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Tags;

namespace Winista.Text.HtmlParser.Filters
{
	/// <summary>
	/// Summary description for MailLinkFilter.
	/// </summary>
	/// 
	[Serializable]
	public class MailLinkFilter:INodeFilter
	{
		/// <summary>
		/// Creates new instance of <see cref="MailLinkFilter"></see>
		/// </summary>
		public MailLinkFilter()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public virtual bool Accept(INode node)
		{
			if (node is ITag)
			{
				return (((ATag) node).MailLink);
			}
			return false;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		virtual public System.Object Clone()
		{
			return null;
		}
	}
}
