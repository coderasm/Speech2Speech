using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Windows;

namespace SpeechToSpeech.Services
{
  public class SettingsService: ISettingsService
  {
    private Settings _settings;
    private string settingsFile = @".\settings.json";
    public Settings settings {
      get {
        return _settings;
      }
    }

    public SettingsService()
    {
      LoadSettings();
    }

    public void LoadSettings()
    {
      try
      {
        var settingsJSON = File.ReadAllText(settingsFile, Encoding.Unicode);
        _settings = JsonConvert.DeserializeObject<Settings>(settingsJSON);
        if (_settings == null)
          _settings = new Settings();
      }
      catch(FileNotFoundException e)
      {
        Console.WriteLine("Settings file not found. Creating one.");
        MessageBox.Show("Settings file not found. Creating one.");
        _settings = new Settings();
        SaveSettings();
      }
    }

    public async void SaveSettings()
    {
      var settingsJSON = JsonConvert.SerializeObject(_settings);
      var buffer = Encoding.Unicode.GetBytes(settingsJSON);
      try
      {
        using (var fileStream = File.Open(settingsFile, FileMode.Create))
        {
          await fileStream.WriteAsync(buffer, 0, buffer.Length);
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
