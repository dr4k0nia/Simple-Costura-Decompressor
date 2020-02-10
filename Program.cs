using dnlib.DotNet;
using System;
using System.IO;
using System.IO.Compression;

namespace Costura_Decompressor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Costura-Decompressor file1 file2 ...");
                Console.WriteLine("Supported File Exentsions: .exe and .compressed");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                //Check if file exists
                string inputFile = args[i];
                if (!File.Exists(inputFile))
                    throw new Exception("[Error]: Invalid file: " + args[i]);

                //Check if the file is an executable
                if (inputFile.EndsWith(".exe"))
                {
                    //Load assembly using dnlib
                    ModuleContext modCtx = ModuleDef.CreateModuleContext();
                    ModuleDefMD module = ModuleDefMD.Load(inputFile, modCtx);

                    //Check if module has resources
                    if (!module.HasResources)
                        throw new Exception("[Error]: No extractable resources found: " + args[i]);

                    string outputDirectory = module.Location.Remove(module.Location.Length - module.FullName.Length) + module.Assembly.Name + @"-decompressed-resources\";

                    //Get all resources
                    foreach (var resource in module.Resources)
                    {
                        //Check loaded module for costura resources
                        if (!resource.Name.StartsWith("costura.") && !resource.Name.EndsWith(".dll.compressed"))
                            continue;

                        string outputFileName = outputDirectory + resource.Name.Substring(8, resource.Name.LastIndexOf(".compressed") - 8); // 8 = Length of "costura." which is added to the resources name by Costura

                        //Check if output file already exists
                        if (File.Exists(outputFileName))
                            Console.WriteLine("Decompressed resource already found: " + outputFileName);

                        //Create folder for the decompressed resources if it does not exist
                        if (!Directory.Exists(outputDirectory))
                            Directory.CreateDirectory(outputDirectory);

                        else
                        {
                            EmbeddedResource er = module.Resources.FindEmbeddedResource(resource.Name);
                            MemoryStream bufferStream = new MemoryStream();
                            bufferStream = DecompressResource(er.CreateReader().AsStream());
                            File.WriteAllBytes(outputFileName, bufferStream.ToArray());
                            Console.WriteLine("[Info]: Extracted and Decompressed Resource: " + resource.Name.Substring(8, resource.Name.LastIndexOf(".compressed") - 8)); // 8 = Length of "costura." which is added to the resources name by Costura
                        }
                    }
                    Console.WriteLine("[Info]: Output Directory: " + outputDirectory);
                    Console.ReadKey();
                }

                else if (inputFile.EndsWith(".compressed"))
                {
                    string outputFileName = inputFile.Remove(inputFile.Length - 11); // 11 = Length of the ".compressed" extension"; I dont use Substring here because the input file is not necessarily named costura.name.extension.compressed
                    using (FileStream bufferStream = File.OpenRead(inputFile))
                        File.WriteAllBytes(outputFileName, DecompressResource(bufferStream).ToArray());
                    Console.WriteLine("Decompressed file: " + args[i]);
                }

                else
                    throw new Exception("[Error]: Unsupported File Extension:" + args[i]);
            }
            Console.ReadKey();
        }

        private static MemoryStream DecompressResource(Stream input)
        {
            MemoryStream output = new MemoryStream();
            using (DeflateStream deflatestream = new DeflateStream(input, CompressionMode.Decompress))
            {
                deflatestream.CopyTo(output);
            }
            return output;
        }
    }
}