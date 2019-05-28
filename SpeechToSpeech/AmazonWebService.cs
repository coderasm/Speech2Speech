using Amazon.Polly;
using Amazon.Polly.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SpeechToSpeech
{
  public class AmazonWebService : ITranscribeAndVocalize<Voice>
  {
    private AmazonPollyClient client;
    private Settings settings = new Settings();
    private List<Voice> voiceCache = new List<Voice>();

    private AmazonWebService(Settings settings)
    {
      this.settings = settings;
      createClients();
    }

    public void createClients()
    {
      try
      {
        if (settings.amazonSettings.AccessKeyId != "" && settings.amazonSettings.SecretAccessKey != "")
          client = new AmazonPollyClient(
            settings.amazonSettings.AccessKeyId,
            settings.amazonSettings.SecretAccessKey,
            settings.amazonSettings.RegionEndpoint
            );
      }
      catch (Exception e)
      {
        Console.WriteLine($"Amazon credentials not set. Error: {e}");
        MessageBox.Show($"Amazon credentials not set. Error: {e}");
      }
    }

    public static AmazonWebService Create(Settings settings)
    {
      return new AmazonWebService(settings);
    }

    public async Task<List<Voice>> GetVoices()
    {
      if (client == null)
        return new List<Voice>();
      var cachedVoices = voiceCache.Where(voice => voice.Language == settings.generalSettings.TextInputLanguage);
      if (cachedVoices.Count() > 0)
        return cachedVoices.ToList();
      var voiceRequest = new DescribeVoicesRequest();
      voiceRequest.LanguageCode = settings.generalSettings.TextInputLanguage;
      var fetchedVoices = await FetchVoices(voiceRequest);
      voiceCache.AddRange(fetchedVoices);
      return fetchedVoices;
    }

    public async Task<List<Voice>> GetVoices(string language)
    {
      if (client == null)
        return new List<Voice>();
      var cachedVoices = voiceCache.Where(voice => voice.Language == language);
      if (cachedVoices.Count() > 0)
        return cachedVoices.ToList();
      var voiceRequest = new DescribeVoicesRequest();
      voiceRequest.LanguageCode = language;
      var fetchedVoices = await FetchVoices(voiceRequest);
      voiceCache.AddRange(fetchedVoices);
      return fetchedVoices;
    }

    public async Task<List<Voice>> FetchVoices(DescribeVoicesRequest voiceRequest)
    {
      try
      {
        var voices = new List<Amazon.Polly.Model.Voice>();
        string nextToken;
        do
        {
          var allVoicesResult = await client.DescribeVoicesAsync(voiceRequest);
          nextToken = allVoicesResult.NextToken;
          voiceRequest.NextToken = nextToken;
          voices.AddRange(allVoicesResult.Voices);
        } while (nextToken != null);
        return voices.Select(voice =>
        new Voice
        {
          Id = voice.Id,
          Name = voice.Name,
          Gender = voice.Gender,
          Language = voiceRequest.LanguageCode
        }
        ).ToList();
      }
      catch (Exception e)
      {
        Console.WriteLine("Exception caught: " + e);
        MessageBox.Show("Exception caught: " + e);
      }
      return new List<Voice>();
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
      synthesizeSpeechRequest.VoiceId = settings.amazonSettings.Voice.Id;
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
