using System;
using System.IO;
using AsmResolver.DotNet;
using static Costura_Decompressor.Logger;
using Console = Colorful.Console;

namespace Costura_Decompressor
{
    internal static class Program
    {
        private static void Main( string[] args )
        {
            if ( args.Length == 0 )
            {
                Console.WriteLine( "Usage: Costura-Decompressor.exe <file1.exe> <file2.dll.compressed> ..." );
                Console.ReadKey();
                return;
            }

            foreach ( string inputFile in args )
            {
                if ( !File.Exists( inputFile ) )
                {
                    Log( $"File not found: {inputFile}", LogType.Error );
                    continue;
                }

                //Check if the file is an executable
                if (inputFile.EndsWith(".exe"))
                {
                    ProcessExecutable(inputFile);
                    continue;
                }


                if (inputFile.EndsWith(".compressed"))
                {
                    inputFile.ProcessCompressedFile();
                    continue;
                }

                Log( $"Unsupported file extension, accepts .exe or .compressed", LogType.Error );
            }

            Console.ReadKey();
        }

        private static void ProcessExecutable(string inputFile)
        {
            try
            {
                var module = ModuleDefinition.FromFile(inputFile);
                var extractor = new ExtractorNew(module);
                extractor.Run();
            }
            catch(Exception e)
            {
                Error(e.Message);
            }
        }
    }
}