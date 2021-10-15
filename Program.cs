using System;
using System.IO;
using AsmResolver.DotNet;
using Console = Colorful.Console;

namespace Costura_Decompressor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Costura-Decompressor.exe <file1.exe> <file2.dll.compressed> ...");
                Console.ReadKey();
                return;
            }

            foreach (string inputFile in args)
            {
                if (!File.Exists(inputFile))
                {
                    Logger.Error($"File not found: {inputFile}");
                    continue;
                }

                //Check if the file is an executable
                if (Path.GetExtension(inputFile) == ".exe")
                {
                    ProcessExecutable(inputFile);
                    continue;
                }


                if (inputFile.EndsWith(".compressed"))
                {
                    inputFile.ProcessCompressedFile();
                    continue;
                }

                Logger.Error("Unsupported file extension, accepts .exe or .compressed");
            }

            Console.ReadKey();
        }

        private static void ProcessExecutable(string inputFile)
        {
            Logger.Info($"Processing executable: {Path.GetFileName(inputFile)}");
            try
            {
                var module = ModuleDefinition.FromFile(inputFile);
                var extractor = new ExtractorNew(module);
                extractor.Run();
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
    }
}