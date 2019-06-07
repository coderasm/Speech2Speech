using IBM.WatsonDeveloperCloud.TextToSpeech.v1.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IBM.WatsonDeveloperCloud.Util;
using IBM.WatsonDeveloperCloud.TextToSpeech.v1;
using IBM.WatsonDeveloperCloud.SpeechToText.v1;
using System.Windows;
using SToSVoice = SpeechToSpeech.Models.Voice;
using SpeechToSpeech.Repositories;

namespace SpeechToSpeech.Services
{
  public class IBMWebService : ITranscribeAndVocalize<SToSVoice>
  {
    private TokenOptions textToSpeechTokenOptions;
    private TokenOptions speechToTextTokenOptions;
    private TextToSpeechService textToSpeechClient;
    private SpeechToTextService speechToTextClient;
    private ISettingsService settingsService;
    private List<SToSVoice> voiceCache = new List<SToSVoice>();
    private IVoiceRepository voiceRepository;
    private int WEB_SERVICE_ID = 3;

    public IBMWebService(ISettingsService settingsService, IVoiceRepository voiceRepository)
    {
      this.settingsService = settingsService;
      this.voiceRepository = voiceRepository;
      CreateClients();
    }

    public void CreateClients()
    {
      try
      {
        if (settingsService.settings.ibmSettings.textToSpeechAPIKey != "" && settingsService.settings.ibmSettings.speechToTextAPIKey != "" &&
          settingsService.settings.ibmSettings.speechToTextURL != "" && settingsService.settings.ibmSettings.textToSpeechURL != "")
        {
          textToSpeechTokenOptions = new TokenOptions
          {
            IamApiKey = settingsService.settings.ibmSettings.textToSpeechAPIKey,
            ServiceUrl = settingsService.settings.ibmSettings.textToSpeechURL
          };
          speechToTextTokenOptions = new TokenOptions
          {
            IamApiKey = settingsService.settings.ibmSettings.speechToTextAPIKey,
            ServiceUrl = settingsService.settings.ibmSettings.speechToTextURL
          };
          textToSpeechClient = new TextToSpeechService(textToSpeechTokenOptions);
          speechToTextClient = new SpeechToTextService(speechToTextTokenOptions);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"IBM credentials not set. Error: {e}");
        MessageBox.Show($"IBM credentials not set. Error: {e}");
      }
    }

    public async Task<List<SToSVoice>> GetVoices()
    {
      if (textToSpeechClient == null)
        return new List<SToSVoice>();
      if (voiceCache.Count() == 0)
        return await FetchVoices(settingsService.settings.generalSettings.TextInputLanguage);
      return voiceCache.Where(voice =>
        voice.Language == settingsService.settings.generalSettings.TextInputLanguage
      )
      .ToList();
    }

    public async Task<List<SToSVoice>> GetVoices(string language)
    {
      if (textToSpeechClient == null)
        return new List<SToSVoice>();
      if (voiceCache.Count() == 0)
        return await FetchVoices(language);
      return voiceCache.Where(voice =>
        voice.Language == language
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
          var ibmVoices = await Task.Run(() =>
          {
            return textToSpeechClient.ListVoices();
          });
          voiceCache = ibmVoices._Voices.Select(voice =>
            new SToSVoice
            {
              ServiceId = WEB_SERVICE_ID,
              Name = voice.Name,
              Gender = voice.Gender,
              Language = voice.Language
            }
          ).ToList();
          voiceRepository.InsertMultiple(voiceCache);
        }
        else
          voiceCache = voices;
        return voiceCache.Where(voice => voice.Language == language).ToList();
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
      if (textToSpeechClient == null)
        return "";
      var BUFFER_SIZE = 2048;
      var timeStamp = DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss");
      var outputFileName = $@".\vocalized\{timeStamp}.mp3";
      var text = new Text();
      text._Text = transcript;
      try
      {
        using (FileStream fileStream = new FileStream(outputFileName, FileMode.Create))
        {
          var result = await Task.Run(() =>
          {
            return textToSpeechClient.Synthesize(
                    text: text,
                    accept: "audio/mp3",
                    voice: settingsService.settings.ibmSettings.Voice.Name
                    );
          });
          byte[] buffer = new byte[BUFFER_SIZE];
          int readBytes;

          using (BufferedStream bufferedInput = new BufferedStream(result))
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
