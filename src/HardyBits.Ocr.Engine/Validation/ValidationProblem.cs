using System;

namespace HardyBits.Ocr.Engine.Validation
{
  public class ValidationProblem
  {
    public ValidationProblem(string description, string parameterName = null)
    {
      Description = description ?? throw new ArgumentNullException(nameof(description));
      ParameterName = parameterName;
    }

    public string ParameterName { get; }
    public string Description { get; }
  }
}