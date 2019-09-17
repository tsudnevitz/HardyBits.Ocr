namespace HardyBits.Wrappers.Tesseract.Results
{
  public class TesseractResult : ITesseractResult
  {
    public string Text { get; set; }
    public string HocrText { get; set; }
    public float? Confidence { get; set; }

    public void Dispose()
    {
      // nothing to dispose yet
    }
  }
}