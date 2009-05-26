// ***************************************************************
//  AttributeRegexFilter   version:  1.0 Date: 08/19/2006
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright (C) 2006 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Winista.Text.HtmlParser.Filters
{
    [Serializable]
    public class AttributeRegexFilter : INodeFilter
    {
        #region Class Members
        private Regex m_obRegex;
        private bool m_bCaseSensitive = false;

        /// <summary> The attribute to check for.</summary>
		protected internal System.String m_strAttribute;
		
		/// <summary> The value regular expression to check for.</summary>
		protected internal System.String m_strValuePattern;
        #endregion

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
				return (m_strAttribute);
			}
			
			set
			{
				m_strAttribute = value;
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
		virtual public System.String AttributeValuePattern
		{
			get
			{
				return (m_strValuePattern);
			}
			
			set
			{
				m_strValuePattern = value;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        public Boolean CaseSensitive
        {
            get { return this.m_bCaseSensitive; }
            set { this.m_bCaseSensitive = value; }
        }

		/// <summary> Creates a new instance of HasAttributeFilter that accepts tags
		/// with the given attribute and value.
		/// </summary>
		/// <param name="attribute">The attribute to search for.
		/// </param>
		/// <param name="value">The value that must be matched,
		/// or null if any value will match.
		/// </param>
        public AttributeRegexFilter(System.String attribute, System.String valuePattern)
		{
            m_strAttribute = attribute.ToUpper(new System.Globalization.CultureInfo("en"));
            m_strValuePattern = valuePattern;
		}

        /// <summary> Creates a new instance of HasAttributeFilter that accepts tags
        /// with the given attribute and value.
        /// </summary>
        /// <param name="attribute">The attribute to search for.
        /// </param>
        /// <param name="value">The value that must be matched,
        /// or null if any value will match.
        /// </param>
        public AttributeRegexFilter(System.String attribute, System.String valuePattern, Boolean caseSensitive)
        {
            m_strAttribute = attribute.ToUpper(new System.Globalization.CultureInfo("en"));
            m_strValuePattern = valuePattern;
            m_bCaseSensitive = caseSensitive;
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
				attribute = tag.GetAttributeEx(m_strAttribute);
				ret = (null != attribute);
				if (ret && (null != m_strValuePattern))
				{
                    CreateMatcher();

					String strValue = attribute.GetValue();
                    ret = m_obRegex.IsMatch(strValue);
				}
			}
			
			return (ret);
		}

		virtual public System.Object Clone()
		{
			return null;
		}

        #region Helper Methods
        private void CreateMatcher()
        {
            if (null == m_obRegex)
            {
                System.Text.RegularExpressions.RegexOptions options = System.Text.RegularExpressions.RegexOptions.Singleline;
                if (!m_bCaseSensitive)
                {
                    options |= System.Text.RegularExpressions.RegexOptions.IgnoreCase;
                }

                m_obRegex = new Regex(m_strValuePattern, options);
            }
        }
        #endregion
    }
}
