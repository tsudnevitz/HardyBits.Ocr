using System;
using HardyBits.Wrappers.Leptonica.Pix;

namespace HardyBits.Wrappers.Leptonica.Filters
{
  public class DeskewFilterResult : IFilterResult
  {
    public DeskewFilterResult(IPix pix, float angle, float confidence)
    {
      Pix = pix ?? throw new ArgumentNullException(nameof(pix));
      Angle = angle;
      Confidence = confidence;
    }

    public IPix Pix { get; }
    public float Angle { get; }
    public float Confidence { get; }
  }
}