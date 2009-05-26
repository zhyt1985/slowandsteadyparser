// ***************************************************************
//  HasAttributeFilter   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright ?2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Filters
{
	/// <summary> This class accepts all tags that have a certain attribute,
	/// and optionally, with a certain value.
	/// </summary>
	[Serializable]
	public class HasAttributeFilter : INodeFilter
	{
		/// <summary> The attribute to check for.</summary>
		protected internal System.String mAttribute;
		
		/// <summary> The value to check for.</summary>
		protected internal System.String mValue;

		/// <summary> Get the attribute name.</summary>
		/// <returns> Returns the name of the attribute that is acceptable.
		/// </returns>
		/// <summary> Set the attribute name.</summary>
		/// <param name="name">The name of the attribute to accept.
		/// </param>
		virtual public System.String AttributeName
		{
			get
			{
				return (mAttribute);
			}
			
			set
			{
				mAttribute = value;
			}
		}

		/// <summary> Get the attribute value.</summary>
		/// <returns> Returns the value of the attribute that is acceptable.
		/// </returns>
		/// <summary> Set the attribute value.</summary>
		/// <param name="value">The value of the attribute to accept.
		/// If <code>null</code>, any tag with the attribute,
		/// no matter what it's value is acceptable.
		/// </param>
		virtual public System.String AttributeValue
		{
			get
			{
				return (mValue);
			}
			
			set
			{
				mValue = value;
			}
		}

		/// <summary> Creates a new instance of HasAttributeFilter.
		/// With no attribute name, this would always return <code>false</code>
		/// from {@link #accept}.
		/// </summary>
		public HasAttributeFilter():this("", null)
		{
		}
		
		/// <summary> Creates a new instance of HasAttributeFilter that accepts tags
		/// with the given attribute.
		/// </summary>
		/// <param name="attribute">The attribute to search for.
		/// </param>
		public HasAttributeFilter(System.String attribute):this(attribute, null)
		{
		}
		
		/// <summary> Creates a new instance of HasAttributeFilter that accepts tags
		/// with the given attribute and value.
		/// </summary>
		/// <param name="attribute">The attribute to search for.
		/// </param>
		/// <param name="value">The value that must be matched,
		/// or null if any value will match.
		/// </param>
		public HasAttributeFilter(System.String attribute, System.String value_Renamed)
		{
			mAttribute = attribute.ToUpper(new System.Globalization.CultureInfo("en"));
			mValue = value_Renamed;
		}
		
		/// <summary> Accept tags with a certain attribute.</summary>
		/// <param name="node">The node to check.
		/// </param>
		/// <returns> <code>true</code> if the node has the attribute
		/// (and value if that is being checked too), <code>false</code> otherwise.
		/// </returns>
		public virtual bool Accept(INode node)
		{
			ITag tag;
			TagAttribute attribute;
			bool ret;
			
			ret = false;
			if (node is ITag)
			{
				tag = (ITag) node;
				attribute = tag.GetAttributeEx(mAttribute);
				ret = null != attribute;
				if (ret && (null != mValue))
				{
					ret = mValue.Equals(attribute.GetValue());
				}
			}
			
			return (ret);
		}
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		virtual public System.Object Clone()
		{
			return null;
		}
	}
}
