using SpeechToSpeech.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity.Attributes;

namespace SpeechToSpeech.ViewModels
{
  public class SettingsViewModel
  {
    private ISettingsService settingsService;
    private IAudioService audioService;
    [Dependency]
    public GoogleWebService googleWebService { get; set; }
    [Dependency]
    public AmazonWebService amazonWebService { get; set; }
    [Dependency]
    public IBMWebService ibmWebService { get; set; }
    public bool ListeningForAppPush2Talk { get; set; } = false;
    public bool ListeningForPush2Talk { get; set; } = false;
    public List<Key> KeysDown = new List<Key>();
    public Settings settings { get; set; }
    public ObservableCollection<KeyValuePair<int, string>> webServiceLookup { get; set; } = new ObservableCollection<KeyValuePair<int, string>>
      {
        new KeyValuePair<int, string>(0, "Google"),
        new KeyValuePair<int, string>(1, "Amazon"),
        new KeyValuePair<int, string>(2, "IBM")

      };
    public ObservableCollection<KeyValuePair<int, string>> audioDevices { get; set; } = new ObservableCollection<KeyValuePair<int, string>>();
    public ObservableCollection<KeyValuePair<string, Voice>> GoogleVoices { get; set; } = new ObservableCollection<KeyValuePair<string, Voice>>();
    public ObservableCollection<KeyValuePair<string, Voice>> AmazonVoices { get; set; } = new ObservableCollection<KeyValuePair<string, Voice>>();
    public ObservableCollection<KeyValuePair<string, Voice>> IBMVoices { get; set; } = new ObservableCollection<KeyValuePair<string, Voice>>();
    public IEnumerable<string> cultures { get; set; } = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(culture => culture.Name);
    private List<Key> keysDown = new List<Key>();
    public Hotkey AppPush2TalkKeys { get; set; }
    public Hotkey Push2TalkKeys { get; set; }

    public SettingsViewModel(
      IAudioService audioService,
      ISettingsService settingsService
      )
    {
      settings = settingsService.settings;
      this.audioService = audioService;
      this.settingsService = settingsService;
      audioService.Devices.ForEach(audioDevice => audioDevices.Add(audioDevice));
      Push2TalkKeys = Hotkey.Create(settings.generalSettings.Push2TalkKey);
      AppPush2TalkKeys = Hotkey.Create(settings.generalSettings.AppPush2TalkKey);
    }

    public void UpdateVoices()
    {
      UpdateVoices(settings.generalSettings.TextInputLanguage);
    }

    public async void UpdateVoices(string language)
    {
      var googleVoices = await googleWebService.GetVoices(language);
      GoogleVoices.Clear();
      googleVoices.ForEach(voice =>
        GoogleVoices.Add(new KeyValuePair<string, Voice>
        (
          voice.Name + ", " + voice.Gender,
          voice
        )
      ));
      var amazonVoices = await amazonWebService.GetVoices(language);
      AmazonVoices.Clear();
      amazonVoices.ForEach(voice =>
        AmazonVoices.Add(new KeyValuePair<string, Voice>
        (
          voice.Name + ", " + voice.Gender,
          voice
        )
      ));
      var ibmVoices = await ibmWebService.GetVoices(language);
      IBMVoices.Clear();
      ibmVoices.ForEach(voice =>
        IBMVoices.Add(new KeyValuePair<string, Voice>
        (
          voice.Name + ", " + voice.Gender,
          voice
        )
      ));
    }

    public void SaveSettings()
    {
      settingsService.SaveSettings();
    }

    public void UpdateGoogleWebService()
    {
      googleWebService.CreateClients();
    }

    public void UpdateAmazonWebService()
    {
      amazonWebService.CreateClients();
    }

    public void UpdateIBMWebService()
    {
      ibmWebService.CreateClients();
    }

    public void StartRecordingPush2TalkKeys(Action<KeyEventHandler> keyDownLamda, Action<KeyEventHandler> keyUpLamda)
    {
      KeysDown.Clear();
      keyDownLamda(handlePush2TalkKeyDown);
      keyUpLamda(removeDownKey);
    }

    public void StopRecordingPush2TalkKeys(Action<KeyEventHandler> keyDownLamda, Action<KeyEventHandler> keyUpLamda)
    {
      keyDownLamda(handlePush2TalkKeyDown);
      keyUpLamda(removeDownKey);
    }

    private void removeDownKey(object sender, KeyEventArgs e)
    {
      var keyUp = e.Key == Key.System ? e.SystemKey : e.Key;
      KeysDown = KeysDown.Where(key => key != keyUp).ToList();
    }

    private void handlePush2TalkKeyDown(object sender, KeyEventArgs e)
    {
      var key = e.Key == Key.System ? e.SystemKey : e.Key;
      UpdatePush2TalkKeys(key);
    }

    public void StartRecordingAppPush2TalkKeys(Action<KeyEventHandler> keyDownLamda, Action<KeyEventHandler> keyUpLamda)
    {
      KeysDown.Clear();
      keyDownLamda(handleAppPush2TalkKeyDown);
      keyUpLamda(removeDownKey);
    }

    public void StopRecordingAppPush2TalkKeys(Action<KeyEventHandler> keyDownLamda, Action<KeyEventHandler> keyUpLamda)
    {
      keyDownLamda(handleAppPush2TalkKeyDown);
      keyUpLamda(removeDownKey);
    }

    private void handleAppPush2TalkKeyDown(object sender, KeyEventArgs e)
    {
      var key = e.Key == Key.System ? e.SystemKey : e.Key;
      UpdateAppPush2TalkKeys(key);
    }

    public void UpdatePush2TalkKeys(Key keyDown)
    {
      AddKeyOnce(keyDown);
      Push2TalkKeys.HotKeys = KeysDown.Select(key => key).ToList();
      settings.generalSettings.Push2TalkKey = Push2TalkKeys.HotKeys;
    }

    public void UpdateAppPush2TalkKeys(Key keyDown)
    {
      AddKeyOnce(keyDown);
      AppPush2TalkKeys.HotKeys = KeysDown.Select(key => key).ToList();
      settings.generalSettings.AppPush2TalkKey = AppPush2TalkKeys.HotKeys;
    }

    public void AddKeyOnce(Key key)
    {
      if (KeysDown.Contains(key))
        return;
      KeysDown.Add(key);
    }
  }
}
