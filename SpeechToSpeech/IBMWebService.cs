using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech
{
  class IBMWebService : ITranscribeAndVocalize<object>
  {
    public List<object> GetVoices()
    {
      throw new NotImplementedException();
    }

    public File ToAudio(string transcript)
    {
      throw new NotImplementedException();
    }

    public string ToTranscript(File audioFile)
    {
      throw new NotImplementedException();
    }
  }
}
