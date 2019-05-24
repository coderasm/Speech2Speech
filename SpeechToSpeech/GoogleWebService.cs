using Google.Cloud.Speech.V1;
using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech
{
  class GoogleWebService : ITranscribeAndVocalize<Voice>
  {
    private TextToSpeechClient toSpeechClient;
    private SpeechClient toTextClient;
    private GoogleSettings settings = new GoogleSettings();

    private GoogleWebService(GoogleSettings settings)
    {
      this.settings = settings;
      toSpeechClient = TextToSpeechClient.Create();
      toTextClient = SpeechClient.Create();
    }

    public GoogleWebService Create(GoogleSettings settings)
    {
      return new GoogleWebService(settings);
    }
    public async Task<List<Voice>> GetVoices()
    {
      var response = await toSpeechClient.ListVoicesAsync(new ListVoicesRequest());
      return response.Voices.Select(voice => voice).ToList();
    }

    public async Task<List<Voice>> GetVoices(string language)
    {
      var response = await toSpeechClient.ListVoicesAsync(new ListVoicesRequest
      {
        LanguageCode = language
      });
      return response.Voices.Select(voice => voice).ToList();
    }

    public async Task<string> ToAudio(string transcript)
    {
      var timeStamp = DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss");
      var outputFileName = $"./vocalized/{timeStamp}.mp3";
      var response = await toSpeechClient.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
      {
        Input = new SynthesisInput
        {
          Text = transcript
        },
        // Note: voices can also be specified by name
        Voice = new VoiceSelectionParams
        {
          LanguageCode = "en-US",
          SsmlGender = SsmlVoiceGender.Female
        },
        AudioConfig = new AudioConfig
        {
          AudioEncoding = AudioEncoding.Mp3
        }
      });
      using (Stream output = File.Create(outputFileName))
      {
        response.AudioContent.WriteTo(output);
      }
      return outputFileName;
    }

    public string ToTranscript(string audioFile)
    {
      throw new NotImplementedException();
    }
  }
}
