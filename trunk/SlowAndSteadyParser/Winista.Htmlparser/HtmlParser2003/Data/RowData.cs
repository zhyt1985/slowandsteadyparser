// ***************************************************************
//  RowData   version:  1.0   Date: 12/22/2005
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
	/// Summary description for RowData.
	/// </summary>
	/// 
	[SerializableAttribute]
	public sealed class RowData : AbstractData
	{
		#region Class Members
		private TableData m_Table;
		private ColumnDataCollection m_Columns;
		#endregion

		#region Class Constructors
		/// <summary>
		/// Creates new instance of <see cref="RowData"></see> object.
		/// </summary>
		public RowData()
			: base()
		{
			m_Columns = new ColumnDataCollection();
		}

		/// <summary>
		/// Creates new instance of <see cref="RowData"></see> object.
		/// </summary>
		/// <param name="trow"></param>
		public RowData(TableRow trow, TableData obTable)
			:base(trow)
		{
			m_Columns = new ColumnDataCollection();
			this.m_Table = obTable;
			ConvertFromTag(trow);
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// 
		/// </summary>
		public TableData Table
		{
			get
			{
				return this.m_Table;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ColumnDataCollection Columns
		{
			get
			{
				return this.m_Columns;
			}
		}
		#endregion

		#region Helper Methods
		internal override void ConvertFromTag(ITag obTag)
		{
			base.ConvertFromTag (obTag);

			TableRow oRowTag = obTag as TableRow;
			TableColumn[] oCols = oRowTag.Columns;
			if (null != oCols)
			{
				Int32 iCount = oCols.Length;
				for(Int32 i = 0; i < iCount; i++)
				{
					ColumnData oColData = new ColumnData(oCols[i], this);
					m_Columns.Add(oColData);
				}
			}
		}

		#endregion
	}
}
