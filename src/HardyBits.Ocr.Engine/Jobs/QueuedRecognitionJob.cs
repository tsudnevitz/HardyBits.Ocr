using System;
using System.Threading;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.Results;
using HardyBits.Wrappers.Tesseract.Results;

namespace HardyBits.Ocr.Engine.Jobs
{
  internal class QueuedRecognitionJob
  {
    private readonly IRecognitionJob _recognitionJob;
    private readonly CancellationToken _token;
    private readonly TaskCompletionSource<IRecognitionResults> _completionSource;

    public QueuedRecognitionJob(IRecognitionJob recognitionJob, CancellationToken token)
    {
      _recognitionJob = recognitionJob ?? throw new ArgumentNullException(nameof(recognitionJob));
      _completionSource = new TaskCompletionSource<IRecognitionResults>();
      _token = token;
    }

    public async Task ExecuteAsync()
    {
      try
      {
        if (_token.IsCancellationRequested)
        {
          _completionSource.TrySetCanceled();
          return;
        }

        var result = await _recognitionJob.ExecuteAsync();
        _completionSource.TrySetResult(result);
      }
      catch (Exception ex)
      {
        _completionSource.TrySetException(ex);
      }
    }

    public Task<IRecognitionResults> GetAwaiter()
    {
      return _completionSource.Task;
    }
  }
}