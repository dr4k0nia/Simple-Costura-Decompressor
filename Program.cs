using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace Costura_Decompressor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Drag a file onto the exe");
                return;
            }

            FileInfo _inputToDecompress = new FileInfo(args[0]);

            using (var inputFileStream = File.OpenRead(args[0]))
            {
                var inputFileName = _inputToDecompress.FullName.ToString();
                var outputFileName = inputFileName.Remove(inputFileName.Length - _inputToDecompress.Extension.Length);

                using (var outputFileStream = File.Create(outputFileName))
                {
                    using (var decompressionStream = new DeflateStream(inputFileStream, CompressionMode.Decompress))
                    {
                        decompressionStream.CopyTo(outputFileStream);
                        Console.WriteLine("Done");
                    }
                }
            }
        }
    }
}
