// ***************************************************************
//  SupportMisc   version:  1.0   date: 12/18/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 Winista - All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

namespace Winista.Text.HtmlParser.Support
{
	/// <summary>
	/// Summary description for SupportMisc.
	/// </summary>
	public class SupportMisc
	{
		/// <summary>
		/// Writes the exception stack trace to the received stream
		/// </summary>
		/// <param name="throwable">Exception to obtain information from</param>
		/// <param name="stream">Output sream used to write to</param>
		public static void WriteStackTrace(System.Exception throwable, System.IO.TextWriter stream)
		{
			stream.Write(throwable.StackTrace);
			stream.Flush();
		}

		/*******************************/
		/// <summary>
		/// This method returns the literal value received
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static long Identity(long literal)
		{
			return literal;
		}

		/// <summary>
		/// This method returns the literal value received
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static ulong Identity(ulong literal)
		{
			return literal;
		}

		/// <summary>
		/// This method returns the literal value received
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static float Identity(float literal)
		{
			return literal;
		}

		/// <summary>
		/// This method returns the literal value received
		/// </summary>
		/// <param name="literal">The literal to return</param>
		/// <returns>The received value</returns>
		public static double Identity(double literal)
		{
			return literal;
		}

		/// <summary>
		/// Reads the serialized fields written by the DefaultWriteObject method.
		/// </summary>
		/// <param name="info">SerializationInfo parameter from the special deserialization constructor.</param>
		/// <param name="context">StreamingContext parameter from the special deserialization constructor</param>
		/// <param name="instance">Object to deserialize.</param>
		public static void DefaultReadObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Object instance)
		{                       
			System.Type thisType = instance.GetType();
			System.Reflection.MemberInfo[] mi = System.Runtime.Serialization.FormatterServices.GetSerializableMembers(thisType, context);
			for (int i = 0 ; i < mi.Length; i++) 
			{
				System.Reflection.FieldInfo fi = (System.Reflection.FieldInfo) mi[i];
				fi.SetValue(instance, info.GetValue(fi.Name, fi.FieldType));
			}
		}

		/// <summary>
		/// Writes the serializable fields to the SerializationInfo object, which stores all the data needed to serialize the specified object object.
		/// </summary>
		/// <param name="info">SerializationInfo parameter from the GetObjectData method.</param>
		/// <param name="context">StreamingContext parameter from the GetObjectData method.</param>
		/// <param name="instance">Object to serialize.</param>
		public static void DefaultWriteObject(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context, System.Object instance)
		{                       
			System.Type thisType = instance.GetType();
			System.Reflection.MemberInfo[] mi = System.Runtime.Serialization.FormatterServices.GetSerializableMembers(thisType, context);
			for (int i = 0 ; i < mi.Length; i++) 
			{
				info.AddValue(mi[i].Name, ((System.Reflection.FieldInfo) mi[i]).GetValue(instance));
			}
		}

		/*******************************/
		/// <summary>
		/// Copies an array of chars obtained from a String into a specified array of chars
		/// </summary>
		/// <param name="sourceString">The String to get the chars from</param>
		/// <param name="sourceStart">Position of the String to start getting the chars</param>
		/// <param name="sourceEnd">Position of the String to end getting the chars</param>
		/// <param name="destinationArray">Array to return the chars</param>
		/// <param name="destinationStart">Position of the destination array of chars to start storing the chars</param>
		/// <returns>An array of chars</returns>
		public static void GetCharsFromString(System.String sourceString, int sourceStart, int sourceEnd, char[] destinationArray, int destinationStart)
		{	
			int sourceCounter;
			int destinationCounter;
			sourceCounter = sourceStart;
			destinationCounter = destinationStart;
			while (sourceCounter < sourceEnd)
			{
				destinationArray[destinationCounter] = (char) sourceString[sourceCounter];
				sourceCounter++;
				destinationCounter++;
			}
		}

		/*******************************/
		/// <summary>Reads a number of characters from the current source Stream and writes the data to the target array at the specified index.</summary>
		/// <param name="sourceStream">The source Stream to read from.</param>
		/// <param name="target">Contains the array of characteres read from the source Stream.</param>
		/// <param name="start">The starting index of the target array.</param>
		/// <param name="count">The maximum number of characters to read from the source Stream.</param>
		/// <returns>The number of characters read. The number will be less than or equal to count depending on the data available in the source Stream. Returns -1 if the end of the stream is reached.</returns>
		public static System.Int32 ReadInput(System.IO.Stream sourceStream, sbyte[] target, int start, int count)
		{
			// Returns 0 bytes if not enough space in target
			if (target.Length == 0)
				return 0;

			byte[] receiver = new byte[target.Length];
			int bytesRead   = sourceStream.Read(receiver, start, count);

			// Returns -1 if EOF
			if (bytesRead == 0)	
				return -1;
                
			for(int i = start; i < start + bytesRead; i++)
				target[i] = (sbyte)receiver[i];
                
			return bytesRead;
		}

