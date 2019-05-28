using Amazon.Polly;
using Google.Cloud.TextToSpeech.V1;

namespace SpeechToSpeech
{
  public class Voice
  {
    public string Name { get; set; }
    public VoiceId Id { get; set; }
    public string Gender { get; set; }
    public SsmlVoiceGender SsmlGender { get; set; }
    public string Language { get; set; }
  }
}
