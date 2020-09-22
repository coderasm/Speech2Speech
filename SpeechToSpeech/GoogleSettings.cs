using System.ComponentModel;
using System.Runtime.CompilerServices;
using SpeechToSpeech.Models;

namespace SpeechToSpeech
{
  public class GoogleSettings: INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
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
          NotifyPropertyChanged();
        }
      }
    }
  }
}
