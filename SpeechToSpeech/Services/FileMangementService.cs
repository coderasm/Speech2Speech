using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech.Services
{
  public class FileMangementService : IFileManagementService
  {
    public void Delete(string fileName)
    {
      if (File.Exists(fileName))
        File.Delete(fileName);
    }
  }
}
