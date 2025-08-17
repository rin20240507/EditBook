using EditBookLib.Crop;
using Microsoft.Extensions.Configuration;

namespace CropBook;

// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

internal static class Program
{
  public static void Main(string[] args)
  {
    if (args.Length < 3)
    {
      Console.WriteLine(Properties.Resources.ResourceManager.GetString("argsMessage"));
      Environment.Exit(0);
    }

    // IConfiguration config = new ConfigurationBuilder();
    var config = new ConfigurationBuilder()
      .AddJsonFile("AppSettings.json")
      .Build();

    var settings = config.Get<AppSettings>();

    int thread = 1;
    // Console.WriteLine(config.GetValue<in>()
    if (settings != null)
    {
      thread = settings.ThreadCount;
    }

    var argsValue = new Args(args);

    var sw = Stopwatch.StartNew();
    
    ProcType procType = argsValue.ProcType switch
    {
      "k" => ProcType.Kindle,
      "b" => ProcType.BookLive,
      "p" => ProcType.PokeMaga,
      "s" => ProcType.ScanBody,
      _ => ProcType.Capture
    };
    
    var cls = new ProcDir
    {
      ThreadCount = thread,
      IsJpeg = argsValue.IsJpeg,
      ProcType = procType,
      IsAllCheck = argsValue.IsAllCheck,
      WriteLog = WriteLog
    };
    cls.Proc(argsValue.InDir, argsValue.OutDir, argsValue.IsProcSingle);
    
    Console.WriteLine(sw.Elapsed);
  }

  private static void WriteLog(string log)
  {
    Console.WriteLine(log);
  }

  private class Args
  {
    public string InDir { get; }
    public string OutDir { get; }
    public string ProcType { get; }
    private int IsMultiDir { get; } = 0;
    public bool IsJpeg { get; } = false;
    public bool IsAllCheck { get; } = false;

    public bool IsProcSingle => IsMultiDir == 0;

    public Args(string[] args)
    {
      InDir = args[0];
      OutDir = args[1];
      ProcType = args[2];

      if (args.Length <= 3) return;
      
      for (var li = 3; li < args.Length; li++)
      {
        switch (args[li])
        {
          case "m":
            IsMultiDir = 1;
            break;
          case "j":
            IsJpeg = true;
            break;
          case "a":
            IsAllCheck = true;
            break;
        }
      }
    }
  }
}