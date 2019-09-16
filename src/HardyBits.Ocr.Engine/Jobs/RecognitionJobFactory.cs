using System;
using System.Collections.Generic;
using HardyBits.Ocr.Engine.IO;
using HardyBits.Ocr.Engine.Pdf;
using HardyBits.Ocr.Engine.Preprocessing;
using HardyBits.Wrappers.Leptonica.Internals;
using HardyBits.Wrappers.Tesseract.Factories;

namespace HardyBits.Ocr.Engine.Jobs
{
  internal class RecognitionJobFactory : IRecognitionJobFactory
  {
    private readonly IPixFactory _pixFactory;
    private readonly IPdfDocumentFactory _pdfDocumentFactory;
    private readonly IImageFileStorage _storage;

    public RecognitionJobFactory(IPixFactory pixFactory, IPdfDocumentFactory pdfDocumentFactory, IImageFileStorage storage)
    {
      _pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
      _pdfDocumentFactory = pdfDocumentFactory ?? throw new ArgumentNullException(nameof(pdfDocumentFactory));
      _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public IRecognitionJob Create(ImageFileTypes fileType, IStoredImageFile file, IConfiguredTesseractEngineFactory engineFactory, IEnumerable<IPreprocessor> preprocessors) =>
      fileType switch
      {
        ImageFileTypes.Pdf => (IRecognitionJob) new PdfRecognitionJob(engineFactory, file, preprocessors, _pdfDocumentFactory, _pixFactory, _storage),
        ImageFileTypes.Bitmap => new BitmapRecognitionJob(engineFactory, file, preprocessors, _pixFactory),
        _ => throw new ArgumentException(message: "Invalid enum value", paramName: nameof(fileType))
      };
  }
}