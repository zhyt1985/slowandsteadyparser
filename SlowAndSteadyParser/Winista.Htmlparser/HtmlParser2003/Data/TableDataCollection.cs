// ***************************************************************
//  TableDataCollection   version:  1.0   Date: 01/06/2006
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2006 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Collections;

namespace Winista.Text.HtmlParser.Data
{
	/// <summary>
	/// Summary description for TableDataCollection.
	/// </summary>
	/// 
	[Serializable]
	public class TableDataCollection : CollectionBase
	{
		/// <summary>
		/// Creates new instance of <see cref="TableDataCollection"></see> object.
		/// </summary>
		public TableDataCollection()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public Int32 Add(TableData val)
		{
			return List.Add(val);
		}

		/// <summary>
		/// 
		/// </summary>
		public TableData this[Int32 idx]
		{
			get
			{
				return List[idx] as TableData;
			}
			set
			{
				List[idx] = value;
			}
		}
	}
}
