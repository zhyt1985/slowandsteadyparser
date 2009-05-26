// ***************************************************************
//  AbstractExtractor   version:  1.0   Date: 12/22/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Extractors
{
	/// <summary>
	/// Summary description for AbstractExtractor.
	/// </summary>
	public abstract class AbstractExtractor
	{
		#region Class Members
		/// <summary> The parser used to extract nodes.</summary>
		protected internal Parser m_obParser;
		#endregion

		public AbstractExtractor()
		{
		}
	}
}
