﻿using Google.Apis.Auth.OAuth2;
using Google.Cloud.Speech.V1;
using Google.Cloud.TextToSpeech.V1;
using Grpc.Auth;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpeechToSpeech
{
  public class GoogleWebService : ITranscribeAndVocalize<Voice>
  {
    private TextToSpeechClient toSpeechClient;
    private SpeechClient toTextClient;
    private Settings settings = new Settings();

    private GoogleWebService(Settings settings)
    {
      this.settings = settings;
      createClients();
    }

    public void createClients()
    {
      try
      {
        if (settings.googleSettings.ServiceAccountKey != "")
        {
          var credential = GoogleCredential.FromFile(settings.googleSettings.ServiceAccountKey)
            .CreateScoped(TextToSpeechClient.DefaultScopes);
          var channel = new Channel(TextToSpeechClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
          toSpeechClient = TextToSpeechClient.Create(channel);
          credential = GoogleCredential.FromFile(settings.googleSettings.ServiceAccountKey)
            .CreateScoped(SpeechClient.DefaultScopes);
          channel = new Channel(SpeechClient.DefaultEndpoint.ToString(), credential.ToChannelCredentials());
          toTextClient = SpeechClient.Create(channel);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"Google credentials not set. Error: {e.Message}");
        MessageBox.Show($"Google credentials not set. Error: {e.Message}");
      }
    }

    public static GoogleWebService Create(Settings settings)
    {
      return new GoogleWebService(settings);
    }

    public async Task<List<Voice>> GetVoices()
    {
      if (toSpeechClient == null)
        return new List<Voice>();
      var response = await toSpeechClient.ListVoicesAsync(new ListVoicesRequest
      {
        LanguageCode = settings.generalSettings.TextInputLanguage
      });
      return response.Voices.Select(voice => voice).ToList();
    }

    public async Task<List<Voice>> GetVoices(string language)
    {
      if (toSpeechClient == null)
        return new List<Voice>();
      var response = await toSpeechClient.ListVoicesAsync(new ListVoicesRequest
      {
        LanguageCode = language
      });
      return response.Voices.Select(voice => voice).ToList();
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
              Name = settings.googleSettings.Voice.Name,
              LanguageCode = settings.generalSettings.TextInputLanguage
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
      catch(Exception e)
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
