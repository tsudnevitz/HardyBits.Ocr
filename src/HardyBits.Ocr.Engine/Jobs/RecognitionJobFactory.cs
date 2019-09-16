using System;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Ocr.Engine.IO;
using HardyBits.Ocr.Engine.Pdf;
using HardyBits.Wrappers.Leptonica.Pix;
using HardyBits.Wrappers.Tesseract;

namespace HardyBits.Ocr.Engine.Jobs
{
  internal class RecognitionJobFactory : IRecognitionJobFactory
  {
    private readonly ITesseractEngineFactory _tesseractFactory;
    private readonly IPixFactory _pixFactory;
    private readonly IPdfDocumentFactory _pdfDocumentFactory;
    private readonly IImageFileStorage _storage;

    public RecognitionJobFactory(ITesseractEngineFactory tesseractFactory, IPixFactory pixFactory, IPdfDocumentFactory pdfDocumentFactory, IImageFileStorage storage)
    {
      _tesseractFactory = tesseractFactory ?? throw new ArgumentNullException(nameof(tesseractFactory));
      _pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
      _pdfDocumentFactory = pdfDocumentFactory ?? throw new ArgumentNullException(nameof(pdfDocumentFactory));
      _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    public IRecognitionJob Create(ImageFileTypes type, IEngineConfiguration config, IStoredImageFile imageFile) =>
      type switch
      {
        ImageFileTypes.Pdf => (IRecognitionJob) new PdfRecognitionJob(config, imageFile, _pdfDocumentFactory, _pixFactory, _tesseractFactory, _storage),
        ImageFileTypes.Bitmap => new BitmapRecognitionJob(config, imageFile, _pixFactory, _tesseractFactory),
        _ => throw new ArgumentException(message: "Invalid enum value", paramName: nameof(type))
      };
  }
}