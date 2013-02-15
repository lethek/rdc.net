using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;

using HRESULT = System.Int32;

namespace Microsoft.RDC
{
	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A89-9DBC-11DA-9E3F-0011114AE311")]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	[ComVisible(true)]
	public class RdcFileReader : IRdcFileReader
	{
		private Stream stream = null;

		public RdcFileReader() { }
		public RdcFileReader(Stream stream)
		{
			this.stream = stream;
		}

		public void GetFileSize(out UInt64 fileSize)
		{
			fileSize = (UInt64)stream.Length;
		}

		[PreserveSig]
		public void Read(UInt64 offsetFileStart, uint bytesToRead, ref uint bytesRead, IntPtr buffer, ref bool eof)
		{
			eof = false;

			byte[] intBuff = new Byte[bytesToRead];

			if (offsetFileStart != 0)
			{
				stream.Seek((long)offsetFileStart, SeekOrigin.Begin);
			}

			bytesRead = (uint)stream.Read(intBuff, 0, (int)bytesToRead );
			eof = (stream.Position >= stream.Length);

			Marshal.Copy(intBuff, 0, buffer, (int)bytesRead);

			if (bytesRead < bytesToRead)
				eof = true;

		}

		public void GetFilePosition(out UInt64 offsetFromStart)
		{
			offsetFromStart = (uint)stream.Position;
		}
	}
}
