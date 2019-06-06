using Amazon;
using System.ComponentModel;
using SpeechToSpeech.Models;

namespace SpeechToSpeech
{
  public class AmazonSettings: INotifyPropertyChanged
  {
    public string AccessKeyId { get; set; } = "";
    public string SecretAccessKey { get; set; } = "";
    public RegionEndpoint RegionEndpoint { get; set; } = RegionEndpoint.USWest1;

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
