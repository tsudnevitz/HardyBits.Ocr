using HardyBits.Ocr.Engine.Configuration;

namespace HardyBits.Ocr.Engine.Preprocessing
{
  public interface IPreprocessorFactory
  {
    IPreprocessor Create(IPreprocessorConfiguration config);
  }
}