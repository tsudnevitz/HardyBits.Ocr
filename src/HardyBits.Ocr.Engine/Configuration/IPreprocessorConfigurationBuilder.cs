namespace HardyBits.Ocr.Engine.Configuration
{
  public interface IPreprocessorConfigurationBuilder
  {
    IPreprocessorConfigurationBuilder SetParameter<T>(string paramName, T paramValue);
    IPreprocessorConfigurationBuilder SetParameters(object anonymousObject);
  }
}