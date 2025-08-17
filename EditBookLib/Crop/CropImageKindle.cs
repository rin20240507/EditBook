namespace EditBookLib.Crop;

public class CropImageKindle : CropImageCapture
{
  public CropImageKindle()
  {
    BaseColorCol = 100;
    CheckColStart = 100;
    CheckColEnd = 180;

    StartY = 1;
    EndY = 1;
  }
}