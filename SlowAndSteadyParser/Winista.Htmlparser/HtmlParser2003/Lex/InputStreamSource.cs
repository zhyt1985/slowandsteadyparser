// ***************************************************************
//  InputStreamSource   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Http;

namespace Winista.Text.HtmlParser.Lex
{
	/// <summary> A source of characters based on an InputStream such as from a URLConnection.</summary>
	[Serializable]
	public class InputStreamSource : Source, System.Runtime.Serialization.ISerializable
	{
		/// <summary> An initial buffer size.
		/// Has a default value of {@value}.
		/// </summary>
		public static int BUFFER_SIZE = 16384;
		
		/// <summary> The stream of bytes.
		/// Set to <code>null</code> when the source is closed.
		/// </summary>
		[NonSerialized]
		protected internal System.IO.Stream mStream;
		
		/// <summary> The character set in use.</summary>
		protected internal System.String mEncoding;
		
		/// <summary> The converter from bytes to characters.</summary>
		[NonSerialized]
		protected internal System.IO.StreamReader mReader;
		
		/// <summary> The characters read so far.</summary>
		protected internal char[] mBuffer;
		
		/// <summary> The number of valid bytes in the buffer.</summary>
		protected internal int mLevel;
		
		/// <summary> The offset of the next byte returned by read().</summary>
		protected internal int mOffset;
		
		/// <summary> The bookmark.</summary>
		protected internal int mMark;

		/// <summary> Create a source of characters using the default character set.</summary>
		/// <param name="stream">The stream of bytes to use.
		/// </param>
		/// <exception cref="UnsupportedEncodingException">If the default character set
		/// is unsupported.
		/// </exception>
		public InputStreamSource(System.IO.Stream stream):this(stream, null, BUFFER_SIZE)
		{
		}

		/// <summary> Create a source of characters.</summary>
		/// <param name="stream">The stream of bytes to use.
		/// </param>
		/// <param name="charset">The character set used in encoding the stream.
		/// </param>
		/// <exception cref="UnsupportedEncodingException">If the character set
		/// is unsupported.
		/// </exception>
		public InputStreamSource(System.IO.Stream stream, System.String charset):this(stream, charset, BUFFER_SIZE)
		{
		}

		/// <summary> Create a source of characters.</summary>
		/// <param name="stream">The stream of bytes to use.
		/// </param>
		/// <param name="charset">The character set used in encoding the stream.
		/// </param>
		/// <param name="size">The initial character buffer size.
		/// </param>
		/// <exception cref="UnsupportedEncodingException">If the character set
		/// is unsupported.
		/// </exception>
		public InputStreamSource(System.IO.Stream stream, System.String charset, int size)
			: base(stream)
		{
			if (null == stream)
				stream = new ParserStream(null);
				// bug #1044707 mark()/reset() issues
			else
			{
				//UPGRADE_ISSUE: Method 'java.io.InputStream.markSupported' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaioInputStreammarkSupported'"
				//if (!stream.markSupported())
					// wrap the stream so we can reset
				//	stream = new Stream(stream);
			}
			// else
			// just because mark is supported doesn't guarantee
			// proper reset operation; there is no call to mark
			// in this code, so if reset misbehaves there is an
			// appropriate message in setEncoding() to suggest
			// wraping it in a Stream.
			// This was deemed better than an attempt to call
			// reset at this point just to check if we would
			// succeed later, or to call mark with an arbitrary
			// lookahead size
			mStream = stream;
			if (null == charset || String.Empty == charset)
			{
				mReader = new System.IO.StreamReader(stream, System.Text.Encoding.Default);
				mEncoding = mReader.CurrentEncoding.EncodingName;
			}
			else
			{
				mEncoding = charset;
				mReader = new System.IO.StreamReader(stream, System.Text.Encoding.GetEncoding(charset));
			}
			mBuffer = new char[size];
			mLevel = 0;
			mOffset = 0;
			mMark = - 1;
		}

