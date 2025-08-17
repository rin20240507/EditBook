using ImageMagick;

namespace EditBookLib;

public static class Function
{
  private const string ExtName = "jpg";
  private const string TempExtName = "png";
  
  /// <summary>
  /// 画像ファイルかチェックする
  /// </summary>
  /// <param name="file">ファイル名</param>
  /// <returns></returns>
  public static bool IsImageFile(string file)
  {
    string ext = Path.GetExtension(file).ToLower();
    return ext == ".png" || ext == ".jpg" || ext == ".bmp";
  }

  public static void SaveImageJpeg(IMagickImage img, string basePath)
  {
    var tempFile = $"{basePath}.{TempExtName}";

    img.Write(tempFile);
    
    var outFile = $"{basePath}.{ExtName}";
    
    Png2Jpeg.SystemDrawing.Convert(tempFile, outFile, 85);
  }
}