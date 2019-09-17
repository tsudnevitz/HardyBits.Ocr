using System;

namespace HardyBits.Ocr.Engine.Preprocessing
{
  public class PreprocessorManifest<TParent> : IManifest
  {
    private const string PreprocessorText = "Preprocessor";

    public string Name => GetName();

    private static string GetName()
    {
      var typeName = typeof(TParent).Name;
      if (!typeName.EndsWith(PreprocessorText))
        return typeName;

      var index = typeName.IndexOf(PreprocessorText, StringComparison.Ordinal);
      return typeName.Substring(0, index);
    }
  }
}