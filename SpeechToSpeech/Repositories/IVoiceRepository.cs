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
    Task<int> Insert(Voice voice);
    int InsertMultiple(List<Voice> voices);
    Task<bool> Delete(int id);
    bool DeleteAll();
    bool DeleteAllById(List<int> ids);
    Task<Voice> Get(int id);
    Task<List<Voice>> GetAllByService(int serviceId);
    Task<List<Voice>> GetAll(int serviceId, string language);
  }
}
