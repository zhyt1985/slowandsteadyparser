// ***************************************************************
//  StringSource   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Lex
{
	/// <summary> A source of characters based on a String.</summary>
	[Serializable]
	public class StringSource : Source
	{
		/// <summary> The source of characters.</summary>
		protected internal System.String mString;
		
		/// <summary> The current offset into the string.</summary>
		protected internal int mOffset;
		
		/// <summary> The encoding to report.
		/// Only used by {@link #getEncoding}.
		/// </summary>
		protected internal System.String mEncoding;
		
		/// <summary> The bookmark.</summary>
		protected internal int mMark;

		/// <summary> Construct a source using the provided string.
		/// Until it is set, the encoding will be reported as ISO-8859-1.
		/// </summary>
		/// <param name="string">The source of characters.
		/// </param>
		public StringSource(System.String string_Renamed):this(string_Renamed, "ISO-8859-1")
		{
		}
		
		/// <summary> Construct a source using the provided string and encoding.
		/// The encoding is only used by {@link #getEncoding}.
		/// </summary>
		/// <param name="string">The source of characters.
		/// </param>
		/// <param name="character_set">The encoding to report.
		/// </param>
		public StringSource(System.String str, System.String character_set)
			:base(new System.IO.MemoryStream())
		{
			mString = (null == str)?"":str;
			mOffset = 0;
			mEncoding = character_set;
			mMark = - 1;
		}

		/// <summary> Get the encoding being used to convert characters.</summary>
		/// <returns> The current encoding.
		/// </returns>
		/// <summary> Set the encoding to the given character set.
		/// This simply sets the encoding reported by {@link #getEncoding}.
		/// </summary>
		/// <param name="character_set">The character set to use to convert characters.
		/// </param>
		/// <exception cref="ParserException"><em>Not thrown</em>.
		/// </exception>
		override public System.String Encoding
		{
			get
			{
				return (mEncoding);
			}
			
			set
			{
				mEncoding = value;
			}
		}

		//
		// Reader overrides
		//
		
		/// <summary> Does nothing.
		/// It's supposed to close the source, but use destroy() instead.
		/// </summary>
		/// <exception cref="IOException"><em>not used</em>
		/// </exception>
		/// <seealso cref="destroy">
		/// </seealso>
		public override void  Close()
		{
		}
		
		/// <summary> Read a single character.</summary>
		/// <returns> The character read, as an integer in the range 0 to 65535
		/// (<tt>0x00-0xffff</tt>), or {@link #EOF EOF} if the source is exhausted.
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		public  override int Read()
		{
			int ret;
			
			if (null == mString)
				throw new System.IO.IOException("source is closed");
			else if (mOffset >= mString.Length)
				ret = EOF;
			else
			{
				ret = mString[mOffset];
				mOffset++;
			}
			
			return (ret);
		}
		
		/// <summary> Read characters into a portion of an array.</summary>
		/// <param name="cbuf">Destination buffer
		/// </param>
		/// <param name="off">Offset at which to start storing characters
		/// </param>
		/// <param name="len">Maximum number of characters to read
		/// </param>
		/// <returns> The number of characters read, or {@link #EOF EOF} if the source
		/// is exhausted.
		/// </returns>
		/// <exception cref="System.IO.IOException">If an I/O error occurs.
		/// </exception>
		public  override int Read(System.Char[] cbuf, int off, int len)
		{
			int length;
			int ret;
			
			if (null == mString)
				throw new System.IO.IOException("source is closed");
			else
			{
				length = mString.Length;
				if (mOffset >= length)
					ret = EOF;
				else
				{
					if (len > length - mOffset)
						len = length - mOffset;
					Support.SupportMisc.GetCharsFromString(mString, mOffset, mOffset + len, cbuf, off);
					mOffset += len;
					ret = len;
				}
			}
			
			return (ret);
		}
		
		/// <summary> Read characters into an array.</summary>
		/// <param name="cbuf">Destination buffer.
		/// </param>
		/// <returns> The number of characters read, or {@link #EOF EOF} if the source
		/// is exhausted.
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		
		public override int Read(char[] cbuf)
		{
			//UPGRADE_TODO: Method 'java.io.Reader.read' was converted to 'System.IO.StreamReader.Read' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioReaderread_char[]_int_int'"
			return (Read(cbuf, 0, cbuf.Length));
		}
		
		/// <summary> Tell whether this source is ready to be read.</summary>
		/// <returns> Equivalent to a non-zero {@link #available()}, i.e. there are
		/// still more characters to read.
		/// </returns>
		/// <exception cref="IOException">Thrown if the source is closed.
		/// </exception>
		public override bool Ready()
		{
			if (null == mString)
				throw new System.IO.IOException("source is closed");
			return (mOffset < mString.Length);
		}
		
		/// <summary> Reset the source.
		/// Repositions the read point to begin at zero.
		/// </summary>
		/// <exception cref="IllegalStateException">If the source has been closed.
		/// </exception>
		public override void Reset()
		{
			if (null == mString)
				throw new System.SystemException("source is closed");
			else if (- 1 != mMark)
				mOffset = mMark;
			else
				mOffset = 0;
		}
		
		/// <summary> Tell whether this source supports the mark() operation.</summary>
		/// <returns> <code>true</code>.
		/// </returns>
		public override bool MarkSupported()
		{
			return (true);
		}
		
		/// <summary> Mark the present position in the source.
		/// Subsequent calls to {@link #reset()}
		/// will attempt to reposition the source to this point.
		/// </summary>
		/// <param name="readAheadLimit"><em>Not used.</em>
		/// </param>
		/// <exception cref="IOException">Thrown if the source is closed.
		/// 
		/// </exception>
		public override void Mark(int readAheadLimit)
		{
			if (null == mString)
				throw new System.IO.IOException("source is closed");
			mMark = mOffset;
		}
		
		/// <summary> Skip characters.
		/// <em>Note: n is treated as an int</em>
		/// </summary>
		/// <param name="n">The number of characters to skip.
		/// </param>
		/// <returns> The number of characters actually skipped
		/// </returns>
		/// <exception cref="IllegalArgumentException">If <code>n</code> is negative.
		/// </exception>
		/// <exception cref="IOException">If the source is closed.
		/// </exception>
		public override long Skip(long n)
		{
			int length;
			long ret;
			
			if (null == mString)
				throw new System.IO.IOException("source is closed");
			if (0 > n)
				throw new System.ArgumentException("cannot skip backwards");
			else
			{
				length = mString.Length;
				if (mOffset >= length)
					n = 0L;
				else if (n > length - mOffset)
					n = length - mOffset;
				mOffset = (int) (mOffset + n);
				ret = n;
			}
			
			return (ret);
		}
		
		//
		// Methods not in your Daddy's Reader
		//
		
		/// <summary> Undo the read of a single character.</summary>
		/// <exception cref="IOException">If no characters have been read or the source is closed.
		/// </exception>
		public override void Unread()
		{
			if (null == mString)
				throw new System.IO.IOException("source is closed");
			else if (mOffset <= 0)
				throw new System.IO.IOException("can't unread no characters");
			else
				mOffset--;
		}
		
		/// <summary> Retrieve a character again.</summary>
		/// <param name="offset">The offset of the character.
		/// </param>
		/// <returns> The character at <code>offset</code>.
		/// </returns>
		/// <exception cref="IOException">If the source is closed or an attempt is made to
		/// read beyond {@link #offset()}.
		/// </exception>
		public override char GetCharacter(int offset)
		{
			char ret;
			
			if (null == mString)
				throw new System.IO.IOException("source is closed");
			else if (offset >= mOffset)
				throw new System.IO.IOException("read beyond current offset");
			else
				ret = mString[offset];
			
			return (ret);
		}
		
		/// <summary> Retrieve characters again.</summary>
		/// <param name="array">The array of characters.
		/// </param>
		/// <param name="offset">The starting position in the array where characters are to be placed.
		/// </param>
		/// <param name="start">The starting position, zero based.
		/// </param>
		/// <param name="end">The ending position
		/// (exclusive, i.e. the character at the ending position is not included),
		/// zero based.
		/// </param>
		/// <exception cref="IOException">If the source is closed or an attempt is made to
		/// read beyond {@link #offset()}.
		/// </exception>
		public override void GetCharacters(char[] array, int offset, int start, int end)
		{
			if (null == mString)
				throw new System.IO.IOException("source is closed");
			else
			{
				if (end > mOffset)
					throw new System.IO.IOException("read beyond current offset");
				else
					Support.SupportMisc.GetCharsFromString(mString, start, end, array, offset);
			}
		}
		
		/// <summary> Retrieve a string comprised of characters already read.
		/// Asking for characters ahead of {@link #offset()} will throw an exception.
		/// </summary>
		/// <param name="offset">The offset of the first character.
		/// </param>
		/// <param name="length">The number of characters to retrieve.
		/// </param>
		/// <returns> A string containing the <code>length</code> characters at <code>offset</code>.
		/// </returns>
		/// <exception cref="IOException">If the source is closed or an attempt is made to
		/// read beyond {@link #offset()}.
		/// </exception>
		public override System.String GetString(int offset, int length)
		{
			System.String ret;
			
			if (null == mString)
				throw new System.IO.IOException("source is closed");
			else
			{
				if (offset + length > mOffset)
					throw new System.IO.IOException("read beyond end of string");
				else
					ret = mString.Substring(offset, (offset + length) - (offset));
			}
			
			return (ret);
		}
		
		/// <summary> Append characters already read into a <code>StringBuffer</code>.
		/// Asking for characters ahead of {@link #offset()} will throw an exception.
		/// </summary>
		/// <param name="buffer">The buffer to append to.
		/// </param>
		/// <param name="offset">The offset of the first character.
		/// </param>
		/// <param name="length">The number of characters to retrieve.
		/// </param>
		/// <exception cref="IOException">If the source is closed or an attempt is made to
		/// read beyond {@link #offset()}.
		/// </exception>
		public override void  GetCharacters(System.Text.StringBuilder buffer, int offset, int length)
		{
			if (null == mString)
				throw new System.IO.IOException("source is closed");
			else
			{
				if (offset + length > mOffset)
					throw new System.IO.IOException("read beyond end of string");
				else
					buffer.Append(mString.Substring(offset, (offset + length) - (offset)));
			}
		}
		
		/// <summary> Close the source.
		/// Once a source has been closed, further {@link #read() read},
		/// {@link #ready ready}, {@link #mark mark}, {@link #reset reset},
		/// {@link #skip skip}, {@link #unread unread},
		/// {@link #getCharacter getCharacter} or {@link #getString getString}
		/// invocations will throw an IOException.
		/// Closing a previously-closed source, however, has no effect.
		/// </summary>
		/// <exception cref="IOException"><em>Not thrown</em>
		/// </exception>
		public override void Destroy()
		{
			mString = null;
		}
		
		/// <summary> Get the position (in characters).</summary>
		/// <returns> The number of characters that have already been read, or
		/// {@link #EOF EOF} if the source is closed.
		/// </returns>
		public override int Offset()
		{
			int ret;
			
			if (null == mString)
				ret = EOF;
			else
				ret = mOffset;
			
			return (ret);
		}
		
		/// <summary> Get the number of available characters.</summary>
		/// <returns> The number of characters that can be read or zero if the source
		/// is closed.
		/// </returns>
		public override int Available()
		{
			int ret;
			
			if (null == mString)
				ret = 0;
			else
				ret = mString.Length - mOffset;
			
			return (ret);
		}
	}
}
