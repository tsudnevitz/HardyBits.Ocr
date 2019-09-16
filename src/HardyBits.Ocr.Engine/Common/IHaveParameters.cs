using HardyBits.Ocr.Engine.Configuration;

namespace HardyBits.Ocr.Engine.Common
{
  public interface IHaveParameters
  {
    IParameterCollection Parameters { get; }
  }
}