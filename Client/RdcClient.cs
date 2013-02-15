///  Copyright (c) Microsoft Corporation.  All rights reserved.
///
///     Sample code developed by JW Secure, Inc.
///     Errata will be published via http://www.jwsecure.com/dan/index.html.
///
///		Further enhanced by David Jade
///		http://blog.mutable.net
///

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Microsoft.RDC;

namespace Client
{
	public class RdcClient
	{
		private string url;
		private string remoteFile;
		private string seedFile;
		private string outputFile;
		private string workingDir;
		private int blockSize = 2 * 4096;
		private int recursionDepth = -1;

//		private int MINIMUM_SIZE = 1024*1024;

		public RdcClient(string url, string workingDir, int recursionDepth)
		{
			this.url = url;
			this.workingDir = workingDir;
			this.recursionDepth = recursionDepth;
		}
		
		public void Synchronize(string remoteFile, string seedFile, string outputFile)
		{   
			this.remoteFile = remoteFile;
			this.seedFile = seedFile;
			this.outputFile = outputFile;

			// Initialize our managed RDC wrapper
			RdcServices rdcServices = new RdcServices();
			SignatureCollection seedSignatures = null;

			// Get the RDC version of the server so we 
			// can make sure this client is supported.
			Client.RdcProxy.RdcService rdcWebService = new Client.RdcProxy.RdcService();
			Client.RdcProxy.RdcVersion rdcVersion = rdcWebService.GetRdcVersion();

			//rdcServices.CheckVersion(rdcVersion);
			
			// Open the local seed file stream
			using (FileStream stream = File.OpenRead(seedFile))
			{
				rdcServices.WorkingDirectory = workingDir;
				rdcServices.RecursionDepth = recursionDepth;

				// Generate the signature files
				seedSignatures = rdcServices.GenerateSignatures(stream, Path.GetFileName(seedFile));
				if (seedSignatures.Count < 1)
					throw new RdcException("Failed to generate the signatures.");               
			}

			// Now make the call the the Rdc web service 
			// and retrieve the copy of the signature 
			// manifest.            
			Client.RdcProxy.SignatureManifest signatureManifest = rdcWebService.GetSignatureManifest(remoteFile, recursionDepth);
			SignatureCollection sourceSignatures = new SignatureCollection();

			/*
			 * Realistically, the soure file length should be checked
			 * against a predetermined minimum limit. So if the source
			 * file length is less than 1MB, just download the source
			 * file instead of generating signaures and needs.
			*/
			//if signatureManifest.FileLength < MINIMUM_SIZE)
			//    DownloadSourceFile(...);
 

			ulong TargetDataWritten = 0;
			ulong TotalSourceData = 0;
			ulong TotalSeedData = 0;
			ulong TotalSigData = 0;

			// Now that we have the signature manifiest, let's go ahead 
			// transfer local copies of the signature files to our
			// working signature directory.
			int Depth = 0;
			foreach (Client.RdcProxy.SignatureInfo sig in signatureManifest.Signatures)
			{
				Console.WriteLine(string.Format("\n----------\nProcessing: {0}\n", sig.Name + ".sig"));

				if (sig.Length > 0)
				{
					GC.WaitForPendingFinalizers();

					// Create the signature stream
					Microsoft.RDC.SignatureInfo sigInfo = new Microsoft.RDC.SignatureInfo(sig.Name, -1, workingDir, true);
					sourceSignatures.Add(sigInfo);	// hang on to them to keep them alive and for clean up

					if (Depth == 0)	// always transfer the complete first remote signature
					{
						Console.WriteLine(string.Format("Transfering: {0}\n", sig.Name + ".sig"));
						for (int i = 0; i < sig.Length; i += blockSize)
						{
							int readBytes = Math.Min((int)(sig.Length - i), blockSize);
							byte[] data = rdcWebService.TransferDataBlock(Path.Combine(sig.Path, sig.Name) + ".sig", i, readBytes);
							sigInfo.InnerStream.Write(data, 0, data.Length);
							TotalSigData += (ulong)data.Length;
						}
					}

					// select source and target stream
					FileStream SourceStream;
					FileStream TargetStream;
					string RemoteSourcePath;

					// if there are other signatures after this one, they become the source and target
					if (Depth < seedSignatures.Count - 1)
					{
						SourceStream  = seedSignatures[Depth + 1].InnerStream;

						TargetStream = File.Create(Path.Combine(workingDir, signatureManifest.Signatures[Depth + 1].Name) + ".sig", blockSize);
						RemoteSourcePath = Path.Combine(signatureManifest.Signatures[Depth + 1].Path, signatureManifest.Signatures[Depth + 1].Name) + ".sig";

						Console.WriteLine(string.Format("Creating: {0}\n----------\n\n", signatureManifest.Signatures[Depth + 1].Name + ".sig"));
					}
					else	// create the final target file
					{
						SourceStream = File.OpenRead(seedFile);

						TargetStream = File.Create(outputFile, blockSize);
						RemoteSourcePath = remoteFile;

						Console.WriteLine(string.Format("Creating: {0}\n----------\n\n", Path.GetFileName(outputFile)));
					}

					// reset signature streams for reading
					seedSignatures[Depth].InnerStream.Position = 0;
					sigInfo.InnerStream.Position = 0;


					// Compare the signatures and get 
					// the needs array that we will use to create the 
					// target output file.                   
					ArrayList needsList = rdcServices.CreateNeedsList(seedSignatures[Depth], sigInfo);
					foreach (RdcNeed need in needsList)
					{
						switch (need.blockType)
						{
							case RdcNeedType.Source:
								// Copy this block from the remote server.
								TotalSourceData += need.blockLength;

								byte[] data = rdcWebService.TransferDataBlock(
										RemoteSourcePath,
										(int)need.fileOffset,
										(int)need.blockLength);

								TargetStream.Write(data, 0, (int)need.blockLength);
								break;  

							case RdcNeedType.Seed:
								TotalSeedData += need.blockLength;
								byte[] seedData = new Byte[need.blockLength];

								SourceStream.Seek((int)need.fileOffset, SeekOrigin.Begin);
								SourceStream.Read(seedData, 0, (int)need.blockLength);

								TargetStream.Write(seedData, 0, (int)need.blockLength);
								break;

							default:
								break;
						}

						Console.WriteLine(string.Format("NEED: length:{0,12}\toffset:{1,12}\tsource:{2,12}\tblock type:{3,12}", need.blockLength, need.fileOffset, TargetDataWritten, need.blockType.ToString()));
						TargetDataWritten += need.blockLength;
					}

					// Close our IO file streams.
					if (Depth == seedSignatures.Count - 1)
						SourceStream.Close();	// only non-signature sources

					TargetStream.Close();            
				}

				Depth++;
			}


			Console.WriteLine(string.Format("\nFrom source:{0,12:N0}\tFrom seed:{1,12:N0}\tTotal:{2,12:N0}", TotalSourceData, TotalSeedData, TotalSourceData + TotalSeedData));

			Console.WriteLine(string.Format("\nTransfer: {0:N0} bytes from source, file size: {1:N0}, RDC Savings: {2:0.00}%\n",
				 TotalSourceData + TotalSigData,
				 TotalSourceData + TotalSeedData,
				 (1.0 - (double)(TotalSourceData + TotalSigData) / (double)(TotalSourceData + TotalSeedData)) * 100.0));

			// release all signature resources
			rdcServices.PurgeSignatureStore(seedSignatures);
			rdcServices.PurgeSignatureStore(sourceSignatures);
			rdcWebService.Finialize(signatureManifest);

			rdcServices.Dispose();
		}
	}
}
