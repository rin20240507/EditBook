using System.Diagnostics;

namespace EditBookLib.Crop;

using ImageMagick;

public enum DirType { Left, Right, Normal, Split }

public abstract class CropImage
{
  protected int BaseColorCol = 3;
  protected int BaseColorRow = 3;
  protected int CheckColStart = 20;
  protected int CheckColEnd = 20;
  protected int StartY = 0;
  protected int EndY = 0;
  private int _startX = 0;
  private int _endX = 0;

  /// <summary>
  /// 切り抜き
  /// </summary>
  /// <param name="inFileName">入力ファイル</param>
  /// <param name="outFileName">出力ファイル</param>
  /// <param name="dirType">ディレクトリの処理タイプ</param>
  public void Crop(string inFileName, string outFileName, DirType dirType)
  {
    try
    {
      using var img = new MagickImage(inFileName);

      // 切り出すX軸を特定
      int startX, endX;
      if (_startX > 0 && _endX > 0)
      {
        // 既に設定済みの場合はその値を使用する
        startX = _startX;
        endX = _endX;
      }
      else
      {
        // それ以外は画像から取得する
        (startX, endX) = GetCropX(img);
      }

      if (dirType == DirType.Left)
      {
        (startX, endX) = CropLeft(startX, endX);
      }

      if (dirType == DirType.Right)
      {
        (startX, endX) = CropRight(startX, endX);
      }
          
      Console.WriteLine($"startX:{startX}:endX:{endX}");

      using var cloneImg = img.Clone();
      cloneImg.Crop(new MagickGeometry(startX, StartY
        , Convert.ToUInt32(endX - startX + 1)
        , Convert.ToUInt32(img.Height - StartY - EndY)));
      cloneImg.Write(outFileName);
      // Console.WriteLine($"leftX:{leftX}");
    }
    catch (ArgumentOutOfRangeException e)
    {
      Console.WriteLine(e.StackTrace);
      throw;
    }
  }

  public void SetCropX(int startX, int endX)
  {
    _startX = startX;
    _endX = endX;
  }

  /// <summary>
  /// 画像のX軸の開始、終了位置を特定
  /// </summary>
  /// <param name="fileName">ファイル名</param>
  /// <returns>(開始位置, 終了位置, 幅)</returns>
  public (int, int, int) GetCropX(string fileName)
  {
    try
    {
      using var img = new MagickImage(fileName);
      (int startX, int endX) = GetCropX(img);
      int width = (int) img.Width;

      return (startX, endX, width);
    }
    catch (ArgumentOutOfRangeException e)
    {
      Console.WriteLine(e.StackTrace);
      throw;
    }
  }

  /// <summary>
  /// 画像のX軸の開始、終了位置を特定
  /// </summary>
  /// <param name="img">画像オブジェクト</param>
  /// <returns>(開始位置, 終了位置)</returns>
  private (int, int) GetCropX(MagickImage img)
  {
    using var pixels = img.GetPixels();
    // 比較の色を取得
    var baseColor = pixels[BaseColorCol, BaseColorRow]?.ToColor();
    
    // 全 Y 軸
    var ySeq = Enumerable.Range(1, (int) img.Height - 1);
    var posList = ySeq
      .Where(i => i % 100 == 0)
      .Select(posY => GetCropXOneLine(pixels, baseColor, (int) img.Width, posY))
      .ToList();
        
    // posList.ForEach(pos =>
    // {
    //   Console.WriteLine($"{pos.Item1}:{pos.Item2}");
    // });

    var startX = posList.Select(t => t.Item1).Where(x=>  x >= 0).Min();
    var endX = posList.Select(t => t.Item2).Where(x => x >= 0).Max();
    
    return (startX, endX);
  }

  /// <summary>
  /// 画像のX軸の開始、終了位置を特定(一行)
  /// </summary>
  /// <param name="pixels"></param>
  /// <param name="baseColor">比較色</param>
  /// <param name="width"></param>
  /// <param name="posY">Y軸</param>
  /// <returns>(開始位置, 終了位置)</returns>
  protected (int, int) GetCropXOneLine(IPixelCollection<ushort> pixels, IMagickColor<ushort>? baseColor, int width, int posY)
  {
    // 全 X 軸
    var xSeq = Enumerable.Range(CheckColStart, width - CheckColEnd);
    
    // 開始位置の特定
    int startX;
    try
    {
      // 色が異なる最初のXを取得
      startX = xSeq.First(i => !CheckColorY(i));
      // Console.WriteLine($"startX:{startX}");
    }
    catch (InvalidOperationException)
    {
      startX = -1;
    }

    // 終了位置の特定
    int endX;
    try
    {
      // 色が異なる最後のXを取得
      endX = xSeq.Reverse().First(i => !CheckColorY(i));
      // Console.WriteLine($"endX:{endX}");
    }
    catch (InvalidOperationException)
    {
      endX = -1;
    }
    
    Debug.WriteLine($"Y:{posY} startX:{startX}:endX:{endX}");

    return (startX, endX);

    // 比較用ファンクション
    bool CheckColorY(int posX) => EqualsColor(pixels, baseColor, posX, posY);
  }

  protected virtual (int, int) CropLeft(int startX, int endX)
  {
    int leftEndX = endX - (endX - startX) / 2;
    return (startX, leftEndX);
  }

  protected virtual (int, int) CropRight(int startX, int endX)
  {
    int rightStartX = startX + (endX - startX) / 2;
    return (rightStartX, endX);
  }

  protected abstract bool EqualsColor(IPixelCollection<ushort> pixels, IMagickColor<ushort>? baseColor, int posX, int posY);
}