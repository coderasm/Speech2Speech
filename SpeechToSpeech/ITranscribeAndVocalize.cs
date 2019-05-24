using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech
{
  public interface ITranscribeAndVocalize<T>
  {
    Task<string> ToAudio(string transcript);
    string ToTranscript(string audioFile);
    Task<List<T>> GetVoices();
    Task<List<T>> GetVoices(string language);
  }
}
