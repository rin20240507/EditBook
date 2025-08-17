// See https://aka.ms/new-console-template for more information

using EditBookLib.Join;

// ProcDir.JoinRun(inDir, args[1], Console.WriteLine);

var inDir = args[0];
int resizedHeight = 0;
if (args.Length > 2 && int.TryParse(args[2], out var buff))
{
  resizedHeight = buff;
}

ProcDir.JoinRunDir(inDir, args[1], resizedHeight, Console.WriteLine);


