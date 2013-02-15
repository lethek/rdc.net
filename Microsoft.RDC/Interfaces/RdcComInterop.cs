///  Copyright (c) Microsoft Corporation.  All rights reserved.
///
///     Sample code developed by JW Secure, Inc.
///     Errata will be published via http://www.jwsecure.com/dan/index.html.
///
///		Further enhanced by David Jade
///		http://blog.mutable.net
///
///		Note: I have a strong suspicion that things in here that are not used in the sample may not be correct
///		When in doubt, check RDC's COM .idl 
///		

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Permissions;

using HRESULT = System.Int32;

namespace Microsoft.RDC
{
	/// <summary>
	/// Internal MSRDC definitions
	/// </summary>
	public struct MSRDC
	{
		public const uint VERSION                   = 0x010000;
		public const uint MINIMUM_COMPATIBLE_APP_VERSION = 0x010000;
		public const uint MINIMUM_DEPTH             = 1;
		public const uint MAXIMUM_DEPTH             = 8;
		public const uint MINIMUM_COMPAREBUFFER     = 100000;
		public const uint MAXIMUM_COMPAREBUFFER     = (1 << 30);
		public const uint DEFAULT_COMPAREBUFFER     = 3200000;
		public const uint MINIMUM_INPUTBUFFERSIZE   = 1024;
		public const uint MINIMUM_HORIZONSIZE       = 128;
		public const uint MAXIMUM_HORIZONSIZE       = 1024 * 16;
		public const uint MINIMUM_HASHWINDOWSIZE    = 2;
		public const uint MAXIMUM_HASHWINDOWSIZE    = 96;
		public const uint DEFAULT_HASHWINDOWSIZE_1  = 48;
		public const uint DEFAULT_HORIZONSIZE_1     = 1024;
		public const uint DEFAULT_HASHWINDOWSIZE_N  = 2;
		public const uint DEFAULT_HORIZONSIZE_N     = 128;
		public const uint MAXIMUM_TRAITVALUE        = 63;
		public const uint MINIMUM_MATCHESREQUIRED   = 1;
		public const uint MAXIMUM_MATCHESREQUIRED   = 16;
	}

	#region Enums
	internal enum GeneratorParametersType : int
	{
		Unused      = 0,
		FilterMax   = 1
	}

	public enum RdcNeedType : int
	{
		Source      = 0,
		Target,
		Seed,
		SeedMax     = 255
	}

	internal enum RdcCreatedTables : int
	{
		InvalidOrUnknown    = 0,
		Existing,
		New
	}

	internal enum RdcMappingAccessMode : int
	{
		Undefined   = 0,
		ReadOnly,
		ReadWrite
	}

	public enum RdcError : uint
	{
		NoError                 = 0,
		HeaderVersionNewer,
		HeaderVersionOlder,
		HeaderMissingOrCorrupt,
		HeaderWrongType,
		DataMissingOrCorrupt,
		DataTooManyRecords,
		FileChecksumMismatch,
		ApplicationError,
		Aborted,
		Win32Error
	}

	#endregion

	#region RDC Structures
		
