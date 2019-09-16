using System;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Ocr.Engine.IO;
using HardyBits.Wrappers.Leptonica.Pix;
using HardyBits.Wrappers.Tesseract;
using HardyBits.Wrappers.Tesseract.Results;

namespace HardyBits.Ocr.Engine.Jobs
{
  public class BitmapRecognitionJob : IRecognitionJob
  {
    private readonly IEngineConfiguration _config;
    private readonly IStoredImageFile _imageFile;
    private readonly ITesseractEngineFactory _tesseractFactory;
    private readonly IPixFactory _pixFactory;

    public BitmapRecognitionJob(IEngineConfiguration config, IStoredImageFile imageFile, IPixFactory pixFactory, ITesseractEngineFactory tesseractFactory)
    {
      _config = config ?? throw new ArgumentNullException(nameof(config));
      _imageFile = imageFile ?? throw new ArgumentNullException(nameof(imageFile));
      _pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
      _tesseractFactory = tesseractFactory ?? throw new ArgumentNullException(nameof(tesseractFactory));
    }

    public Task<IRecognitionResults> ExecuteAsync()
    {
      var pixes = _pixFactory.Create(_imageFile.Path);

      var results = new RecognitionResults();
      var options = new ParallelOptions{ MaxDegreeOfParallelism = 10 };
      Parallel.ForEach(pixes, options, pix =>
      {
        using var tesseract = _tesseractFactory.Create(_config.TessData, _config.Language, _config.EngineMode);
        var result = tesseract.Process(pix);
        results.BlockingAdd(result);
      });

      return Task.FromResult<IRecognitionResults>(results);
    }
  }
}