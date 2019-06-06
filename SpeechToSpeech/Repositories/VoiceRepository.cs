using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeechToSpeech.Models;
using SpeechToSpeech.Services;

namespace SpeechToSpeech.Repositories
{
  public class VoiceRepository : IVoiceRepository
  {
    private IDatabaseService databaseService;
    public VoiceRepository(IDatabaseService databaseService)
    {
      this.databaseService = databaseService;
    }
    public int CreateVoice(Voice voice)
    {
      throw new NotImplementedException();
    }

    public List<int> CreateVoices(List<Voice> voices)
    {
      throw new NotImplementedException();
    }

    public bool DeleteVoice(int id)
    {
      throw new NotImplementedException();
    }

    public bool DeleteVoices()
    {
      throw new NotImplementedException();
    }

    public bool DeleteVoices(List<int> ids)
    {
      throw new NotImplementedException();
    }

    public Voice GetVoice(int id)
    {
      throw new NotImplementedException();
    }

    public List<Voice> GetVoices(int serviceId, string language)
    {
      throw new NotImplementedException();
    }

    public List<Voice> GetVoicesByService(int serviceId)
    {
      throw new NotImplementedException();
    }
  }
}
