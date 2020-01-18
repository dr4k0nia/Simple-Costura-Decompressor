using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace Costura_Decompressor
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.Title = "Cosutra-Decompressor";

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: Costura-Decompressor file1 file2 ...");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string inputFile = ResolveFilePath(args[i]);
                if (string.IsNullOrEmpty(inputFile) || !inputFile.Contains(".compressed"))
                {
                    throw new Exception("[Error] Invalid file:" + args[i]);
                }

                string inputFileName = Path.GetFileName(inputFile);
                byte[] buffer = Decompress(File.ReadAllBytes(inputFile));
                string outputFileName = inputFile.Remove(inputFile.Length - ".compressed".Length);
                File.WriteAllBytes(outputFileName, buffer);
                Console.WriteLine("Decompressed file: " + args[i]);
            }
        }

        private static byte[] Decompress(byte[] input)
        {
            MemoryStream inputstream = new MemoryStream(input);
            MemoryStream output = new MemoryStream();
            using (DeflateStream deflatestream = new DeflateStream(inputstream, CompressionMode.Decompress))
            {
                deflatestream.CopyTo(output);
            }
            return output.ToArray();
        }

        private static string ResolveFilePath(string filepath)
        {
            if (File.Exists(filepath))
            {
                return filepath;
            }
            return string.Empty;
        }
    }
}