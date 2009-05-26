// ***************************************************************
//  GZIPUtils   version:  1.0   Date: 12/19/2005
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  Copyright © 2005 - Winista All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;

using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip;

namespace Winista.Text.HtmlParser.Util
{
	/// <summary>
	/// Summary description for GZIPUtils.
	/// </summary>
	public class GZIPUtils
	{
		private const int EXPECTED_COMPRESSION_RATIO = 5;
		private const int BUF_SIZE = 4096;

		/// <summary>
		/// 
		/// </summary>
		public GZIPUtils()
		{
		}

		/// <summary> Returns an gunzipped copy of the input array.  If the gzipped
		/// input has been truncated or corrupted, a best-effort attempt is
		/// made to unzip as much as possible.  If no data can be extracted
		/// <code>null</code> is returned.
		/// </summary>
		public static byte[] GUnzipBestEffort(System.IO.Stream srcStream)
		{
			return GUnzipBestEffort(srcStream, System.Int32.MaxValue);
		}
		
		/// <summary> Returns an gunzipped copy of the input array, truncated to
		/// <code>sizeLimit</code> bytes, if necessary.  If the gzipped input
		/// has been truncated or corrupted, a best-effort attempt is made to
		/// unzip as much as possible.  If no data can be extracted
		/// <code>null</code> is returned.
		/// </summary>
		public static byte[] GUnzipBestEffort(System.IO.Stream srcStream, int sizeLimit)
		{
			try
			{
				// decompress using GZIPInputStream 
				System.IO.MemoryStream outStream = new System.IO.MemoryStream(EXPECTED_COMPRESSION_RATIO * (Int32)srcStream.Length);
				
				GZipInputStream inStream = new GZipInputStream(srcStream);
								
				byte[] buf = new byte[BUF_SIZE];
				int size = BUF_SIZE;
				int written = 0;
				while (true) 
				{
					size = inStream.Read(buf, 0, size);
					if (size <= 0)
					{
						break;
					}
					if ((written + size) > sizeLimit)
					{
						outStream.Write(buf, 0, sizeLimit - written);
					}
					outStream.Write(buf, 0, size);
					written += size;
				}
				inStream.Close();
				
				return outStream.ToArray();
			}
			catch(Exception ex)
			{
				System.Diagnostics.Trace.WriteLine(ex.Message);
				return null;
			}
		}
		
		/// <summary> Returns an gunzipped copy of the input array.  </summary>
		/// <throws>  IOException if the input cannot be properly decompressed </throws>
		public static byte[] GUnzip(System.IO.Stream srcStream)
		{
			// decompress using GZIPInputStream 
			System.IO.MemoryStream outStream = new System.IO.MemoryStream((Int32)(EXPECTED_COMPRESSION_RATIO * srcStream.Length));
			
			GZipInputStream inStream = new GZipInputStream(srcStream);
			
			byte[] buf = new byte[BUF_SIZE];
			int size = BUF_SIZE;
			while (true)
			{
				size = inStream.Read(buf, 0, size);
				if (size <= 0)
				{
					break;
				}
				outStream.Write(buf, 0, size);
			}
			outStream.Close();
			
			return outStream.ToArray();
		}

		public static byte[] Unzip(System.IO.Stream srcStream)
		{
			// decompress using GZIPInputStream 
			System.IO.MemoryStream outStream = new System.IO.MemoryStream((Int32)(EXPECTED_COMPRESSION_RATIO * srcStream.Length));
			
			ZipInputStream inStream = new ZipInputStream(srcStream);
			
			byte[] buf = new byte[BUF_SIZE];
			int size = BUF_SIZE;
			while (true)
			{
				size = inStream.Read(buf, 0, size);
				if (size <= 0)
				{
					break;
				}
				outStream.Write(buf, 0, size);
			}
			outStream.Close();
			
			return outStream.ToArray();
		}
	}
}
