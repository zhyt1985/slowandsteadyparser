// ***************************************************************
//  TableData   version:  1.0   Date: 12/22/2005
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
	/// Summary description for TableData.
	/// </summary>
	public sealed class TableData : AbstractData
	{
		#region Class Members
		private RowDataCollection m_Rows;
		#endregion

		#region Class Constructors
		/// <summary>
		/// Creates new instance of <see cref="TableData"></see> object.
		/// </summary>
		public TableData()
			:base()
		{
			m_Rows = new RowDataCollection();
		}

		public TableData(TableTag obTag)
			: base(obTag)
		{
			m_Rows = new RowDataCollection();
			ConvertFromTag(obTag);
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// 
		/// </summary>
		public RowDataCollection Rows
		{
			get
			{
				return this.m_Rows;
			}
		}
		#endregion

		#region Helper Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obTag"></param>
		internal override void ConvertFromTag(ITag obTag)
		{
			base.ConvertFromTag (obTag);

			//
			// Get the rows from table.
			//

			TableTag obTableTag = obTag as TableTag;
			TableRow [] oRows = obTableTag.Rows;
			if (null != oRows)
			{
				Int32 iCount = oRows.Length;
				for(Int32 i = 0; i < iCount; i++)
				{
					RowData oRowData = new RowData(oRows[i], this);
					m_Rows.Add(oRowData);
				}
			}
		}

		#endregion
	}
}
