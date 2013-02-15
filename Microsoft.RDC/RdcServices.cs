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
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.RDC
{
    public sealed class RdcServices : IDisposable
    {
        private string workingDir = Directory.GetCurrentDirectory();
        private int recursionDepth = -1;
        private uint outputBufferSize = 1024;
        private uint inputBufferSize = 8 * 1024;
        private IRdcLibrary rdcLibrary;
        IRdcSimilarityGenerator rdcSimGenerator;
        private Hashtable _sigCache = Hashtable.Synchronized(new Hashtable());

        public RdcServices()
        {
            // Create our instance to the RdcLibrary 
            // COM interface.
            rdcLibrary = (IRdcLibrary)new RdcLibrary();
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the working directory used generate and store signature files.  Defaults to the
        /// current folder.
        /// </summary>
        public string WorkingDirectory
        {
            get { return workingDir; }
            set { workingDir = value; }
        }

        /// <summary>
        /// Gets or sets the recursion depth to use.  If defaulted or set to -1, MSRDC will calculate the 
        /// appropriate recursion depth based off of the target file's size.
        /// </summary>
        public int RecursionDepth
        {
            get { return recursionDepth; }
            set {
                if (value > MSRDC.MAXIMUM_DEPTH)
                    throw new RdcException("Invalid recursion depth.  Valid range is 1-8");

                recursionDepth = value; 
            }
        }

        #endregion


        /// <summary>
        /// Generates and computes signatures for a given stream and stores the signature 
        /// files in the provided working directory.
        /// </summary>
        /// <param name="source">Source stream to process</param>
        /// <returns>A collection of signature objects</returns>
        public SignatureCollection GenerateSignatures(Stream source)
        {
            int hr = 0;

            // Compute the appropriate recursion depth for 
            // this stream if the depth is not provided.
            if (recursionDepth < 1)
            {
                hr = rdcLibrary.ComputeDefaultRecursionDepth(source.Length, out this.recursionDepth);
                if (hr != 0) {
                    throw new RdcException("Failed to compute the recursion depth.", hr);
                }
            }

            // Initalize our signature collection.  For this 
            // sample we will create a unique file in the 
            // working directory for each recursion level.
            SignatureCollection signatures = InitializeAndPrepareSignatures();

             // Array of pointers to generation parameters
            IRdcGeneratorParameters[] generatorParameters = new IRdcGeneratorParameters[recursionDepth];

            for (int i = 0; i < recursionDepth; i++)
            {
                hr = rdcLibrary.CreateGeneratorParameters(
                    GeneratorParametersType.FilterMax,
                    (uint)i + 1,
                    out generatorParameters[i]);

                if (hr != 0) {
                    throw new RdcException("Failed to create the generator parameters.", hr);
                }
                
                IRdcGeneratorFilterMaxParameters maxParams = (IRdcGeneratorFilterMaxParameters)generatorParameters[i];

                // Set the default properties
                maxParams.SetHashWindowSize(i == 0 ? MSRDC.DEFAULT_HASHWINDOWSIZE_1 : MSRDC.DEFAULT_HASHWINDOWSIZE_N);
                maxParams.SetHorizonSize(i == 0 ? MSRDC.DEFAULT_HORIZONSIZE_1 : MSRDC.DEFAULT_HORIZONSIZE_N);

            }

            // Create our RdcGenerator
            IRdcGenerator rdcGenerator = null;            
            hr = rdcLibrary.CreateGenerator((uint)recursionDepth, generatorParameters, out rdcGenerator);
            if (hr != 0) {
                throw new RdcException("Failed to create the RdcGenerator.", hr);
            }
            
            // Enable similarity
            rdcSimGenerator = (IRdcSimilarityGenerator)rdcGenerator;
            rdcSimGenerator.EnableSimilarity();

            // Create our output buffers
            IntPtr[] outputBuffers = new IntPtr[recursionDepth];
            RdcBufferPointer[] outputPointer = new RdcBufferPointer[recursionDepth];
            IntPtr[] outputPointers = new IntPtr[recursionDepth];

            for (int i = 0; i < recursionDepth; i++)
            {
                outputBuffers[i] = Marshal.AllocCoTaskMem((int)outputBufferSize + 16);
                outputPointer[i].size = outputBufferSize;
                outputPointer[i].data = outputBuffers[i];
                outputPointer[i].used = 0;

                // Marshal the managed structure to a native
                // pointer and add it to our array.                
                outputPointers[i] = Marshal.AllocCoTaskMem(Marshal.SizeOf(outputPointer[i]));
                Marshal.StructureToPtr(outputPointer[i], outputPointers[i], false);
            }

            // Create and allocate memory for our input buffer.
            IntPtr inputBuffer = Marshal.AllocCoTaskMem((int)inputBufferSize+16);
            RdcBufferPointer inputPointer = new RdcBufferPointer();
            inputPointer.size = 0;
            inputPointer.used = 0;
            inputPointer.data = inputBuffer;                      
                        
            long totalBytesRead = 0;
            bool eof = false;
            bool eofOutput = false;

            while (hr == 0 && !eofOutput)
            {
                if (inputPointer.size == inputPointer.used)
                {
                    if (eof)
                    {
                        inputPointer.size = 0;
                        inputPointer.used = 0;
                    }
                    else
                    {
                        // When the input buffer is completely empty
                        // refill it.
                        int bytesRead = 0;
                        try
                        {
                            bytesRead = IntPtrCopy(source, inputBuffer, 0, (int)inputBufferSize);
                        }
                        catch (Exception ex)
                        {
                            // TODO: Cleanup
                            throw new RdcException("Failed to read from the source stream.", ex);
                        }
                        
                        totalBytesRead += bytesRead;
                        inputPointer.size = (uint)bytesRead;
                        inputPointer.used = 0;

                        if (bytesRead < inputBufferSize)
                            eof = true;

                    }
                }
                
                RdcError rdcErrorCode = RdcError.NoError ;

                //Force garbage collection.
                GC.Collect();

                // Wait for all finalizers to complete before continuing.
                // Without this call to GC.WaitForPendingFinalizers, 
                // the worker loop below might execute at the same time 
                // as the finalizers.
                // With this call, the worker loop executes only after
                // all finalizers have been called.
                GC.WaitForPendingFinalizers();                

                hr = rdcGenerator.Process(
                    eof,
                    ref eofOutput,
                    ref inputPointer,
                    (uint)recursionDepth,
                    outputPointers,
                    out rdcErrorCode);                
                
                if (hr != 0 || rdcErrorCode != RdcError.NoError)
                {
                    throw new RdcException("RdcGenerator failed to process the signature block.", rdcErrorCode);
                }

                RdcBufferTranslate(outputPointers, outputPointer);

                for (int i = 0; i < recursionDepth; i++)
                {
                    int bytesWritten = 0;

                    // Write the signature block to the file.
                    bytesWritten = IntPtrCopy(
                                        outputBuffers[i],
                                        signatures[i].InnerStream,
                                        0,
                                        (int)outputPointer[i].used);

                    signatures[i].InnerStream.Flush();
                    signatures[i].Length += bytesWritten;

                    if (outputPointer[i].used != bytesWritten)
                        throw new RdcException("Failed to write to the signature file.");

                    outputPointer[i].used = 0;
                }

                RdcBufferTranslate(outputPointer, outputPointers);
            }

            Marshal.ReleaseComObject(rdcGenerator);
            rdcGenerator = null;
            
            if (inputBuffer != IntPtr.Zero)
                Marshal.FreeCoTaskMem(inputBuffer);

            // To make it easier on the consuming application,
            // reverse the order of the signatures in our collection.
            SignatureCollection orderedSigs = new SignatureCollection();
            for (int i = signatures.Count - 1; i >= 0; i--)
                orderedSigs.Add(signatures[i]);

            return orderedSigs;
        }


        /// <summary>
        /// Performs a version check of the local RDC environment against 
        /// a provided RDC version (server\client).
        /// </summary>
        /// <param name="version">Version to compare against</param>
        public void CheckVersion(RdcVersion version)
        {            
            uint currentVersion = 0;
            uint minimumCompatibleAppVersion = 0;

            // lookup the RDC version
            int hr = rdcLibrary.GetRDCVersion(
                            out currentVersion,
                            out minimumCompatibleAppVersion);

            if (hr == 0)
            {
                if (currentVersion < version.MinimumCompatibleAppVersion)
                {
                    throw new RdcException("Incompatible: The RDC library is too old.");
                }
                else if (minimumCompatibleAppVersion > version.CurrentVersion)
                {
                    throw new RdcException("Incompatible: The RDC library is newer than expected.");
                }
            }            
        }

        /// <summary>
        /// Interrogates the RDC library version on this local machine.
        /// </summary>
        /// <returns>Current and minimum supported version.</returns>
        public static RdcVersion GetRdcVersion()
        {
            // Create our instance to the RdcLibrary 
            // COM interface.
            IRdcLibrary rdcLibrary = (IRdcLibrary)new RdcLibrary();

            uint currentVersion = 0;
            uint minimumCompatibleAppVersion = 0;

            // Retrieve the RDC version.
            int hr = rdcLibrary.GetRDCVersion(
                            out currentVersion,
                            out minimumCompatibleAppVersion);

            if (hr != 0)
                throw new RdcException("Failed to retrieve the version of the RDC server.");

            // Build our managed RdcVersion object.
            return (new RdcVersion(currentVersion, minimumCompatibleAppVersion));
        }


        /// <summary>
        /// Builds a comparator and perform the RDC comparison logic against the provided signatures to generate a needs list.
        /// </summary>
        /// <param name="seedSignatures">Seed signatures to compare</param>
        /// <param name="sourceSignatures">Source signatures to compare</param>
        /// <returns>RDC Needs list</returns>
        public ArrayList CreateNeedsList(SignatureCollection seedSignatures, SignatureCollection sourceSignatures)
        {
            ArrayList completeNeedsList = new ArrayList();

            for (int i = 0; i < seedSignatures.Capacity; i++)
            {
                ArrayList needsList = CreateNeedsList(seedSignatures[i], sourceSignatures[i]);
                completeNeedsList.AddRange(needsList);
            }

            return completeNeedsList;
        }

        /// <summary>
        /// Builds a comparator and perform the RDC comparison logic against the provided signatures to generate a needs list.
        /// </summary>
        /// <param name="seedSignature">Seed signature to compare</param>
        /// <param name="sourceSignature">Source signature to compare</param>
        /// <returns>RDC Needs list</returns>
        public ArrayList CreateNeedsList(SignatureInfo seedSignature, SignatureInfo sourceSignature)
        {
            int hr = 0;

            IRdcFileReader fileReader = (IRdcFileReader)new RdcFileReader(seedSignature.InnerStream);
            IRdcComparator comparator;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            hr = rdcLibrary.CreateComparator(fileReader, 1000000, out comparator);

            // Create and allocate memory for our input buffer.
            IntPtr inputBuffer = Marshal.AllocCoTaskMem((int)inputBufferSize + 16);
            RdcBufferPointer inputPointer = new RdcBufferPointer();
            inputPointer.size = 0;
            inputPointer.used = 0;
            inputPointer.data = inputBuffer;

            //RdcNeed[] needsArray;
            ArrayList needsList = new ArrayList();

            IntPtr outputBuffer = Marshal.AllocCoTaskMem(
                Marshal.SizeOf(typeof(RdcNeed)) * 256);

            RdcNeedPointer outputPointer = new RdcNeedPointer();
            
            long totalBytesRead = 0;
            bool eof = false;
            bool eofOutput = false;

            while (hr == 0 && !eofOutput)
            {
                if (inputPointer.size == inputPointer.used && !eof)
                {
                    // Fill our input buffer with the signature
                    // data from the source.
                    // When the input buffer is completely empty
                    // refill it.
                    int bytesRead = 0;
                    try
                    {
                        bytesRead = IntPtrCopy(sourceSignature.InnerStream, inputBuffer, 0, (int)inputBufferSize);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Cleanup
                        throw new RdcException("Failed to read from the source stream.", ex);
                    }

                    totalBytesRead += bytesRead;
                    inputPointer.size = (uint)bytesRead;
                    inputPointer.used = 0;

                    if (bytesRead < inputBufferSize)
                        eof = true;

                }

                // Initialize our output needs array
                outputPointer.size = 256;
                outputPointer.used = 0;
                outputPointer.data = outputBuffer;

                RdcError error = RdcError.NoError;

                // Perform the signature comparison.
                // This function may not produce needs output every time.
                // Also, it may not use all available input each time either.
                // You may call it with any number of input bytes
                // and output needs array entries. Obviously, it is more
                // efficient to give it reasonably sized buffers for each.
                // This sample waits until Process() consumes an entire input
                // buffer before reading more data from the source signatures file.
                // Continue calling this function until it sets "eofOutput" to true.
                hr = comparator.Process(
                            eof,
                            ref eofOutput,
                            ref inputPointer,
                            ref outputPointer,
                            out error);

                if (hr != 0)
                    throw new RdcException("Failed to process the signature block!");                

                if (error != RdcError.NoError)
                    throw new RdcException("Failed!");
                
                // Convert the stream to a Needs array.
                RdcNeed[] needs = GetRdcNeedList(outputPointer);
               
                foreach (RdcNeed need in needs)
                {
                    // assign the needs to our arraylist.
                    needsList.Add(need);
                }

            }

            if (hr != 0)
                throw new RdcException("Failed!");

            // Free our resources
            if (outputBuffer != IntPtr.Zero)
                Marshal.FreeCoTaskMem(outputBuffer);

            if (inputBuffer != IntPtr.Zero)
                Marshal.FreeCoTaskMem(inputBuffer);

            return needsList;
        } 
    

        /* Not used in the sample, but could be
         * extended in a future project.
         */
        public SimilarityData GetSimilarityData()
        {
            SimilarityData similarityData = new SimilarityData();

            if (rdcSimGenerator != null)
            {
                int hr = rdcSimGenerator.Results(out similarityData);
                if (hr != 0)
                    throw new RdcException("Failed to load similarity data.");
            }

            return similarityData;
        }

        /// <summary>
        /// Deletes all the signature files in the working directory.
        /// </summary>
        /// <param name="signatures">Signatures to delete</param>
        public void PurgeSignatureStore(SignatureCollection signatures)
        {
            foreach (SignatureInfo sig in signatures)
            {
                File.Delete(sig.FullPath);
            }
        }


        /// <summary>
        /// Creates a base signature collection for use by the signature generator.
        /// </summary>
        /// <returns>Collection of signatures</returns>
        private SignatureCollection InitializeAndPrepareSignatures()
        {
            SignatureCollection signatures = new SignatureCollection();

            // Initalize our signature collection.  For this 
            // implemenatation we will create a unique file in the 
            // working directory for each recursion level.
            for (int i = 0; i < recursionDepth; i++)
            {
                signatures.Add(new SignatureInfo(this.workingDir, true));
                signatures[i].Index = i;
            }            

            return signatures;
        }

        private int IntPtrCopy(Stream source, IntPtr dest, int offset, int length)
        {
            byte[] buffer = new Byte[length];

            // fill the buffer with data from the stream
            int bytes = source.Read(buffer, offset, length);

            // Now write it out to our pointer
            Marshal.Copy(buffer, offset, dest, bytes);

            return (bytes);
        }

        private int IntPtrCopy(IntPtr source, Stream dest, int offset, int length)
        {
            byte[] buffer = new Byte[length];

            Marshal.Copy(source, buffer, offset, length);

            dest.Write(buffer, 0, length);

            return (length);
        }

        private void RdcBufferTranslate(RdcBufferPointer[] source, IntPtr[] dest)
        {
            // Marshal the managed structure to a native
            // pointer and add it to our array.
            for (int i = 0; i < recursionDepth; i++)
            {
                dest[i] = Marshal.AllocCoTaskMem(Marshal.SizeOf(source[i]));
                Marshal.StructureToPtr(source[i], dest[i], false);
            }
        }

        private void RdcBufferTranslate(IntPtr[] source, RdcBufferPointer[] dest)
        {
            // Marshal the native pointer back to the 
            // managed structure.
            for (int i = 0; i < recursionDepth; i++)
            {
                dest[i] = (RdcBufferPointer)Marshal.PtrToStructure(source[i], typeof(RdcBufferPointer));
                Marshal.FreeCoTaskMem(source[i]);
            }
        }

        private RdcNeed[] GetRdcNeedList(RdcNeedPointer pointer)
        {
            RdcNeed[] needs = new RdcNeed[pointer.used];

            IntPtr ptr = new IntPtr(pointer.data.ToInt32());
            int needSize = Marshal.SizeOf(typeof(RdcNeed));

            // Get our native needs pointer 
            // and deserialize to our managed 
            // RdcNeed array.
            for (int i = 0; i < needs.Length; i++)
            {                
                needs[i] = (RdcNeed)Marshal.PtrToStructure(ptr, typeof(RdcNeed));

                // Advance the intermediate pointer
                // to our next RdcNeed struct.
                ptr = (IntPtr)(ptr.ToInt32() + needSize);
            }            

            return needs;
        }


        #region IDisposable Members

        public void Dispose()
        {
            if (rdcLibrary != null)
                Marshal.ReleaseComObject(rdcLibrary);
        }
        
        [DllImport("C:\\Projects\\MSFT\\RDC\\RDC.NET\\debug\\invoketest.dll")]
        private static extern uint Process([MarshalAs(UnmanagedType.Interface)] IRdcGenerator rdcGenerator, [MarshalAs(UnmanagedType.Bool)] bool endOfInput, [MarshalAs(UnmanagedType.U1)] ref bool endOfOutput, ref RdcBufferPointer inputBuffer,
            uint depth, [MarshalAs(UnmanagedType.LPArray)] IntPtr[] outputBuffers, out RdcError errorCode);

        #endregion
    }
}
