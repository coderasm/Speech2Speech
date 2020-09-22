using System.ComponentModel;
using System.Runtime.CompilerServices;
using SpeechToSpeech.Models;

namespace SpeechToSpeech
{
  public class IBMSettings: INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
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
          NotifyPropertyChanged();
        }
      }
    }
  }
}
