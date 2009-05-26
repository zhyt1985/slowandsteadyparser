// ***************************************************************
//  MetaTagDataCollection   version:  1.0   date: 12/30/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Collections;

namespace Winista.Text.HtmlParser.Data
{
	/// <summary>
	/// Summary description for MetaTagDataCollection.
	/// </summary>
	public class MetaTagDataCollection : CollectionBase
	{
		/// <summary>
		/// Creates new instance of <see cref="MetaTagDataCollection"></see> object.
		/// </summary>
		public MetaTagDataCollection()
		{
		}

		public MetaTagData this[Int32 idx]
		{
			get
			{
				return List[idx] as MetaTagData;
			}
			set
			{
				if (-1 == IndexOf(value))
				{
					List[idx] = value;
				}
			}
		}

		public Int32 Add(MetaTagData val)
		{
			if (-1 == IndexOf(val))
			{
				return List.Add(val);
			}
			return -1;
		}

		public Int32 IndexOf(MetaTagData val)
		{
			return List.IndexOf(val);
		}

		public MetaTagData Find(String strTagName)
		{
			Int32 idx = IndexOf(new MetaTagData(strTagName, String.Empty));
			if (idx != -1)
			{
				return this[idx];
			}
			return null;
		}
	}
}
