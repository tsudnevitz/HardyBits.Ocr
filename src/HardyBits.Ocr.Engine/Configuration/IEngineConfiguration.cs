using HardyBits.Ocr.Engine.Common;
using HardyBits.Wrappers.Tesseract.Enums;

namespace HardyBits.Ocr.Engine.Configuration
{
  public interface IEngineConfiguration : IHaveType, IHaveParameters
  {
    string TessData { get; }
    EngineMode EngineMode { get; }
    string Language { get; }
  }
}