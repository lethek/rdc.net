///  Copyright (c) Microsoft Corporation.  All rights reserved.
///
///     Sample code developed by JW Secure, Inc.
///     Errata will be published via http://www.jwsecure.com/dan/index.html.
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
            string url;
            string remoteFile;
            string localFile;
            string targetFile;

            // Validate args
            if (args.Length < 4 || args[0] == "/?")
            {
                ShowHelp();
                return;
            }

            url = args[1];
            remoteFile = args[2];
            localFile = args[3];
            targetFile = args[4];

            // Instantiate our RDC Client helper class. For now,
            // we will just use the current application directory
            // for our working directory.  This is configurable.
            RdcClient client = new RdcClient(url, Directory.GetCurrentDirectory());
            
            // Start the synchronization process
            client.Synchronize(remoteFile, localFile, targetFile);

            Console.WriteLine("Completed!");

            return;
      
        }

        static void ShowHelp()
        {
            Console.WriteLine("\nUSAGE: client.exe -u remoteUrl remoteFile(source) localFile(seed) targetFile(output)");
            Console.WriteLine("\n   -u\t\tAbsolute URL to the remote RDC Web Service.");
            Console.WriteLine("   remoteFile\tAbsolute path to the remote file.");
            Console.WriteLine("   localFile\tAbsolute path to the local seed file.");
            Console.WriteLine("   targetFile\tAbsolute path to the local target file.  This is the output.\n");
        }
    }
}
