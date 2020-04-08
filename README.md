
# Simple Costura Decompressor
**Simple tool to extract and decompress embedded resources processed by Fody Costura**

## Usage
Using command line arguments:

    Costura-Decompressor.exe file1.exe file.dll.compressed
The tool supports executable files with costura processed resources and the compressed resources as single files which can be extracted using tools like dnSpy, ILSpy etc.

Output looks like this:
![example output](https://i.imgur.com/Fcl2EMi.png)

You can also just drag the file you want to decompress or extract and decompress resources from onto the executable.

## Dependencies
- dnlib using [nuget package v3.3.1](https://www.nuget.org/packages/dnlib/)
- Colorful.Console using [nuget package v1.2.10](https://www.nuget.org/packages/Colorful.Console/1.2.10)
- Costura using [nuget package v4.1.0](https://www.nuget.org/packages/Costura.Fody/4.1.0) (Yeah the Costura Decompressor is using Costura 👀)
