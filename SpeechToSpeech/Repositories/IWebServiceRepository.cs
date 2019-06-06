using SpeechToSpeech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech.Repositories
{
  public interface IWebServiceRepository
  {
    Task<List<WebService>> GetWebServices();
    Task<int> InsertWebService(WebService webService);
    Task<List<int>> InsertWebServices(List<WebService> webService);
  }
}
