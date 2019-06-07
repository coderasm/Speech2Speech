using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using SpeechToSpeech.Models;
using SpeechToSpeech.Services;

namespace SpeechToSpeech.Repositories
{
  public class VoiceRepository : IVoiceRepository
  {
    private ISettingsService settingsService;
    private Settings settings;
    private string ConnectionString
    {
      get
      {
        return string.Format("DataSource=\"{0}\";Max Database Size={1};", settings.generalSettings.Database, settings.databaseSettings.MaxDatabaseSize);
      }
    }
    public VoiceRepository(ISettingsService settingsService)
    {
      this.settingsService = settingsService;
      settings = settingsService.settings;
    }
    public async Task<int> Insert(Voice voice)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        return await cnn.InsertAsync(voice);
      }
    }

    public int InsertMultiple(List<Voice> voices)
    {
      var ids = new List<int>();
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        return (int)cnn.Insert(voices);
      }
    }

    public async Task<bool> Delete(int id)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        return await cnn.DeleteAsync(new Voice { Id = id });
      }
    }

    public bool DeleteAll()
    {
      throw new NotImplementedException();
    }

    public bool DeleteAllById(List<int> ids)
    {
      throw new NotImplementedException();
    }

    public async Task<Voice> Get(int id)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        return await cnn.GetAsync<Voice>(id);
      }
    }

    public async Task<List<Voice>> GetAll(int serviceId, string language)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        var obj = new { ServiceId = serviceId, Language = language };
        var results = await cnn.QueryAsync<Voice>("select * from voice where ServiceId = @ServiceId && Language = @Language", obj);
        return results.ToList();
      }
    }

    public async Task<List<Voice>> GetAllByService(int serviceId)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        var obj = new { ServiceId = serviceId };
        var results = await cnn.QueryAsync<Voice>("select * from voice where ServiceId = @ServiceId", obj);
        return results.ToList();
      }
    }
  }
}
