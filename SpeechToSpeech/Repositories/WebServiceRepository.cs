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
  public class WebServiceRepository : IWebServiceRepository
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
    public WebServiceRepository(ISettingsService settingsService)
    {
      this.settingsService = settingsService;
      settings = settingsService.settings;
    }
    public async Task<List<WebService>> GetAll()
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        var result = await cnn.GetAllAsync<WebService>();
        return result.ToList();
      }
    }

    public async Task<int> Insert(WebService webService)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {

        return await cnn.InsertAsync(webService);
      }
    }

    public int InsertMultiple(List<WebService> webServices)
    {
      var ids = new List<int>();
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        return (int)cnn.Insert(webServices);
      }
    }

    public async Task<bool> Delete(int id)
    {
      using (var cnn = new SqlCeConnection(ConnectionString))
      {
        return await cnn.DeleteAsync(new WebService { Id = id });
      }
    }
  }
}
