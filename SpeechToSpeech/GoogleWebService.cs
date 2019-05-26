using Google.Cloud.Speech.V1;
using Google.Cloud.TextToSpeech.V1;
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
      //toSpeechClient = TextToSpeechClient.Create();
      //toTextClient = SpeechClient.Create();
    }

    public static GoogleWebService Create(Settings settings)
    {
      return new GoogleWebService(settings);
    }

    public async Task<List<Voice>> GetVoices()
    {
      var response = await toSpeechClient.ListVoicesAsync(new ListVoicesRequest
      {
        LanguageCode = settings.generalSettings.TextInputLanguage
      });
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
      var BUFFER_SIZE = 2048;
      var timeStamp = DateTime.Now.ToString("MM-dd-yyyy_HH_mm_ss");
      var outputFileName = $@".\vocalized/{timeStamp}.mp3";
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
              Name = settings.googleSettings.Voice.Name
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
