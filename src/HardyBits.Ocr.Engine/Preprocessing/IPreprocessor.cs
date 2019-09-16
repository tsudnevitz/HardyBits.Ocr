using HardyBits.Ocr.Engine.Common;
using HardyBits.Wrappers.Leptonica.Internals;

namespace HardyBits.Ocr.Engine.Preprocessing
{
  public interface IPreprocessor : IHaveType
  {
    IPix Run(IPix image);
  }
}