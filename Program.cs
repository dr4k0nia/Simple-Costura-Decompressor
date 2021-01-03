using System.IO;
using Colorful;
using dnlib.DotNet;
using static Costura_Decompressor.Logger;

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
                if ( inputFile.EndsWith( ".exe" ) )
                {
                    try
                    {
                        var extractor = new Extractor(ModuleDefMD.Load( inputFile ));
                        extractor.SaveResources();
                    }
                    catch
                    {
                        Log( "Failed to load module, make sure you are inputting a valid .net module", LogType.Error ); 
                    }
                }
                else if ( inputFile.EndsWith( ".compressed" ) )
                    inputFile.ProcessCompressedFile();
                else
                    Log( $"Unsupported file extension, accepts .exe or .compressed", LogType.Error );
            }

            Console.ReadKey();
        }
    }
}