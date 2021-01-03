using System.Collections.Generic;
using System.IO;
using dnlib.DotNet;
using static Costura_Decompressor.Logger;

namespace Costura_Decompressor
{
    public class Extractor
    {
        private readonly Dictionary<byte[], string> _extractedResources = new Dictionary<byte[], string>();
        private readonly string _outputDirectory;

        public Extractor( ModuleDef module )
        {
            Log( $"Processing module: {module.FullName}", LogType.Info  );
            
            _outputDirectory = module.Location.Remove( module.Location.Length - module.FullName.Length ) +
                               module.Assembly.Name + @"-decompressed-resources\";

            //Check if module has resources
            if ( !module.HasResources )
            {
                Log( $"Could not find any embedded resources", LogType.Error );
                return;
            }

            //Get all resources
            foreach ( var resource in module.Resources )
            {
                if ( resource.Name.Length < 19 )
                    continue;
                
                //Check loaded module for costura resources
                if ( !resource.Name.StartsWith( "costura." ) && !resource.Name.EndsWith( ".compressed" ) )
                    continue;

                var er = module.Resources.FindEmbeddedResource( resource.Name );
                //decompress extracted resource
                var reader = er.CreateReader();
                string name = resource.Name.Substring( 8,
                    resource.Name.LastIndexOf( ".compressed" ) -
                    8 ); 
                using ( var bufferStream = reader.AsStream() )
                    _extractedResources.Add( bufferStream.DecompressResource(), name );
                Log( $"Extracted resource: {name}", LogType.Success );
            }

            if ( _extractedResources.Count == 0 )
                Log( $"Could not find costura embedded resource in module: {module.FullName}", LogType.Error );
        }

        public void SaveResources()
        {
            if ( _extractedResources.Count == 0 )
                return;

            if ( !Directory.Exists( _outputDirectory ) )
                Directory.CreateDirectory( _outputDirectory );
            
            foreach ( var resource in _extractedResources )
            {
                File.WriteAllBytes( _outputDirectory + resource.Value, resource.Key);
            }

            Log( $"Extracted and saved {_extractedResources.Count} decompressed resources", LogType.Info );
        }
    }
}