using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech
{
  public class GoogleSettings
  {
    public string ServiceAccountKey { get; set; }
    public Voice Voice { get; set; }
  }
}
