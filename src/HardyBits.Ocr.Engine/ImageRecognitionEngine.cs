using System;
using System.Linq;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Ocr.Engine.IO;
using HardyBits.Ocr.Engine.Jobs;
using HardyBits.Ocr.Engine.Pdf;
using HardyBits.Ocr.Engine.Preprocessing;
using HardyBits.Ocr.Engine.Results;
using HardyBits.Wrappers.Leptonica.Internals;
using HardyBits.Wrappers.Tesseract.Enums;
using HardyBits.Wrappers.Tesseract.Factories;

namespace HardyBits.Ocr.Engine
{
  public class ImageRecognitionEngine : IRecognitionEngine
  {
    private readonly IImageFileTypeRecognizer _recognizer;
    private readonly IImageFileStorage _storage;
    private readonly IRecognitionJobFactory _jobFactory;
    private readonly IRecognitionJobQueue _jobQueue;
    private readonly ITesseractEngineFactory _tesseractFactory;
    private readonly IPreprocessorFactory _preprocessorFactory;

    private bool _isDisposed;

    public ImageRecognitionEngine()
     : this(
       new ImageFileTypeRecognizer(
         new PixHelper()), 
       new ImageFileStorage(), 
       new RecognitionJobFactory( 
         new PixFactory(), 
         new PdfDocumentFactory(),
         new ImageFileStorage()), 
       new RecognitionJobQueue(),
       new TesseractEngineFactory(),
       new PreprocessorFactory())
    {
    }

    internal ImageRecognitionEngine(
      IImageFileTypeRecognizer recognizer,
      IImageFileStorage storage,
      IRecognitionJobFactory recognitionJobFactory,
      IRecognitionJobQueue recognitionJobQueue, 
      ITesseractEngineFactory tesseractFactory,
      IPreprocessorFactory preprocessorFactory)
    {
      _recognizer = recognizer ?? throw new ArgumentNullException(nameof(recognizer));
      _storage = storage ?? throw new ArgumentNullException(nameof(storage));
      _jobFactory = recognitionJobFactory ?? throw new ArgumentNullException(nameof(recognitionJobFactory));
      _jobQueue = recognitionJobQueue ?? throw new ArgumentNullException(nameof(recognitionJobQueue));
      _tesseractFactory = tesseractFactory ?? throw new ArgumentNullException(nameof(tesseractFactory));
      _preprocessorFactory = preprocessorFactory ?? throw new ArgumentNullException(nameof(preprocessorFactory));
    }

    public async Task<IRecognitionResults> RecognizeAsync(IRecognitionConfiguration config, bool runParallel = true)
    {
      if (config == null)
        throw new ArgumentNullException(nameof(config));

      var type = await _recognizer.GetFileTypeAsync(config.File.DataStream);
      if (type == ImageFileTypes.Unrecognized)
        throw new InvalidOperationException("Image file type not recognized.");

      var tesseractFactory = _tesseractFactory.CreateFactory(config.Engine.TessData, config.Engine.Language, (EngineMode) config.Engine.EngineMode);
      var preprocessors = config.Preprocessors.Select(x => _preprocessorFactory.Create(x)).ToArray();
      using var storedFile = await _storage.StoreAsync(config.File);

      if (config.File.DisposeStream)
        config.File.DataStream.Dispose();

      var job = _jobFactory.Create(type, storedFile, tesseractFactory, preprocessors);

      if (runParallel)
        return await _jobQueue.EnqueueAsync(job);
      return await job.ExecuteAsync();
    }

    public IRecognitionResults Recognize(IRecognitionConfiguration config, bool runParallel = true)
    {
      return RecognizeAsync(config, runParallel)
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