// ***************************************************************
//  Translate   version:  1.0   Date: 12/19/2005
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
	/// <summary> Translate numeric character references and character entity references to unicode characters.
	/// Based on tables found at <a href="http://www.w3.org/TR/REC-html40/sgml/entities.html">
	/// http://www.w3.org/TR/REC-html40/sgml/entities.html</a>
	/// <p>Typical usage:
	/// <pre>
	/// String s = Translate.decode (getTextFromHtmlPage ());
	/// </pre>
	/// or
	/// <pre>
	/// String s = "&lt;HTML&gt;" + Translate.encode (getArbitraryText ()) + "&lt;/HTML&gt;";
	/// </pre>
	/// </summary>
	public class Translate
	{
		#region Class Members
		/// <summary> If this member is set <code>true</code>, decoding of streams is
		/// done line by line in order to reduce the maximum memory required.
		/// </summary>
		public static bool DECODE_LINE_BY_LINE = false;
		
		/// <summary> If this member is set <code>true</code>, encoding of numeric character
		/// references uses hexadecimal digits, i.e. &amp;#x25CB;, instead of decimal
		/// digits.
		/// </summary>
		public static bool ENCODE_HEXADECIMAL = false;

		/// <summary> Table mapping entity reference kernel to character.
		/// This is sorted by kernel when the class is loaded.
		/// </summary>
		protected internal static readonly CharacterReference[] mCharacterReferences = null;

		/// <summary> The dividing point between a simple table lookup and a binary search.
		/// Characters below the break point are stored in a sparse array allowing
		/// direct index lookup.
		/// </summary>
		protected internal const int BREAKPOINT = 0x100;
		
		/// <summary> List of references sorted by character.
		/// The first part of this array, up to <code>BREAKPOINT</code> is stored
		/// in a direct translational table, indexing into the table with a character
		/// yields the reference. The second part is dense and sorted by character,
		/// suitable for binary lookup.
		/// </summary>
		protected internal static CharacterReference[] mCharacterList;

		#endregion

		/// <summary> Private constructor.
		/// This class is fully static and thread safe.
		/// </summary>
		private Translate()
		{
		}

		/// <summary> Binary search for a reference.</summary>
		/// <param name="array">The array of <code>CharacterReference</code> objects.
		/// </param>
		/// <param name="ref_Renamedref">The character to search for.
		/// </param>
		/// <param name="lo">The lower index within which to look.
		/// </param>
		/// <param name="hi">The upper index within which to look.
		/// </param>
		/// <returns> The index at which reference was found or is to be inserted.
		/// </returns>
		protected internal static int LookUp(CharacterReference[] array, char ref_Renamed, int lo, int hi)
		{
			int num;
			int mid;
			int half;
			int result;
			int ret;
			
			ret = - 1;
			
			num = (hi - lo) + 1;
			while ((- 1 == ret) && (lo <= hi))
			{
				half = num / 2;
				mid = lo + ((0 != (num & 1))?half:half - 1);
				result = ref_Renamed - array[mid].Character;
				if (0 == result)
					ret = mid;
				else if (0 > result)
				{
					hi = mid - 1;
					num = ((0 != (num & 1))?half:half - 1);
				}
				else
				{
					lo = mid + 1;
					num = half;
				}
			}
			if (- 1 == ret)
				ret = lo;
			
			return (ret);
		}

		/// <summary> Look up a reference by character.
		/// Use a combination of direct table lookup and binary search to find
		/// the reference corresponding to the character.
		/// </summary>
		/// <param name="character">The character to be looked up.
		/// </param>
		/// <returns> The entity reference for that character or <code>null</code>.
		/// </returns>
		public static CharacterReference LookUp(char character)
		{
			int index;
			CharacterReference ret;
			
			if (character < BREAKPOINT)
			{
				ret = mCharacterList[character];
			}
			else
			{
				index = LookUp(mCharacterList, character, BREAKPOINT, mCharacterList.Length - 1);
				if (index < mCharacterList.Length)
				{
					ret = mCharacterList[index];
					if (character != ret.Character)
						ret = null;
				}
				else
					ret = null;
			}
			
			return (ret);
		}

		/// <summary> Look up a reference by kernel.
		/// Use a binary search on the ordered list of known references.
		/// Since the binary search returns the position at which a new item should
		/// be inserted, we check the references earlier in the list if there is
		/// a failure.
		/// </summary>
		/// <param name="key">A character reference with the kernel set to the string
		/// to be found. It need not be truncated at the exact end of the reference.
		/// </param>
		protected internal static CharacterReference LookUp(CharacterReference key)
		{
			System.String string_Renamed;
			int index;
			System.String kernel;
			char character;
			CharacterReference test;
			CharacterReference ret;
			
			// Care should be taken here because some entity references are
			// prefixes of others, i.e.:
			// \u2209[notin] \u00ac[not]
			// \u00ba[ordm] \u2228[or]
			// \u03d6[piv] \u03c0[pi]
			// \u00b3[sup3] \u2283[sup]
			ret = null;
			index = SortImpl.Bsearch(mCharacterReferences, key);
			string_Renamed = key.Kernel;
			if (index < mCharacterReferences.Length)
			{
				ret = mCharacterReferences[index];
				kernel = ret.Kernel;
				if (!(String.Compare(string_Renamed, 0, kernel, 0, kernel.Length) == 0))
				{
					// not exact, check references starting with same character
					// to see if a subset matches
					ret = null;
				}
			}
			if (null == ret)
			{
				character = string_Renamed[0];
				while (--index >= 0)
				{
					test = mCharacterReferences[index];
					kernel = test.Kernel;
					if (character == kernel[0])
					{
						if (String.Compare(string_Renamed, 0, kernel, 0, kernel.Length) == 0)
						{
							ret = test;
							break;
						}
					}
					else
						break;
				}
			}
			
			return (ret);
		}
		
		/// <summary> Look up a reference by kernel.
		/// Use a binary search on the ordered list of known references.
		/// <em>This is not very efficient, use Translate.Lookup(CharacterReference)
		/// instead.</em>
		/// </summary>
		/// <param name="kernel">The string to lookup, i.e. "amp".
		/// </param>
		/// <param name="start">The starting point in the string of the kernel.
		/// </param>
		/// <param name="end">The ending point in the string of the kernel.
		/// This should be the index of the semicolon if it exists, or failing that,
		/// at least an index past the last character of the kernel.
		/// </param>
		/// <returns> The reference that matches the given string, or <code>null</code>
		/// if it wasn't found.
		/// </returns>
		public static CharacterReference LookUp(System.String kernel, int start, int end)
		{
			CharacterReferenceEx probe;
			
			probe = new CharacterReferenceEx();
			probe.Kernel = kernel;
			probe.Start = start;
			probe.End = end;
			
			return (LookUp(probe));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="strEncoded"></param>
		/// <returns></returns>
		public static String Decode (String strEncoded)
		{
			CharacterReferenceEx key;
			int amp;
			int index;
			int length;
			System.Text.StringBuilder buffer = new System.Text.StringBuilder();
			char character;
			int number;
			int radix;
			int i;
			int semi;
			bool done;
			CharacterReference item;
			String ret;

			if (-1 == (amp = strEncoded.IndexOf ('&')))
				ret = strEncoded;
			else
			{
				key = null;
				index = 0;
				length = strEncoded.Length;
				buffer = new System.Text.StringBuilder (length);
				do
				{
					// equivalent to buffer.append (string.substring (index, amp));
					// but without the allocation of a new String
				while (index < amp)
				{
					buffer.Append (strEncoded[index++]);
				}
                
					index++;
					if (index < length)
					{
						character = strEncoded[index];
						if ('#' == character)
						{
							// numeric character reference
							index++;
							number = 0;
							radix = 0;
							i = index;
							done = false;
							while ((i < length) && !done)
							{
								character = strEncoded[i];
								switch (character)
								{
									case '0':
									case '1':
									case '2':
									case '3':
									case '4':
									case '5':
									case '6':
									case '7':
									case '8':
									case '9':
										if (0 == radix)
										{
											radix = 10;
										}
										number = number * radix + (character - '0');
										break;
									case 'A':
									case 'B':
									case 'C':
									case 'D':
									case 'E':
									case 'F':
										if (16 == radix)
											number = number * radix + (character - 'A' + 10);
										else
											done = true;
										break;
									case 'a':
									case 'b':
									case 'c':
									case 'd':
									case 'e':
									case 'f':
										if (16 == radix)
											number = number * radix + (character - 'a' + 10);
										else
											done = true;
										break;
									case 'x':
									case 'X':
										if (0 == radix)
											radix = 16;
										else
											done = true;
										break;
									case ';':
										done = true;
										i++;
										break;
									default:
										done = true;
										break;
								}
								if (!done)
								{
									i++;
								}
							}
							if (0 != number)
							{
								buffer.Append ((char)number);
								index = i;
								amp = index;
							}
                        
						}
						else if (char.IsLetter (character)) // Can't start with a digit
						{
							// character entity reference
							i = index + 1;
							done = false;
							semi = length;
							while ((i < length) && !done)
							{
								character = strEncoded[i];
								if (';' == character)
								{
									done = true;
									semi = i;
									i++;
								}
								else if (char.IsLetterOrDigit (character))
									i++;
								else
								{
									done = true;
									semi = i;
								}
							}
							// new CharacterReference (string.substring (index, semi), 0);
							if (null == key)
								key = new CharacterReferenceEx ();
							key.Kernel = strEncoded;
							key.Start = index;
							key.End = semi;
							item = LookUp (key);
							if (null != item)
							{
								buffer.Append ((char)item.Character);
								index += item.Kernel.Length;
								if ((index < length) && (';' == strEncoded[index]))
									index++;
								amp = index;
							}
						}
						else
						{
							// need do nothing here, the ampersand will be consumed below
						}
					}
					// gather up unconsumed characters
				while (amp < index)
				{
					buffer.Append (strEncoded[amp++]);
				}
				}
				while ((index < length) && (-1 != (amp = strEncoded.IndexOf ('&', index))));
				// equivalent to buffer.append (string.substring (index));
				// but without the allocation of a new String
				while (index < length)
					buffer.Append (strEncoded[index++]);
				ret = buffer.ToString ();
			}

			return (ret);
		}

		/// <summary>
		/// Decode the characters in a string buffer containing references.
		/// Change all numeric character reference and character entity references
		/// to unicode characters.
		/// </summary>
		/// <param name="buffer">The StringBuffer containing references.</param>
		/// <returns>The decoded string.</returns>
		public static String Decode (System.Text.StringBuilder buffer)
		{
			return Decode (buffer.ToString());
		}

		#region Static Constructor
		static Translate()
		{
			Int32 idx = 0;
			mCharacterReferences = new CharacterReference[252];
			mCharacterReferences[idx++] = new CharacterReference("nbsp", '\u00a0');
			mCharacterReferences[idx++] = new CharacterReference("iexcl", '\u00a1');
			mCharacterReferences[idx++] = new CharacterReference("cent", '\u00a2');
			mCharacterReferences[idx++] = new CharacterReference("pound", '\u00a3');
			mCharacterReferences[idx++] = new CharacterReference("curren", '\u00a4');
			mCharacterReferences[idx++] = new CharacterReference("yen", '\u00a5');
			mCharacterReferences[idx++] = new CharacterReference("brvbar", '\u00a6');
			mCharacterReferences[idx++] = new CharacterReference("sect", '\u00a7');
			mCharacterReferences[idx++] = new CharacterReference("uml", '\u00a8');
			mCharacterReferences[idx++] = new CharacterReference("copy", '\u00a9');
			mCharacterReferences[idx++] = new CharacterReference("ordf", '\u00aa');
			mCharacterReferences[idx++] = new CharacterReference("laquo", '\u00ab');
			mCharacterReferences[idx++] = new CharacterReference("not", '\u00ac');
			mCharacterReferences[idx++] = new CharacterReference("shy", '\u00ad');
			mCharacterReferences[idx++] = new CharacterReference("reg", '\u00ae');
			mCharacterReferences[idx++] = new CharacterReference("macr", '\u00af');
			mCharacterReferences[idx++] = new CharacterReference("deg", '\u00b0');
			mCharacterReferences[idx++] = new CharacterReference("plusmn", '\u00b1');
			mCharacterReferences[idx++] = new CharacterReference("sup2", '\u00b2');
			mCharacterReferences[idx++] = new CharacterReference("sup3", '\u00b3');
			mCharacterReferences[idx++] = new CharacterReference("acute", '\u00b4');
			mCharacterReferences[idx++] = new CharacterReference("micro", '\u00b5');
			mCharacterReferences[idx++] = new CharacterReference("para", '\u00b6');
			mCharacterReferences[idx++] = new CharacterReference("middot", '\u00b7');
			mCharacterReferences[idx++] = new CharacterReference("cedil", '\u00b8');
			mCharacterReferences[idx++] = new CharacterReference("sup1", '\u00b9');
			mCharacterReferences[idx++] = new CharacterReference("ordm", '\u00ba');
			mCharacterReferences[idx++] = new CharacterReference("raquo", '\u00bb');
			mCharacterReferences[idx++] = new CharacterReference("frac14", '\u00bc');
			mCharacterReferences[idx++] = new CharacterReference("frac12", '\u00bd');
			mCharacterReferences[idx++] = new CharacterReference("frac34", '\u00be');
			mCharacterReferences[idx++] = new CharacterReference("iquest", '\u00bf');
			mCharacterReferences[idx++] = new CharacterReference("Agrave", '\u00c0');
			mCharacterReferences[idx++] = new CharacterReference("Aacute", '\u00c1');
			mCharacterReferences[idx++] = new CharacterReference("Acirc", '\u00c2');
			mCharacterReferences[idx++] = new CharacterReference("Atilde", '\u00c3');
			mCharacterReferences[idx++] = new CharacterReference("Auml", '\u00c4');
			mCharacterReferences[idx++] = new CharacterReference("Aring", '\u00c5');
			mCharacterReferences[idx++] = new CharacterReference("AElig", '\u00c6');
			mCharacterReferences[idx++] = new CharacterReference("Ccedil", '\u00c7');
			mCharacterReferences[idx++] = new CharacterReference("Egrave", '\u00c8');
			mCharacterReferences[idx++] = new CharacterReference("Eacute", '\u00c9');
			mCharacterReferences[idx++] = new CharacterReference("Ecirc", '\u00ca');
			mCharacterReferences[idx++] = new CharacterReference("Euml", '\u00cb');
			mCharacterReferences[idx++] = new CharacterReference("Igrave", '\u00cc');
			mCharacterReferences[idx++] = new CharacterReference("Iacute", '\u00cd');
			mCharacterReferences[idx++] = new CharacterReference("Icirc", '\u00ce');
			mCharacterReferences[idx++] = new CharacterReference("Iuml", '\u00cf');
			mCharacterReferences[idx++] = new CharacterReference("ETH", '\u00d0');
			mCharacterReferences[idx++] = new CharacterReference("Ntilde", '\u00d1');
			mCharacterReferences[idx++] = new CharacterReference("Ograve", '\u00d2');
			mCharacterReferences[idx++] = new CharacterReference("Oacute", '\u00d3');
			mCharacterReferences[idx++] = new CharacterReference("Ocirc", '\u00d4');
			mCharacterReferences[idx++] = new CharacterReference("Otilde", '\u00d5');
			mCharacterReferences[idx++] = new CharacterReference("Ouml", '\u00d6');
			mCharacterReferences[idx++] = new CharacterReference("times", '\u00d7');
			mCharacterReferences[idx++] = new CharacterReference("Oslash", '\u00d8');
			mCharacterReferences[idx++] = new CharacterReference("Ugrave", '\u00d9');
			mCharacterReferences[idx++] = new CharacterReference("Uacute", '\u00da');
			mCharacterReferences[idx++] = new CharacterReference("Ucirc", '\u00db');
			mCharacterReferences[idx++] = new CharacterReference("Uuml", '\u00dc');
			mCharacterReferences[idx++] = new CharacterReference("Yacute", '\u00dd');
			mCharacterReferences[idx++] = new CharacterReference("THORN", '\u00de');
			mCharacterReferences[idx++] = new CharacterReference("szlig", '\u00df');
			mCharacterReferences[idx++] = new CharacterReference("agrave", '\u00e0');
			mCharacterReferences[idx++] = new CharacterReference("aacute", '\u00e1');
			mCharacterReferences[idx++] = new CharacterReference("acirc", '\u00e2');
			mCharacterReferences[idx++] = new CharacterReference("atilde", '\u00e3');
			mCharacterReferences[idx++] = new CharacterReference("auml", '\u00e4');
			mCharacterReferences[idx++] = new CharacterReference("aring", '\u00e5');
			mCharacterReferences[idx++] = new CharacterReference("aelig", '\u00e6');
			mCharacterReferences[idx++] = new CharacterReference("ccedil", '\u00e7');
			mCharacterReferences[idx++] = new CharacterReference("egrave", '\u00e8');
			mCharacterReferences[idx++] = new CharacterReference("eacute", '\u00e9');
			mCharacterReferences[idx++] = new CharacterReference("ecirc", '\u00ea');
			mCharacterReferences[idx++] = new CharacterReference("euml", '\u00eb');
			mCharacterReferences[idx++] = new CharacterReference("igrave", '\u00ec');
			mCharacterReferences[idx++] = new CharacterReference("iacute", '\u00ed');
			mCharacterReferences[idx++] = new CharacterReference("icirc", '\u00ee');
			mCharacterReferences[idx++] = new CharacterReference("iuml", '\u00ef');
			mCharacterReferences[idx++] = new CharacterReference("eth", '\u00f0');
			mCharacterReferences[idx++] = new CharacterReference("ntilde", '\u00f1');
			mCharacterReferences[idx++] = new CharacterReference("ograve", '\u00f2');
			mCharacterReferences[idx++] = new CharacterReference("oacute", '\u00f3');
			mCharacterReferences[idx++] = new CharacterReference("ocirc", '\u00f4');
			mCharacterReferences[idx++] = new CharacterReference("otilde", '\u00f5');
			mCharacterReferences[idx++] = new CharacterReference("ouml", '\u00f6');
			mCharacterReferences[idx++] = new CharacterReference("divide", '\u00f7');
			mCharacterReferences[idx++] = new CharacterReference("oslash", '\u00f8');
			mCharacterReferences[idx++] = new CharacterReference("ugrave", '\u00f9');
			mCharacterReferences[idx++] = new CharacterReference("uacute", '\u00fa');
			mCharacterReferences[idx++] = new CharacterReference("ucirc", '\u00fb');
			mCharacterReferences[idx++] = new CharacterReference("uuml", '\u00fc');
			mCharacterReferences[idx++] = new CharacterReference("yacute", '\u00fd');
			mCharacterReferences[idx++] = new CharacterReference("thorn", '\u00fe');
			mCharacterReferences[idx++] = new CharacterReference("yuml", '\u00ff');
			mCharacterReferences[idx++] = new CharacterReference("fnof", '\u0192');
			mCharacterReferences[idx++] = new CharacterReference("Alpha", '\u0391');
			mCharacterReferences[idx++] = new CharacterReference("Beta", '\u0392');
			mCharacterReferences[idx++] = new CharacterReference("Gamma", '\u0393');
			mCharacterReferences[idx++] = new CharacterReference("Delta", '\u0394');
			mCharacterReferences[idx++] = new CharacterReference("Epsilon", '\u0395');
			mCharacterReferences[idx++] = new CharacterReference("Zeta", '\u0396');
			mCharacterReferences[idx++] = new CharacterReference("Eta", '\u0397');
			mCharacterReferences[idx++] = new CharacterReference("Theta", '\u0398');
			mCharacterReferences[idx++] = new CharacterReference("Iota", '\u0399');
			mCharacterReferences[idx++] = new CharacterReference("Kappa", '\u039a');
			mCharacterReferences[idx++] = new CharacterReference("Lambda", '\u039b');
			mCharacterReferences[idx++] = new CharacterReference("Mu", '\u039c');
			mCharacterReferences[idx++] = new CharacterReference("Nu", '\u039d');
			mCharacterReferences[idx++] = new CharacterReference("Xi", '\u039e');
			mCharacterReferences[idx++] = new CharacterReference("Omicron", '\u039f');
			mCharacterReferences[idx++] = new CharacterReference("Pi", '\u03a0');
			mCharacterReferences[idx++] = new CharacterReference("Rho", '\u03a1');
			mCharacterReferences[idx++] = new CharacterReference("Sigma", '\u03a3');
			mCharacterReferences[idx++] = new CharacterReference("Tau", '\u03a4');
			mCharacterReferences[idx++] = new CharacterReference("Upsilon", '\u03a5');
			mCharacterReferences[idx++] = new CharacterReference("Phi", '\u03a6');
			mCharacterReferences[idx++] = new CharacterReference("Chi", '\u03a7');
			mCharacterReferences[idx++] = new CharacterReference("Psi", '\u03a8');
			mCharacterReferences[idx++] = new CharacterReference("Omega", '\u03a9');
			mCharacterReferences[idx++] = new CharacterReference("alpha", '\u03b1');
			mCharacterReferences[idx++] = new CharacterReference("beta", '\u03b2');
			mCharacterReferences[idx++] = new CharacterReference("gamma", '\u03b3');
			mCharacterReferences[idx++] = new CharacterReference("delta", '\u03b4');
			mCharacterReferences[idx++] = new CharacterReference("epsilon", '\u03b5');
			mCharacterReferences[idx++] = new CharacterReference("zeta", '\u03b6');
			mCharacterReferences[idx++] = new CharacterReference("eta", '\u03b7');
			mCharacterReferences[idx++] = new CharacterReference("theta", '\u03b8');
			mCharacterReferences[idx++] = new CharacterReference("iota", '\u03b9');
			mCharacterReferences[idx++] = new CharacterReference("kappa", '\u03ba');
			mCharacterReferences[idx++] = new CharacterReference("lambda", '\u03bb');
			mCharacterReferences[idx++] = new CharacterReference("mu", '\u03bc');
			mCharacterReferences[idx++] = new CharacterReference("nu", '\u03bd');
			mCharacterReferences[idx++] = new CharacterReference("xi", '\u03be');
			mCharacterReferences[idx++] = new CharacterReference("omicron", '\u03bf');
			mCharacterReferences[idx++] = new CharacterReference("pi", '\u03c0');
			mCharacterReferences[idx++] = new CharacterReference("rho", '\u03c1');
			mCharacterReferences[idx++] = new CharacterReference("sigmaf", '\u03c2');
			mCharacterReferences[idx++] = new CharacterReference("sigma", '\u03c3');
			mCharacterReferences[idx++] = new CharacterReference("tau", '\u03c4');
			mCharacterReferences[idx++] = new CharacterReference("upsilon", '\u03c5');
			mCharacterReferences[idx++] = new CharacterReference("phi", '\u03c6');
			mCharacterReferences[idx++] = new CharacterReference("chi", '\u03c7');
			mCharacterReferences[idx++] = new CharacterReference("psi", '\u03c8');
			mCharacterReferences[idx++] = new CharacterReference("omega", '\u03c9');
			mCharacterReferences[idx++] = new CharacterReference("thetasym", '\u03d1');
			mCharacterReferences[idx++] = new CharacterReference("upsih", '\u03d2');
			mCharacterReferences[idx++] = new CharacterReference("piv", '\u03d6');
			mCharacterReferences[idx++] = new CharacterReference("bull", '\u2022');
			mCharacterReferences[idx++] = new CharacterReference("hellip", '\u2026');
			mCharacterReferences[idx++] = new CharacterReference("prime", '\u2032');
			mCharacterReferences[idx++] = new CharacterReference("Prime", '\u2033');
			mCharacterReferences[idx++] = new CharacterReference("oline", '\u203e');
			mCharacterReferences[idx++] = new CharacterReference("frasl", '\u2044');
			mCharacterReferences[idx++] = new CharacterReference("weierp", '\u2118');
			mCharacterReferences[idx++] = new CharacterReference("image", '\u2111');
			mCharacterReferences[idx++] = new CharacterReference("real", '\u211c');
			mCharacterReferences[idx++] = new CharacterReference("trade", '\u2122');
			mCharacterReferences[idx++] = new CharacterReference("alefsym", '\u2135');
			mCharacterReferences[idx++] = new CharacterReference("larr", '\u2190');
			mCharacterReferences[idx++] = new CharacterReference("uarr", '\u2191');
			mCharacterReferences[idx++] = new CharacterReference("rarr", '\u2192');
			mCharacterReferences[idx++] = new CharacterReference("darr", '\u2193');
			mCharacterReferences[idx++] = new CharacterReference("harr", '\u2194');
			mCharacterReferences[idx++] = new CharacterReference("crarr", '\u21b5');
			mCharacterReferences[idx++] = new CharacterReference("lArr", '\u21d0');
			mCharacterReferences[idx++] = new CharacterReference("uArr", '\u21d1');
			mCharacterReferences[idx++] = new CharacterReference("rArr", '\u21d2');
			mCharacterReferences[idx++] = new CharacterReference("dArr", '\u21d3');
			mCharacterReferences[idx++] = new CharacterReference("hArr", '\u21d4');
			mCharacterReferences[idx++] = new CharacterReference("forall", '\u2200');
			mCharacterReferences[idx++] = new CharacterReference("part", '\u2202');
			mCharacterReferences[idx++] = new CharacterReference("exist", '\u2203');
			mCharacterReferences[idx++] = new CharacterReference("empty", '\u2205');
			mCharacterReferences[idx++] = new CharacterReference("nabla", '\u2207');
			mCharacterReferences[idx++] = new CharacterReference("isin", '\u2208');
			mCharacterReferences[idx++] = new CharacterReference("notin", '\u2209');
			mCharacterReferences[idx++] = new CharacterReference("ni", '\u220b');
			mCharacterReferences[idx++] = new CharacterReference("prod", '\u220f');
			mCharacterReferences[idx++] = new CharacterReference("sum", '\u2211');
			mCharacterReferences[idx++] = new CharacterReference("minus", '\u2212');
			mCharacterReferences[idx++] = new CharacterReference("lowast", '\u2217');
			mCharacterReferences[idx++] = new CharacterReference("radic", '\u221a');
			mCharacterReferences[idx++] = new CharacterReference("prop", '\u221d');
			mCharacterReferences[idx++] = new CharacterReference("infin", '\u221e');
			mCharacterReferences[idx++] = new CharacterReference("ang", '\u2220');
			mCharacterReferences[idx++] = new CharacterReference("and", '\u2227');
			mCharacterReferences[idx++] = new CharacterReference("or", '\u2228');
			mCharacterReferences[idx++] = new CharacterReference("cap", '\u2229');
			mCharacterReferences[idx++] = new CharacterReference("cup", '\u222a');
			mCharacterReferences[idx++] = new CharacterReference("int", '\u222b');
			mCharacterReferences[idx++] = new CharacterReference("there4", '\u2234');
			mCharacterReferences[idx++] = new CharacterReference("sim", '\u223c');
			mCharacterReferences[idx++] = new CharacterReference("cong", '\u2245');
			mCharacterReferences[idx++] = new CharacterReference("asymp", '\u2248');
			mCharacterReferences[idx++] = new CharacterReference("ne", '\u2260');
			mCharacterReferences[idx++] = new CharacterReference("equiv", '\u2261');
			mCharacterReferences[idx++] = new CharacterReference("le", '\u2264');
			mCharacterReferences[idx++] = new CharacterReference("ge", '\u2265');
			mCharacterReferences[idx++] = new CharacterReference("sub", '\u2282');
			mCharacterReferences[idx++] = new CharacterReference("sup", '\u2283');
			mCharacterReferences[idx++] = new CharacterReference("nsub", '\u2284');
			mCharacterReferences[idx++] = new CharacterReference("sube", '\u2286');
			mCharacterReferences[idx++] = new CharacterReference("supe", '\u2287');
			mCharacterReferences[idx++] = new CharacterReference("oplus", '\u2295');
			mCharacterReferences[idx++] = new CharacterReference("otimes", '\u2297');
			mCharacterReferences[idx++] = new CharacterReference("perp", '\u22a5');
			mCharacterReferences[idx++] = new CharacterReference("sdot", '\u22c5');
			mCharacterReferences[idx++] = new CharacterReference("lceil", '\u2308');
			mCharacterReferences[idx++] = new CharacterReference("rceil", '\u2309');
			mCharacterReferences[idx++] = new CharacterReference("lfloor", '\u230a');
			mCharacterReferences[idx++] = new CharacterReference("rfloor", '\u230b');
			mCharacterReferences[idx++] = new CharacterReference("lang", '\u2329');
			mCharacterReferences[idx++] = new CharacterReference("rang", '\u232a');
			mCharacterReferences[idx++] = new CharacterReference("loz", '\u25ca');
			mCharacterReferences[idx++] = new CharacterReference("spades", '\u2660');
			mCharacterReferences[idx++] = new CharacterReference("clubs", '\u2663');
			mCharacterReferences[idx++] = new CharacterReference("hearts", '\u2665');
			mCharacterReferences[idx++] = new CharacterReference("diams", '\u2666');
			mCharacterReferences[idx++] = new CharacterReference("quot", '\u0022');
			mCharacterReferences[idx++] = new CharacterReference("amp", '\u0026');
			mCharacterReferences[idx++] = new CharacterReference("lt", '\u003c');
			mCharacterReferences[idx++] = new CharacterReference("gt", '\u003e');
			mCharacterReferences[idx++] = new CharacterReference("OElig", '\u0152');
			mCharacterReferences[idx++] = new CharacterReference("oelig", '\u0153');
			mCharacterReferences[idx++] = new CharacterReference("Scaron", '\u0160');
			mCharacterReferences[idx++] = new CharacterReference("scaron", '\u0161');
			mCharacterReferences[idx++] = new CharacterReference("Yuml", '\u0178');
			mCharacterReferences[idx++] = new CharacterReference("circ", '\u02c6');
			mCharacterReferences[idx++] = new CharacterReference("tilde", '\u02dc');
			mCharacterReferences[idx++] = new CharacterReference("ensp", '\u2002');
			mCharacterReferences[idx++] = new CharacterReference("emsp", '\u2003');
			mCharacterReferences[idx++] = new CharacterReference("thinsp", '\u2009');
			mCharacterReferences[idx++] = new CharacterReference("zwnj", '\u200c');
			mCharacterReferences[idx++] = new CharacterReference("zwj", '\u200d');
			mCharacterReferences[idx++] = new CharacterReference("lrm", '\u200e');
			mCharacterReferences[idx++] = new CharacterReference("rlm", '\u200f');
			mCharacterReferences[idx++] = new CharacterReference("ndash", '\u2013');
			mCharacterReferences[idx++] = new CharacterReference("mdash", '\u2014');
			mCharacterReferences[idx++] = new CharacterReference("lsquo", '\u2018');
			mCharacterReferences[idx++] = new CharacterReference("rsquo", '\u2019');
			mCharacterReferences[idx++] = new CharacterReference("sbquo", '\u201a');
			mCharacterReferences[idx++] = new CharacterReference("ldquo", '\u201c');
			mCharacterReferences[idx++] = new CharacterReference("rdquo", '\u201d');
			mCharacterReferences[idx++] = new CharacterReference("bdquo", '\u201e');
			mCharacterReferences[idx++] = new CharacterReference("dagger", '\u2020');
			mCharacterReferences[idx++] = new CharacterReference("Dagger", '\u2021');
			mCharacterReferences[idx++] = new CharacterReference("permil", '\u2030');
			mCharacterReferences[idx++] = new CharacterReference("lsaquo", '\u2039');
			mCharacterReferences[idx++] = new CharacterReference("rsaquo", '\u203a');
			mCharacterReferences[idx++] = new CharacterReference("euro", '\u20ac');

			int index;
			CharacterReference item;
			int character;
				
			// count below the break point
			index = 0;
			for (int i = 0; i < mCharacterReferences.Length; i++)
			{
				if (mCharacterReferences[i].Character < BREAKPOINT)
				{
					index++;
				}
			}
			// allocate enough for the linear table and remainder
			mCharacterList = new CharacterReference[BREAKPOINT + mCharacterReferences.Length - index];
			index = BREAKPOINT;
			for (int i = 0; i < mCharacterReferences.Length; i++)
			{
				item = mCharacterReferences[i];
				character = mCharacterReferences[i].Character;
				if (character < BREAKPOINT)
					mCharacterList[character] = item;
				else
				{
					// use a linear search and insertion sort, done only once
					int x = BREAKPOINT;
					while (x < index)
						if (mCharacterList[x].Character > character)
							break;
						else
							x++;
					int y = index - 1;
					while (y >= x)
					{
						mCharacterList[y + 1] = mCharacterList[y];
						y--;
					}
					mCharacterList[x] = item;
					index++;
				}
			}
			// reorder the original array into kernel order
			SortImpl.QuickSort(mCharacterReferences);
		}
		#endregion
	}
}

