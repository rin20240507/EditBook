using System.Drawing;

namespace Png2Jpeg;

public static class SystemDrawing
{
  public static void Convert(string inFile, string outFile, long quality)
  {
    using var bmp = new Bitmap(inFile);
    //EncoderParameterオブジェクトを1つ格納できる
    //EncoderParametersクラスの新しいインスタンスを初期化
    //ここでは品質のみ指定するため1つだけ用意する
    System.Drawing.Imaging.EncoderParameters eps =
      new System.Drawing.Imaging.EncoderParameters(1);
    //品質を指定
    System.Drawing.Imaging.EncoderParameter ep =
      new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
    //EncoderParametersにセットする
    eps.Param[0] = ep;
    
    //イメージエンコーダに関する情報を取得する
    //GDI+ に組み込まれたイメージ エンコーダに関する情報をすべて取得
    System.Drawing.Imaging.ImageCodecInfo[] encs =
      System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
    System.Drawing.Imaging.ImageCodecInfo ici = encs.First(enc => enc.MimeType == "image/jpeg");
    
    bmp.Save(outFile, ici, eps);
    bmp.Dispose();
    File.Delete(inFile);
  }
}