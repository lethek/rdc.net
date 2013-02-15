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
    [Guid("96236A82-8DBC-11DA-9E3F-0011114AE311")]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    public class RdcFileWriter : IRdcFileWriter
    {
        private Stream stream = null;

        public RdcFileWriter(Stream stream)
        {
            this.stream = stream;    
        }

        public void Write(UInt64 offsetFileStart, uint bytesToWrite, ref IntPtr buffer)
        {
            byte[] outBuff = new Byte[bytesToWrite];

            Marshal.Copy(buffer, outBuff, 0, (int)bytesToWrite);

            stream.Seek((long)offsetFileStart, SeekOrigin.Begin);
            stream.Write(outBuff, 0, (int)bytesToWrite);
        }

        public void Truncate()
        {
        }

        public void DeleteOnClose()
        {
        }
    }
}
