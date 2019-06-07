using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Grpc.Core;
using SpeechToSpeech.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SToSVoice = SpeechToSpeech.Models.Voice;

namespace SpeechToSpeech.Services
{
  public class GoogleWebService : ITranscribeAndVocalize<SToSVoice>
  {
    private TextToSpeechClient toSpeechClient;
    private SpeechClient toTextClient;
    private ISettingsService settingsService;
    private List<SToSVoice> voiceCache = new List<SToSVoice>();
    private IVoiceRepository voiceRepository;
    private int WEB_SERVICE_ID = 2;

    public GoogleWebService(ISettingsService settingsService, IVoiceRepository voiceRepository)
    {
      this.settingsService = settingsService;
      this.voiceRepository = voiceRepository;
      CreateClients();
    }

    public void CreateClients()
    {
      try
      {
        if (settingsService.settings.googleSettings.ServiceAccountKey != "")
        {
          //Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", settings.googleSettings.ServiceAccountKey);
          var credential = GoogleCredential.FromFile(settingsService.settings.googleSettings.ServiceAccountKey)
            .CreateScoped(TextToSpeechClient.DefaultScopes);
          var channel = new Channel(TextToSpeechClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
          toSpeechClient = TextToSpeechClient.Create(channel);
          credential = GoogleCredential.FromFile(settingsService.settings.googleSettings.ServiceAccountKey)
            .CreateScoped(SpeechClient.DefaultScopes);
          channel = new Channel(SpeechClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
          toTextClient = SpeechClient.Create(channel);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"Google credentials not set. Error: {e}");
        MessageBox.Show($"Google credentials not set. Error: {e}");
      }
    }

    public async Task<List<SToSVoice>> GetVoices()
    {
      if (toSpeechClient == null)
        return new List<SToSVoice>();
      if (voiceCache.Count() == 0)
        return await FetchVoices(settingsService.settings.generalSettings.TextInputLanguage);
      return voiceCache.Where(voice =>
        voice.Language.Contains(settingsService.settings.generalSettings.TextInputLanguage)
      )
      .ToList();
    }

    public async Task<List<SToSVoice>> GetVoices(string language)
    {
      if (toSpeechClient == null)
        return new List<SToSVoice>();
      if (voiceCache.Count() == 0)
        return await FetchVoices(language);
      return voiceCache.Where(voice =>
        voice.Language.Contains(language)
      )
      .ToList();
    }

    private async Task<List<SToSVoice>> FetchVoices(string language)
    {
      try
      {
        var voices = await voiceRepository.GetAllByService(WEB_SERVICE_ID);
        if (voices.Count == 0)
        {
          var request = new ListVoicesRequest();
          var response = await toSpeechClient.ListVoicesAsync(request);
          voiceCache = response.Voices.Select(voice =>
           new SToSVoice
           {
             ServiceId = WEB_SERVICE_ID,
             Name = voice.Name,
             SsmlGender = voice.SsmlGender,
             Language = string.Join(",", voice.LanguageCodes.ToArray())
           }
          ).ToList();
          voiceRepository.InsertMultiple(voiceCache);
        }
        else
          voiceCache = voices;
        return voiceCache.Where(voice => voice.Language.Contains(language)).ToList();
      }
      catch (Exception e)
      {
        Console.WriteLine("Exception caught: " + e);
        MessageBox.Show("Exception caught: " + e);
      }
      return new List<SToSVoice>();
    }

    public async Task<string> ToAudio(string transcript)
    {
      if (toSpeechClient == null)
        return "";
      var BUFFER_SIZE = 2048;
      var timeStamp = DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss");
      var outputFileName = $@".\vocalized\{timeStamp}.mp3";
      try
      {
        using (var fileStream = File.Create(outputFileName))
        {
          var response = await toSpeechClient.SynthesizeSpeechAsync(new SynthesizeSpeechRequest
          {
            Input = new SynthesisInput
            {
              Text = transcript
            },
            // Note: voices can also be specified by name
            Voice = new VoiceSelectionParams
            {
              Name = settingsService.settings.googleSettings.Voice.Name,
              LanguageCode = settingsService.settings.generalSettings.TextInputLanguage
            },
            AudioConfig = new AudioConfig
            {
              AudioEncoding = AudioEncoding.Mp3
            }
          });
          var ms = new MemoryStream(response.AudioContent.ToByteArray());
          byte[] buffer = new byte[BUFFER_SIZE];
          int readBytes;
          using (BufferedStream bufferedInput = new BufferedStream(ms))
          {
            while ((readBytes = await bufferedInput.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
              await fileStream.WriteAsync(buffer, 0, readBytes);
            }
          }
        }
        return outputFileName;
      }
      catch (Exception e)
      {
        Console.WriteLine("Exception caught: " + e);
        MessageBox.Show("Exception caught: " + e);
      }
      return outputFileName;
    }

    public string ToTranscript(string audioFile)
    {
      throw new NotImplementedException();
    }
  }
}
