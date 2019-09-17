namespace HardyBits.Ocr.Engine.Results
{
  public interface IRecognitionResult
  {
    string Text { get; }
    string HocrText { get; }
    float? Confidence { get; }
  }
}