using HardyBits.Wrappers.Leptonica.Pix;

namespace HardyBits.Ocr.Engine
{
  public interface IPreprocessor
  {
    IPix Run(IPix image);
  }
}