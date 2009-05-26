// ***************************************************************
//  MetaTagData   version:  1.0   date: 12/30/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Data
{
	/// <summary>
	/// Summary description for MetaTagData.
	/// </summary>
	/// 
	[Serializable]
	public class MetaTagData : AbstractData, IComparable
	{
		#region Class Members
		private String m_strName = String.Empty;
		private String m_strContent = String.Empty;
		#endregion

		/// <summary>
		/// Creates new instance of <see cref="MetaTagData"></see>
		/// </summary>
		public MetaTagData(String strName, String strContent)
		{
			this.m_strName = strName;
			this.m_strContent = strContent;
		}

		#region Public Properties
		/// <summary>
		/// Gets name of meta tag.
		/// </summary>
		public String Name
		{
			get
			{
				return this.m_strName;
			}
		}

		/// <summary>
		/// Gets content of meta tag.
		/// </summary>
		public String Content
		{
			get
			{
				return this.m_strContent;
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			MetaTagData obOther = obj as MetaTagData;
			if (null != obOther)
			{
				return (String.Compare(obOther.Name, this.Name, true) == 0);
			}
			throw new ArgumentException("Object is not of " + this.GetType().Name);
		}

		#endregion

		#region IComparable Members

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			MetaTagData obOther = obj as MetaTagData;
			if (null != obOther)
			{
				return String.Compare(obOther.Name, this.Name, true);
			}
			throw new ArgumentException("Object is not of " + this.GetType().Name);
		}

		#endregion
	}
}
