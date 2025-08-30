using EditBookLib.Crop;
using EditBookLib.Join;

namespace TestUnit;

public class UnitTest1
{
  [Fact]
  public void TestJoin()
  {
    string inFile1 = @"X:\scan\01\[クロックドロー] 今日もホシノがかわいい！2\S_20250320_0046.bmp";
    string inFile2 = @"X:\scan\01\[クロックドロー] 今日もホシノがかわいい！2\S_20250320_0047.bmp";
    string outDir = @"Z:\WORK";
    int resizedHeight = 1600;
    
    JoinImage.Run(inFile1, inFile2, outDir, resizedHeight, Console.WriteLine);
  }

  [Fact]
  public void TestNoJoin()
  {
    string inFile1 = @"X:\scan\01\[クロックドロー] 今日もホシノがかわいい！2\S_20250320_0045.bmp";
    string outDir = @"Z:\WORK";
    int resizedHeight = 1600;
    
    JoinImage.RunNoJoin(inFile1, outDir, resizedHeight, Console.WriteLine);
  }

  [Fact]
  public void TestCropFanza()
  {
    const string inFile = @"X:\Prg\sample\fanza_2-002.png";
    const string outFile = @"Z:\WORK\002.jpg";

    CropImage img = new CropImageFanza4K();
    img.Crop(inFile, outFile, DirType.Normal);
  }
}