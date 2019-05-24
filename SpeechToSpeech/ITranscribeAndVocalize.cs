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
    Task<FileStream> ToAudio(string transcript);
    string ToTranscript(File audioFile);
    Task<List<List<T>>> GetVoices();
  }
}
