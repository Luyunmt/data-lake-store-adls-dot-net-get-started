﻿using Microsoft.Azure.DataLake.Store;
using System;
using System.IO;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest.Azure.Authentication;

namespace AdlsSDKGettingStarted
{
    public class Program
    {
        private static string clientId = "FILL-IN-HERE";         // Also called application id in portal
        private static string clientSecret = "FILL-IN-HERE";
        private static string domain = "FILL-IN-HERE";            // Also called tenant Id
        private static string clientAccountPath = "FILL-IN-HERE";

        public static void Main(string[] args)
        {
            // Obtain AAD token
            var creds = new ClientCredential(clientId, clientSecret);
            var clientCreds = ApplicationTokenProvider.LoginSilentAsync(domain, creds).GetAwaiter().GetResult();

            // Create ADLS client object
            AdlsClient client = AdlsClient.CreateClient(clientAccountPath, clientCreds);

            try
            {
                string fileName = "/Test/testFilename1.txt";

                // Create a file
                using (var streamWriter = new StreamWriter(client.CreateFile(fileName, IfExists.Overwrite)))
                {
                    streamWriter.WriteLine("This is test data to write");
                    streamWriter.WriteLine("This is line 2");
                }

                // Append to existing file
                using (var streamWriter = new StreamWriter(client.GetAppendStream(fileName)))
                {
                    streamWriter.WriteLine("This is the added line");
                }

                //Read file contents
                using (var readStream = new StreamReader(client.GetReadStream(fileName)))
                {
                    string line;
                    while ((line = readStream.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
                    }
                }

                // Get the properties of the file
                var diren = client.GetDirectoryEntry(fileName);
                PrintDirectoryEntry(diren);

                // Rename a file
                string destFilePath = "/Test/testRenameDest3.txt";
                Console.WriteLine(client.Rename(fileName, destFilePath, true));

                // Enumerate directory
                foreach (var dir in client.EnumerateDirectory("/Test"))
                {
                    PrintDirectoryEntry(dir);
                }

                // Delete a dirtectory and all it's subdirectories and files
                client.DeleteRecursive("/Test");

            }
            catch (AdlsException e)
            {
                PrintAdlsException(e);
            }

            Console.WriteLine("Done. Press ENTER to continue ...");
            Console.ReadLine();
        }
        
        private static void PrintDirectoryEntry(DirectoryEntry diren)
        {
            Console.WriteLine($"Name: {diren.Name}");
            Console.WriteLine($"FullName: {diren.FullName}");
            Console.WriteLine($"Length: {diren.Length}");
            Console.WriteLine($"Type: {diren.Type}");
            Console.WriteLine($"User: {diren.User}");
            Console.WriteLine($"Group: {diren.Group}");
            Console.WriteLine($"Permission: {diren.Permission}");
            Console.WriteLine($"Modified Time: {diren.LastModifiedTime}");
            Console.WriteLine($"Last Accessed Time: {diren.LastAccessTime}");
            Console.WriteLine();
        }

        private static void PrintAdlsException(AdlsException exp)
        {
            Console.WriteLine("ADLException");
            Console.WriteLine($"   Http Status: {exp.HttpStatus}");
            Console.WriteLine($"   Http Message: {exp.HttpMessage}");
            Console.WriteLine($"   Remote Exception Name: {exp.RemoteExceptionName}");
            Console.WriteLine($"   Server Trace Id: {exp.TraceId}");
            Console.WriteLine($"   Exception Message: {exp.Message}");
            Console.WriteLine($"   Exception Stack Trace: {exp.StackTrace}");
            Console.WriteLine();
        }
    }
}