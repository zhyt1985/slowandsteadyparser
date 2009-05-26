using System;

namespace Winista.Text.HtmlParser.Lex
{
	/// <summary> A buffered source of characters.
	/// A Source is very similar to a Reader, like:
	/// <pre>
	/// new InputStreamReader (connection.getInputStream (), charset)
	/// </pre>
	/// It differs from the above, in three ways:
	/// <ul>
	/// <li>the fetching of bytes may be asynchronous</li>
	/// <li>the character set may be changed, which resets the input stream</li>
	/// <li>characters may be requested more than once, so in general they
	/// will be buffered</li>
	/// </ul>
	/// </summary>
	[Serializable]
	public abstract class Source:System.IO.StreamReader
	{
		public Source(System.IO.Stream strm) : base(strm)
		{}

		public Source(System.String str) : base(str)
		{}

		/// <summary> Get the encoding being used to convert characters.</summary>
		/// <returns> The current encoding.
		/// </returns>
		/// <summary> Set the encoding to the given character set.
		/// If the current encoding is the same as the requested encoding,
		/// this method is a no-op. Otherwise any subsequent characters read from
		/// this source will have been decoded using the given character set.<p>
		/// If characters have already been consumed from this source, it is expected
		/// that an exception will be thrown if the characters read so far would
		/// be different if the encoding being set was used from the start.
		/// </summary>
		/// <param name="character_set">The character set to use to convert characters.
		/// </param>
		/// <exception cref="ParserException">If a character mismatch occurs between
		/// characters already provided and those that would have been returned
		/// had the new character set been in effect from the beginning. An
		/// exception is also thrown if the character set is not recognized.
		/// </exception>
		public abstract System.String Encoding{get;set;}
		/// <summary> Return value when the source is exhausted.
		/// Has a value of {@value}.
		/// </summary>
		public const int EOF = - 1;
		
		//
		// Reader overrides
		//
		
		/// <summary> Does nothing.
		/// It's supposed to close the source, but use {@link #destroy} instead.
		/// </summary>
		/// <exception cref="IOException"><em>not used</em>
		/// </exception>
		/// <seealso cref="destroy">
		/// </seealso>
		abstract public override void  Close();
		
		/// <summary> Read a single character.
		/// This method will block until a character is available,
		/// an I/O error occurs, or the source is exhausted.
		/// </summary>
		/// <returns> The character read, as an integer in the range 0 to 65535
		/// (<tt>0x00-0xffff</tt>), or {@link #EOF} if the source is exhausted.
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		abstract public  override int Read();
		
		/// <summary> Read characters into a portion of an array.  This method will block
		/// until some input is available, an I/O error occurs, or the source is
		/// exhausted.
		/// </summary>
		/// <param name="cbuf">Destination buffer
		/// </param>
		/// <param name="off">Offset at which to start storing characters
		/// </param>
		/// <param name="len">Maximum number of characters to read
		/// </param>
		/// <returns> The number of characters read, or {@link #EOF} if the source is
		/// exhausted.
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		abstract public  override int Read(System.Char[] cbuf, int off, int len);
		
		/// <summary> Read characters into an array.
		/// This method will block until some input is available, an I/O error occurs,
		/// or the source is exhausted.
		/// </summary>
		/// <param name="cbuf">Destination buffer.
		/// </param>
		/// <returns> The number of characters read, or {@link #EOF} if the source is
		/// exhausted.
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		//UPGRADE_NOTE: The equivalent of method 'java.io.Reader.read' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
		abstract public int Read(char[] cbuf);
		
		/// <summary> Tell whether this source is ready to be read.</summary>
		/// <returns> <code>true</code> if the next read() is guaranteed not to block
		/// for input, <code>false</code> otherwise.
		/// Note that returning false does not guarantee that the next read will block.
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		//UPGRADE_NOTE: The equivalent of method 'java.io.Reader.ready' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
		abstract public bool Ready();
		
		/// <summary> Reset the source.
		/// Repositions the read point to begin at zero.
		/// </summary>
		//UPGRADE_NOTE: The equivalent of method 'java.io.Reader.reset' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
		abstract public void Reset();
		
