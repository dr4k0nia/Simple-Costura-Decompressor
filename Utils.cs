using System.IO;
using System.IO.Compression;
using static Costura_Decompressor.Logger;

namespace Costura_Decompressor
{
    public static class Utils
    {
        public static byte[] DecompressResource( this Stream input )
        {
            using var output = new MemoryStream();
            using var deflatestream = new DeflateStream( input, CompressionMode.Decompress );
            deflatestream.CopyTo( output );
                
            return output.ToArray();
        }
        
        public static void ProcessCompressedFile( this string inputFile )
        {
            string outputFileName = inputFile.Replace( "costura.", null );
            outputFileName = outputFileName.Replace( ".compressed", null );
            using ( var bufferStream = File.OpenRead( inputFile ) )
                File.WriteAllBytes( outputFileName, bufferStream.DecompressResource() );
            Log( $"Decompressed costura file: {inputFile}", LogType.Success);
        }
    }
}