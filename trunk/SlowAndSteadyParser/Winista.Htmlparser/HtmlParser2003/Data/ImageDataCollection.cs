// ***************************************************************
//  ImageDataCollection   version:  1.0   Date: 12/22/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista, All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Data
{
	/// <summary>
	/// Summary description for ImageDataCollection.
	/// </summary>
	public class ImageDataCollection : System.Collections.CollectionBase
	{
		/// <summary>
		/// Creates new instance of <see cref="ImageDataCollection"></see> object.
		/// </summary>
		public ImageDataCollection()
		{
		}

		/// <summary>
		/// Gets or sets <see cref="LinkData"></see> object at specified index.
		/// </summary>
		public ImageData this[Int32 idx]
		{
			get
			{
				return List[idx] as ImageData;
			}
			set
			{
				List[idx] = value;
			}
		}

		/// <summary>
		/// Adds <see cref="LinkData"></see> object into collection.
		/// </summary>
		/// <param name="val"></param>
		/// <returns></returns>
		public Int32 Add(ImageData val)
		{
			return List.Add(val);
		}
	}
}
