using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.RDC
{
    [Serializable()]
    public class SignatureInfo : IDisposable    
    {
        private int index = 0;
        private string name;
        private string path = String.Empty;
        private long length = 0;
        [NonSerialized()] private FileStream stream;
        [NonSerialized()] private RdcNeed[] needs;

        #region Constructor(s)

        public SignatureInfo() : this(Directory.GetCurrentDirectory(), false) { }
        
        public SignatureInfo(string path, bool create)
        {
            // Auto-generate a new unique name 
            // for this signature entry.
            this.name = Guid.NewGuid().ToString();
            this.path = path;

            // If the create flag was passed in, lets go 
            // ahead and create the file.
            if (create)
                stream = File.Create(this.FullPath);
        }

        #endregion

        #region Public Properties

        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Gets or sets the signature name.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the source path for this signature.
        /// </summary>
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public long Length
        {
            get { return length; }
            set { length = value; }
        }

        /// <summary>
        /// Gets the signatures full file path.
        /// </summary>
        public string FullPath
        {
            get { return(System.IO.Path.Combine(path, name) + ".sig"); }
        }

        /// <summary>
        /// Gets or sets the underlying stream.
        /// </summary>                
        [System.Xml.Serialization.SoapIgnore()]
        [XmlIgnore()]
        public FileStream InnerStream
        {
            get { return stream; }
            set { stream = value; }
        }

        /// <summary>
        /// Gets or sets the Rdc Needs array.
        /// </summary>
        [System.Xml.Serialization.SoapIgnore()]
        [XmlIgnore()]
        public RdcNeed[] Needs
        {
            get { return needs; }
            set { needs = value; }
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            if (this.stream != null)
                stream.Dispose();
        }

        #endregion
    }
}
