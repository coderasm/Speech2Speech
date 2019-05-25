using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SpeechToSpeech
{
  public class SettingsService
  {
    private Settings _settings = new Settings();
    private string settingsFile = @".\settings.json";
    public Settings settings {
      get {
        return _settings;
      }
    }

    private SettingsService()
    {
      loadSettings();
    }

    public static SettingsService Create()
    {
      return new SettingsService();
    }

    public void loadSettings()
    {
      try
      {
        var settingsJSON = File.ReadAllText(settingsFile);
        _settings = JsonConvert.DeserializeObject<Settings>(settingsJSON);
      }
      catch(FileNotFoundException e)
      {
        Console.WriteLine("Settings file not found.");
        MessageBox.Show("Settings file not found.");
      }
    }

    public void saveSettings()
    {
      var settingsJSON = JsonConvert.SerializeObject(_settings);
      var unicodeEncoding = new UnicodeEncoding();
      var buffer = unicodeEncoding.GetBytes(settingsJSON);
      try
      {
        using (var fileStream = File.Open(settingsFile, FileMode.Create))
        {
          fileStream.Seek(0, SeekOrigin.End);
          fileStream.WriteAsync(buffer, 0, buffer.Length);
        }
      }
      catch(Exception e)
      {
        Console.WriteLine(e.Message);
        MessageBox.Show(e.Message);
      }
    }
  }
}
