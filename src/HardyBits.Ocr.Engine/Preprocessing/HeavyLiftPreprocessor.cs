using System;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Wrappers.Leptonica.Enums;
using HardyBits.Wrappers.Leptonica.Internals;
using HardyBits.Wrappers.Leptonica.Interop;

namespace HardyBits.Ocr.Engine.Preprocessing
{
  public class HeavyLiftPreprocessor : PreprocessorBase<PreprocessorManifest<HeavyLiftPreprocessor>>
  {
    public HeavyLiftPreprocessor(ILeptonicaInterop leptonicaInterop, IParameterCollection parameters)
      : base(leptonicaInterop, parameters)
    {
    }

    private DeskewReductionFactor ReductionFactor => GetParameterOrDefault(DeskewReductionFactor.Default);
    private float MinUpConfidence => GetParameterOrDefault(4f);
    private float MinRatio => GetParameterOrDefault(2.5f);
    private int Threshold => GetParameterOrDefault(128);
    private int MinDistance => GetParameterOrDefault(50);
    private int EraseDistance => GetParameterOrDefault(70);

    public override IPix Run(IPix image)
    {
      if (image == null)
        throw new ArgumentNullException(nameof(image));

      using var deskewedPix = leptonicaInterop.DeskewBoth(image, ReductionFactor);
      using var oneBitPix = leptonicaInterop.PrepareOneBitPerPixel(deskewedPix);
      using var correctedPix = leptonicaInterop.OrientationCorrect(oneBitPix, MinUpConfidence, MinRatio);
      using var foregroundBox = leptonicaInterop.FindPageForeground(correctedPix, Threshold, MinDistance, EraseDistance);
      return leptonicaInterop.ClipRectangle(correctedPix, foregroundBox);
    }
  }
}