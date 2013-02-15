using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Microsoft.RDC
{
    [Serializable()]
    public class SignatureManifest
    {
        private string file;
        private SignatureCollection signatures;
        private long fileLength;

        public SignatureManifest() { }

        public SignatureManifest(string file, SignatureCollection signatures)
        {
            this.signatures = signatures;
            this.file = file;
        }

        public string File
        {
            get { return file; }
            set { file = value; }
        }

        public SignatureCollection Signatures
        {
            get { return signatures; }
            set { signatures = value; }
        }

        public long FileLength
        {
            get { return fileLength; }
            set { fileLength = value; }
        }

    }
}
