using System.Diagnostics;
using EditBookLib;
using ImageMagick;

namespace ResizeWallPaper;

public static class ProcDir
{
  public static void RunDir(string inDir, string outDir, int resizedHeight, Action<string>? logAction = null)
  {
    var dirList = Directory.GetDirectories(inDir).ToList();

    if (!Directory.Exists(outDir))
    {
      Directory.CreateDirectory(outDir);
    }
    dirList.ForEach(ProcDir);


    var fileList = Directory.EnumerateFiles(inDir, "*", SearchOption.TopDirectoryOnly)
      .Where(Function.IsImageFile).ToList();
    fileList.AsParallel()
      .WithDegreeOfParallelism(4)
      .ForAll(ProcFile);
    
    return;

    void ProcDir(string procInDir)
    {
      string proc = Path.GetFileNameWithoutExtension(procInDir);
      string procOutDir = Path.Combine(outDir, proc);
      logAction?.Invoke($"in:{procInDir} out:{procOutDir}");
      
      RunDir(procInDir, procOutDir, resizedHeight, logAction);
    }

    void ProcFile(string file)
    {
      logAction?.Invoke($"file:{file}");
      ResizeImageFile(file, outDir, resizedHeight);
    }
  }

  private static void ResizeImageFile(string inFile, string outDir, int resizedHeight)
  {
    using var img = new MagickImage(inFile);
    
    string baseName = Path.GetFileNameWithoutExtension(inFile);
    string outFile = Path.Combine(outDir, baseName);
    
    ResizeImage.ResizeHeight(img, resizedHeight);
    
    // img.Write(outFile);
    Function.SaveImageJpeg(img, outFile);
  }
}