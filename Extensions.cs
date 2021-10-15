using System.IO;
using System.IO.Compression;
using AsmResolver.DotNet;
using static Costura_Decompressor.Logger;

namespace Costura_Decompressor
{
    public static class Extensions
    {
        private static byte[] DecompressResource(this Stream input)
        {
            using var output = new MemoryStream();
            using var deflateStream = new DeflateStream(input, CompressionMode.Decompress);
            deflateStream.CopyTo(output);

            return output.ToArray();
        }

        public static string GetOutputPath(this ModuleDefinition module)
        {
            if (module.FilePath == null) return null;
            if (module.Assembly == null) return null;
            string name = Path.GetFileName(module.FilePath);
            return Path.Combine(module.FilePath.Remove(module.FilePath.Length - name.Length),
                @$"{module.Assembly.Name}-decompressed-resources\");
        }

        public static void Decompress(this byte[] data)
        {
            using var input = new MemoryStream(data);
            using var output = new MemoryStream();
            using var deflateStream = new DeflateStream(input, CompressionMode.Decompress);
            deflateStream.CopyTo(output);

            output.ToArray();
        }

        public static void ProcessCompressedFile(this string inputFile)
        {
            string outputFileName = inputFile.Replace("costura.", null);
            outputFileName = outputFileName.Replace(".compressed", null);
            using (var bufferStream = File.OpenRead(inputFile))
                File.WriteAllBytes(outputFileName, bufferStream.DecompressResource());
            Log($"Decompressed costura file: {inputFile}", LogType.Success);
        }
    }
}