using System;
using System.Collections.Generic;

namespace HardyBits.Ocr.Engine.Results
{
  public class RecognitionResults : List<IRecognitionResult>, IRecognitionResults
  {
    private readonly object _lock = new object();

    public RecognitionResults(IEnumerable<IRecognitionResult> collection) : base(collection)
    {
    }

    public RecognitionResults()
    {
    }

    public void BlockingAdd(IRecognitionResult item)
    {
      if (item == null)
        throw new ArgumentNullException(nameof(item));

      lock (_lock) Add(item);
    }
  }
}