// ***************************************************************
//  ColumnData   version:  1.0   Date: 12/22/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista, All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Tags;

namespace Winista.Text.HtmlParser.Data
{
	/// <summary>
	/// Summary description for ColumnData.
	/// </summary>
	/// 
	[SerializableAttribute]
	public class ColumnData : AbstractData
	{
		#region Class Members
		private RowData m_Row;
		private String m_strText;
		#endregion

		#region Class Constrcutors
		/// <summary>
		/// Creates new instance of <see cref="ColumnData"></see> object.
		/// </summary>
		public ColumnData()
		{
		}

		/// <summary>
		/// Creates new instance of <see cref="ColumnData"></see> object.
		/// </summary>
		/// <param name="tcol"></param>
		public ColumnData(TableColumn tcol, RowData obRow)
			:base(tcol)
		{
			this.m_Row = obRow;
			ConvertFromTag(tcol);
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// 
		/// </summary>
		public RowData Row
		{
			get
			{
				return this.m_Row;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public String Value
		{
			get
			{
				return this.m_strText;
			}
		}
		#endregion

		#region Helper Methods
		internal override void ConvertFromTag(ITag obTag)
		{
			base.ConvertFromTag (obTag);
			TableColumn obColumnTag = obTag as TableColumn;
			m_strText = obColumnTag.ToPlainTextString();
		}

		#endregion
	}
}
