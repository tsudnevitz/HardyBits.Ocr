using HardyBits.Wrappers.Leptonica.Pix;

namespace HardyBits.Ocr.Engine.Preporcessing
{
  public interface IPreprocessor
  {
    IPix Run(IPix image);
  }
}