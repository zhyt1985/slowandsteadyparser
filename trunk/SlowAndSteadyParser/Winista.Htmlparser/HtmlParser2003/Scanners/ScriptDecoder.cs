// ***************************************************************
//  ScriptDecoder   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Util;

namespace Winista.Text.HtmlParser.Scanners
{
	/// <summary>
	/// Summary description for ScriptDecoder.
	/// </summary>
	public class ScriptDecoder
	{
		/// <summary> Termination state.</summary>
		public const int STATE_DONE = 0;
		
		/// <summary> State on entry.</summary>
		public const int STATE_INITIAL = 1;
		
		/// <summary> State while reading the encoded length.</summary>
		protected internal const int STATE_LENGTH = 2;
		
		/// <summary> State when reading up to decoded text.</summary>
		protected internal const int STATE_PREFIX = 3;
		
		/// <summary> State while decoding.</summary>
		protected internal const int STATE_DECODE = 4;
		
		/// <summary> State when reading an escape sequence.</summary>
		protected internal const int STATE_ESCAPE = 5;
		
		/// <summary> State when reading the checksum.</summary>
		protected internal const int STATE_CHECKSUM = 6;
		
		/// <summary> State while exiting.</summary>
		protected internal const int STATE_FINAL = 7;
		
		/// <summary> The state to enter when decrypting is complete.
		/// If this is STATE_DONE, the decryption will return with any characters
		/// following the encoded text still unconsumed.
		/// Otherwise, if this is STATE_INITIAL, the input will be exhausted and
		/// all following characters will be contained in the return value
		/// of the <code>Decode()</code> method.
		/// </summary>
		public static int LAST_STATE = STATE_DONE;
		
		/// <summary> Table of lookup choice.
		/// The decoding cycles between three flavours determined
		/// by this sequence of 64 choices, corresponding to the
		/// first dimension of the lookup table.
		/// </summary>
		protected internal static sbyte[] mEncodingIndex = new sbyte[]{1, 2, 0, 1, 2, 0, 2, 0, 0, 2, 0, 2, 1, 0, 2, 0, 1, 0, 2, 0, 1, 1, 2, 0, 0, 2, 1, 0, 2, 0, 0, 2, 1, 1, 0, 2, 0, 2, 0, 1, 0, 1, 1, 2, 0, 1, 0, 2, 1, 0, 2, 0, 1, 1, 2, 0, 0, 1, 1, 2, 0, 1, 0, 2};
		
		/// <summary> Two dimensional lookup table.
		/// The decoding uses this table to determine the plaintext for
		/// characters that aren't mEscaped.
		/// </summary>
		protected internal static char[][] mLookupTable = new char[][]{new char[]{'{', '2', '0', '!', ')', '[', '8', '3', '=', 'X', ':', '5', 'e', '9', '\\', 'V', 's', 'f', 'N', 'E', 'k', 'b', 'Y', 'x', '^', '}', 'J', 'm', 'q', (char) (0), '`', (char) (0), 'S', (char) (0), 'B', '\'', 'H', 'r', 'u', '1', '7', 'M', 'R', '"', 'T', 'j', 'G', 'd', '-', ' ', '', '.', 'L', ']', '~', 'l', 'o', 'y', 't', 'C', '&', 'v', '%', '$', '+', '(', '#', 'A', '4', '\t', '*', 'D', '?', 'w', ';', 'U', 'i', 'a', 'c', 'P', 'g', 'Q', 'I', 'O', 'F', 'h', '|', '6', 'p', 'n', 'z', '/', '_', 'K', 'Z', ',', 'W'}, new char[]{'W', '.', 'G', 'z', 'V', 'B', 'j', '/', '&', 'I', 'A', '4', '2', '[', 'v', 'r', 'C', '8', '9', 'p', 'E', 'h', 'q', 'O', '\t', 'b', 'D', '#', 'u', (char) (0), '~', (char) (0), '^', (char) (0), 'w', 'J', 'a', ']', '"', 'K', 'o', 'N', ';', 'L', 'P', 'g', '*', '}', 't', 'T', '+', '-', ',', '0', 'n', 'k', 'f', '5', '%', '!', 'd', 'M', 'R', 'c', '?', '{', 'x', ')', '(', 's', 'Y', '3', '', 'm', 'U', 'S', '|', ':', '_', 'e', 'F', 'X', '1', 'i', 'l', 'Z', 'H', '\'', '\\', '=', '$', 'y', '7', '`', 'Q', ' ', '6'}, new char[]{'n', '-', 'u', 'R', '`', 'q', '^', 'I', '\\', 'b', '}', ')', '6', ' ', '|', 'z', '', 'k', 'c', '3', '+', 'h', 'Q', 'f', 'v', '1', 'd', 'T', 'C', (char) (0), ':', (char) (0), '~', (char) (0), 'E', ',', '*', 't', '\'', '7', 'D', 'y', 'Y', '/', 'o', '&', 'r', 'j', '9', '{', '?', '8', 'w', 'g', 'S', 'G', '4', 'x', ']', '0', '#', 'Z', '[', 'l', 'H', 'U', 'p', 'i', '.', 'L', '!', '$', 'N', 'P', '\t', 'V', 's', '5', 'a', 'K', 'X', ';', 'W', '"', 'm', 'M', '%', '(', 'F', 'J', '2', 'A', '=', '_', 'O', 'B', 'e'}};
		
