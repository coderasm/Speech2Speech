using IBM.WatsonDeveloperCloud.TextToSpeech.v1.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech
{
  class IBMWebService : ITranscribeAndVocalize<Voice>
  {
    public Task<List<Voice>> GetVoices()
    {
      throw new NotImplementedException();
    }

    public Task<List<Voice>> GetVoices(string language)
    {
      throw new NotImplementedException();
    }

    public Task<string> ToAudio(string transcript)
    {
      throw new NotImplementedException();
    }

    public string ToTranscript(string audioFile)
    {
      throw new NotImplementedException();
    }
  }
}
