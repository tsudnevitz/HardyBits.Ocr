using System;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Ocr.Engine.IO;
using HardyBits.Ocr.Engine.Jobs;
using HardyBits.Ocr.Engine.Pdf;
using HardyBits.Wrappers.Leptonica.Pix;
using HardyBits.Wrappers.Tesseract;
using HardyBits.Wrappers.Tesseract.Results;

namespace HardyBits.Ocr.Engine
{
  public class ImageRecognitionEngine : IRecognitionEngine
  {
    private readonly IImageFileTypeRecognizer _recognizer;
    private readonly IImageFileStorage _storage;
    private readonly IRecognitionJobFactory _jobFactory;
    private readonly IRecognitionJobQueue _jobQueue;
    private bool _isDisposed;

    public ImageRecognitionEngine()
     : this(
       new ImageFileTypeRecognizer(
         new PixHelper()), 
       new ImageFileStorage(), 
       new RecognitionJobFactory(
         new TesseractEngineFactory(), 
         new PixFactory(), 
         new PdfDocumentFactory(),
         new ImageFileStorage()), 
       new RecognitionJobQueue())
    {
    }

    internal ImageRecognitionEngine(
      IImageFileTypeRecognizer recognizer,
      IImageFileStorage storage,
      IRecognitionJobFactory recognitionJobFactory,
      IRecognitionJobQueue recognitionJobQueue)
    {
      _recognizer = recognizer ?? throw new ArgumentNullException(nameof(recognizer));
      _storage = storage ?? throw new ArgumentNullException(nameof(storage));
      _jobFactory = recognitionJobFactory ?? throw new ArgumentNullException(nameof(recognitionJobFactory));
      _jobQueue = recognitionJobQueue ?? throw new ArgumentNullException(nameof(recognitionJobQueue));
    }

    public async Task<IRecognitionResults> RecognizeAsync(IRecognitionConfiguration config, bool isAsync = true)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));

      var type = _recognizer.GetFileType(config.Image.Data);
      if (type == ImageFileTypes.Unrecognized)
        throw new InvalidOperationException("Image file type not recognized.");

      var storedFile = await _storage.StoreAsync(config.Image);
      var job = _jobFactory.Create(type, config.Engine, storedFile);

      if (isAsync)
        return await _jobQueue.EnqueueAsync(job);
      return await job.ExecuteAsync();
    }

    public IRecognitionResults Recognize(IRecognitionConfiguration config, bool isAsync = true)
    {
      return RecognizeAsync(config, isAsync)
        .ConfigureAwait(false)
        .GetAwaiter()
        .GetResult();
    }

    protected virtual void Dispose(bool disposing)
    {
      if (_isDisposed)
        return;

      if (disposing)
      {
        _jobQueue?.Dispose();
      }

      _isDisposed = true;
    }

    ~ImageRecognitionEngine()
    {
      Dispose(false);
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}