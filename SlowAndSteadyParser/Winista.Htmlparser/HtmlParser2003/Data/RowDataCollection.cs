// ***************************************************************
//  RowDataCollection   version:  1.0   Date: 12/22/2005
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
	/// Summary description for RowDataCollection.
	/// </summary>
	/// 
	[SerializableAttribute]
	public sealed class RowDataCollection : System.Collections.CollectionBase
	{
		/// <summary>
		/// Creates new instance of <see cref="RowDataCollection"></see> object.
		/// </summary>
		public RowDataCollection()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="oData"></param>
		/// <returns></returns>
		public Int32 Add(RowData oData)
		{
			return List.Add(oData);
		}

		/// <summary>
		/// 
		/// </summary>
		public RowData this[Int32 idx]
		{
			get
			{
				return List[idx] as RowData;
			}
			set
			{
				List[idx] = value;
			}
		}
	}
}