		/// <summary> Get the input stream being used.</summary>
		/// <returns> The current input stream.
		/// </returns>
		virtual public System.IO.Stream Stream
		{
			get
			{
				return (mStream);
			}
			
		}

		/// <summary> Get the encoding being used to convert characters.</summary>
		/// <returns> The current encoding.
		/// </returns>
		/// <summary> Begins reading from the source with the given character set.
		/// If the current encoding is the same as the requested encoding,
		/// this method is a no-op. Otherwise any subsequent characters read from
		/// this page will have been decoded using the given character set.<p>
		/// Some magic happens here to obtain this result if characters have already
		/// been consumed from this source.
		/// Since a Reader cannot be dynamically altered to use a different character
		/// set, the underlying stream is reset, a new Source is constructed
		/// and a comparison made of the characters read so far with the newly
		/// read characters up to the current position.
		/// If a difference is encountered, or some other problem occurs,
		/// an exception is thrown.
		/// </summary>
		/// <param name="character_set">The character set to use to convert bytes into
		/// characters.
		/// </param>
		/// <exception cref="ParserException">If a character mismatch occurs between
		/// characters already provided and those that would have been returned
		/// had the new character set been in effect from the beginning. An
		/// exception is also thrown if the underlying stream won't put up with
		/// these shenanigans.
		/// </exception>
		override public System.String Encoding
		{
			get
			{
				return (mEncoding);
			}
			
			set
			{
				if (String.Compare(value, "x-user-defined", true) == 0)
				{
					return;
				}
				System.String encoding;
				System.IO.Stream stream;
				char[] buffer;
				int offset;
				char[] new_chars;
				
				encoding = Encoding;
				if (!encoding.ToUpper().Equals(value.ToUpper()))
				{
					stream = Stream;
					try
					{
						buffer = mBuffer;
						offset = mOffset;
						//UPGRADE_ISSUE: Method 'java.io.InputStream.reset' was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1000_javaioInputStreamreset'"
						stream.Seek(0, System.IO.SeekOrigin.Begin);
						try
						{
							mEncoding = value;
							//UPGRADE_TODO: Constructor 'java.io.InputStreamReader.InputStreamReader' was converted to 'System.IO.StreamReader' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioInputStreamReaderInputStreamReader_javaioInputStream_javalangString'"
							mReader = new System.IO.StreamReader(stream, System.Text.Encoding.GetEncoding(value));
							mBuffer = new char[mBuffer.Length];
							mLevel = 0;
							mOffset = 0;
							mMark = - 1;
							if (0 != offset)
							{
								new_chars = new char[offset];
								if (offset != Read(new_chars))
									throw new ParserException("reset stream failed");
								for (int i = 0; i < offset; i++)
									if (new_chars[i] != buffer[i])
									{
										String strMsg = "character mismatch (new: " + new_chars[i] + " [0x" + System.Convert.ToString(new_chars[i], 16) + "] != old: " + " [0x" + System.Convert.ToString(buffer[i], 16) + buffer[i] + "]) for encoding change from " + encoding + " to " + value + " at character offset " + i;
										if (HttpProtocol.CONTENTTYPE_CHANGE_STRICT)
										{
											throw new EncodingChangeException(strMsg);
										}
										else
										{
											System.Diagnostics.Trace.WriteLine(strMsg);
										}
									}
							}
						}
						catch (System.IO.IOException ioe)
						{
							//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.getMessage' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
							throw new ParserException(ioe.Message, ioe);
						}
					}
					catch (System.IO.IOException ioe)
					{
						// bug #1044707 mark()/reset() issues
						throw new ParserException("Stream reset failed (" + ioe.Message + "), try wrapping it with a Winista.Text.HtmlParser.Lex.Stream", ioe);
					}
				}
			}
		}


		//
		// Serialization support
		//
		
