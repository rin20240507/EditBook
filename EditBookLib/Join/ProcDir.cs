using ImageMagick;

namespace EditBookLib.Join;

public static class ProcDir
{
  /// <summary>
  /// 結合前処理
  /// </summary>
  /// <param name="dir">処理ディレクトリ</param>
  public static void RunPreDir(string dir)
  {
    var dirList = Directory.GetDirectories(dir);
    
    foreach (var procDir in dirList)
    {
      PreProcDir.Run(procDir);
    }
  }

  /// <summary>
  /// 結合処理(以下のディレクトリ全てを処理)
  /// </summary>
  /// <param name="inDir">入力ディレクトリ</param>
  /// <param name="outPath">出力ディレクトリ</param>
  /// <param name="resizedHeight">リサイズ</param>
  /// <param name="logAction">ログ処理</param>
  public static void JoinRunDir(string inDir, string outPath, int resizedHeight, Action<string>? logAction = null)
  {
    var dirList = Directory.GetDirectories(inDir);
    
    foreach (var procDir in dirList)
    {
      JoinRun(procDir, outPath, resizedHeight, logAction);
    }
    
    logAction?.Invoke($"終了");
  }

  /// <summary>
  /// 結合処理(出力ディレクトリ作成など)
  /// </summary>
  /// <param name="inDir">入力ディレクトリ</param>
  /// <param name="outPath">出力ディレクトリ</param>
  /// <param name="resizedHeight">リサイズ</param>
  /// <param name="logAction">ログ処理</param>
  public static void JoinRun(string inDir, string outPath, int resizedHeight, Action<string>? logAction)
  {
    logAction?.Invoke($"inDir:{inDir}");
    // 出力先を特定 ＆ 作成
    var outDir = Path.Combine(outPath, Path.GetFileName(inDir));
    Directory.CreateDirectory(outDir);

    // 個別
    JoinFiles(inDir, outDir, resizedHeight, logAction);
    // 結合無し
    NoJoinFiles(inDir, outDir, resizedHeight, logAction);
    
    // リネーム
    RenameFiles(outDir, logAction);
  }

  /// <summary>
  /// 結合処理(メイン)
  /// </summary>
  /// <param name="inDir"></param>
  /// <param name="outDir"></param>
  /// <param name="resizedHeight">リサイズ</param>
  /// <param name="logAction">ログ処理</param>
  /// <exception cref="JoinException"></exception>
  private static void JoinFiles(string inDir, string outDir, int resizedHeight, Action<string>? logAction)
  {
    // ファイル一覧を取得
    var files = Directory.EnumerateFiles(inDir, "*", SearchOption.TopDirectoryOnly)
      .Where(Function.IsImageFile).ToList();
    
    if (files.Count == 0) return;
    
    // 結合処理
    var procFiles = files.Where(IsProcImageFile).ToList();
    JoinFilesJoin(procFiles, outDir, resizedHeight, logAction);
    
    
    // 非結合処理
    var noProfFiles = files.Where(f => !Function.IsImageFile(f)).ToList();
    noProfFiles.AsParallel()
      .WithDegreeOfParallelism(4)
      .ForAll(f => JoinImage.RunNoJoin(f, outDir, resizedHeight, logAction));
  }

  /// <summary>
  /// 処理するファイルかチェックする
  /// ※横より縦の方が長いファイルのみ処理する
  /// </summary>
  /// <param name="file">ファイル名</param>
  /// <returns></returns>
  public static bool IsProcImageFile(string file)
  {
    using var img = new MagickImage(file);
    
    return img.Width < img.Height;
  }

  private static void JoinFilesJoin(List<string> files, string outDir, int resizedHeight, Action<string>? logAction)
  {
    var files1 = files.Where((file, index) => index % 2 == 0).ToList();
    var files2 = files.Where((file, index) => index % 2 == 1).ToList();
    
    if (files1.Count != files2.Count) throw new JoinException("ファイル数が奇数です。");
    
    var pairs = new List<(string, string)>();
    
    for (var i = 0; i < files1.Count; i++)
    {
      var file1 = files1[i];
      var file2 = files2[i];
      
      pairs.Add((file1, file2));
      // JoinImage.Proc(file1, file2, outDir);
    }
    
    pairs.AsParallel()
      .WithDegreeOfParallelism(4)
      .ForAll(t => JoinImage.Run(t.Item1, t.Item2, outDir, resizedHeight, logAction));
    
  }

  /// <summary>
  /// 結合しないファイル群を処理
  /// </summary>
  /// <param name="inDir"></param>
  /// <param name="outDir"></param>
  /// <param name="resizedHeight">リサイズ</param>
  /// <param name="logAction"></param>
  private static void NoJoinFiles(string inDir, string outDir, int resizedHeight, Action<string>? logAction)
  {
    // ファイル一覧を取得
    var files = Directory.EnumerateFiles(Path.Combine(inDir, "no"), "*", SearchOption.TopDirectoryOnly)
      .Where(Function.IsImageFile).ToList();
    if (files.Count == 0) return;
    
    var noProfFiles = files.Where(Function.IsImageFile).ToList();
    noProfFiles.AsParallel()
      .WithDegreeOfParallelism(4)
      .ForAll(f => JoinImage.RunNoJoin(f, outDir, resizedHeight, logAction));
  }

  /// <summary>
  /// 連番に名前変更
  /// </summary>
  /// <param name="dir"></param>
  /// <param name="logAction"></param>
  private static void RenameFiles(string dir, Action<string>? logAction)
  {
    var files = Directory.EnumerateFiles(dir).ToList();

    files.Sort();

    int num = 0;
    foreach (var file in files)
    {
      num++;
      var ext = Path.GetExtension(file);
      var outFileName = Path.Combine(dir, $"{num:0000}{ext}");
      
      File.Move(file, outFileName);
    }
  }
}