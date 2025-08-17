namespace EditBookLib.Crop;

using System.IO;

public enum ProcType { Kindle, BookLive, PokeMaga, Capture, ScanBody }


public class ProcDir
{
  public int ThreadCount { private get; init; } = 1;
  public bool IsJpeg { private get; init; }
  public bool IsAllCheck { private get; init; }
  /// <summary>
  /// 処理 b:BookLive k:Kindle その他:一般
  /// </summary>
  public ProcType ProcType { private get; init; }
  public Action<string>? WriteLog { private get; init; }

  // public bool IsMultiDir { private get; init; } = false;

  public void Proc(string inDir, string outDir, bool isSingle)
  {
    if (isSingle)
    {
      ProcOne(inDir, outDir);
    }
    else
    {
      MultiProc(inDir, outDir);
    }
  }

  private void MultiProc(string inDir, string outDir)
  {
    string[] directories = Directory.GetDirectories(inDir);
    
    foreach (var dir in directories)
    {
      ProcOne(dir, outDir);
    }
  }

  /// <summary>
  /// 1冊分を処理する
  /// </summary>
  /// <param name="inDir">入力ディレクトリ</param>
  /// <param name="outPath">出力先ディレクトリ</param>
  private void ProcOne(string inDir, string outPath)
  {
    // 出力先を特定 ＆ 作成
    var outDir = Path.Combine(outPath, Path.GetFileName(inDir));
    // if (Directory.Exists(outDir))
    // {
    //   Console.WriteLine(Properties.Resources.ResourceManager.GetString("existsDir") ?? string.Empty, outDir);
    // }
    Directory.CreateDirectory(outDir);
    
    // メイン処理
    if (IsAllCheck)
    {
      // サイズを統一
      ProcFilesAllCheck(inDir, outDir);
    }
    else
    {
      // 個別
      ProcFiles(inDir, outDir);
    }
  }

  /// <summary>
  /// 1フォルダ分を処理する(従来)
  /// </summary>
  /// <param name="inDir">入力ディレクトリ</param>
  /// <param name="outDir">出力ディレクトリ</param>
  /// <param name="dirType">入力ディレクトリタイプ</param>
  private void ProcFiles(string inDir, string outDir, DirType dirType = DirType.Normal)
  {
    CropImage img = MakeCropImage();
    
    // ファイル一覧を取得
    var files = Directory.EnumerateFiles(inDir, "*", SearchOption.TopDirectoryOnly).ToList();

    files.AsParallel()
      .WithDegreeOfParallelism(ThreadCount)
      .ForAll(file => Cut(img, file, outDir, dirType));
  }

  /// <summary>
  /// 1フォルダ分を処理する(1ファイルずつチェック)
  /// </summary>
  /// <param name="inDir">入力ディレクトリ</param>
  /// <param name="outDir">出力ディレクトリ</param>
  private void ProcFilesAllCheck(string inDir, string outDir)
  {
    CropImage img = MakeCropImage();
    
    // ファイル一覧を取得
    var files = Directory.EnumerateFiles(inDir, "*", SearchOption.TopDirectoryOnly).ToList();
    
    List<FileInfo> fileInfos =
    files.AsParallel().WithDegreeOfParallelism(ThreadCount)
      .Select(file =>
      {
        var info = new FileInfo(file);
        (info.StartX, info.EndX) = img.GetCropX(file);
        return info;
      }).ToList();
    // 単ページと見開きページに分ける
    var group = fileInfos.GroupBy(info => info.IsDoublePage());
    
    // それぞれに処理
    foreach (IGrouping<bool, FileInfo> grouping in group)
    {
      Console.WriteLine(grouping.Key ? "multi page" : "single page");

      List<FileInfo> procFileInfos = grouping.ToList();
      

      ProcFilesAllCheckOne(procFileInfos, outDir);
    }
    
    var dirs = Directory.EnumerateDirectories(inDir, "*", SearchOption.TopDirectoryOnly).ToList();
    foreach (string dir in dirs)
    {
      Console.WriteLine(dir);
      DirType dirType = Path.GetFileName(dir) switch
      {
        "l" => DirType.Left,
        "r" => DirType.Right,
        _ => DirType.Normal
      };
      
      WriteLog?.Invoke(dir);
      ProcFiles(dir, outDir, dirType);
    }
  }
  
  /// <summary>
  /// 1フォルダ分を処理する(1フォルダ内で同じサイズで切り出す)
  /// </summary>
  /// <param name="fileInfos">入力ファイル</param>
  /// <param name="outDir">出力ディレクトリ</param>
  private void ProcFilesAllCheckOne(List<FileInfo> fileInfos, string outDir)
  {
    CropImage img = MakeCropImage();

    // 全ファイルを精査して、開始位置と終了位置を特定する
    var g = fileInfos
      .Select(info => (info.StartX, info.EndX))
      .GroupBy(pos => pos)
      .OrderByDescending(g => g.Count())
      .First()
      ;
    // 半数以上でなければ対象外とする
    if (g.Count() > fileInfos.Count / 2)
    {
      (int startX, int endX) = g.Key;
      img.SetCropX(startX, endX);
    }
      
    // カット処理
    fileInfos.AsParallel()
      .WithDegreeOfParallelism(ThreadCount)
      .ForAll(info => Cut(img, info.FileName, outDir));
  }

  /// <summary>
  /// カット処理
  /// </summary>
  /// <param name="img">画像処理オブジェクト</param>
  /// <param name="inFileName">入力ファイル名</param>
  /// <param name="outDir">出力ディレクトリ</param>
  /// <param name="dirType">入力ディレクトリタイプ</param>
  private void Cut(CropImage img, string inFileName, string outDir, DirType dirType = DirType.Normal)
  {
    // var outFileName = $"{outDir}\\{Path.GetFileName(inFileName)}";
    string outFileName = Path.Combine(outDir, MakeOutFileName(inFileName));
    
    Console.WriteLine($"{inFileName} -> {outFileName}");
    WriteLog?.Invoke($"{inFileName} -> {outFileName}");
    // カット
    img.Crop(inFileName, outFileName, dirType);
  }

  /// <summary>
  /// カットオブジェクトを生成
  /// </summary>
  /// <returns>カットオブジェクト</returns>
  private CropImage MakeCropImage()
  {
    // 処理を特定
    CropImage img = ProcType switch
    {
      ProcType.BookLive => new CropImageBookLive(),
      ProcType.Kindle => new CropImageKindle(),
      ProcType.PokeMaga => new CropImagePokeMaga(),
      ProcType.ScanBody => new CropImageScanBody(),
      _ => new CropImageCapture()
    };

    return img;
  }

  /// <summary>
  /// 出力ファイル名の生成
  /// </summary>
  /// <param name="inFileName">入力ファイル名</param>
  /// <returns>出力ファイル名</returns>
  private string MakeOutFileName(string inFileName)
  {
    if (IsJpeg)
    {
        // JPEGに変換
      return Path.GetFileNameWithoutExtension(inFileName) + ".jpg";
    }
    else
    {
        // そのまま
      return Path.GetFileName(inFileName);
    }
  }
}