		/// <summary> The base 64 decoding table.
		/// This array determines the value of decoded base 64 elements.
		/// </summary>
		protected internal static int[] mDigits;
		
		/// <summary> The leader.
		/// The prefix to the encoded script is #@~^nnnnnn== where the n are the
		/// length digits in base64.
		/// </summary>
		protected internal static char[] mLeader = new char[]{'#', '@', '~', '^'};
		
		/// <summary> The prefix.
		/// The prfix separates the encoded text from the length.
		/// </summary>
		protected internal static char[] mPrefix = new char[]{'=', '='};
		
		/// <summary> The trailer.
		/// The suffix to the encoded script is nnnnnn==^#~@ where the n are the
		/// checksum digits in base64. These characters are the part after the checksum.
		/// </summary>
		protected internal static char[] mTrailer = new char[]{'=', '=', '^', '#', '~', '@'};
		
		/// <summary> Escape sequence characters.</summary>
		protected internal static char[] mEscapes = new char[]{'#', '&', '!', '*', '$'};
		
		/// <summary> The escaped characters corresponding to the each escape sequence.</summary>
		protected internal static char[] mEscaped = new char[]{'\r', '\n', '<', '>', '@'};
		
		public ScriptDecoder()
		{
		}

		/// <summary> Extract the base 64 encoded number.
		/// This is a very limited subset of base 64 encoded characters.
		/// Six characters are expected. These are translated into a single long
		/// value. For a more complete base 64 codec see for example the base64
		/// package of <A href="http://sourceforge.net/projects/iharder/" target="_parent">iHarder.net</A>
		/// </summary>
		/// <param name="p">Six base 64 encoded digits.
		/// </param>
		/// <returns> The value of the decoded number.
		/// </returns>
		protected internal static long DecodeBase64(char[] p)
		{
			long ret;
			
			ret = 0;
			
			ret += (mDigits[p[0]] << 2);
			ret += (mDigits[p[1]] >> 4);
			ret += ((mDigits[p[1]] & 0xf) << 12);
			ret += ((mDigits[p[2]] >> 2) << 8);
			ret += ((mDigits[p[2]] & 0x3) << 22);
			ret += (mDigits[p[3]] << 16);
			ret += ((mDigits[p[4]] << 2) << 24);
			ret += ((mDigits[p[5]] >> 4) << 24);
			
			return (ret);
		}

