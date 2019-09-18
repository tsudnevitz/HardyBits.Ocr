using System;
using HardyBits.Wrappers.Leptonica.Internals;

namespace HardyBits.Ocr.Engine.Extensions
{
  internal static class PixExtensions
  {
    public static IPix ChainOutput(this IPix pix, Func<IPix, IPix> invocation, bool disposePreviousPix = true)
    {
      if (pix == null)
        throw new ArgumentNullException(nameof(pix));

      if (invocation == null)
        throw new ArgumentNullException(nameof(invocation));
      
      if (!disposePreviousPix)
        return invocation(pix);

      using (pix)
        return invocation(pix);
    }
  }
}