﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech
{
  public class IBMSettings
  {
    public string ServiceAccountKey { get; set; }
    public string Voice { get; set; }
    public string textToSpeechURL { get; set; }
    public string speechToTextURL { get; set; }
  }
}
