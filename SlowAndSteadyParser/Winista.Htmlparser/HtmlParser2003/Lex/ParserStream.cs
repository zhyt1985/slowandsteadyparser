// ***************************************************************
//  ParserStream   version:  1.0   date: 12/18/2005
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
	/// <summary>
	/// Summary description for ParserStream.
	/// </summary>
	public class ParserStream:System.IO.Stream
	{
		/// <summary> The number of calls to fill.
		/// Note: to be removed.
		/// </summary>
		public int fills = 0;
		
		/// <summary> The number of reallocations.
		/// Note: to be removed.
		/// </summary>
		public int reallocations = 0;
		
		/// <summary> The number of synchronous (blocking) fills.
		/// Note: to be removed.
		/// </summary>
		public int synchronous = 0;
		
		/// <summary> An initial buffer size.</summary>
		protected internal const int BUFFER_SIZE = 4096;
		
		/// <summary> Return value when no more characters are left.</summary>
		protected internal const int EOF = - 1;
		
		/// <summary> The underlying stream.</summary>
		protected internal volatile System.IO.Stream mIn;
		
		/// <summary> The bytes read so far.</summary>
		public volatile sbyte[] mBuffer;
		
		/// <summary> The number of valid bytes in the buffer.</summary>
		public volatile int mLevel;
		
		/// <summary> The offset of the next byte returned by read().</summary>
		protected internal int mOffset;
		
		/// <summary> The content length from the HTTP header.</summary>
		protected internal int mContentLength;
		
		/// <summary> The bookmark.</summary>
		protected internal int mMark;

		/// <summary> Construct a stream with no assumptions about the number of bytes available.</summary>
		/// <param name="in">The input stream to use.
		/// </param>
		public ParserStream(System.IO.Stream in_Renamed):this(in_Renamed, 0)
		{
		}

		/// <summary> Construct a stream to read the given number of bytes.</summary>
		/// <param name="in">The input stream to use.
		/// </param>
		/// <param name="bytes">The maximum number of bytes to read.
		/// This should be set to the ContentLength from the HTTP header.
		/// A negative or zero value indicates an unknown number of bytes.
		/// </param>
		public ParserStream(System.IO.Stream in_Renamed, int bytes)
		{
			mIn = in_Renamed;
			mBuffer = null;
			mLevel = 0;
			mOffset = 0;
			mContentLength = bytes < 0?0:bytes;
			mMark = - 1;
		}

		/// <summary> Fetch more bytes from the underlying stream.
		/// Has no effect if the underlying stream has been drained.
		/// </summary>
		/// <param name="force">If <code>true</code>, an attempt is made to read from the
		/// underlying stream, even if bytes are available, If <code>false</code>,
		/// a read of the underlying stream will not occur if there are already
		/// bytes available.
		/// </param>
		/// <returns> <code>true</code> if not at the end of the input stream.
		/// </returns>
		/// <exception cref="IOException">If the underlying stream read() or available() throws one.
		/// </exception>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'fill'. Lock expression was added. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1027'"
		protected internal virtual bool Fill(bool force)
		{
			lock (this)
			{
				int size;
				sbyte[] buffer;
				int read;
				bool ret;
				
				ret = false;
				
				if (null != mIn)
					// mIn goes null when it's been sucked dry
				{
					if (!force)
					{
						// check for change of state while waiting on the monitor in a synchronous call
						if (0 != Available())
							return (true);
						synchronous++;
					}
					
					// get some buffer space
					if (0 == mContentLength)
					{
						// unknown content length... keep doubling
						if (null == mBuffer)
						{
							long available;
							available = mIn.Length - mIn.Position;
							mBuffer = new sbyte[System.Math.Max(BUFFER_SIZE, mIn is ParserStream?((ParserStream) mIn).Available():(int) available)];
							buffer = mBuffer;
						}
						else
						{
							if (mBuffer.Length - mLevel < BUFFER_SIZE / 2)
							{
								long available2;
								available2 = mIn.Length - mIn.Position;
								buffer = new sbyte[System.Math.Max(mBuffer.Length * 2, mBuffer.Length + (mIn is ParserStream?((ParserStream) mIn).Available():(int) available2))];
							}
							else
								buffer = mBuffer;
						}
						size = buffer.Length - mLevel;
					}
					else
					{
						// known content length... allocate once
						size = mContentLength - mLevel;
						if (null == mBuffer)
							mBuffer = new sbyte[size];
						buffer = mBuffer;
					}
					
					// read into the end of the 'new' buffer
					read = Support.SupportMisc.ReadInput(mIn, buffer, mLevel, size);
					if (- 1 == read)
					{
						mIn.Close();
						mIn = null;
					}
					else
					{
						if (mBuffer != buffer)
						{
							// copy the bytes previously read
							Array.Copy(mBuffer, 0, buffer, 0, mLevel);
							mBuffer = buffer;
							reallocations++;
						}
						mLevel += read;
						if ((0 != mContentLength) && (mLevel == mContentLength))
						{
							mIn.Close();
							mIn = null;
						}
						ret = true;
						fills++;
					}
				}
				
				return (ret);
			}
		}
		
		//
		// Runnable interface
		//
		
		/// <summary> Continually read the underlying stream untill exhausted.</summary>
		/// <seealso cref="java.lang.Thread.run()">
		/// </seealso>
		public virtual void Run()
		{
			bool filled;
			
			do 
			{
				// keep hammering the socket with no delay, it's metered upstream
				try
				{
					filled = Fill(true);
				}
				catch (System.IO.IOException ioe)
				{
					Support.SupportMisc.WriteStackTrace(ioe, Console.Error);
					// exit the thread if there is a problem,
					// let the synchronous reader find out about it
					filled = false;
				}
			}
			while (filled);
		}
		
		//
		// InputStream overrides
		//
		
		/// <summary> Reads the next byte of data from the input stream. The value byte is
		/// returned as an <code>int</code> in the range <code>0</code> to
		/// <code>255</code>. If no byte is available because the end of the stream
		/// has been reached, the value <code>-1</code> is returned. This method
		/// blocks until input data is available, the end of the stream is detected,
		/// or an exception is thrown.
		/// </summary>
		/// <returns>  The next byte of data, or <code>-1</code> if the end of the
		/// stream is reached.
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		public override int ReadByte()
		{
			int ret;
			
			// The following is unsynchronized code.
			// Some would argue that unsynchronized access isn't thread safe
			// but I think I can rationalize it in this case...
			// The two volatile members are mLevel and mBuffer (besides mIn).
			// If (mOffset >= mLevel) turns false after the test, fill is
			// superflously called, but it's synchronized and figures it out.
			// (mOffset < mLevel) only goes more true by the operation of the
			// background thread, it increases the value of mLevel
			// and volatile int access is atomic.
			// If mBuffer changes by the operation of the background thread,
			// the array pointed to can only be bigger than the previous buffer,
			// and hence no array bounds exception can be raised.
			if (0 == Available())
				Fill(false);
			if (0 != Available())
				ret = mBuffer[mOffset++] & 0xff;
			else
				ret = EOF;
			
			return (ret);
		}
		
		/// <summary> Returns the number of bytes that can be read (or skipped over) from
		/// this input stream without blocking by the next caller of a method for
		/// this input stream.  The next caller might be the same thread or or
		/// another thread.
		/// </summary>
		/// <returns> The number of bytes that can be read from this input stream
		/// without blocking.
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		//UPGRADE_NOTE: The equivalent of method 'java.io.InputStream.available' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
		public int Available()
		{
			return (mLevel - mOffset);
		}
		
		/// <summary> Closes this input stream and releases any system resources associated
		/// with the stream.
		/// </summary>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		public override void Close()
		{
			if (null != mIn)
			{
				mIn.Close();
				mIn = null;
			}
			mBuffer = null;
			mLevel = 0;
			mOffset = 0;
			mContentLength = 0;
			mMark = - 1;
		}
		
		/// <summary> Repositions this stream to the position at the time the
		/// <code>mark</code> method was last called on this input stream.
		/// 
		/// <p> The general contract of <code>reset</code> is:
		/// 
		/// <p><ul>
		/// 
		/// <li> If the method <code>markSupported</code> returns
		/// <code>true</code>, then:
		/// 
		/// <ul><li> If the method <code>mark</code> has not been called since
		/// the stream was created, or the number of bytes read from the stream
		/// since <code>mark</code> was last called is larger than the argument
		/// to <code>mark</code> at that last call, then an
		/// <code>IOException</code> might be thrown.
		/// 
		/// <li> If such an <code>IOException</code> is not thrown, then the
		/// stream is reset to a state such that all the bytes read since the
		/// most recent call to <code>mark</code> (or since the start of the
		/// file, if <code>mark</code> has not been called) will be resupplied
		/// to subsequent callers of the <code>read</code> method, followed by
		/// any bytes that otherwise would have been the next input data as of
		/// the time of the call to <code>reset</code>. </ul>
		/// 
		/// <li> If the method <code>markSupported</code> returns
		/// <code>false</code>, then:
		/// 
		/// <ul><li> The call to <code>reset</code> may throw an
		/// <code>IOException</code>.
		/// 
		/// <li> If an <code>IOException</code> is not thrown, then the stream
		/// is reset to a fixed state that depends on the particular type of the
		/// input stream and how it was created. The bytes that will be supplied
		/// to subsequent callers of the <code>read</code> method depend on the
		/// particular type of the input stream. </ul></ul>
		/// 
		/// </summary>
		/// <exception cref="IOException"><em>Never thrown. Just for subclassers.</em>
		/// </exception>
		/// <seealso cref="java.io.InputStream.mark(int)">
		/// </seealso>
		/// <seealso cref="java.io.IOException">
		/// 
		/// </seealso>
		//UPGRADE_NOTE: The equivalent of method 'java.io.InputStream.reset' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
		public void Reset()
		{
			if (- 1 != mMark)
				mOffset = mMark;
			else
				mOffset = 0;
		}
		
		/// <summary> Tests if this input stream supports the <code>mark</code> and
		/// <code>reset</code> methods. Whether or not <code>mark</code> and
		/// <code>reset</code> are supported is an invariant property of a
		/// particular input stream instance. The <code>markSupported</code> method
		/// of <code>InputStream</code> returns <code>false</code>.
		/// 
		/// </summary>
		/// <returns> <code>true</code>.
		/// </returns>
		/// <seealso cref="java.io.InputStream.mark(int)">
		/// </seealso>
		/// <seealso cref="java.io.InputStream.reset()">
		/// 
		/// </seealso>
		//UPGRADE_NOTE: The equivalent of method 'java.io.InputStream.markSupported' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
		public bool MarkSupported()
		{
			return (true);
		}
		
		/// <summary> Marks the current position in this input stream. A subsequent call to
		/// the <code>reset</code> method repositions this stream at the last marked
		/// position so that subsequent reads re-read the same bytes.
		/// 
		/// <p> The <code>readlimit</code> arguments tells this input stream to
		/// allow that many bytes to be read before the mark position gets
		/// invalidated.
		/// 
		/// <p> The general contract of <code>mark</code> is that, if the method
		/// <code>markSupported</code> returns <code>true</code>, the stream somehow
		/// remembers all the bytes read after the call to <code>mark</code> and
		/// stands ready to supply those same bytes again if and whenever the method
		/// <code>reset</code> is called.  However, the stream is not required to
		/// remember any data at all if more than <code>readlimit</code> bytes are
		/// read from the stream before <code>reset</code> is called.
		/// 
		/// </summary>
		/// <param name="readlimit"><em>Not used.</em>
		/// </param>
		/// <seealso cref="java.io.InputStream.reset()">
		/// 
		/// </seealso>
		//UPGRADE_NOTE: The equivalent of method 'java.io.InputStream.mark' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
		public void Mark(int readlimit)
		{
			mMark = mOffset;
		}
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		public override void  Flush()
		{
			mIn.Flush();
		}
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		public override System.Int64 Seek(System.Int64 offset, System.IO.SeekOrigin origin)
		{
			return mIn.Seek(offset, origin);
		}
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		public override void  SetLength(System.Int64 value)
		{
			mIn.SetLength(value);
		}

		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		public override System.Int32 Read(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
		{
			return mIn.Read(buffer, offset, count);
		}
		//UPGRADE_TODO: The following method was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		public override void  Write(System.Byte[] buffer, System.Int32 offset, System.Int32 count)
		{
			mIn.Write(buffer, offset, count);
		}

		//UPGRADE_TODO: The following property was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		public override System.Boolean CanRead
		{
			get
			{
				return mIn.CanRead;
			}
			
		}
		//UPGRADE_TODO: The following property was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		public override System.Boolean CanSeek
		{
			get
			{
				return mIn.CanSeek;
			}
			
		}
		//UPGRADE_TODO: The following property was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		public override System.Boolean CanWrite
		{
			get
			{
				return mIn.CanWrite;
			}
			
		}
		//UPGRADE_TODO: The following property was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		public override System.Int64 Length
		{
			get
			{
				return mIn.Length;
			}
			
		}
		//UPGRADE_TODO: The following property was automatically generated and it must be implemented in order to preserve the class logic. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1232'"
		public override System.Int64 Position
		{
			get
			{
				return mIn.Position;
			}
			
			set
			{
			}
		}
	}
}
