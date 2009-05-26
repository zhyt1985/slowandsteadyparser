// ***************************************************************
//  TableTag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************

using System;

using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> A table tag.</summary>
	[Serializable]
	public class TableTag:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"TABLE"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"BODY", "HTML"};

		public TableTag()
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
		/// <summary> Get the row tags within this table.</summary>
		/// <returns> The rows directly contained by this table.
		/// </returns>
		virtual public TableRow[] Rows
		{
			get
			{
				NodeList kids;
				NodeClassFilter cls;
				HasParentFilter recursion;
				INodeFilter filter;
				TableRow[] ret;
				
				kids = Children;
				if (null != kids)
				{
					cls = new NodeClassFilter(typeof(TableTag));
					recursion = new HasParentFilter(null);
					filter = new OrFilter(new AndFilter(cls, new IsEqualFilter(this)), new AndFilter(new NotFilter(cls), recursion));
					recursion.ParentFilter = filter;
					kids = kids.ExtractAllNodesThatMatch(new AndFilter(new NodeClassFilter(typeof(TableRow)), filter), true);
					ret = new TableRow[kids.Size()];
					kids.CopyToNodeArray(ret);
				}
				else
					ret = new TableRow[0];
				
				return (ret);
			}
			
		}
		/// <summary> Get the number of rows in this table.</summary>
		/// <returns> The number of rows in this table.
		/// <em>Note: this is a a simple count of the number of {@.html <TR>} tags and
		/// may be incorrect if the {@.html <TR>} tags span multiple rows.</em>
		/// </returns>
		virtual public int RowCount
		{
			get
			{
				return (Rows.Length);
			}
			
		}

		/// <summary> Get the row at the given index.</summary>
		/// <param name="index">The row number (zero based) to get. 
		/// </param>
		/// <returns> The row for the given index.
		/// </returns>
		public virtual TableRow GetRow(int index)
		{
			TableRow[] rows;
			TableRow ret;
			
			rows = Rows;
			if (index < rows.Length)
				ret = rows[index];
			else
				ret = null;
			
			return (ret);
		}
		
		/// <summary> Return a string suitable for debugging display.</summary>
		/// <returns> The table as HTML, sorry.
		/// </returns>
		public override System.String ToString()
		{
			return "TableTag\n" + "********\n" + ToHtml();
		}
	}
}