	/// <summary>
	/// Make sure we align on the 8 byte boundry.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct RdcNeed
	{
		public RdcNeedType blockType;
		public UInt64 fileOffset;
		public UInt64 blockLength;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct RdcBufferPointer
	{
		public uint size;
		public uint used;
		public IntPtr data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct RdcNeedPointer
	{
		public uint size;
		public uint used;
		public IntPtr data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct RdcSignature
	{
		public IntPtr signature;
		public ushort blockLength;        
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct RdcSignaturePointer
	{
		public uint size;
		public uint used;
		public IntPtr data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct SimilarityMappedViewInfo
	{
		public string data;
		public uint length;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	public struct SimilarityData
	{
		public char[] data;     // m_Data[16]
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct FindSimilarFileIndexResults
	{
		public uint fileIndex;
		public uint matchCount;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct SimilarityDumpData
	{
		public uint fileIndex;
		public SimilarityData data;
	}

	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct SimilarityFileId
	{
		public byte[] fileId;   // m_FileId[32]
	}

	#endregion  

	#region RdcLibrary

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A78-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface IRdcLibrary
	{
		HRESULT ComputeDefaultRecursionDepth(
			[In] Int64 fileSize, 
			[Out] out int depth);
		
		HRESULT CreateGeneratorParameters(
			[In] GeneratorParametersType parametersType, 
			[In] uint level, 
			[Out] out IRdcGeneratorParameters iGeneratorParameters);

		HRESULT OpenGeneratorParameters(
			[In] uint size, 
			[In] IntPtr parametersBlob, 
			[Out] out IRdcGeneratorParameters iGeneratorParameters);

		HRESULT CreateGenerator(
			[In] uint depth, 
			[In] [MarshalAs(UnmanagedType.LPArray)] IRdcGeneratorParameters[] iGeneratorParametersArray,
			[Out] [MarshalAs(UnmanagedType.Interface)] out IRdcGenerator iGenerator);

		HRESULT CreateComparator(
			[In, MarshalAs(UnmanagedType.Interface)] IRdcFileReader iSeedSignatureFiles, 
			[In] uint comparatorBufferSize, 
			[Out, MarshalAs(UnmanagedType.Interface)] out IRdcComparator iComparator);

		HRESULT CreateSignatureReader(
			[In, MarshalAs(UnmanagedType.Interface)] IRdcFileReader iFileReader, 
			[Out] out IRdcSignatureReader iSignatureReader);
		
		HRESULT GetRDCVersion(
			[Out] out uint currentVersion, 
			[Out] out uint minimumCompatibileAppVersion);

	}

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A85-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class RdcLibrary { }

	#endregion

	#region RdcSimilarityGenerator
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A80-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface IRdcSimilarityGenerator
	{
		HRESULT EnableSimilarity();

		HRESULT Results([Out] out SimilarityData similarityData);
	}

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A92-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class RdcSimilarityGenerator { }

	#endregion

	#region RDC COM Interfaces

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A71-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface IRdcGeneratorParameters
	{
		HRESULT GetGeneratorParametersType([Out] out GeneratorParametersType parametersType);

		HRESULT GetParametersVersion(
			[Out] out uint currentVersion, 
			[Out] out uint minimumCompatabileAppVersion);

		HRESULT GetSerializeSize([Out] out uint size);

		HRESULT Serialize(
			[In] uint size, 
			[Out] out IntPtr parametersBlob, 
			[Out] out uint bytesWritten);
	}


	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A72-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface IRdcGeneratorFilterMaxParameters
	{
		HRESULT GetHorizonSize([Out] out uint horizonSize);

		HRESULT SetHorizonSize([In] uint horizonSize);

		HRESULT GetHashWindowSize([Out] out uint hashWindowSize);

		HRESULT SetHashWindowSize([In] uint hashWindowSize);
	}


	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A73-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]    
	internal interface IRdcGenerator
	{
		HRESULT GetGeneratorParameters(
			[In] uint level, 
			[Out] out IRdcGeneratorParameters iGeneratorParameters);

		HRESULT Process(
			[In, MarshalAs(UnmanagedType.Bool)] bool endOfInput, 
			[In, Out, MarshalAs(UnmanagedType.Bool)] ref bool endOfOutput,
			[In, Out] ref RdcBufferPointer inputBuffer,
			[In] uint depth, 
			[Out, MarshalAs(UnmanagedType.LPArray)] IntPtr[] outputBuffers, 
			[Out] out RdcError errorCode);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A74-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface IRdcFileReader
	{
		void GetFileSize([Out] out UInt64 fileSize);

		void Read(
			[In] UInt64 offsetFileStart, 
			[In] uint bytesToRead,
			[In, Out] ref uint bytesRead,
			[In] IntPtr buffer, 
			[In, Out] ref bool eof);

		void GetFilePosition(out UInt64 offsetFromStart);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A75-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface IRdcFileWriter   // inherits IRdcFileReader
	{
		void Write(
			[In] UInt64 offsetFileStart, 
			[In] uint bytesToWrite, 
			[In, Out] ref IntPtr buffer);

		void Truncate();

		void DeleteOnClose();
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A76-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface IRdcSignatureReader
	{
		HRESULT ReaderHeader([Out] out RdcError errorCode);

		HRESULT ReadSignatures(
			[In, Out, MarshalAs(UnmanagedType.Struct)] ref RdcSignaturePointer rdcSignaturePointer,
			[In, Out, MarshalAs(UnmanagedType.Bool)] ref bool endOfOutput);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A77-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface IRdcComparator
	{
		HRESULT Process(
			[In, MarshalAs(UnmanagedType.Bool)] bool endOfInput, 
			[In, Out, MarshalAs(UnmanagedType.Bool)] ref bool endOfOutput, 
			[In, Out] ref RdcBufferPointer inputBuffer,
			[In, Out] ref RdcNeedPointer outputBuffer, 
			[Out] out RdcError errorCode);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A7A-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface ISimilarityReportProgress
	{
		HRESULT ReportProgress([In] uint percentCompleted);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A7B-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface ISimilarityTableDumpState
	{
		HRESULT GetNextData(
			[In] uint resultsSize,
			[Out] out uint resultsUsed,
			[Out, MarshalAs(UnmanagedType.Bool)] out bool eof,
			[In, Out] ref SimilarityDumpData results);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A7C-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface ISimilarityTraitsMappedView
	{
		HRESULT Flush();

		HRESULT Unmap();

		HRESULT Get(
			[In] UInt64 index,
			[In, MarshalAs(UnmanagedType.Bool)] bool dirty,
			[In] uint numElements,
			[Out] out SimilarityMappedViewInfo viewInfo);

		HRESULT GetView(
			[Out] out string mappedPateBegin,
			[Out] out string mappedPageEnd);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A7D-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface ISimilarityTraitsMapping
	{
		HRESULT CloseMapping();

		HRESULT SetFileSize(UInt64 fileSize);

		HRESULT GetFileSize(out UInt64 fileSize);

		HRESULT OpenMapping(RdcMappingAccessMode accessMode, UInt64 begin, UInt64 end, out UInt64 actualEnd);

		HRESULT ResizeMapping(RdcMappingAccessMode accessMode, UInt64 begin, UInt64 end, out UInt64 actualEnd);

		HRESULT GetPageSize(out uint pageSize);

		HRESULT CreateView(uint minimumMappedPages, RdcMappingAccessMode accessMode,
			out ISimilarityTraitsMappedView mappedView);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A7E-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface ISimilarityTraitsTable
	{
		HRESULT CreateTable([MarshalAs(UnmanagedType.LPWStr)] string path, bool truncate,
			IntPtr securityDescriptor, out RdcCreatedTables isNew);

		HRESULT CreateTableIndirect(ISimilarityTraitsMapping mapping, bool truncate, out RdcCreatedTables isNew);

		HRESULT CloseTable(bool isValid);

		HRESULT Append(SimilarityData data, uint fileIndex);

		HRESULT FindSimilarFileIndex(SimilarityData similarityData, ushort numberOfMatchesRequired,
			out FindSimilarFileIndexResults findSimilarFileIndexResults, uint resultsSize,
			out uint resultsUsed);

		HRESULT BeginDump(out ISimilarityTableDumpState similarityTableDumpState);

		HRESULT GetLastIndex(out uint fileIndex);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A7F-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface ISimilarityFileIdTable
	{
		HRESULT CreateTable([MarshalAs(UnmanagedType.LPWStr)] string path, bool truncate,
			IntPtr securityDescriptor, uint recordSize, out RdcCreatedTables isNew);

		HRESULT CreateTableIndirect(IRdcFileWriter fileIdFile, bool truncate, uint recordSize, out RdcCreatedTables isNew);

		HRESULT CloseTable(bool isValid);

		HRESULT Append(SimilarityFileId similarityFileId, out uint similarityFileIndex);

		HRESULT Lookup(uint similarityFileIndex, out SimilarityFileId similarityFileId);

		HRESULT Invalidate(uint similarityFileIndex);

		HRESULT GetRecordCount(out uint recordCount);

	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A81-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface IFindSimilarResults
	{
		HRESULT GetSize(out uint size);

		HRESULT GetNextFileId(out uint numTraitsMatched, out SimilarityFileId similarityFileId);
	}

	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96236A83-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	internal interface ISimilarity
	{
		HRESULT CreateTable([MarshalAs(UnmanagedType.LPWStr)] string path, bool truncate,
			IntPtr securityDescriptor, uint recordSize, out RdcCreatedTables isNew);

		HRESULT CreateTableIndirect(ISimilarityTraitsMappedView mapping, IRdcFileWriter fileIdFile,
			bool truncate, uint recordSize, out RdcCreatedTables isNew);

		HRESULT CloseTable(bool isValid);

		HRESULT Append(SimilarityFileId similarityFileId, SimilarityData similarityData);

		HRESULT FindSimilarFileId(SimilarityData similarityData, ushort numberOfMatchesRequired,
			uint resultsSize, out IFindSimilarResults findSimilarResults);

		HRESULT CopyAndSwap(ISimilarity newSimilarityTables, ISimilarityReportProgress reportProgress);

		HRESULT GetRecordCount(out uint recordCount);
	}

	#endregion

	#region COM Class Imports

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A86-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class RdcGeneratorParameters   { }

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A87-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class RdcGeneratorFilterMaxParameters  { }

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A88-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class RdcGenerator { }    

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A8A-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class RdcSignatureReader { }

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A8B-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class RdcComparator { }

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A8D-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class SimilarityReportProgress { }

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A8E-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class SimilarityTableDumpState { }

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A8F-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class SimilarityTraitsTable { }

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A90-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class SimilarityFileIdTable { }

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A91-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class Similarity { }

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A93-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class FindSimilarResults { }

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A94-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class SimilarityTraitsMapping { }

	[ClassInterface(ClassInterfaceType.None)]
	[Guid("96236A95-9DBC-11DA-9E3F-0011114AE311")]
	[ComImport()]
	[SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
	internal class SimilarityTraitsMappedView { }

	#endregion
}
