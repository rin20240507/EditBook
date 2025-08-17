using System.Diagnostics;

namespace EditBookLib.Join;

/// <summary>
/// 結合しないファイルをnoディレクトリに移動する
/// </summary>
public static class PreProcDir
{
  public static void Run(string dir)
  {
    var files = Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly)
      .Where(Function.IsImageFile)
      .ToList();
    if (files.Count == 0) return;
    
    var noDir = Path.Combine(dir, "no");
    
    if (!Directory.Exists(noDir)) Directory.CreateDirectory(noDir);

    // 表紙ファイルを移動する
    var moveFiles = files.Where(IsMoveFile).ToList();
    foreach (var file in moveFiles)
    {
      MoveNoDir(file, noDir);
    }
    
    // 本体ファイルの先頭と末尾を移動する
    var noMoveFiles = files.Where(f => !IsMoveFile(f)).ToList();
    MoveNoDir(noMoveFiles.First(), noDir);
    MoveNoDir(noMoveFiles.Last(), noDir);
  }

  private static bool IsMoveFile(string file)
  {
    return Path.GetFileName(file).StartsWith("img_", StringComparison.CurrentCultureIgnoreCase);
  }

  private static void MoveNoDir(string inFile, string outDir)
  {
    string outFile = Path.Combine(outDir, Path.GetFileName(inFile));
    
    Debug.WriteLine($"in: {inFile} out: {outFile}");
    
    File.Move(inFile, outFile);
  }
  
}