		/// <summary> Tell whether this source supports the mark() operation.</summary>
		/// <returns> <code>true</code> if and only if this source supports the mark
		/// operation.
		/// </returns>
		//UPGRADE_NOTE: The equivalent of method 'java.io.Reader.markSupported' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
		abstract public bool MarkSupported();
		
		/// <summary> Mark the present position.
		/// Subsequent calls to {@link #reset}
		/// will attempt to reposition the source to this point.  Not all
		/// sources support the mark() operation.
		/// </summary>
		/// <param name="readAheadLimit">The minimum number of characters that can be read
		/// before this mark becomes invalid.
		/// </param>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		//UPGRADE_NOTE: The equivalent of method 'java.io.Reader.mark' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
		abstract public void  Mark(int readAheadLimit);
		
		/// <summary> Skip characters.
		/// This method will block until some characters are available,
		/// an I/O error occurs, or the source is exhausted.
		/// <em>Note: n is treated as an int</em>
		/// </summary>
		/// <param name="n">The number of characters to skip.
		/// </param>
		/// <returns> The number of characters actually skipped
		/// </returns>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		//UPGRADE_NOTE: The equivalent of method 'java.io.Reader.skip' is not an override method. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1143'"
		abstract public long Skip(long n);
		
		//
		// Methods not in your Daddy's Reader
		//
		
		/// <summary> Undo the read of a single character.</summary>
		/// <exception cref="IOException">If the source is closed or no characters have
		/// been read.
		/// </exception>
		public abstract void Unread();
		
		/// <summary> Retrieve a character again.</summary>
		/// <param name="offset">The offset of the character.
		/// </param>
		/// <returns> The character at <code>offset</code>.
		/// </returns>
		/// <exception cref="IOException">If the source is closed or the offset is beyond
		/// {@link #offset()}.
		/// </exception>
		public abstract char GetCharacter(int offset);
		
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
		/// <exception cref="IOException">If the source is closed or the start or end is
		/// beyond {@link #offset()}.
		/// </exception>
		public abstract void GetCharacters(char[] array, int offset, int start, int end);
		
		/// <summary> Retrieve a string comprised of characters already read.</summary>
		/// <param name="offset">The offset of the first character.
		/// </param>
		/// <param name="length">The number of characters to retrieve.
		/// </param>
		/// <returns> A string containing the <code>length</code> characters at <code>offset</code>.
		/// </returns>
		/// <exception cref="IOException">If the source is closed.
		/// </exception>
		public abstract System.String GetString(int offset, int length);
		
		/// <summary> Append characters already read into a <code>StringBuffer</code>.</summary>
		/// <param name="buffer">The buffer to append to.
		/// </param>
		/// <param name="offset">The offset of the first character.
		/// </param>
		/// <param name="length">The number of characters to retrieve.
		/// </param>
		/// <exception cref="IOException">If the source is closed or the offset or
		/// (offset + length) is beyond {@link #offset()}.
		/// </exception>
		public abstract void GetCharacters(System.Text.StringBuilder buffer, int offset, int length);
		
		/// <summary> Close the source.
		/// Once a source has been closed, further {@link #read() read},
		/// {@link #ready ready}, {@link #mark mark}, {@link #reset reset},
		/// {@link #skip skip}, {@link #unread unread},
		/// {@link #getCharacter getCharacter} or {@link #getString getString}
		/// invocations will throw an IOException.
		/// Closing a previously-closed source, however, has no effect.
		/// </summary>
		/// <exception cref="IOException">If an I/O error occurs.
		/// </exception>
		public abstract void Destroy();
		
		/// <summary> Get the position (in characters).</summary>
		/// <returns> The number of characters that have already been read, or
		/// {@link #EOF} if the source is closed.
		/// </returns>
		public abstract int Offset();
		
		/// <summary> Get the number of available characters.</summary>
		/// <returns> The number of characters that can be read without blocking.
		/// </returns>
		public abstract int Available();
	}
}
