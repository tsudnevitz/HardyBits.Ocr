using HardyBits.Wrappers.Leptonica.Pix;

namespace HardyBits.Wrappers.Leptonica.Filters
{
  public interface IFilterResult
  {
    IPix Pix { get; }
  }
}