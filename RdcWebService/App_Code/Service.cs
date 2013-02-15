///  Copyright (c) Microsoft Corporation.  All rights reserved.
///
///     Sample code developed by JW Secure, Inc.
///     Errata will be published via http://www.jwsecure.com/dan/index.html.
///
///		Further enhanced by David Jade
///		http://blog.mutable.net
///

using System;
using System.Web;
using System.IO;
using System.Web.Services;
using System.Web.Services.Protocols;

using Microsoft.RDC;


[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
public class RdcService: System.Web.Services.WebService
{
	private const long MAX_BLOCKSIZE = 16 * 4096;
	
	public RdcService () {
		// Constructor logic here.
	}


	[WebMethod]
	public RdcVersion GetRdcVersion()
	{
		return RdcServices.GetRdcVersion();
	}

	[WebMethod]
	[AspNetHostingPermission(System.Security.Permissions.SecurityAction.Demand)]
	public SignatureManifest GetSignatureManifest(string file, int recursionDepth)
	{
		SignatureManifest manifest;
		SignatureCollection signatures;

		// Open the source Stream 
		using (FileStream stream = File.OpenRead(file))
		{
			// Initialize our managed RDC wrapper
			RdcServices rdcServices = new RdcServices();

			rdcServices.WorkingDirectory = Path.GetTempPath();
			rdcServices.RecursionDepth = recursionDepth;

			GC.Collect();
			GC.WaitForPendingFinalizers();

			// Generate the signature files
			signatures = rdcServices.GenerateSignatures(stream, Path.GetFileName(file));
			if (signatures.Count < 1)
				throw new RdcException("Failed to generate the signatures.");

			manifest = new SignatureManifest(file, signatures);
			manifest.FileLength = stream.Length;

			// Let's close the signature streams.  
			foreach (SignatureInfo sig in signatures)
				sig.InnerStream.Close();
		}  
		
		return (manifest);
	}

	[WebMethod]
	public void GetSimilarityData()
	{
		/* If similarity is enabled, 
		 * implement here */
	}


	[WebMethod]
	public byte[] TransferDataBlock(string file, int offset, int length)
	{
		// for this sample, we allow any transfer size but ideally requests should be chunked into smaller blocks so failures can be retried
		//if (length > MAX_BLOCKSIZE)
			//throw new RdcException("Block size too large.  You can only transfer a maximum of 65536 bytes per request.");

		byte[] block = new Byte[length];

		// TODO - cache this for performance optimization.  
		using (FileStream fileStream = File.OpenRead(file))
		{
			fileStream.Seek(offset, SeekOrigin.Begin);
			int bytes = fileStream.Read(block, 0, length);            
		}

		return (block);
	}

	[WebMethod]
	public void Finialize(SignatureManifest manifest)
	{
		// Here we want to make sure that all 
		// resources have been released and
		// all signature/temp files have been
		// erased.
		using (RdcServices rdcServices = new RdcServices())
		{
			rdcServices.PurgeSignatureStore(manifest.Signatures);
		}
	}
	
	
}
