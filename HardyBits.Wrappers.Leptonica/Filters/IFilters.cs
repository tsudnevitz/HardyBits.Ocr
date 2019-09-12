using HardyBits.Wrappers.Leptonica.Pix;

namespace HardyBits.Wrappers.Leptonica.Filters
{
  public interface IFilters
  {
    DeskewFilterResult Deskew(IPix pix, int searchReductionFactor);
    FixOrientationResult FixOrientation(IPix pix, float minimumConfidence = 8.0f, float minimumRatio = 2.5f);
    Filters.PixFilterResult Test(IPix pix);
  }
}
