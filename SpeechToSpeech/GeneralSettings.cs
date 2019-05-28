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
    public List<Key> Push2TalkKey { get; set; } = new List<Key>();
    public List<Key> AppPush2TalkKey { get; set; } = new List<Key>();
    public string SpeechInputLanguage { get; set; } = CultureInfo.CurrentCulture.Name;
    public string TextInputLanguage { get; set; } = CultureInfo.CurrentCulture.Name;
    public int AudioOutDevice { get; set; } = -1;
    public int AudioInDevice { get; set; } = -1;
    public int ActiveTextToSpeechService { get; set; } = 0;
  }
}
