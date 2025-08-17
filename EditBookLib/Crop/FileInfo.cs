namespace EditBookLib.Crop;

public struct FileInfo(string fileName)
{
  public readonly string FileName = fileName;
  public int StartX;
  public int EndX;

  public bool IsDoublePage()
  {
    return StartX < 300 || EndX > 1500;
  }
}