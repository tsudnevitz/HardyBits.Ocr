using HardyBits.Wrappers.Tesseract.Results;

namespace HardyBits.Ocr.Engine.Results
{
  public class RecognitionResult : IRecognitionResult
  {
    public string Text { get; }
    public string HocrText { get; }
    public float? Confidence { get; }

    public RecognitionResult(ITesseractResult result)
    {
      Text = result.Text;
      HocrText = result.HocrText;
      Confidence = result.Confidence;
    }

    public RecognitionResult(string text, string hocrText = null, float? confidence = default)
    {
      Text = text;
      HocrText = hocrText;
      Confidence = confidence;
    }
  }
}