﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech.Services
{
  public interface IDatabaseService
  {
    string ConnectionString { get; }
  }
}
