using IBM.WatsonDeveloperCloud.TextToSpeech.v1.Model;
using System;
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
    public string textToSpeechAPIKey { get; set; } = "";
    public string speechToTextAPIKey { get; set; } = "";
    public Voice Voice { get; set; }
    public string textToSpeechURL { get; set; } = $"https://stream.watsonplatform.net/text-to-speech/api";
    public string speechToTextURL { get; set; } = $"https://stream.watsonplatform.net/speech-to-text/api";
  }
}
