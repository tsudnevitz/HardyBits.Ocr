using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using HardyBits.Ocr.Engine.Results;
using HardyBits.Wrappers.Tesseract.Results;

namespace HardyBits.Ocr.Engine.Jobs
{
  internal class RecognitionJobQueue : IRecognitionJobQueue
  {
    private readonly ActionBlock<QueuedRecognitionJob> _jobQueue;
    private readonly CancellationTokenSource _cancellationSource;

    public RecognitionJobQueue()
    {
      _cancellationSource = new CancellationTokenSource();
      var execOptions = new ExecutionDataflowBlockOptions
      {
        MaxDegreeOfParallelism = 10,
        CancellationToken = _cancellationSource.Token
      };

      _jobQueue = new ActionBlock<QueuedRecognitionJob>(async job => await job.ExecuteAsync(), execOptions);
    }

    public async Task<IRecognitionResults> EnqueueAsync(IRecognitionJob recognitionJob)
    {
      var queueItem = new QueuedRecognitionJob(recognitionJob, _cancellationSource.Token);
      if(!await _jobQueue.SendAsync(queueItem))
        throw new ApplicationException("Unable to enqueue job.");

      return await queueItem.GetAwaiter();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing) 
        return;

      _jobQueue.Complete();
      _cancellationSource.Cancel();
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    ~RecognitionJobQueue()
    {
      Dispose(false);
    }
  }
}