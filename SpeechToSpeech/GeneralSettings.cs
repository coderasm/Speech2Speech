using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech
{
  public class GeneralSettings
  {
    public bool? IsPush2Talk { get; set; } = false;
    public bool? IsAppPush2Talk { get; set; } = false;
    public Key Push2TalkKey { get; set; } = Key.None;
    public Key AppPush2TalkKey { get; set; } = Key.None;
    public string SpeechInputLanguage { get; set; } = CultureInfo.CurrentCulture.Name;
    public string TextInputLanguage { get; set; } = CultureInfo.CurrentCulture.Name;
    public int AudioOutDevice { get; set; } = -1;
  }
}
