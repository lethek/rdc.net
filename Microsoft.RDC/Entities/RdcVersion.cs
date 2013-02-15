using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.RDC
{
    [Serializable]
    public class RdcVersion
    {
        private uint version;
        private uint minCompatVer;

        public RdcVersion() { }

        public RdcVersion(uint currentVer, uint minAppVer)
        {
            version = currentVer;
            minCompatVer = minAppVer;
        }

        public uint CurrentVersion
        {
            get { return version; }
            set { version = value; }
        }

        public uint MinimumCompatibleAppVersion
        {
            get { return minCompatVer; }
            set { minCompatVer = value; }
        }

    }
}
