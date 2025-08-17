using System.Diagnostics;
using ImageMagick;

namespace EditBookLib.Crop;

public class CropImageScanBody : CropImageCapture
{
  public CropImageScanBody()
  {
    BaseColorCol = 0;
    CheckColStart = 0;
    CheckColEnd = 0;
  }

  protected override bool EqualsColor(IPixelCollection<ushort> pixels, IMagickColor<ushort>? baseColor, int posX, int posY)
  {
    // Debug.WriteLine($"baseColor: r: {baseColor.R}, g: {baseColor.G}, b: {baseColor.B}");
    
    const int margin = 8196;
    const int baseNum = 49000;
    const int baseLow = baseNum - margin;
    const int baseHigh = baseNum + margin;
    var color = pixels[posX, posY]?.ToColor();
    var ret = color is
    {
      R: >= baseLow and <= baseHigh,
      G: >= baseLow and <= baseHigh,
      B: >= baseLow and <= baseHigh
    };
    if (posY == 2600)
    {
      Debug.WriteLine($"X: {posX},Y:{posY},ret:{ret} color: r: {color.R}, g: {color.G}, b: {color.B}");
    }
    return ret;
    
    // if (baseColor == null) return true;
    //
    // Debug.WriteLine($"baseColor: r: {baseColor.R}, g: {baseColor.G}, b: {baseColor.B}");
    //
    // const int margin = 8196;
    // var color = pixels[posX, posY]?.ToColor();
    // if (color == null) return true;
    //
    // var rLow = baseColor.R - margin;
    // var gLow = baseColor.G - margin;
    // var bLow = baseColor.B - margin;
    // var rHigh = baseColor.R + margin;
    // var gHigh = baseColor.G + margin;
    // var bHigh = baseColor.B + margin;
    //
    // return (color.R >= rLow && color.R <= rHigh) &&
    //        (color.G >= gLow && color.G <= gHigh) &&
    //        (color.B >= bLow && color.B <= bHigh);

    // if (!ret)
    // {
    //   Console.WriteLine($"X:{posX} Y:{posY} R:{color.R}, G:{color.G}, B:{color.B}");
    // }
  }
  
}