using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using ImageMagick;

namespace EditBookLib.Join;

public static class JoinImage
{
  private const string WorkHeadName = "work";

  /// <summary>
  /// 画像結合
  /// </summary>
  /// <param name="inFile1">右側ファイル</param>
  /// <param name="inFile2">左側ファイル</param>
  /// <param name="outDir">出力ディレクトリ</param>
  /// <param name="resizedHeight">縦サイズ</param>
  /// <param name="logAction">ログ出力処理</param>
  public static void Run(string inFile1, string inFile2, string outDir, int resizedHeight, Action<string>? logAction)
  {
    logAction?.Invoke($"Path:{Path.GetDirectoryName(inFile1)} File1: {Path.GetFileName(inFile1)}, File2: {Path.GetFileName(inFile2)}");
    
    using var img1 = new MagickImage(inFile1);
    using var img2 = new MagickImage(inFile2);
    
    using IMagickImage<ushort> img = Join(img1, img2);
    ResizeImage.ResizeHeight(img, resizedHeight);

    string baseName = $"{WorkHeadName}_{Path.GetFileNameWithoutExtension(inFile1)}_{Path.GetFileNameWithoutExtension(inFile2)}";
    string basePath = Path.Combine(outDir, baseName);

    Function.SaveImageJpeg(img, basePath);

    // logAction?.Invoke($"outFile:join:{tempFile}");
  }

  public static IMagickImage<ushort> Join(MagickImage img1, MagickImage img2)
  {
    if (img1.Height > img2.Height)
    {
      // img2の方が小さいのでimg2の縦サイズをimg1と同じにする
      IMagickGeometry geo = new MagickGeometry($"x{img1.Height}");
      img2.Resize(geo);
    }
    else if (img1.Height < img2.Height)
    {
      // img1の方が小さいのでimg1の縦サイズをimg2と同じにする
      IMagickGeometry geo = new MagickGeometry($"x{img2.Height}");
      img1.Resize(geo);
    }
    

    var imgCollection = new MagickImageCollection();
    imgCollection.Add(img2);
    imgCollection.Add(img1);

    return imgCollection.AppendHorizontally();
  }

  /// <summary>
  /// 結合しないファイル処理(リサイズのみ)
  /// </summary>
  /// <param name="inFile">入力ファイル</param>
  /// <param name="outDir">出力ディレクトリ</param>
  /// <param name="resizedHeight">高さ</param>
  /// <param name="logAction">ログ出力処理</param>
  public static void RunNoJoin(string inFile, string outDir, int resizedHeight, Action<string>? logAction)
  {
    logAction?.Invoke($"NoInFile:{inFile}");
    
    using var img = new MagickImage(inFile);
    ResizeImage.ResizeHeight(img, resizedHeight);
    
    // var outFile = Path.Combine(outDir, $"{Path.GetFileNameWithoutExtension(inFile)}.{ExtName}");
    // logAction?.Invoke($"outFile:nojoin:{outFile}");
    // img.Write(outFile);
    string baseName = $"{WorkHeadName}_{Path.GetFileNameWithoutExtension(inFile)}";
    
    var basePath = Path.Combine(outDir, baseName);
    Function.SaveImageJpeg(img, basePath);
  }
  
}