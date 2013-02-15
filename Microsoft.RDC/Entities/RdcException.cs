using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.RDC
{
    public class RdcException : Exception
    {
        public RdcException(string message, Exception innerException) : 
            base(message, innerException) { }

        public RdcException(string format, params object[] args) : 
            base(String.Format(format, args)) { }

        public RdcException(string message, int hr) :
            base(String.Format("{0} hr={1}", message, hr)) { }
        
    }
}
