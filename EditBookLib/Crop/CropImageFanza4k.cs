using ImageMagick;

namespace EditBookLib.Crop;

public class CropImageFanza4K : CropImage
{
  public CropImageFanza4K()
  {
    BaseColorCol = 50;
    CheckColStart = 50;
    CheckColEnd = 50;
  }

  protected override bool EqualsColor(IPixelCollection<ushort> pixels, IMagickColor<ushort>? baseColor, int posX, int posY)
  {
    // Console.WriteLine($"EqualsColor x:{posX},y:{posY}");
    var color = pixels[posX, posY]?.ToColor();
    return color != null && color.Equals(baseColor);
    // return !(color.R.Equals(0) && color.G.Equals(0) && color.B.Equals(0));
  }
  
}