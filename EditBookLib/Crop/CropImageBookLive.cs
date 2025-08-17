using ImageMagick;

namespace EditBookLib.Crop;

public class CropImageBookLive : CropImage
{
  protected override bool EqualsColor(IPixelCollection<ushort> pixels, IMagickColor<ushort>? baseColor, int posX, int posY)
  {
    var color = pixels[posX, posY]?.ToColor();
    var ret = color is
    {
      R: >= 60652 and <= 62965,
      G: >= 60652 and <= 62708,
      B: >= 60652 and <= 62194
    };

    // if (!ret)
    // {
    //   Console.WriteLine($"X:{posX} Y:{posY} R:{color.R}, G:{color.G}, B:{color.B}");
    // }
    return ret;
  }
}