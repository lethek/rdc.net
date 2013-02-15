///  Copyright (c) Microsoft Corporation.  All rights reserved.
///
///     Sample code developed by JW Secure, Inc.
///     Errata will be published via http://www.jwsecure.com/dan/index.html.
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
        private int MINIMUM_SIZE = 1024*1024;

        public RdcClient(string url, string workingDir)
        {
            this.url = url;
            this.workingDir = workingDir;
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
            Microsoft.RDC.RdcVersion rdcVersion = (Microsoft.RDC.RdcVersion)rdcWebService.GetRdcVersion();

            //rdcServices.CheckVersion(rdcVersion);
            
            // Open the local seed file stream
            using (FileStream stream = File.OpenRead(seedFile))
            {
                rdcServices.WorkingDirectory = workingDir;
                rdcServices.RecursionDepth = -1;    // Let RDC calculate the depth

                // Generate the signature files
                seedSignatures = rdcServices.GenerateSignatures(stream);
                if (seedSignatures.Count < 1)
                    throw new RdcException("Failed to generate the signatures.");               
            }

            // Now make the call the the Rdc web service 
            // and retrieve the copy of the signature 
            // manifest.            
            Client.RdcProxy.SignatureManifest signatureManifest = rdcWebService.GetSignatureManifest(remoteFile);
            SignatureCollection sourceSignatures = new SignatureCollection();
            int index = 0;

            /*
             * Realistically, the soure file length should be checked
             * against a predetermined minimum limit. So if the source
             * file length is less than 1MB, just download the source
             * file instead of generating signaures and needs.
            */
            //if signatureManifest.FileLength < MINIMUM_SIZE)
            //    DownloadSourceFile(...);
 
            // Create our input and output file streams.
            FileStream seedStream = File.OpenRead(seedFile);
            FileStream outStream = File.Create(outputFile, blockSize);

            // Now that we have the signature manifiest, let's go ahead 
            // transfer local copies of the signature files to our
            // working signature directory.
            foreach (Client.RdcProxy.SignatureInfo sig in signatureManifest.Signatures)
            {
                if (sig.Length > 0)
                {
                    GC.WaitForPendingFinalizers();

                    // Create the signature stream
                    Microsoft.RDC.SignatureInfo sigInfo = new Microsoft.RDC.SignatureInfo(workingDir, true);
                    sourceSignatures.Add(sigInfo);
                    int readBytes = 0;
                    
                    for (int i=0;i<sig.Length;i+=blockSize)
                    {
                        readBytes = Math.Min((int)(sig.Length - i) , blockSize);
                        byte[] data = rdcWebService.TransferDataBlock(Path.Combine(sig.Path, sig.Name) + ".sig", i, readBytes);
                        sigInfo.InnerStream.Write(data, 0, data.Length);                        
                    }

                    seedSignatures[index].InnerStream.Position = 0;
                    sigInfo.InnerStream.Position = 0;

                    // Compare the signatures and get 
                    // the needs array that we will use to create the 
                    // target output file.                   
                    ArrayList needsList = rdcServices.CreateNeedsList(seedSignatures[index], sigInfo);
                    foreach (RdcNeed need in needsList)
                    {
                        switch (need.blockType)
                        {
                            case RdcNeedType.Source:
                                // Copy this block from the remote server.
                                byte[] data = rdcWebService.TransferDataBlock(
                                        remoteFile,
                                        (int)need.fileOffset,
                                        (int)need.blockLength);

                                outStream.Write(data, 0, (int)need.blockLength);                                
                                
                                break;  

                            case RdcNeedType.Seed:
                                byte[] seedData = new Byte[need.blockLength];

                                seedStream.Seek((int)need.fileOffset, SeekOrigin.Begin);
                                seedStream.Read(seedData, 0, (int)need.blockLength);

                                outStream.Write(seedData, 0, (int)need.blockLength);
                                break;

                            default:
                                break;
                        }
                    }
                }

                index++;
            }
            
            // Close our IO file streams.
            seedStream.Close();
            outStream.Close();            

            rdcServices.Dispose();
        }
    }
}
