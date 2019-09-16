using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Ocr.Engine.IO;
using HardyBits.Ocr.Engine.Pdf;
using HardyBits.Wrappers.Leptonica.Pix;
using HardyBits.Wrappers.Tesseract;
using HardyBits.Wrappers.Tesseract.Results;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.Advanced;

namespace HardyBits.Ocr.Engine.Jobs
{
  internal class PdfRecognitionJob : IRecognitionJob
  {
    private readonly IEngineConfiguration _config;
    private readonly IPdfDocumentFactory _pdfDocumentFactory;
    private readonly IStoredImageFile _pdfFile;
    private readonly ITesseractEngineFactory _tesseractFactory;
    private readonly IPixFactory _pixFactory;
    private readonly IImageFileStorage _storage;

    public PdfRecognitionJob(
      IEngineConfiguration config, 
      IStoredImageFile imageFile, 
      IPdfDocumentFactory pdfDocumentFactory, 
      IPixFactory pixFactory, 
      ITesseractEngineFactory tesseractFactory, 
      IImageFileStorage storage)
    {
      _config = config ?? throw new ArgumentNullException(nameof(config));
      _pdfFile = imageFile ?? throw new ArgumentNullException(nameof(imageFile));
      _pdfDocumentFactory = pdfDocumentFactory ?? throw new ArgumentNullException(nameof(pdfDocumentFactory));
      _pixFactory = pixFactory ?? throw new ArgumentNullException(nameof(pixFactory));
      _tesseractFactory = tesseractFactory ?? throw new ArgumentNullException(nameof(tesseractFactory));
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
        using var tesseract = _tesseractFactory.Create(_config.TessData, _config.Language, _config.EngineMode);
        var result = tesseract.Process(pix);
        results.BlockingAdd(result);
      });

      return results;
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
      if (pdfName != (string) null)
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