using System;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Ocr.Engine.Extensions;
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

      return image
        .ChainOutput(x => leptonicaInterop.DeskewBoth(x, ReductionFactor), disposePreviousPix: false)
        .ChainOutput(x => leptonicaInterop.PrepareOneBitPerPixel(x))
        .ChainOutput(x => leptonicaInterop.OrientationCorrect(x, MinUpConfidence, MinRatio))
        .ChainOutput(x =>
        {
          using var box = leptonicaInterop.FindPageForeground(x, Threshold, MinDistance, EraseDistance);
          return leptonicaInterop.ClipRectangle(x, box);
        });
    }
  }
}