// ***************************************************************
//  CharacterReferenceEx   version:  1.0   Date: 12/19/2005
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
	/// <summary> Extended character entity reference.
	/// Handles kernels within other strings, just for lookup purposes.
	/// </summary>
	[Serializable]
	internal class CharacterReferenceEx : CharacterReference
	{
		/// <summary> The starting point in the string.</summary>
		protected internal int mStart;
		
		/// <summary> The ending point in the string.</summary>
		protected internal int mEnd;

		/// <summary> Zero args constructor.
		/// This object is only ever used after setting the kernel, start and end.
		/// </summary>
		public CharacterReferenceEx():base("", 0)
		{
		}


		/// <summary> Set the starting point of the kernel.</summary>
		virtual public int Start
		{
			set
			{
				mStart = value;
			}
		}

		/// <summary> Set the supposed ending point.
		/// This only specifies an upper bound on the kernel length.
		/// </summary>
		virtual public int End
		{
			set
			{
				mEnd = value;
			}
		}

		/// <summary> Get this CharacterReference's kernel.</summary>
		/// <returns> The kernel in the equivalent character entity reference.
		/// </returns>
		public override System.String Kernel
		{
			get
			{
				return (m_strKernel.Substring(mStart, (mEnd) - (mStart)));
			}
		}

		/// <summary> Compare one reference to another.</summary>
		/// <seealso cref="Winista.Text.HtmlParser.Util.Sort.IOrdered">IOrdered</seealso>
		public override int Compare(System.Object that)
		{
			CharacterReference r;
			System.String kernel;
			int length;
			int ret;
			
			ret = 0;
			r = (CharacterReference) that;
			kernel = r.Kernel;
			length = kernel.Length;
			for (int i = mStart, j = 0; i < mEnd; i++, j++)
			{
				if (j >= length)
				{
					ret = 1;
					break;
				}
				ret = m_strKernel[i] - kernel[j];
				if (0 != ret)
					break;
			}
			
			return (ret);
		}
	}
}
