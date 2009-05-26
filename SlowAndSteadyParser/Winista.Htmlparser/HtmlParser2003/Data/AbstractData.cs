// ***************************************************************
//  AbstractData   version:  1.0   Date: 12/22/2005
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
	/// This is abstract base class for all data result objects.
	/// </summary>
	public abstract class AbstractData
	{
		#region Private Members
		internal Int32 m_iDepth;
		internal String m_strId = String.Empty;
		internal String m_strName = String.Empty;
		internal ITag m_Tag = null;
		#endregion

		#region Class Constructors
		/// <summary>
		/// 
		/// </summary>
		public AbstractData()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obTag"></param>
		public AbstractData(ITag obTag)
		{
			this.m_Tag = obTag;
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Gets depth (level) of the result page.
		/// </summary>
		public Int32 Depth
		{
			get
			{
				return this.m_iDepth;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public String Id
		{
			get
			{
				return this.m_strId;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public String Name
		{
			get
			{
				return this.m_strName;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="ITag"></see> object associated
		/// with this data.
		/// </summary>
		public ITag ElementTag
		{
			get
			{
				return this.m_Tag;
			}
		}
		#endregion

		#region Helper Methods
		internal virtual void ConvertFromTag(ITag obTag)
		{
			this.m_strId = obTag.GetAttribute("ID");
			this.m_strName = obTag.GetAttribute("NAME");
		}
		#endregion
	}
}
