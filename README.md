# Simple Costura Decompressor
**Simple tool to extract and decompress embedded resources processed by Fody Costura**

## Usage
Using command line arguments:

    Costura-Decompressor.exe file1.exe file.dll.compressed
The tool supports executable files with costura processed resources and the compressed resources as single files which can be extracted using tools like dnSpy, ILSpy etc.

You can also just drap the file you want to decompress or extract and decompress resources from onto the executable.

## Dependencies
- [dnlib](https://github.com/0xd4d/dnlib) using [nuget package v3.3.1](https://www.nuget.org/packages/dnlib/)
