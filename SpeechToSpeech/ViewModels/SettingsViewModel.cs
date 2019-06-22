using SpeechToSpeech.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using Unity.Attributes;
using SpeechToSpeech.Models;
using SpeechToSpeech.Repositories;
using System.Windows.Threading;

namespace SpeechToSpeech.ViewModels
{
  public class SettingsViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private ISettingsService settingsService;
    private IAudioPlayer audioService;
    [Dependency]
    public GoogleWebService googleWebService { get; set; }
    [Dependency]
    public AmazonWebService amazonWebService { get; set; }
    [Dependency]
    public IBMWebService ibmWebService { get; set; }
    [Dependency]
    public IWebServiceRepository webServiceRepository { get; set; }
    public bool ListeningForAppPush2Talk { get; set; } = false;
    public bool ListeningForPush2Talk { get; set; } = false;
    public List<Key> KeysDown = new List<Key>();
    public Settings settings { get; set; }
    private ObservableCollection<WebService> _webServices = new ObservableCollection<WebService>();
    public ObservableCollection<WebService> webServices
    {
      get
      {
        if (_webServices.Count == 0)
          Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
          {
            var collection = new ObservableCollection<WebService>();
            var results = await webServiceRepository.GetAll();
            results.ForEach(service => collection.Add(service));
            webServices = collection;
            settings.generalSettings.notifySelectedWebService();
          });
        return _webServices;
      }
      set
      {
        _webServices.Clear();
        _webServices.AddRange(value);
      }
    }
    public ObservableCollection<KeyValuePair<int, string>> audioDevices { get; set; } = new ObservableCollection<KeyValuePair<int, string>>();
    public ObservableCollection<KeyValuePair<string, Voice>> GoogleVoices { get; set; } = new ObservableCollection<KeyValuePair<string, Voice>>();
    public ObservableCollection<KeyValuePair<string, Voice>> AmazonVoices { get; set; } = new ObservableCollection<KeyValuePair<string, Voice>>();
    public ObservableCollection<KeyValuePair<string, Voice>> IBMVoices { get; set; } = new ObservableCollection<KeyValuePair<string, Voice>>();
    public IEnumerable<string> cultures { get; set; } = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(culture => culture.Name);
    private List<Key> keysDown = new List<Key>();
    public Hotkey AppPush2TalkKeys { get; set; }
    public Hotkey Push2TalkKeys { get; set; }

    public SettingsViewModel(
      IAudioPlayer audioService,
      ISettingsService settingsService
      )
    {
      settings = settingsService.settings;
      this.audioService = audioService;
      this.settingsService = settingsService;
      audioDevices.AddRange(audioService.Devices);
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
      var voices = googleVoices.Select(
        voice => new KeyValuePair<string, Voice>(
            voice.Name + ", " + voice.Gender,
            voice
        )
      );
      GoogleVoices.AddRange(voices);
      var amazonVoices = await amazonWebService.GetVoices(language);
      AmazonVoices.Clear();
      voices = amazonVoices.Select(
          voice => new KeyValuePair<string, Voice>(
              voice.Name + ", " + voice.Gender,
              voice
          )
        );
      AmazonVoices.AddRange(voices);
      var ibmVoices = await ibmWebService.GetVoices(language);
      IBMVoices.Clear();
      voices = ibmVoices.Select(
          voice => new KeyValuePair<string, Voice>(
              voice.Name + ", " + voice.Gender,
              voice
          )
        );
      IBMVoices.AddRange(voices);
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
      KeysDown = KeysDown.Where(key => key != e.Key).ToList();
    }

    private void handlePush2TalkKeyDown(object sender, KeyEventArgs e)
    {
      UpdatePush2TalkKeys(e.Key);
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
      UpdateAppPush2TalkKeys(e.Key);
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

    private void NotifyPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}
