using HardyBits.Wrappers.Tesseract.Enums;

namespace HardyBits.Wrappers.Tesseract.Factories
{
  public interface ITesseractEngineFactory
  {
    IConfiguredTesseractEngineFactory CreateFactory(string dataPath, string language, EngineMode engineMode);
    ITesseractEngine Create(string dataPath, string language, EngineMode engineMode);
  }
}