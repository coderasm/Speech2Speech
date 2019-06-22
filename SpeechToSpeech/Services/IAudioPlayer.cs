using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech.Services
{
  public interface IAudioPlayer
  {
    List<KeyValuePair<int, string>> Devices { get; }
    int OutputDevice { set; }
    int InputDevice { set; }
    string AudioFileReader { set; }
    double Volume { get; set; }
    double Position { get; set; }
    double Length { get;}
    IAudioPlayer OnPlayStopped(Action handler);
    IAudioPlayer OnPlay(Action handler);
    IAudioPlayer Play(string fileName);
    IAudioPlayer Play(string fileName, int deviceNumber);
    IAudioPlayer Stop();
    IAudioPlayer Pause();
  }
}
