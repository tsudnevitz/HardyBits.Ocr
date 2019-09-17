namespace HardyBits.Wrappers.Leptonica.Internals
{
  public interface IPix : ILeptonicaObject
  {
    int Width { get; }
    int Height { get; }
    int Depth { get; }
    int XRes { get; }
    int YRes { get; }
    IPix Clone();
  }
}