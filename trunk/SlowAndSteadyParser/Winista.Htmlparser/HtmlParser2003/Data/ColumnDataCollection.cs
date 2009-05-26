// ***************************************************************
//  ColumnDataCollection   version:  1.0   Date: 12/22/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista, All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Collections;

namespace Winista.Text.HtmlParser.Data
{
	/// <summary>
	/// Summary description for ColumnDataCollection.
	/// </summary>
	public sealed class ColumnDataCollection : System.Collections.CollectionBase
	{
		/// <summary>
		/// Creates new instance of <see cref="ColumnDataCollection"></see> object.
		/// </summary>
		public ColumnDataCollection()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oData"></param>
		/// <returns></returns>
		public Int32 Add(ColumnData oData)
		{
			return List.Add(oData);
		}

		/// <summary>
		/// 
		/// </summary>
		public ColumnData this[Int32 idx]
		{
			get
			{
				return List[idx] as ColumnData;
			}
			set
			{
				List[idx] = value;
			}
		}
	}
}
