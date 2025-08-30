namespace EditBookLib.Crop;

public struct FileInfo(string fileName)
{
  public readonly string FileName = fileName;
  public int StartX;
  public int EndX;
  public int Width;

  public bool IsDoublePage()
  {
    return (EndX - StartX) > (Width / 3 * 2);
    // return StartX < 300 || EndX > 1500;
  }
}