		/// <summary> Serialization support.</summary>
		/// <param name="out">Where to write this object.
		/// </param>
		/// <exception cref="IOException">If serialization has a problem.
		/// </exception>
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo out_Renamed, System.Runtime.Serialization.StreamingContext context)
		{
			int offset;
			char[] buffer;
			
			if (null != mStream)
			{
				// remember the offset, drain the input stream, restore the offset
				offset = mOffset;
				buffer = new char[4096];
				while (EOF != Read(buffer))
					;
				mOffset = offset;
			}
			
			Support.SupportMisc.DefaultWriteObject(out_Renamed, context, this);
		}
		
		/// <summary> Deserialization support.</summary>
		/// <param name="in">Where to read this object from.
		/// </param>
		/// <exception cref="IOException">If deserialization has a problem.
		/// </exception>
		//UPGRADE_TODO: Class 'java.io.ObjectInputStream' was converted to 'System.IO.BinaryReader' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioObjectInputStream'"
		//UPGRADE_TODO: Method 'readObject' was converted to 'InputStreamSource' and its parameters were changed. The code must be reviewed in order to assure that no calls to non-member methods of the converted parameters are made. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1192'"
		protected InputStreamSource(System.Runtime.Serialization.SerializationInfo in_Renamed, System.Runtime.Serialization.StreamingContext context)
			:base(new System.IO.MemoryStream())
		{
			Support.SupportMisc.DefaultReadObject(in_Renamed, context, this);
			if (null != mBuffer)
				// buffer is null when destroy's been called
				// pretend we're open, mStream goes null when exhausted
				mStream = new System.IO.MemoryStream();
		}
		
