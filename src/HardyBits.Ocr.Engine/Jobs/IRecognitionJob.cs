﻿using System.Threading.Tasks;
using HardyBits.Ocr.Engine.Results;

namespace HardyBits.Ocr.Engine.Jobs
{
  public interface IRecognitionJob
  {
    Task<IRecognitionResults> ExecuteAsync();
  }
}