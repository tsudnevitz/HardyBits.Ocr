using HardyBits.Wrappers.Leptonica.Pix;

namespace HardyBits.Wrappers.Leptonica.Filters
{
  public class FixOrientationResult : IFilterResult
  {
    public FixOrientationResult(IPix pix, float pageUpConfidence, float pageLeftConfidence, int pageRotation)
    {
      Pix = pix;
      PageUpConfidence = pageUpConfidence;
      PageLeftConfidence = pageLeftConfidence;
      PageRotation = pageRotation;
    }

    public IPix Pix { get; }
    public float PageUpConfidence { get; } 
    public float PageLeftConfidence { get; } 
    public int PageRotation { get; }
  }
}