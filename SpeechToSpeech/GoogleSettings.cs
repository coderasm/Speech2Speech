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
    private string serviceAccountKey = "";
    public string ServiceAccountKey
    {
      get
      {
        return serviceAccountKey;
      }
      set
      {
        if (value != null)
        {
          serviceAccountKey = value;
          NotifyPropertyChanged();
        }
      }
    }
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
