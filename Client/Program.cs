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
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Permissions;

using Microsoft.RDC;

namespace Client
{
	class Program
	{
		static void Main(string[] args)
		{
			// Validate args
			if (args.Length < 4 || args[0] == "/?")
			{
				ShowHelp();
				return;
			}

			int argIndex = 0;

			int recursionDepth = -1;	// default - let RDC decide
			if (args[argIndex] == "-r")
			{
				if (int.TryParse(args[++argIndex], out recursionDepth) == false)
				{
					Console.WriteLine("Missing or invalid recursion depth value");
					return;
				}
				argIndex++;
			}

			string url = args[argIndex++];
			string remoteFile = args[argIndex++];
			string localFile = args[argIndex++];
			string targetFile = args[argIndex++];

			// Instantiate our RDC Client helper class. For now,
			// we will just use the current application directory
			// for our working directory.  This is configurable.
			RdcClient client = new RdcClient(url, Directory.GetCurrentDirectory(), recursionDepth);
			
			// Start the synchronization process
			client.Synchronize(remoteFile, localFile, targetFile);

			Console.WriteLine("\n\nCompleted! Press any key...");
			Console.ReadKey();

			return;
	  
		}

		static void ShowHelp()
		{
			Console.WriteLine("\nUSAGE: client.exe [-r x] remoteUrl remoteFile(source) localFile(seed) targetFile(output)");

			Console.WriteLine("\n   -r\t\t[optional] recursion depth (1-8)\n");
			Console.WriteLine("   remoteUrl\tAbsolute URL to the remote RDC Web Service.");
			Console.WriteLine("   remoteFile\tAbsolute path to the remote file.");
			Console.WriteLine("   localFile\tAbsolute path to the local seed file.");
			Console.WriteLine("   targetFile\tAbsolute path to the local target file.  This is the output.\n");
		}
	}
}