		/// <summary>Reads a number of characters from the current source TextReader and writes the data to the target array at the specified index.</summary>
		/// <param name="sourceTextReader">The source TextReader to read from</param>
		/// <param name="target">Contains the array of characteres read from the source TextReader.</param>
		/// <param name="start">The starting index of the target array.</param>
		/// <param name="count">The maximum number of characters to read from the source TextReader.</param>
		/// <returns>The number of characters read. The number will be less than or equal to count depending on the data available in the source TextReader. Returns -1 if the end of the stream is reached.</returns>
		public static System.Int32 ReadInput(System.IO.TextReader sourceTextReader, sbyte[] target, int start, int count)
		{
			// Returns 0 bytes if not enough space in target
			if (target.Length == 0) return 0;

			char[] charArray = new char[target.Length];
			int bytesRead = sourceTextReader.Read(charArray, start, count);

			// Returns -1 if EOF
			if (bytesRead == 0) return -1;

			for(int index=start; index<start+bytesRead; index++)
				target[index] = (sbyte)charArray[index];

			return bytesRead;
		}

		/// <summary>
		/// Converts an array of sbytes to an array of chars
		/// </summary>
		/// <param name="sByteArray">The array of sbytes to convert</param>
		/// <returns>The new array of chars</returns>
		public static char[] ToCharArray(sbyte[] sByteArray) 
		{
			return System.Text.UTF8Encoding.UTF8.GetChars(ToByteArray(sByteArray));
		}

		/// <summary>
		/// Converts an array of bytes to an array of chars
		/// </summary>
		/// <param name="byteArray">The array of bytes to convert</param>
		/// <returns>The new array of chars</returns>
		public static char[] ToCharArray(byte[] byteArray) 
		{
			return System.Text.UTF8Encoding.UTF8.GetChars(byteArray);
		}

		/// <summary>
		/// Receives a byte array and returns it transformed in an sbyte array
		/// </summary>
		/// <param name="byteArray">Byte array to process</param>
		/// <returns>The transformed array</returns>
		public static sbyte[] ToSByteArray(byte[] byteArray)
		{
			sbyte[] sbyteArray = null;
			if (byteArray != null)
			{
				sbyteArray = new sbyte[byteArray.Length];
				for(int index=0; index < byteArray.Length; index++)
					sbyteArray[index] = (sbyte) byteArray[index];
			}
			return sbyteArray;
		}
		/// <summary>
		/// Converts a string to an array of bytes
		/// </summary>
		/// <param name="sourceString">The string to be converted</param>
		/// <returns>The new array of bytes</returns>
		public static byte[] ToByteArray(System.String sourceString)
		{
			return System.Text.UTF8Encoding.UTF8.GetBytes(sourceString);
		}

		/// <summary>
		/// Converts a array of object-type instances to a byte-type array.
		/// </summary>
		/// <param name="tempObjectArray">Array to convert.</param>
		/// <returns>An array of byte type elements.</returns>
		public static byte[] ToByteArray(System.Object[] tempObjectArray)
		{
			byte[] byteArray = null;
			if (tempObjectArray != null)
			{
				byteArray = new byte[tempObjectArray.Length];
				for (int index = 0; index < tempObjectArray.Length; index++)
					byteArray[index] = (byte)tempObjectArray[index];
			}
			return byteArray;
		}

		/// <summary>
		/// Converts a array of object-type instances to a byte-type array.
		/// </summary>
		/// <param name="tempObjectArray">Array to convert.</param>
		/// <returns>An array of byte type elements.</returns>
		public static byte[] ToByteArray(sbyte[] tempObjectArray)
		{
			byte[] byteArray = null;
			if (tempObjectArray != null)
			{
				byteArray = new byte[tempObjectArray.Length];
				for (int index = 0; index < tempObjectArray.Length; index++)
					byteArray[index] = (byte)tempObjectArray[index];
			}
			return byteArray;
		}
	}
}
