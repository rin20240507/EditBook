using ImageMagick;

namespace EditBookLib;

public static class ResizeImage
{
  /// <summary>
  /// 縦サイズを基準にしてリサイズ
  /// </summary>
  /// <param name="img">img</param>
  /// <param name="resizedHeight">縦サイズ</param>
  public static void ResizeHeight(IMagickImage img, int resizedHeight)
  {
    if (resizedHeight > 0 && img.Height > resizedHeight)
    {
      IMagickGeometry geo = new MagickGeometry($"x{resizedHeight}");
      img.Resize(geo);
    }
  }
}