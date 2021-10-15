
# Simple Costura Decompressor
**Simple tool to extract and decompress embedded resources processed by Fody Costura**

## Usage
Using command line arguments:

    Costura-Decompressor.exe file1.exe file.dll.compressed
The tool supports executable files with costura processed resources and the compressed resources as single files which can be extracted using tools like dnSpy, ILSpy etc.

Output looks like this:
![example output](https://i.imgur.com/0bOrPqe.png)

You can also just drag the file you want to decompress or extract and decompress resources from onto the executable.

## Dependencies
- AsmResolver
- Colorful.Console
