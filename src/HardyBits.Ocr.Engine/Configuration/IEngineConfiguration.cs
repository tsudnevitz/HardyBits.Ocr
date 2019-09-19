using HardyBits.Ocr.Engine.Common;

namespace HardyBits.Ocr.Engine.Configuration
{
  public interface IEngineConfiguration
  {
    string TessData { get; }
    int EngineMode { get; }
    string Language { get; }
  }
}