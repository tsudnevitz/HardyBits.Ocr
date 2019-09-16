using System;

namespace HardyBits.Wrappers.Tesseract.Factories
{
  internal class ConfiguredTesseractEngineFactory : IConfiguredTesseractEngineFactory
  {
    private readonly Func<ITesseractEngine> _delegate;

    public ConfiguredTesseractEngineFactory(Func<ITesseractEngine> @delegate)
    {
      _delegate = @delegate ?? throw new ArgumentNullException(nameof(@delegate));
    }

    public ITesseractEngine Create()
    {
      return _delegate();
    }
  }
}