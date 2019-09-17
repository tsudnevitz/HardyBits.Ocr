using HardyBits.Wrappers.Leptonica.Internals;

namespace HardyBits.Ocr.Engine.Preprocessing
{
  public interface IPreprocessor
  {
    IPix Run(IPix image);
  }
}