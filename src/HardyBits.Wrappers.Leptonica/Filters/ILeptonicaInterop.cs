using HardyBits.Wrappers.Leptonica.Enums;
using HardyBits.Wrappers.Leptonica.Pix;

namespace HardyBits.Wrappers.Leptonica.Filters
{
  public interface ILeptonicaInterop
  {
    IPix DeskewBoth(IPix pix, DeskewReductionFactor searchReduction);
    IPix PrepareOneBitPerPixel(IPix pix);
    IPix OrientationCorrect(IPix pix, float minUpConfidence = 4f, float minRatio = 2.5f);
    IBox FindPageForeground(IPix pix, int threshold = 128, int minDistance = 50, int eraseDistance = 70);
    IPix ClipRectangle(IPix pix, IBox box);
  }
}
