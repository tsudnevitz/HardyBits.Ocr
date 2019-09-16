using HardyBits.Wrappers.Leptonica.Pix;

namespace HardyBits.Ocr.Engine.Preprocessing
{
  public interface IPreprocessor
  {
    IPix Run(IPix image);
  }
}