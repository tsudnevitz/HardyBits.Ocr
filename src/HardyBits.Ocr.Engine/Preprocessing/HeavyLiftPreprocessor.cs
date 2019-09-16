using System;
using HardyBits.Wrappers.Leptonica.Enums;
using HardyBits.Wrappers.Leptonica.Internals;
using HardyBits.Wrappers.Leptonica.Interop;

namespace HardyBits.Ocr.Engine.Preprocessing
{
  public class HeavyLiftPreprocessor : IPreprocessor
  {
    private readonly ILeptonicaInterop _leptonicaInterop;

    public HeavyLiftPreprocessor(ILeptonicaInterop leptonicaInterop)
    {
      _leptonicaInterop = leptonicaInterop ?? throw new ArgumentNullException(nameof(leptonicaInterop));
    }

    public string Type { get; } = "HeavyLift";

    public IPix Run(IPix image)
    {
      if (image == null)
        throw new ArgumentNullException(nameof(image));

      using var deskewedPix = _leptonicaInterop.DeskewBoth(image, DeskewReductionFactor.Default);
      using var oneBitPix = _leptonicaInterop.PrepareOneBitPerPixel(deskewedPix);
      using var correctedPix = _leptonicaInterop.OrientationCorrect(oneBitPix);
      using var foregroundBox = _leptonicaInterop.FindPageForeground(correctedPix);
      return _leptonicaInterop.ClipRectangle(correctedPix, foregroundBox);
    }
  }
}