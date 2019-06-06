﻿using Amazon.Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SToSVoice = SpeechToSpeech.Models.Voice;
using Amazon.Polly.Model;

namespace SpeechToSpeech.Services
{
  public class AmazonWebService : ITranscribeAndVocalize<SToSVoice>
  {
    private AmazonPollyClient client;
    private ISettingsService settingsService;
    private List<SToSVoice> voiceCache = new List<SToSVoice>();

    public AmazonWebService(ISettingsService settingsService)
    {
      this.settingsService = settingsService;
      CreateClients();
    }

    public void CreateClients()
    {
      try
      {
        if (settingsService.settings.amazonSettings.AccessKeyId != "" && settingsService.settings.amazonSettings.SecretAccessKey != "")
          client = new AmazonPollyClient(
            settingsService.settings.amazonSettings.AccessKeyId,
            settingsService.settings.amazonSettings.SecretAccessKey,
            settingsService.settings.amazonSettings.RegionEndpoint
            );
      }
      catch (Exception e)
      {
        Console.WriteLine($"Amazon credentials not set. Error: {e}");
        MessageBox.Show($"Amazon credentials not set. Error: {e}");
      }
    }

    public async Task<List<SToSVoice>> GetVoices()
    {
      if (client == null)
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
      if (client == null)
        return new List<SToSVoice>();
      if (voiceCache.Count() == 0)
        return await FetchVoices(language);
      return voiceCache.Where(voice =>
        voice.Language == language
      )
      .ToList();
    }

    public async Task<List<SToSVoice>> FetchVoices(string language)
    {
      try
      {
        var voiceRequest = new DescribeVoicesRequest();
        var voices = new List<Amazon.Polly.Model.Voice>();
        string nextToken;
        do
        {
          var allVoicesResult = await client.DescribeVoicesAsync(voiceRequest);
          nextToken = allVoicesResult.NextToken;
          voiceRequest.NextToken = nextToken;
          voices.AddRange(allVoicesResult.Voices);
        } while (nextToken != null);
        voiceCache = voices.Select(voice =>
        new SToSVoice
        {
          VoiceId = voice.Id,
          Name = voice.Name,
          Gender = voice.Gender,
          Language = voice.LanguageCode
        }
        ).ToList();
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
      if (client == null)
        return "";
      var BUFFER_SIZE = 2048;
      var timeStamp = DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss");
      var outputFileName = $@".\vocalized\{timeStamp}.mp3";

      SynthesizeSpeechRequest synthesizeSpeechRequest = new SynthesizeSpeechRequest();
      synthesizeSpeechRequest.OutputFormat = OutputFormat.Mp3;
      synthesizeSpeechRequest.VoiceId = settingsService.settings.amazonSettings.Voice.VoiceId;
      synthesizeSpeechRequest.Text = transcript;
      try
      {
        using (FileStream fileStream = new FileStream(outputFileName, FileMode.Create))
        {
          var synthesizeSpeechResponse = await client.SynthesizeSpeechAsync(synthesizeSpeechRequest);
          byte[] buffer = new byte[BUFFER_SIZE];
          int readBytes;

          using (BufferedStream bufferedInput = new BufferedStream(synthesizeSpeechResponse.AudioStream))
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
