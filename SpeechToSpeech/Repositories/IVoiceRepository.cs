using System;
using System.Collections.Generic;
using SpeechToSpeech.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech.Repositories
{
  public interface IVoiceRepository
  {
    int CreateVoice(Voice voice);
    List<int> CreateVoices(List<Voice> voices);
    bool DeleteVoice(int id);
    bool DeleteVoices();
    bool DeleteVoices(List<int> ids);
    Voice GetVoice(int id);
    List<Voice> GetVoicesByService(int serviceId);
    List<Voice> GetVoices(int serviceId, string language);
  }
}
