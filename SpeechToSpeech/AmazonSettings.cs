using Amazon;
using Amazon.Polly.Model;

namespace SpeechToSpeech
{
  public class AmazonSettings
  {
    public string AccessKeyId { get; set; } = "";
    public string SecretAccessKey { get; set; } = "";
    public Voice Voice { get; set; }
    public RegionEndpoint RegionEndpoint { get; set; } = RegionEndpoint.USWest1;
  }
}
