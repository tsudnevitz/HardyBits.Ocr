using System;
using System.Collections.Generic;
using HardyBits.Wrappers.Leptonica.Enums;
using HardyBits.Wrappers.Leptonica.Extensions;
using HardyBits.Wrappers.Leptonica.Imports;

namespace HardyBits.Wrappers.Leptonica.Pix
{
  public class PixFactory : IPixFactory
  {
    public unsafe IEnumerable<IPix> Create(ReadOnlyMemory<byte> data)
    {
      using var pointer = data.Pin();
      if (Leptonica5Pix.findFileFormatBuffer(pointer.Pointer, out var format) != 0)
        throw new InvalidOperationException("File format not supported.");

      var sizeInMemory = data.Length;

      switch (format)
      {
        case ImageFileFormat.Tiff:
        case ImageFileFormat.TiffPackbits:
        case ImageFileFormat.TiffRle:
        case ImageFileFormat.TiffG3:
        case ImageFileFormat.TiffG4:
        case ImageFileFormat.TiffLzw:
        case ImageFileFormat.TiffZip:
          return ReadTiff(pointer.Pointer, sizeInMemory);
        default:
          return new []{ReadImage(pointer.Pointer, sizeInMemory)};
      }
    }

    public IEnumerable<IPix> Create(int imageWidth, int imageHeight, int imageDepth)
    {
      return new[] {new Pix(imageWidth, imageHeight, imageDepth)};
    }

    public IEnumerable<IPix> Create(string imageFilePath)
    {
      if (Leptonica5Pix.findFileFormat(imageFilePath, out var format) != 0)
        throw new InvalidOperationException("File format not supported.");

      switch (format)
      {
        case ImageFileFormat.Default:
        case ImageFileFormat.Tiff:
        case ImageFileFormat.TiffPackbits:
        case ImageFileFormat.TiffRle:
        case ImageFileFormat.TiffG3:
        case ImageFileFormat.TiffG4:
        case ImageFileFormat.TiffLzw:
        case ImageFileFormat.TiffZip:
          return ReadTiff(imageFilePath);
        default:
          return new []{ReadImage(imageFilePath)};
      }
    }

    private static unsafe IPix ReadImage(void* pointer, int size)
    {
      var pixPointer = Leptonica5Pix.pixReadMem(pointer, size).GetPointerOrThrow();
      return new Pix(pixPointer);
    }

    private static IPix ReadImage(string filename)
    {
      var pixPointer = Leptonica5Pix.pixRead(filename).GetPointerOrThrow();
      return new Pix(pixPointer);
    }

    private static IEnumerable<IPix> ReadTiff(string filename)
    {
      var pointer = Leptonica5Pix.pixaReadMultipageTiff(filename).GetPointerOrThrow();
      return ReadTiff(pointer);
    }

    private static unsafe IEnumerable<IPix> ReadTiff(void* nativePointer, int size)
    {
      var pointer = Leptonica5Pix.pixaReadMemMultipageTiff(nativePointer, size).GetPointerOrThrow();
      return ReadTiff(pointer);
    }

    private static IEnumerable<IPix> ReadTiff(IntPtr pixaPointer)
    {
      try
      {
        var pagesCount = Leptonica5Pix.pixaGetCount(pixaPointer);
        if (pagesCount <= 0)
          throw new InvalidOperationException("File has no pages.");

        var pages = new List<Pix>();
        for (var i = 0; i < pagesCount; i++)
        {
          try
          {
            var pagePointer = Leptonica5Pix.pixaGetPix(pixaPointer, i, 2).GetPointerOrThrow();
            var page = new Pix(pagePointer);
            pages.Add(page);
          }
          catch
          {
            Destroy(pages);
            throw;
          }
        }

        return pages;
      }
      finally
      {
        Leptonica5Pix.pixaDestroy(ref pixaPointer);
      }
    }

    private static void Destroy(IEnumerable<Pix> pages)
    {
      foreach (var page in pages)
      {
        var pointer = page.HandleRef.Handle;
        Leptonica5Pix.pixDestroy(ref pointer);
      }
    }
  }
}