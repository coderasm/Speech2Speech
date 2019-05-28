﻿using IBM.WatsonDeveloperCloud.TextToSpeech.v1.Model;
using IBM.WatsonDeveloperCloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBM.WatsonDeveloperCloud.Util;
using IBM.WatsonDeveloperCloud.TextToSpeech.v1;
using IBM.WatsonDeveloperCloud.SpeechToText.v1;
using System.Windows;

namespace SpeechToSpeech
{
  public class IBMWebService : ITranscribeAndVocalize<Voice>
  {
    private TokenOptions textToSpeechTokenOptions;
    private TokenOptions speechToTextTokenOptions;
    private TextToSpeechService textToSpeechClient;
    private SpeechToTextService speechToTextClient;
    private Settings settings = new Settings();

    private IBMWebService(Settings settings)
    {
      this.settings = settings;
      createClients();
    }

    public void createClients()
    {
      try
      {
        if (settings.ibmSettings.IamAPIKey != "" && settings.ibmSettings.speechToTextURL != "" && settings.ibmSettings.textToSpeechURL != "")
        {
          textToSpeechTokenOptions = new TokenOptions
          {
            IamApiKey = settings.ibmSettings.IamAPIKey,
            ServiceUrl = settings.ibmSettings.textToSpeechURL
          };
          speechToTextTokenOptions = new TokenOptions
          {
            IamApiKey = settings.ibmSettings.IamAPIKey,
            ServiceUrl = settings.ibmSettings.speechToTextURL
          };
          textToSpeechClient = new TextToSpeechService(textToSpeechTokenOptions);
          speechToTextClient = new SpeechToTextService(speechToTextTokenOptions);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"IBM credentials not set. Error: {e.Message}");
        MessageBox.Show($"IBM credentials not set. Error: {e.Message}");
      }
    }

    public static IBMWebService Create(Settings settings)
    {
      return new IBMWebService(settings);
    }

    public async Task<List<Voice>> GetVoices()
    {
      if (textToSpeechClient == null)
        return new List<Voice>();
      var voices = await Task.Run(() =>
      {
        return textToSpeechClient.ListVoices();
      });
      return voices._Voices.Where(voice => voice.Language.Contains(settings.generalSettings.TextInputLanguage)).ToList();
    }

    public async Task<List<Voice>> GetVoices(string language)
    {
      if (textToSpeechClient == null)
        return new List<Voice>();
      var voices = await Task.Run(() =>
      {
        return textToSpeechClient.ListVoices();
      });
      return voices._Voices.Where(voice => voice.Language.Contains(language)).ToList();
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
                    voice: settings.ibmSettings.Voice.Name
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
