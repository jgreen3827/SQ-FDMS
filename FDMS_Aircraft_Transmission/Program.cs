/* 
* FILE : Program.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the main Program file to run the Aircaft Transmission System.
*/

using FDMS_Aircraft_Transmission.ATS_Client_Socket;
using System;
using System.Collections.Generic;
using System.IO;

namespace FDMS_Aircraft_Transmission
{
    /*
    * NAME : Program
    * PURPOSE : The Program class holds the main which will run the Aircraft Transmission System.
    */
    internal class Program
    {
        private static readonly List<string> _FileNames = new List<string>() { "Sample_Files\\C-FGAX.txt", "Sample_Files\\C-GEFC.txt", "Sample_Files\\C-QWWT.txt" };

        /* 
        * FUNCTION : Main
        * DESCRIPTION :
        *   This function will what is used to run the Aircraft Transmission System. It will get all the files to
        *   be read and then will pass them to the client once for it to begin.
        * PARAMETERS :
        *   string[] args : These are the program arguments which can be passed into the program as a path to the files.
        * RETURNS :
        *   void : none
        */
        static void Main(string[] args)
        {
            List<string> fileNames = new List<string>();

            if(args.Length > 0)
            {   
                foreach (string fileName in args)
                {
                    if (Path.GetExtension(fileName) != ".txt")
                    {
                        Console.WriteLine("Error with input files. Only \".txt\" files are compatible.");
                        return;
                    }
                    fileNames.Add(fileName);   
                }
            }
            else
            {
                string _filePath = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
                _filePath = Directory.GetParent(Directory.GetParent(Directory.GetParent(_filePath).FullName).FullName).FullName;

                foreach (string fileName in _FileNames)
                {
                    fileNames.Add(Path.Combine(_filePath, fileName));
                }
            }


            Console.WriteLine("File type(s) validated.");

            // Start parsing and sending file data
            AsyncClient.StartClient(fileNames);
        }
    }
}
