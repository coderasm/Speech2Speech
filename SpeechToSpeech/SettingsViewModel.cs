using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech
{
  class SettingsViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private SettingsService settingsService;
    private GoogleWebService googleWebService;
    private AmazonWebService amazonWebService;
    private IBMWebService ibmWebService;
    public Settings settings { get; set; }
    public ObservableCollection<KeyValuePair<int, string>> webServiceLookup { get; set; } = new ObservableCollection<KeyValuePair<int, string>>
      {
        new KeyValuePair<int, string>(0, "Google"),
        new KeyValuePair<int, string>(1, "Amazon"),
        new KeyValuePair<int, string>(2, "IBM")

      };
    public ObservableCollection<KeyValuePair<int, string>> audioDevices { get; set; } = new ObservableCollection<KeyValuePair<int, string>>();
    public IEnumerable<string> cultures { get; set; } = CultureInfo.GetCultures(CultureTypes.AllCultures).Select(culture => culture.Name);
    private bool listeningForPush2Talk = false;
    private bool listeningForAppPush2Talk = false;
    private List<Key> keysDown = new List<Key>();
    private Hotkey push2TalkKeys;
    private Hotkey appPush2TalkKeys;

    public SettingsViewModel()
    {

    }

    private void NotifyPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}
