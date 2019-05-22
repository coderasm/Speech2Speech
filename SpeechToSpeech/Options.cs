using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech
{
  public class Options
  {
    public bool IsPush2Talk { get; set; } = false;
    public Key Push2TalkKey { get; set; } = Key.None;
    public string SpeechInputLanguage = CultureInfo.CurrentCulture.Name;
    public string TextInputLanguage = CultureInfo.CurrentCulture.Name;
    public string SpeechOutputGender = "male";
  }
}
