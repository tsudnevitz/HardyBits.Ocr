using System;
using HardyBits.Wrappers.Tesseract.Enums;

namespace HardyBits.Wrappers.Tesseract.Factories
{
  public class TesseractEngineFactory : ITesseractEngineFactory
  {
    public IConfiguredTesseractEngineFactory CreateFactory(string dataPath, string language, EngineMode engineMode)
    {
      return new ConfiguredTesseractEngineFactory(() => Create(dataPath, language, engineMode));
    }

    public ITesseractEngine Create(string dataPath, string language, EngineMode engineMode)
    {
      if (dataPath == null)
        throw new ArgumentNullException(nameof(dataPath));

      if (language == null)
        throw new ArgumentNullException(nameof(language));

      return new TesseractEngine(dataPath, language, engineMode);
    }
  }
}