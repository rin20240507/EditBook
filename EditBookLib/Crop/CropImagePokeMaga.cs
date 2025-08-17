namespace EditBookLib.Crop;

public class CropImagePokeMaga : CropImageCapture
{
  public CropImagePokeMaga()
  {
    BaseColorCol = 100;
    CheckColStart = 100;
    CheckColEnd = 180;
    
    StartY = 6;
    EndY = 6;
  }

  protected override (int, int) CropLeft(int startX, int endX)
  {
    endX = endX - (endX - startX) / 2 - 1;
    return (startX, endX);
  }

  protected override (int, int) CropRight(int startX, int endX)
  {
    startX = startX + (endX - startX) / 2 + 1;
    return (startX, endX);
  }
}