using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using SpeechToSpeech.Models;
using SpeechToSpeech.Services;

namespace SpeechToSpeech.Repositories
{
  public class WebServiceRepository : IWebServiceRepository
  {
    private ISettingsService settingsService;
    private DatabaseSettings settings;
    public WebServiceRepository(ISettingsService settingsService)
    {
      this.settingsService = settingsService;
      this.settings = settingsService.settings.databaseSettings;
    }
    public async Task<List<WebService>> GetWebServices()
    {
      using (var cnn = new SqlConnection(settings.ConnectionString))
      {
        var result = await cnn.QueryAsync<WebService>("select * from service");
        return result.ToList();
      }
    }

    public async Task<int> InsertWebService(WebService webService)
    {
      using (var cnn = new SqlConnection(settings.ConnectionString))
      {
        var reader = await cnn.ExecuteReaderAsync("insert into service(Name) value(@Name)", webService);
        return reader.Parse<WebService>().FirstOrDefault().Id;
      }
    }

    public async Task<List<int>> InsertWebServices(List<WebService> webServices)
    {
      var ids = new List<int>();
      using (var cnn = new SqlConnection(settings.ConnectionString))
      {
        var reader = await cnn.ExecuteReaderAsync("insert into service(Name) value(@Name)", webServices);
        return reader.Parse<WebService>().Select(service => service.Id).ToList();
      }
    }
  }
}
