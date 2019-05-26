using Amazon.Polly;
using Amazon.Polly.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpeechToSpeech
{
  public class AmazonWebService : ITranscribeAndVocalize<Voice>
  {
    private AmazonPollyClient client;
    private Settings settings = new Settings();

    private AmazonWebService(Settings settings)
    {
      this.settings = settings;
      //client = new AmazonPollyClient(settings.amazonSettings.AccessKeyId, settings.amazonSettings.SecretAccessKey);
    }

    public static AmazonWebService Create(Settings settings)
    {
      return new AmazonWebService(settings);
    }

    public async Task<List<Voice>> GetVoices(string language)
    {
      var voiceRequest = new DescribeVoicesRequest();
      voiceRequest.LanguageCode = language;
      return await FetchVoices(voiceRequest);
    }

    public async Task<List<Voice>> GetVoices()
    {
      var voiceRequest = new DescribeVoicesRequest();
      return await FetchVoices(voiceRequest);
    }

    public async Task<List<Voice>> FetchVoices(DescribeVoicesRequest voiceRequest)
    {
      var voices = new List<Voice>();
      try
      {
        string nextToken;
        do
        {
          var allVoicesResult = await client.DescribeVoicesAsync(voiceRequest);
          nextToken = allVoicesResult.NextToken;
          voiceRequest.NextToken = nextToken;
          voices = allVoicesResult.Voices;
        } while (nextToken != null);
        return voices;
      }
      catch (Exception e)
      {
        Console.WriteLine("Exception caught: " + e);
        MessageBox.Show("Exception caught: " + e);
      }
      return voices;
    }

    public async Task<string> ToAudio(string transcript)
    {
      var BUFFER_SIZE = 2048;
      var timeStamp = DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss");
      var outputFileName = $@".\vocalized/{timeStamp}.mp3";

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
