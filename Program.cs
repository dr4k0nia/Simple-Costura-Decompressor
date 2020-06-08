using dnlib.DotNet;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using Console = Colorful.Console;

namespace Costura_Decompressor
{
    internal class Program
    {
        private static void Main( string[] args )
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            if ( args.Length == 0 )
            {
                Console.WriteLine( "Usage: Costura-Decompressor.exe file1.exe file2.dll.compressed ..." );
                Console.ReadKey();
                return;
            }

            for ( int i = 0; i < args.Length; i++ )
            {
                //Check if file exists
                string inputFile = args[i];
                if ( !File.Exists( inputFile ) )
                {
                    PrintFormattedError( "File not found", inputFile );
                    continue;
                }

                //Check if the file is an executable
                if ( inputFile.EndsWith( ".exe" ) )
                {
                    ExtractFromAssembly( inputFile );
                }
                else if ( inputFile.EndsWith( ".compressed" ) )
                    ProcessSingleResource( inputFile );
                else
                {
                    PrintFormattedError( "Unsupported file extension, accepts .exe or .compressed", inputFile );
                    continue;
                }
            }

            Console.ReadKey();
        }

        private static void ExtractFromAssembly( string inputFile )
        {
            //Load assembly using dnlib
            ModuleDefMD module = ModuleDefMD.Load( inputFile );

            //Check if module has resources
            if ( !module.HasResources )
            {
                PrintFormattedError( "No extractable resources found", inputFile );
                return;
            }

            //get path for decompressed output
            string outputDirectory = module.Location.Remove( module.Location.Length - module.FullName.Length ) +
                                     module.Assembly.Name + @"-decompressed-resources\";

            const string template = "{0} Exracting and decompressing resources from Module: {1}";
            var output = new Colorful.Formatter[]
            {
                new Colorful.Formatter( "[Log]:", Color.DeepSkyBlue ),
                new Colorful.Formatter( module.Name, Color.Cyan )
            };
            Console.WriteLineFormatted( template, Color.White, output );

            int count = 0;
            //Get all resources
            foreach ( var resource in module.Resources )
            {
                //Check loaded module for costura resources
                if ( !resource.Name.StartsWith( "costura." ) && !resource.Name.EndsWith( ".compressed" ) )
                    continue;

                string outputFileName = outputDirectory +
                                        resource.Name.Substring( 8,
                                            resource.Name.LastIndexOf( ".compressed" ) -
                                            8 ); // 8 = Length of "costura." which is added to the resources name by Costura

                if ( File.Exists( outputFileName ) )
                {
                    PrintFormattedOutput( "Decompressed resource already exists", outputFileName, Color.White,
                        Color.Yellow );
                    count--;
                    continue;
                }
                else
                {
                    //Create folder for the decompressed resources if it does not exist
                    if ( !Directory.Exists( outputDirectory ) )
                        Directory.CreateDirectory( outputDirectory );

                    EmbeddedResource er = module.Resources.FindEmbeddedResource( resource.Name );
                    //decompress extracted resource
                    var reader = er.CreateReader();
                    using ( var bufferStream = DecompressResource( reader.AsStream() ) )
                        File.WriteAllBytes( outputFileName, bufferStream.ToArray() );
                    Console.WriteLine(
                        " ┕► " + resource.Name.Substring( 8,
                            resource.Name.LastIndexOf( ".compressed" ) -
                            8 ) ); // 8 = Length of "costura." which is added to the resources name by Costura
                    count++;
                }
            }

            if ( count == 0 )
                PrintFormattedError( "No costura embedded resources found", inputFile );
            else if ( count > 0 )
                PrintFormattedOutput( $"Extracted {count} resources", outputDirectory, Color.White, Color.Yellow );
        }

        private static void ProcessSingleResource( string inputFile )
        {
            string outputFileName = inputFile.Replace( "costura.", null );
            outputFileName = outputFileName.Replace( ".compressed", null );
            using ( FileStream bufferStream = File.OpenRead( inputFile ) )
                File.WriteAllBytes( outputFileName, DecompressResource( bufferStream ).ToArray() );
            PrintFormattedOutput( "Decompressed costura file", outputFileName, Color.White, Color.Yellow );
        }

        private static MemoryStream DecompressResource( Stream input )
        {
            using ( MemoryStream output = new MemoryStream() )
            {
                using ( DeflateStream deflatestream = new DeflateStream( input, CompressionMode.Decompress ) )
                    deflatestream.CopyTo( output );
                
                return output;
            }
        }

        private static void PrintFormattedOutput( string name, string file, Color nameColor, Color fileColor )
        {
            const string template = "{0} {1} ► {2}";
            var output = new Colorful.Formatter[]
            {
                new Colorful.Formatter( "[Log]:", Color.DeepSkyBlue ),
                new Colorful.Formatter( name, nameColor ),
                new Colorful.Formatter( file, fileColor )
            };
            Console.WriteLineFormatted( template, Color.White, output );
        }

        private static void PrintFormattedError( string message, string file )
        {
            const string template = "{0} {1} ► {2}";
            var output = new Colorful.Formatter[]
            {
                new Colorful.Formatter( "[Error]:", Color.OrangeRed ),
                new Colorful.Formatter( message, Color.White ),
                new Colorful.Formatter( file, Color.Yellow )
            };
            Console.WriteLineFormatted( template, Color.White, output );
        }
    }
}