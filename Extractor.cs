using System;
using System.Collections.Generic;
using System.IO;
using AsmResolver.DotNet;

namespace Costura_Decompressor
{
    public class ExtractorNew
    {
        private readonly ModuleDefinition _module;

        private readonly string _outputPath;

        public ExtractorNew(ModuleDefinition module)
        {
            _module = module;
            _outputPath = module.GetOutputPath();
        }

        public void Run()
        {
            if (ExtractResources(out var resources))
            {
                SaveResources(resources);
                return;
            }

            Logger.Error("Could not find or extract costura embedded resources");
        }

        private bool ExtractResources(out Dictionary<byte[], string> resources)
        {
            resources = new Dictionary<byte[], string>();

            if (_module.Resources.Count == 0)
                return false;

            foreach (var resource in _module.Resources)
            {
                if (!resource.IsEmbedded)
                    continue;

                string name = resource.Name;

                if (name.Length < 19)
                    continue;

                if (!name.StartsWith("costura.") || !name.EndsWith(".compressed"))
                    continue;

                // Strip costura. and .compressed from the resource name
                name = name.Substring(8, name.LastIndexOf(".compressed") - 8);

                byte[] data = resource.GetData();

                if (data != null)
                    resources.Add(data.Decompress(), name);

                Logger.Success($"Extracted costura resource {name}");
            }

            return resources.Count != 0;
        }

        private void SaveResources(Dictionary<byte[], string> extractedResources)
        {
            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);

            foreach (var entry in extractedResources)
            {
                string fullPath = Path.Combine(_outputPath, entry.Value);
                File.WriteAllBytes(fullPath, entry.Key);
            }

            Logger.Info($"Saved {extractedResources.Count} extracted Resources to {_outputPath}");
        }
    }
}