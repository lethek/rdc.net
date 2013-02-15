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

		public SignatureInfo() : this(null, 0, Directory.GetCurrentDirectory(), false) { }

		public SignatureInfo(string BaseName, int Depth, string path, bool OpenOrCreate)
		{
			// Set a name based on the file's name and signature recursion depth
			if (BaseName != null)
			{
				this.name = BaseName + (Depth != -1 ? "-" + Depth.ToString() : null);
			}
			this.path = path;

			// If the open or create flag was passed in, lets go 
			// ahead and create the stream.
			if (OpenOrCreate)
				stream = File.Open(this.FullPath, FileMode.OpenOrCreate);
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
			// get rid of managed resources
			if (this.stream != null)
				stream.Dispose();
		}

		#endregion
	}
}
