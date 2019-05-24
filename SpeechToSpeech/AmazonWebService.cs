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
    private AmazonPollyClient client = new AmazonPollyClient();

    public List<Voice> GetVoices(string language)
    {
      throw new NotImplementedException();
    }

    public async Task<List<List<Voice>>> GetVoices()
    {
      var allVoicesRequest = new DescribeVoicesRequest();
      var voices = new List<List<Voice>>();
      try
      {
        string nextToken;
        do
        {
          var allVoicesResult = await client.DescribeVoicesAsync(allVoicesRequest);
          nextToken = allVoicesResult.NextToken;
          allVoicesRequest.NextToken = nextToken;
          voices.Add(allVoicesResult.Voices);
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

    public async Task<FileStream> ToAudio(string transcript)
    {
      var BUFFER_SIZE = 2048;
      var timeStamp = DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss");
      var outputFileName = $"./vocalized/{timeStamp}.mp3";

      SynthesizeSpeechRequest synthesizeSpeechRequest = new SynthesizeSpeechRequest();
      synthesizeSpeechRequest.OutputFormat = OutputFormat.Mp3;
      synthesizeSpeechRequest.VoiceId = VoiceId.Joanna;
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
        return File.OpenRead(outputFileName);
      }
      catch (Exception e)
      {
        Console.WriteLine("Exception caught: " + e);
        MessageBox.Show("Exception caught: " + e);
      }
      return File.OpenRead(outputFileName);
    }

    public string ToTranscript(File audioFile)
    {
      throw new NotImplementedException();
    }
  }
}
