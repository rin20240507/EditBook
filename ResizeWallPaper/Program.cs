// See https://aka.ms/new-console-template for more information

// Console.WriteLine("Hello, World!");

using ResizeWallPaper;

var inDir = args[0];
var outDir = args[1];
int resizedHeight = 0;
if (args.Length > 2 && int.TryParse(args[2], out var buff))
{
  resizedHeight = buff;
}

ProcDir.RunDir(inDir, outDir, resizedHeight, Console.WriteLine);