// ***************************************************************
//  CharacterReference   version:  1.0   Date: 12/19/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util.Sort;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary> Structure to hold a character and it's equivalent entity reference kernel.
	/// For the character reference &amp;copy; the character would be '&copy;' and
	/// the kernel would be "copy", for example.<p>
	/// Character references are described at <a href="Character references">http://www.w3.org/TR/REC-html40/charset.html#entities</a>
	/// Supports the Ordered interface so it's easy to create a list sorted by
	/// kernel, to perform binary searches on.<p>
	/// </summary>
	[Serializable]
	public class CharacterReference : System.ICloneable, IOrdered
	{
		#region Class Members
		/// <summary> The character value as an integer.</summary>
		protected internal int m_iCharacter;
		
		/// <summary> This entity reference kernel.
		/// The text between the ampersand and the semicolon.
		/// </summary>
		protected internal System.String m_strKernel = String.Empty;
		#endregion

		/// <summary> Construct a <code>CharacterReference</code> with the character and kernel given.</summary>
		/// <param name="kernel">The kernel in the equivalent character entity reference.
		/// </param>
		/// <param name="character">The character needing encoding.
		/// </param>
		public CharacterReference(System.String kernel, int character)
		{
			m_strKernel = kernel;
			m_iCharacter = character;
			if (null == m_strKernel)
			{
				m_strKernel = "";
			}
		}

		#region Public Properties
		/// <summary>
		/// Gets or sets CharacterReference's kernel. 
		/// </summary>
		/// <value> The kernel in the equivalent character entity reference.
		/// </value>
		virtual public String Kernel
		{
			get
			{
				return this.m_strKernel;
			}
			set
			{
				this.m_strKernel = value;
			}
		}

		/// <summary>
		/// Gets or sets the character.
		/// </summary>
		/// <value>The character needing translation.</value>
		virtual public Int32 Character
		{
			get
			{
				return this.m_iCharacter;
			}
			set
			{
				this.m_iCharacter = value;
			}
		}

		#endregion

		/// <summary> Visualize this character reference as a string.</summary>
		/// <returns> A string with the character and kernel.
		/// </returns>
		public override System.String ToString()
		{
			System.String hex;
			System.Text.StringBuilder ret;
			
			ret = new System.Text.StringBuilder(6 + 8 + 2); // max 8 in string
			hex = System.Convert.ToString(Character, 16);
			ret.Append("\\u");
			for (int i = hex.Length; i < 4; i++)
			{
				ret.Append("0");
			}
			ret.Append(hex);
			ret.Append("[");
			ret.Append(Kernel);
			ret.Append("]");
			
			return (ret.ToString());
		}

		//
		#region IOrdered interface
		//
		
		/// <summary> Compare one reference to another.</summary>
		/// <seealso cref="IOrdered">
		/// </seealso>
		public virtual int Compare(System.Object that)
		{
			CharacterReference r;
			
			r = (CharacterReference) that;
			
			return (String.CompareOrdinal(Kernel, r.Kernel));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		virtual public System.Object Clone()
		{
			return null;
		}
		#endregion
	}
}
