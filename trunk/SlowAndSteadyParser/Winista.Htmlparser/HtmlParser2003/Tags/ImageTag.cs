// ***************************************************************
//  ImageTag   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Nodes;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Tags
{
	/// <summary> Identifies an image tag.</summary>
	[Serializable]
	public class ImageTag:TagNode
	{
		/// <summary> The set of names handled by this tag.</summary>
		private static readonly System.String[] mIds = new System.String[]{"IMG"};
		
		/// <summary> Holds the set value of the SRC attribute, since this can differ
		/// from the attribute value due to relative references resolved by
		/// the scanner.
		/// </summary>
		protected internal System.String imageURL;

		/// <summary>
		/// 
		/// </summary>
		public ImageTag()
		{
			imageURL = null;
		}

		/// <summary> Return the set of names handled by this tag.</summary>
		/// <returns> The names to be matched that create tags of this type.
		/// </returns>
		override public System.String[] Ids
		{
			get
			{
				return (mIds);
			}
			
		}

		/// <summary> Returns the location of the image.</summary>
		/// <returns> The absolute URL for this image.
		/// </returns>
		/// <summary> Set the <code>SRC</code> attribute.</summary>
		virtual public System.String ImageURL
		{
			get
			{
				if (null == imageURL)
				{
					if (null != Page)
					{
						imageURL = Page.GetAbsoluteURL(ExtractImageLocn());
					}
				}
				
				return (imageURL);
			}
			
			set
			{
				imageURL = value;
				SetAttribute("SRC", imageURL);
			}
		}

		/// <summary> Extract the location of the image
		/// Given the tag (with attributes), and the url of the html page in which
		/// this tag exists, perform best effort to extract the 'intended' URL.
		/// Attempts to handle such attributes as:
		/// <pre>
		/// &lt;IMG SRC=http://www.redgreen.com&gt; - normal
		/// &lt;IMG SRC =http://www.redgreen.com&gt; - space between attribute name and equals sign
		/// &lt;IMG SRC= http://www.redgreen.com&gt; - space between equals sign and attribute value
		/// &lt;IMG SRC = http://www.redgreen.com&gt; - space both sides of equals sign
		/// </pre>
		/// </summary>
		/// <returns> The relative URL for the image.
		/// </returns>
		public virtual System.String ExtractImageLocn()
		{
			System.Collections.ArrayList attributes;
			int size;
			TagAttribute attribute;
			System.String string_Renamed;
			System.String data;
			int state;
			System.String name;
			System.String ret;
			
			// TODO: move this logic into the lexer?
			
			ret = "";
			state = 0;
			attributes = AttributesEx;
			size = attributes.Count;
			for (int i = 0; (i < size) && (state < 3); i++)
			{
				attribute = (TagAttribute) attributes[i];
				string_Renamed = attribute.GetName();
				data = attribute.GetValue();
				switch (state)
				{
					
					case 0:  // looking for 'src'
						if (null != string_Renamed)
						{
							name = string_Renamed.ToUpper(new System.Globalization.CultureInfo("en"));
							if (name.Equals("SRC"))
							{
								state = 1;
								if (null != data)
								{
									if ("".Equals(data))
										state = 2;
										// empty attribute, SRC= 
									else
									{
										ret = data;
										i = size; // exit fast
									}
								}
							}
							else if (name.StartsWith("SRC"))
							{
								// missing equals sign
								string_Renamed = string_Renamed.Substring(3);
								// remove any double quotes from around string
								if (string_Renamed.StartsWith("\"") && string_Renamed.EndsWith("\"") && (1 < string_Renamed.Length))
									string_Renamed = string_Renamed.Substring(1, (string_Renamed.Length - 1) - (1));
								// remove any single quote from around string
								if (string_Renamed.StartsWith("'") && string_Renamed.EndsWith("'") && (1 < string_Renamed.Length))
									string_Renamed = string_Renamed.Substring(1, (string_Renamed.Length - 1) - (1));
								ret = string_Renamed;
								state = 0; // go back to searching for SRC
								// because, maybe we found SRCXXX
								// where XXX isn't a URL
							}
						}
						break;
					
					case 1:  // looking for equals sign
						if (null != string_Renamed)
						{
							if (string_Renamed.StartsWith("="))
							{
								state = 2;
								if (1 < string_Renamed.Length)
								{
									ret = string_Renamed.Substring(1);
									state = 0; // keep looking ?
								}
								else if (null != data)
								{
									ret = string_Renamed.Substring(1);
									state = 0; // keep looking ?
								}
							}
						}
						break;
					
					case 2:  // looking for a valueless attribute that could be a relative or absolute URL
						if (null != string_Renamed)
						{
							if (null == data)
								ret = string_Renamed;
							state = 0; // only check first non-whitespace item
							// not every valid attribute after an equals
						}
						break;
					
					default: 
						throw new System.SystemException("we're not supposed to in state " + state);
					
				}
			}
			ret = ParserUtils.RemoveChars(ret, '\n');
			ret = ParserUtils.RemoveChars(ret, '\r');
			
			return (ret);
		}
	}
}
