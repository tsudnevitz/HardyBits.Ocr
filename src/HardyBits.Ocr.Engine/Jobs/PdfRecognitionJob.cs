using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.IO;
using HardyBits.Ocr.Engine.Pdf;
using HardyBits.Ocr.Engine.Preprocessing;
using HardyBits.Wrappers.Leptonica.Pix;
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
    public async Task<IRecognitionResults> ExecuteAsync()
    {
      using var document = _pdfDocumentFactory.Open(_pdfFile.Path);
      if (document.ContainsText())
        return document.ExtractText();

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

      return results;
    }

    private IPix Preprocess(IPix pix)
    {
      IPix result = null;
      foreach (var preprocessor in _preprocessors)
        result = preprocessor.Run(pix);
      return result;
    }

    public IEnumerable<IStoredImageFile> ExtractImages(PdfDocument document)
    {
      var files = new List<IStoredImageFile>();
      foreach (PdfPage page in document.Pages)
      {
        // Get resources dictionary
        PdfDictionary resources = page.Elements.GetDictionary("/Resources");
        if (resources != null)
        {
          // Get external objects dictionary
          PdfDictionary xObjects = resources.Elements.GetDictionary("/XObject");
          if (xObjects != null)
          {
            ICollection<PdfItem> items = xObjects.Elements.Values;
            // Iterate references to external objects
            foreach (PdfItem item in items)
            {
              PdfReference reference = item as PdfReference;
              if (reference != null)
              {
                PdfDictionary xObject = reference.Value as PdfDictionary;
                // Is external object an image?
                if (xObject != null && xObject.Elements.GetString("/Subtype") == "/Image")
                {
                  var image = ExportImage(xObject);
                  if(image != null)
                    files.Add(image);
                }
              }
            }
          }
        }
      }

      return files;
    }

    public IStoredImageFile ExportImage(PdfDictionary image)
    {
      string filter = GetName(image.Elements, "/Filter");
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

    public string GetName(PdfDictionary.DictionaryElements dict, string key)
    {
      object obj = (object) dict[key];
      if (obj == null)
        return string.Empty;
      PdfReference pdfReference = obj as PdfReference;
      if (pdfReference != null)
        obj = (object) pdfReference.Value;
      PdfName pdfName = obj as PdfName;
      string nullString = null;
      if (pdfName != nullString)
        return pdfName.Value;

      if (obj is PdfNameObject pdfNameObject)
        return pdfNameObject.Value;

      return null;
    }

    public IStoredImageFile ExportJpegImage(PdfDictionary image)
    {
      // Fortunately JPEG has native support in PDF and exporting an image is just writing the stream to a file.
      byte[] stream = image.Stream.Value;
      return _storage.StoreAsync(stream).Result;
    }
  }
}