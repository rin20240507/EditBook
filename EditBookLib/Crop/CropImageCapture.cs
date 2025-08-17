using ImageMagick;

namespace EditBookLib.Crop;

public class CropImageCapture : CropImage
{
  public CropImageCapture()
  {
    BaseColorCol = 100;
    CheckColStart = 100;
    CheckColEnd = 180;

    StartY = 1;
    EndY = 1;
  }

  protected override bool EqualsColor(IPixelCollection<ushort> pixels, IMagickColor<ushort>? baseColor, int posX, int posY)
  {
    // Console.WriteLine($"EqualsColor x:{posX},y:{posY}");
    var color = pixels[posX, posY]?.ToColor();
    return color != null && color.Equals(baseColor);
    // return !(color.R.Equals(0) && color.G.Equals(0) && color.B.Equals(0));
  }
}