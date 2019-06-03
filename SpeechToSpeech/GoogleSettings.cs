using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech
{
  public class GoogleSettings: INotifyPropertyChanged
  {
    public string ServiceAccountKey { get; set; } = "";
    private Voice _voice;
    public Voice Voice
    {
      get
      {
        return _voice;
      }
      set
      {
        if (value != null)
        {
          _voice = value;
          NotifyPropertyChanged("Voice");
        }
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}
