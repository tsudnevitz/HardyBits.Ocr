using HardyBits.Ocr.Engine.Common;

namespace HardyBits.Ocr.Engine.Configuration
{
  public interface IEngineConfiguration : IHaveType, IHaveParameters
  {
    string TessData { get; }
    int EngineMode { get; }
    string Language { get; }
  }
}