		/// <summary> Fetch more characters from the underlying reader.
		/// Has no effect if the underlying reader has been drained.
		/// </summary>
		/// <param name="min">The minimum to read.
		/// </param>
		/// <exception cref="IOException">If the underlying reader read() throws one.
		/// </exception>
		protected internal virtual void  Fill(int min)
		{
			char[] buffer;
			int size;
			int read;
			
			if (null != mReader)
				// mReader goes null when it's been sucked dry
			{
				size = mBuffer.Length - mLevel; // available space
				if (size < min)
					// oops, better get some buffer space
				{
					// unknown length... keep doubling
					size = mBuffer.Length * 2;
					read = mLevel + min;
					if (size < read)
						// or satisfy min, whichever is greater
						size = read;
					else
						min = size - mLevel; // read the max
					buffer = new char[size];
				}
				else
				{
					buffer = mBuffer;
					min = size;
				}
				
				// read into the end of the 'new' buffer
				//UPGRADE_TODO: Method 'java.io.InputStreamReader.read' was converted to 'System.IO.StreamReader.Read' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioInputStreamReaderread_char[]_int_int'"
				read = mReader.Read(buffer, mLevel, min);
				if (EOF == read)
				{
					mReader.Close();
					mReader = null;
				}
				else
				{
					if (mBuffer != buffer)
					{
						// copy the bytes previously read
						Array.Copy(mBuffer, 0, buffer, 0, mLevel);
						mBuffer = buffer;
					}
					mLevel += read;
				}
				// todo, should repeat on read shorter than original min
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
		
		/// <summary> Read a single character.
		/// This method will block until a character is available,
		/// an I/O error occurs, or the end of the stream is reached.
		/// </summary>
		/// <returns> The character read, as an integer in the range 0 to 65535
		/// (<tt>0x00-0xffff</tt>), or {@link #EOF EOF} if the end of the stream has
		/// been reached
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		public  override int Read()
		{
			int ret;
			
			if (mLevel - mOffset < 1)
			{
				if (null == mStream)
					throw new System.IO.IOException("source is closed");
				Fill(1);
				if (mOffset >= mLevel)
					ret = EOF;
				else
					ret = mBuffer[mOffset++];
			}
			else
				ret = mBuffer[mOffset++];
			
			return (ret);
		}
		
		/// <summary> Read characters into a portion of an array.  This method will block
		/// until some input is available, an I/O error occurs, or the end of the
		/// stream is reached.
		/// </summary>
		/// <param name="cbuf">Destination buffer
		/// </param>
		/// <param name="off">Offset at which to start storing characters
		/// </param>
		/// <param name="len">Maximum number of characters to read
		/// </param>
		/// <returns> The number of characters read, or {@link #EOF EOF} if the end of
		/// the stream has been reached
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		public  override int Read(System.Char[] cbuf, int off, int len)
		{
			int ret;
			
			if (null == mStream)
				throw new System.IO.IOException("source is closed");
			if ((null == cbuf) || (0 > off) || (0 > len))
				throw new System.IO.IOException("illegal argument read (" + ((null == cbuf)?"null":"cbuf") + ", " + off + ", " + len + ")");
			if (mLevel - mOffset < len)
				Fill(len - (mLevel - mOffset)); // minimum to satisfy this request
			if (mOffset >= mLevel)
				ret = EOF;
			else
			{
				ret = System.Math.Min(mLevel - mOffset, len);
				Array.Copy(mBuffer, mOffset, cbuf, off, ret);
				mOffset += ret;
			}
			
			return (ret);
		}
		
		/// <summary> Read characters into an array.
		/// This method will block until some input is available, an I/O error occurs,
		/// or the end of the stream is reached.
		/// </summary>
		/// <param name="cbuf">Destination buffer.
		/// </param>
		/// <returns> The number of characters read, or {@link #EOF EOF} if the end of
		/// the stream has been reached.
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		public override int Read(char[] cbuf)
		{
			//UPGRADE_TODO: Method 'java.io.Reader.read' was converted to 'System.IO.StreamReader.Read' which has a different behavior. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1073_javaioReaderread_char[]_int_int'"
			return (Read(cbuf, 0, cbuf.Length));
		}
		
		/// <summary> Reset the source.
		/// Repositions the read point to begin at zero.
		/// </summary>
		/// <exception cref="IllegalStateException">If the source has been closed.
		/// </exception>
		public override void  Reset()
		{
			if (null == mStream)
				throw new System.SystemException("source is closed");
			if (- 1 != mMark)
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
		/// <exception cref="IOException">If the source is closed.
		/// 
		/// </exception>
		public override void Mark(int readAheadLimit)
		{
			if (null == mStream)
				throw new System.IO.IOException("source is closed");
			mMark = mOffset;
		}
		
		/// <summary> Tell whether this source is ready to be read.</summary>
		/// <returns> <code>true</code> if the next read() is guaranteed not to block
		/// for input, <code>false</code> otherwise.
		/// Note that returning false does not guarantee that the next read will block.
		/// </returns>
		/// <exception cref="IOException">If the source is closed.
		/// </exception>
		public override bool Ready()
		{
			if (null == mStream)
				throw new System.IO.IOException("source is closed");
			return (mOffset < mLevel);
		}
		
		/// <summary> Skip characters.
		/// This method will block until some characters are available,
		/// an I/O error occurs, or the end of the stream is reached.
		/// <em>Note: n is treated as an int</em>
		/// </summary>
		/// <param name="n">The number of characters to skip.
		/// </param>
		/// <returns> The number of characters actually skipped
		/// </returns>
		/// <exception cref="IllegalArgumentException">If <code>n</code> is negative.
		/// </exception>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		public override long Skip(long n)
		{
			long ret;
			
			if (null == mStream)
				throw new System.IO.IOException("source is closed");
			if (0 > n)
				throw new System.ArgumentException("cannot skip backwards");
			else
			{
				if (mLevel - mOffset < n)
					Fill((int) (n - (mLevel - mOffset))); // minimum to satisfy this request
				if (mOffset >= mLevel)
					ret = EOF;
				else
				{
					ret = System.Math.Min(mLevel - mOffset, n);
					mOffset = (int) (mOffset + ret);
				}
			}
			
			return (ret);
		}
		
		//
		// Methods not in your Daddy's Reader
		//
		
		/// <summary> Undo the read of a single character.</summary>
		/// <exception cref="IOException">If the source is closed or no characters have
		/// been read.
		/// </exception>
		public override void Unread()
		{
			if (null == mStream)
				throw new System.IO.IOException("source is closed");
			if (0 < mOffset)
				mOffset--;
			else
				throw new System.IO.IOException("can't unread no characters");
		}
		
		/// <summary> Retrieve a character again.</summary>
		/// <param name="offset">The offset of the character.
		/// </param>
		/// <returns> The character at <code>offset</code>.
		/// </returns>
		/// <exception cref="IOException">If the offset is beyond {@link #offset()} or the
		/// source is closed.
		/// </exception>
		public override char GetCharacter(int offset)
		{
			char ret;
			
			if (null == mStream)
				throw new System.IO.IOException("source is closed");
			if (offset >= mBuffer.Length)
				throw new System.IO.IOException("illegal read ahead");
			else
				ret = mBuffer[offset];
			
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
		/// <exception cref="IOException">If the start or end is beyond {@link #offset()}
		/// or the source is closed.
		/// </exception>
		public override void  GetCharacters(char[] array, int offset, int start, int end)
		{
			if (null == mStream)
				throw new System.IO.IOException("source is closed");
			Array.Copy(mBuffer, start, array, offset, end - start);
		}
		
		/// <summary> Retrieve a string.</summary>
		/// <param name="offset">The offset of the first character.
		/// </param>
		/// <param name="length">The number of characters to retrieve.
		/// </param>
		/// <returns> A string containing the <code>length</code> characters at <code>offset</code>.
		/// </returns>
		/// <exception cref="IOException">If the offset or (offset + length) is beyond
		/// {@link #offset()} or the source is closed.
		/// </exception>
		public override System.String GetString(int offset, int length)
		{
			System.String ret;
			
			if (null == mStream)
				throw new System.IO.IOException("source is closed");
			if (offset + length >= mBuffer.Length)
				throw new System.IO.IOException("illegal read ahead");
			else
				ret = new System.String(mBuffer, offset, length);
			
			return (ret);
		}
		
		/// <summary> Append characters already read into a <code>StringBuffer</code>.</summary>
		/// <param name="buffer">The buffer to append to.
		/// </param>
		/// <param name="offset">The offset of the first character.
		/// </param>
		/// <param name="length">The number of characters to retrieve.
		/// </param>
		/// <exception cref="IOException">If the offset or (offset + length) is beyond
		/// {@link #offset()} or the source is closed.
		/// </exception>
		public override void  GetCharacters(System.Text.StringBuilder buffer, int offset, int length)
		{
			if (null == mStream)
				throw new System.IO.IOException("source is closed");
			buffer.Append(mBuffer, offset, length);
		}
		
		/// <summary> Close the source.
		/// Once a source has been closed, further {@link #read() read},
		/// {@link #ready ready}, {@link #mark mark}, {@link #reset reset},
		/// {@link #skip skip}, {@link #unread unread},
		/// {@link #getCharacter getCharacter} or {@link #getString getString}
		/// invocations will throw an IOException.
		/// Closing a previously-closed source, however, has no effect.
		/// </summary>
		/// <exception cref="IOException">If an I/O error occurs
		/// </exception>
		public override void Destroy()
		{
			mStream = null;
			if (null != mReader)
				mReader.Close();
			mReader = null;
			mBuffer = null;
			mLevel = 0;
			mOffset = 0;
			mMark = - 1;
		}
		
		/// <summary> Get the position (in characters).</summary>
		/// <returns> The number of characters that have already been read, or
		/// {@link #EOF EOF} if the source is closed.
		/// </returns>
		public override int Offset()
		{
			int ret;
			
			if (null == mStream)
				ret = EOF;
			else
				ret = mOffset;
			
			return (ret);
		}
		
		/// <summary> Get the number of available characters.</summary>
		/// <returns> The number of characters that can be read without blocking or
		/// zero if the source is closed.
		/// </returns>
		public override int Available()
		{
			int ret;
			
			if (null == mStream)
				ret = 0;
			else
				ret = mLevel - mOffset;
			
			return (ret);
		}
		//UPGRADE_NOTE: A parameterless constructor was added for a serializable class to avoid compile errors. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1268'"
		public InputStreamSource() : base(new System.IO.MemoryStream())
		{
		}
	}
}
