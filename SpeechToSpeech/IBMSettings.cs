using System.ComponentModel;
using SpeechToSpeech.Models;

namespace SpeechToSpeech
{
  public class IBMSettings: INotifyPropertyChanged
  {
    public string textToSpeechAPIKey { get; set; } = "";
    public string speechToTextAPIKey { get; set; } = "";
    public string textToSpeechURL { get; set; } = $"https://stream.watsonplatform.net/text-to-speech/api";
    public string speechToTextURL { get; set; } = $"https://stream.watsonplatform.net/speech-to-text/api";
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
