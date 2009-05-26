// ***************************************************************
//  ImageData   version:  1.0   Date: 12/22/2005
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
	/// Summary description for ImageData.
	/// </summary>
	public class ImageData : AbstractData
	{
		#region Class Members
		private PageData m_Page;
		internal String m_strBaseUrl = String.Empty;
		internal String m_strUrl = String.Empty;
		internal LinkStatus m_Status;
		#endregion

		/// <summary>
		/// Creates new instance of <see cref="LinkData"></see> object.
		/// </summary>
		/// <param name="obPage"></param>
		public ImageData(PageData obPage)
		{
			if (null == obPage)
			{
				throw new ArgumentNullException("obPage", "Null Page object specified");
			}
			this.m_Page = obPage;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obPage"></param>
		/// <param name="obTag"></param>
		public ImageData(PageData obPage, ImageTag obTag)
			:this(obPage)
		{
			this.ConvertFromImageTag(obTag);
		}

		#region Public Properties
		/// <summary>
		/// 
		/// </summary>
		public String Url
		{
			get
			{
				return this.m_strUrl;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public String BaseUrl
		{
			get
			{
				return this.m_strBaseUrl;
			}
		}

		/// <summary>
		/// Gets or sets LinkStatus value.
		/// </summary>
		public LinkStatus LinkStatus
		{
			get
			{
				return this.m_Status;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public PageData PageData
		{
			get
			{
				return this.m_Page;
			}
		}
		#endregion

		#region Helper Methods
		private void ConvertFromImageTag(ImageTag obTag)
		{
			if (null == obTag)
			{
				throw new ArgumentNullException("obTag", "Null ImageTag object specified");
			}

			base.ConvertFromTag(obTag);
			this.m_strUrl = obTag.ImageURL;
		}
		#endregion
	}
}
