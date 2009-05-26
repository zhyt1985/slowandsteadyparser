// ***************************************************************
//  ContentProperties   version:  1.0   Date: 12/15/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary>
	/// Summary description for ContentProperties.
	/// </summary>
	/// 
	[Serializable]
	public class ContentProperties : System.Collections.Specialized.NameValueCollection
	{
		/// <summary>
		/// 
		/// </summary>
		public ContentProperties()
		{
		}

		/// <summary> initialize with default values
		/// </summary>
		/// <param name="defaults"></param>
		public ContentProperties(System.Collections.Specialized.NameValueCollection defaults)
			:base(defaults)
		{
		}

		/// <param name="key"></param>
		/// <returns> the property value or null
		/// </returns>
		public virtual System.String GetProperty(System.String key)
		{
			return (System.String) this[key];
		}

		/// <summary> sets the key value tuple
		/// 
		/// </summary>
		/// <param name="key">
		/// </param>
		/// <param name="value_Renamed">
		/// </param>
		public virtual void SetProperty(System.String key, System.String value_Renamed)
		{
			System.Object tempObject;
			tempObject = this[key];
			this[key] = value_Renamed;
			System.Object generatedAux2 = tempObject;
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public virtual System.Collections.IEnumerator PropertyNames()
		{
			return base.Keys.GetEnumerator();
		}
	}
}
