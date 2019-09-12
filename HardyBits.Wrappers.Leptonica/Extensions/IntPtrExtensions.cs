using System;
using System.Runtime.InteropServices;

namespace HardyBits.Wrappers.Leptonica.Extensions
{
  public static class IntPtrExtensions
  {
    public static IntPtr GetPointerOrThrow(this IntPtr pointer, string errorMessage = null)
    {
      const string defaultMessage = "Pointer not set.";
      errorMessage ??= defaultMessage;

      if (pointer == null || pointer == IntPtr.Zero)
        throw new NullReferenceException(errorMessage);

      return pointer;
    }

    public static HandleRef GetHandleOrThrow(this IntPtr pointer, object wrapper, string errorMessage = null)
    {
      if (wrapper == null)
        throw new ArgumentNullException(nameof(wrapper));

      pointer.GetPointerOrThrow(errorMessage);
      return new HandleRef(wrapper, pointer);
    }
  }
}
