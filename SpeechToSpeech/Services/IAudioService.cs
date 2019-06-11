using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech.Services
{
  public interface IAudioService
  {
    List<KeyValuePair<int, string>> Devices { get; }
    int OutputDevice { set; }
    int InputDevice { set; }
    double Volume { get; set; }
    IAudioService OnPlayStopped(Action handler);
    IAudioService OnPlay(Action handler);
    IAudioService Play(string fileName);
    IAudioService Play(string fileName, int deviceNumber);
    IAudioService Stop();
    IAudioService Pause();
  }
}
