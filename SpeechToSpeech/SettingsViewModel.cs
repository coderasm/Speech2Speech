using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech.ViewModels
{
  public class SettingsViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private ISettingsService settingsService;
    private IAudioService audioService;
    private GoogleWebService googleWebService;
    private AmazonWebService amazonWebService;
    private IBMWebService ibmWebService;
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
    private bool listeningForPush2Talk = false;
    private bool listeningForAppPush2Talk = false;
    private List<Key> keysDown = new List<Key>();
    public Hotkey Push2TalkKeys { get; set; }
    public Hotkey AppPush2TalkKeys { get; set; }

    public SettingsViewModel(
      IAudioService audioService,
      ISettingsService settingsService,
      GoogleWebService googleWebService,
      AmazonWebService amazonWebService,
      IBMWebService ibmWebService
      )
    {
      settings = settingsService.settings;
      this.audioService = audioService;
      this.settingsService = settingsService;
      this.googleWebService = googleWebService;
      this.amazonWebService = amazonWebService;
      this.ibmWebService = ibmWebService;
      audioService.Devices.ForEach(audioDevice => audioDevices.Add(audioDevice));
      Push2TalkKeys = Hotkey.Create(settingsService.settings.generalSettings.Push2TalkKey);
      AppPush2TalkKeys = Hotkey.Create(settingsService.settings.generalSettings.AppPush2TalkKey);
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
      googleWebService.createClients();
    }

    public void UpdateAmazonWebService()
    {
      amazonWebService.createClients();
    }

    public void UpdateIBMWebService()
    {
      ibmWebService.createClients();
    }

    public void StopRecordingPush2TalkKeys(Action<Action> keyDownHandler, Action<Action> keyUpHandler)
    {

      KeyDown -= handlePush2TalkKeyDown;
      KeyUp -= removeDownKey;
      push2talkRecordButton.Content = "Record";
      ViewModel.ListeningForPush2Talk = false;
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
