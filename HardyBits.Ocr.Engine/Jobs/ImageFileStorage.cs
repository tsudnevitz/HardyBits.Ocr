using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using HardyBits.Wrappers.Tesseract.Results;

namespace HardyBits.Ocr.Engine.Jobs
{
  internal class ImageRecognitionEngine : IRecognitionEngine
  {
    private readonly IImageFileTypeRecognizer _recognizer;
    private readonly IImageFileStorage _storage;
    private readonly IRecognitionJobFactory _jobFactory;
    private readonly IRecognitionJobQueue _jobQueue;
    private bool _isDisposed = false;

    public ImageRecognitionEngine(
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

      var type = await _recognizer.GetFileTypeAsync(config.Image.Data);
      if (type == ImageFileTypes.Unrecognized)
        throw new InvalidOperationException("Image file type not recognized.");

      var storedFile = await _storage.StoreAsync(config.Image.Data);
      var job = _jobFactory.Create(type, config, storedFile);

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

  internal interface IRecognitionJobQueue : IDisposable
  {
    Task<IRecognitionResults> EnqueueAsync(IRecognitionJob recognitionJob);
  }

  internal interface IImageFileTypeRecognizer
  {
    Task<ImageFileTypes> GetFileTypeAsync(ReadOnlyMemory<byte> imageData);
  }

  internal class ImageFileTypeRecognizer : IImageFileTypeRecognizer
  {
    public async Task<ImageFileTypes> GetFileTypeAsync(ReadOnlyMemory<byte> imageData)
    {
      if (CheckPdfType(imageData))
        return ImageFileTypes.Pdf;

      return TryGetImageType(imageData, out var type)
        ? type
        : ImageFileTypes.Unrecognized;
    }

    private bool TryGetImageType(in ReadOnlyMemory<byte> imageData, out ImageFileTypes type)
    {
      type = ImageFileTypes.Unrecognized;
      return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe bool CheckPdfType(in ReadOnlyMemory<byte> imageData)
    {
      var bytesLength = imageData.Length < 1024 ? imageData.Length : 1024;
      var headerBytes = imageData.Slice(0, bytesLength).Span;

      fixed (byte* firstByte = &headerBytes.GetPinnableReference()){
        var ansiHeader = Encoding.ASCII.GetString(firstByte, bytesLength);
        if (ansiHeader.Contains("%PDF"))
          return true;

        var utfHeader = Encoding.UTF8.GetString(firstByte, bytesLength);
        if (utfHeader.Contains("%PDF"))
          return true;
      }

      return false;
    }
  }

  internal interface IImageFileStorage
  {
    Task<IStoredImageFile> StoreAsync(in ReadOnlyMemory<byte> imageData);
  }

  public interface IStoredImageFile : IDisposable
  {
    string Path { get; }
  }

  public enum ImageFileTypes
  {
    Pdf, Tiff, Bitmap, Unrecognized
  }

  public interface IRecognitionJobFactory
  {
    IRecognitionJob Create(ImageFileTypes type, IRecognitionConfiguration config, IStoredImageFile imageFile);
  }

  public interface IRecognitionJob
  {
    Task<IRecognitionResults> ExecuteAsync();
  }
}