		/// <summary> Decode script encoded by the Microsoft obfuscator.</summary>
		/// <param name="page">The source for encoded text.
		/// </param>
		/// <param name="cursor">The position at which to start decoding.
		/// This is advanced to the end of the encoded text.
		/// </param>
		/// <returns> The plaintext.
		/// </returns>
		/// <exception cref="ParserException">If an error is discovered while decoding.
		/// </exception>
		public static System.String Decode(Page page, Cursor cursor)
		{
			int state;
			int substate_initial;
			int substate_length;
			int substate_prefix;
			int substate_checksum;
			int substate_final;
			long checksum;
			long length;
			char[] buffer;
			buffer = new char[6];
			int index;
			char character;
			int input_character;
			bool found;
			System.Text.StringBuilder ret;
			
			ret = new System.Text.StringBuilder(1024);
			
			state = STATE_INITIAL;
			substate_initial = 0;
			substate_length = 0;
			substate_prefix = 0;
			substate_checksum = 0;
			substate_final = 0;
			length = 0L;
			checksum = 0L;
			index = 0;
			while (STATE_DONE != state)
			{
				input_character = page.GetCharacter(cursor);
				character = (char) input_character;
				if (Page.EOF == input_character)
				{
					if ((STATE_INITIAL != state) || (0 != substate_initial) || (0 != substate_length) || (0 != substate_prefix) || (0 != substate_checksum) || (0 != substate_final))
						throw new ParserException("illegal state for exit");
					state = STATE_DONE;
				}
				else
					switch (state)
					{
						
						case STATE_INITIAL: 
							if (character == mLeader[substate_initial])
							{
								substate_initial++;
								if (substate_initial == mLeader.Length)
								{
									substate_initial = 0;
									state = STATE_LENGTH;
								}
							}
							else
							{
								// oops, flush
								for (int k = 0; 0 < substate_initial; k++)
								{
									ret.Append(mLeader[k++]);
									substate_initial--;
								}
								ret.Append(character);
							}
							break;
						
						
						case STATE_LENGTH: 
							buffer[substate_length] = character;
							substate_length++;
							if (substate_length >= buffer.Length)
							{
								length = DecodeBase64(buffer);
								if (0 > length)
									throw new ParserException("illegal length: " + length);
								substate_length = 0;
								state = STATE_PREFIX;
							}
							break;
						
						
						case STATE_PREFIX: 
							if (character == mPrefix[substate_prefix])
								substate_prefix++;
							else
								throw new ParserException("illegal character encountered: " + (int) character + " ('" + character + "')");
							if (substate_prefix >= mPrefix.Length)
							{
								substate_prefix = 0;
								state = STATE_DECODE;
							}
							break;
						
						
						case STATE_DECODE: 
							if ('@' == character)
								state = STATE_ESCAPE;
							else
							{
								if (input_character < 0x80)
								{
									if (input_character == '\t')
										input_character = 0;
									else if (input_character >= ' ')
										input_character -= (' ' - 1);
									else
										throw new ParserException("illegal encoded character: " + input_character + " ('" + character + "')");
									char ch = mLookupTable[mEncodingIndex[index % 64]][input_character];
									ret.Append(ch);
									checksum += ch;
									index++;
								}
								else
									ret.Append(character);
							}
							length--;
							if (0 == length)
							{
								index = 0;
								state = STATE_CHECKSUM;
							}
							break;
						
						
						case STATE_ESCAPE: 
							found = false;
							for (int i = 0; i < mEscapes.Length; i++)
								if (character == mEscapes[i])
								{
									found = true;
									character = mEscaped[i];
								}
							if (!found)
								throw new ParserException("unexpected escape character: " + (int) character + " ('" + character + "')");
							ret.Append(character);
							checksum += character;
							index++;
							state = STATE_DECODE;
							length--;
							if (0 == length)
							{
								index = 0;
								state = STATE_CHECKSUM;
							}
							break;
						
						
						case STATE_CHECKSUM: 
							buffer[substate_checksum] = character;
							substate_checksum++;
							if (substate_checksum >= buffer.Length)
							{
								long check = DecodeBase64(buffer);
								if (check != checksum)
									throw new ParserException("incorrect checksum, expected " + check + ", calculated " + checksum);
								checksum = 0;
								substate_checksum = 0;
								state = STATE_FINAL;
							}
							break;
						
						
						case STATE_FINAL: 
							if (character == mTrailer[substate_final])
								substate_final++;
							else
								throw new ParserException("illegal character encountered: " + (int) character + " ('" + character + "')");
							if (substate_final >= mTrailer.Length)
							{
								substate_final = 0;
								state = LAST_STATE;
							}
							break;
						
						default: 
							throw new ParserException("invalid state: " + state);
						
					}
			}
			
			return (ret.ToString());
		}
		
		//    /**
		//     * Example mainline for decrypting script.
		//     * Change a file with encrypted script into one without.
		//     * <em>WARNING: This does not preserve DOS type line endings.</em>
		//     * @param args Command line arguments. Two file names, input and output.
		//     * Optionally, the character set to use as a third argument.
		//     * @exception IOException If the input file doesn't exist, or the output
		//     * file cannot be created.
		//     * @exception ParserException If there is a decryption problem.
		//     */
		//    public static void main (String[] args)
		//         throws
		//            IOException,
		//            ParserException
		//    {
		//        String charset;
		//        FileInputStream in;
		//        Page page;
		//        Cursor cursor;
		//        String string;
		//        int ret;
		//        
		//        if (args.length < 2)
		//        {
		//            System.Console.WriteLine ("Usage: ScriptDecoder <infile> <outfile> [charset]");
		//            ret = 1;
		//        }
		//        else
		//        {
		//            if (2 < args.length)
		//                charset = args[2];
		//            else
		//                charset = "ISO-8859-1";
		//            in = new FileInputStream (args[0]);
		//            page = new Page (in, charset);
		//            cursor = new Cursor (page, 0);
		//            ScriptDecoder.LAST_STATE = STATE_INITIAL;
		//            string = ScriptDecoder.Decode (page, cursor);
		//            in.close ();
		//            
		//            FileOutputStream outfile = new FileOutputStream (args[1]);
		//            outfile.write (string.getBytes (charset));
		//            outfile.close ();
		//            ret = (0 != string.length ()) ? 0 : 1;
		//        }
		//        
		//        System.exit (ret);
		//    }
		static ScriptDecoder()
		{
			{
				mDigits = new int[0x7b];
				for (int i = 0; i < 26; i++)
				{
					mDigits['A' + i] = i;
					mDigits['a' + i] = i + 26;
				}
				for (int i = 0; i < 10; i++)
					mDigits['0' + i] = i + 52;
				mDigits[0x2b] = '>';
				mDigits[0x2f] = '?';
			}
		}
	}
}
