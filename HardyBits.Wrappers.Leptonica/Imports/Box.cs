using System.Runtime.InteropServices;

namespace HardyBits.Wrappers.Leptonica.Imports
{
  [StructLayout(LayoutKind.Sequential)]
  public struct Box
  {
    public int x;
    public int y;
    public int w;
    public int h;
    public uint refcount;
  };
}