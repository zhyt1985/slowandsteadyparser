// ***************************************************************
//  TableRow   version:  1.0   date: 12/18/2005
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
	/// <summary> A table row tag.</summary>
	[Serializable]
	public class TableRow:CompositeTag
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"TR"};
		
		/// <summary> The set of end tag names that indicate the end of this tag.</summary>
		private static readonly System.String[] mEndTagEnders = new System.String[]{"TABLE"};

		/// <summary> Create a new table row tag.</summary>
		public TableRow()
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
		/// <summary> Get the column tags within this <code>TR</code> (table row) tag.</summary>
		/// <returns> The {@.html <TD>} tags contained by this tag.
		/// </returns>
		virtual public TableColumn[] Columns
		{
			get
			{
				NodeList kids;
				NodeClassFilter cls;
				HasParentFilter recursion;
				INodeFilter filter;
				TableColumn[] ret;
				
				kids = Children;
				if (null != kids)
				{
					cls = new NodeClassFilter(typeof(TableRow));
					recursion = new HasParentFilter(null);
					filter = new OrFilter(new AndFilter(cls, new IsEqualFilter(this)), new AndFilter(new NotFilter(cls), recursion));
					recursion.ParentFilter = filter;
					kids = kids.ExtractAllNodesThatMatch(new AndFilter(new NodeClassFilter(typeof(TableColumn)), filter), true);
					ret = new TableColumn[kids.Size()];
					kids.CopyToNodeArray(ret);
				}
				else
					ret = new TableColumn[0];
				
				return (ret);
			}
			
		}
		/// <summary> Get the number of columns in this row.</summary>
		/// <returns> The number of columns in this row.
		/// <em>Note: this is a a simple count of the number of {@.html <TD>} tags and
		/// may be incorrect if the {@.html <TD>} tags span multiple columns.</em>
		/// </returns>
		virtual public int ColumnCount
		{
			get
			{
				return (Columns.Length);
			}
			
		}
		/// <summary> Get the header of this table</summary>
		/// <returns> Table header tags contained in this row.
		/// </returns>
		virtual public TableHeader[] Headers
		{
			get
			{
				NodeList kids;
				NodeClassFilter cls;
				HasParentFilter recursion;
				INodeFilter filter;
				TableHeader[] ret;
				
				kids = Children;
				if (null != kids)
				{
					cls = new NodeClassFilter(typeof(TableRow));
					recursion = new HasParentFilter(null);
					filter = new OrFilter(new AndFilter(cls, new IsEqualFilter(this)), new AndFilter(new NotFilter(cls), recursion));
					recursion.ParentFilter = filter;
					kids = kids.ExtractAllNodesThatMatch(new AndFilter(new NodeClassFilter(typeof(TableHeader)), filter), true);
					ret = new TableHeader[kids.Size()];
					kids.CopyToNodeArray(ret);
				}
				else
					ret = new TableHeader[0];
				
				return (ret);
			}
			
		}
		/// <summary> Get the number of headers in this row.</summary>
		/// <returns> The count of header tags in this row.
		/// </returns>
		virtual public int HeaderCount
		{
			get
			{
				return (Headers.Length);
			}
			
		}

		/// <summary> Checks if this table has a header</summary>
		/// <returns> <code>true</code> if there is a header tag.
		/// </returns>
		public virtual bool HasHeader()
		{
			return (0 != HeaderCount);
		}
	}
}
