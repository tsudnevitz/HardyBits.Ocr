using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.IO;
using HardyBits.Ocr.Engine.Pdf;
using HardyBits.Ocr.Engine.Preprocessing;
using HardyBits.Wrappers.Leptonica.Internals;
using HardyBits.Wrappers.Tesseract.Factories;
using HardyBits.Wrappers.Tesseract.Results;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.Advanced;

namespace HardyBits.Ocr.Engine.Jobs
{
  internal class PdfRecognitionJob : IRecognitionJob
  {
    private readonly IPdfDocumentFactory _pdfDocumentFactory;
    private readonly IStoredImageFile _pdfFile;
    private readonly IPixFactory _pixFactory;
    private readonly IImageFileStorage _storage;
    private readonly IConfiguredTesseractEngineFactory _engineFactory;
    private readonly IEnumerable<IPreprocessor> _preprocessors;

    public PdfRecognitionJob(
      IConfiguredTesseractEngineFactory engineFactory, 
      IStoredImageFile imageFile, 
      IEnumerable<IPreprocessor> preprocessors,
      IPdfDocumentFactory pdfDocumentFactory, 
      IPixFactory pixFactory, 
      IImageFileStorage storage)
    {
      _engineFactory = engineFactory ?? throw new ArgumentNullException(nameof(engineFactory));
      _pdfFile = imageFile ?? throw new ArgumentNullException(nameof(imageFile));
      _preprocessors = preprocessors ?? throw new ArgumentNullException(nameof(preprocessors));
      _pdfDocumentFactory = pdfDocumentFactory ?? throw new ArgumentNullException(nameof(pdfDocumentFactory));
      _pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
      _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }
    public Task<IRecognitionResults> ExecuteAsync()
    {
      using var document = _pdfDocumentFactory.Open(_pdfFile.Path);
      if (document.ContainsText())
        return Task.FromResult(document.ExtractText());

      var storedImages = ExtractImages(document);
      var pixes = storedImages.SelectMany(x => _pixFactory.Create(x.Path));

      var results = new RecognitionResults();
      var options = new ParallelOptions{ MaxDegreeOfParallelism = 10 };
      Parallel.ForEach(pixes, options, pix =>
      {
        var preprocessedPix = Preprocess(pix);
        using var engine = _engineFactory.Create();
        var result = engine.Process(preprocessedPix);
        results.BlockingAdd(result);
      });

      return Task.FromResult((IRecognitionResults) results);
    }

    private IPix Preprocess(IPix pix)
    {
      IPix result = null;
      foreach (var preprocessor in _preprocessors)
        result = preprocessor.Run(pix);
      return result;
    }

    private IEnumerable<IStoredImageFile> ExtractImages(PdfDocument document)
    {
      var files = new List<IStoredImageFile>();
      foreach (var page in document.Pages)
      {
        var resources = page.Elements.GetDictionary("/Resources");
        var xObjects = resources?.Elements.GetDictionary("/XObject");
        if (xObjects == null) 
          continue;

        var items = xObjects.Elements.Values;
        foreach (var item in items)
        {
          if (!(item is PdfReference reference)) 
            continue;

          if (!(reference.Value is PdfDictionary xObject) ||
              xObject.Elements.GetString("/Subtype") != "/Image") 
            continue;

          var image = ExportImage(xObject);
          if(image != null)
            files.Add(image);
        }
      }

      return files;
    }

    private IStoredImageFile ExportImage(PdfDictionary image)
    {
      var filter = GetName(image.Elements, "/Filter");
      switch (filter)
      {
        case "/DCTDecode":
          return ExportJpegImage(image);
        case "/FlateDecode":
          throw new NotImplementedException();
        default:
          return null;
      }
    }

    private static string GetName(PdfDictionary.DictionaryElements dict, string key)
    {
      var obj = (object) dict[key];

      if (obj == null)
        return string.Empty;

      if (obj is PdfReference pdfReference)
        obj = pdfReference.Value;

      var pdfName = obj as PdfName;

      // do not change! hacky casting to string - there's a bug in PdfSharp
      string nullString = null;
      if (pdfName != nullString)
        return pdfName.Value;

      if (obj is PdfNameObject pdfNameObject)
        return pdfNameObject.Value;

      return null;
    }

    private IStoredImageFile ExportJpegImage(PdfDictionary image)
    {
      var stream = image.Stream.Value;
      return _storage.StoreAsync(stream).Result;
    }
  }
}