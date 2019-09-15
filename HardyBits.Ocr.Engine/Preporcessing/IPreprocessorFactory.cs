using System;
using HardyBits.Ocr.Engine.Configuration;
using HardyBits.Wrappers.Leptonica.Filters;
using HardyBits.Wrappers.Leptonica.Pix;

namespace HardyBits.Ocr.Engine.Preporcessing
{
  public interface IPreprocessorFactory
  {
    IPreprocessor Create(IPreprocessorConfiguration config);
  }

  internal class PreprocessorFactory : IPreprocessorFactory
  {
    private readonly IFilters _filters;

    public PreprocessorFactory() : this(new Filters())
    {
    }

    internal PreprocessorFactory(IFilters filters)
    {
      _filters = filters ?? throw new ArgumentNullException(nameof(filters));
    }

    public IPreprocessor Create(IPreprocessorConfiguration config)
    {
      return new DelegatePreprocessor(pix =>
      {
        //using var pix1 = _filters.Deskew(pix, 4).Pix;
        //return _filters.FixOrientation(pix1).Pix;
        return _filters.Test(pix).Pix;
      });
    }
  }

  //public abstract class PreprocessorBase : IPreprocessor
  //{
  //  public IPix Run(IPix image)
  //  {
      
  //  }
  //}

  public class CloningPreprocessor : IPreprocessor
  {
    public IPix Run(IPix image)
    {
      if (image == null)
        throw new ArgumentNullException(nameof(image));

      return image.Clone();
    }
  }

  public class DelegatePreprocessor : IPreprocessor
  {
    private readonly Func<IPix, IPix> _action;

    public DelegatePreprocessor(Func<IPix, IPix> action)
    {
      _action = action ?? throw new ArgumentNullException(nameof(action));
    }

    public IPix Run(IPix image) => _action(image);
  }
}