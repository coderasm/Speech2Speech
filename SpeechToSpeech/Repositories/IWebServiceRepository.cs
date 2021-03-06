﻿using SpeechToSpeech.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech.Repositories
{
  public interface IWebServiceRepository
  {
    Task<List<WebService>> GetAll();
    Task<int> Insert(WebService webService);
    int InsertMultiple(List<WebService> webService);
    Task<bool> Delete(int id);
  }
}
