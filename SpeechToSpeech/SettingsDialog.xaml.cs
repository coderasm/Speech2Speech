using Google.Cloud.TextToSpeech.V1;
using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SpeechToSpeech
{
  /// <summary>
  /// Interaction logic for Options.xaml
  /// </summary>
  public partial class SettingsDialog : Window, IDisposable
  {
    private SettingsService settingsService;
    private GoogleWebService googleWebService;
    private AmazonWebService amazonWebService;
    private IBMWebService ibmWebService;
    private List<KeyValuePair<int, string>> webServiceLookup = new List<KeyValuePair<int, string>>
      {
        new KeyValuePair<int, string>(0, "Google"),
        new KeyValuePair<int, string>(1, "Amazon"),
        new KeyValuePair<int, string>(2, "IBM")
      };
    private CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
    private OpenFileDialog openFileDialog = new OpenFileDialog();
    private bool listeningForPush2Talk = false;
    private bool listeningForAppPush2Talk = false;
    private List<Key> keysDown = new List<Key>();
    private Hotkey push2TalkKeys;
    private Hotkey appPush2TalkKeys;

    public SettingsDialog(
      SettingsService settingsService,
      GoogleWebService googleWebService,
      AmazonWebService amazonWebService,
      IBMWebService ibmWebService
      )
    {
      this.settingsService = settingsService;
      this.googleWebService = googleWebService;
      this.amazonWebService = amazonWebService;
      this.ibmWebService = ibmWebService;
      InitializeComponent();
      Owner = Application.Current.Windows[0];
      var languages = cultures.Select(culture => culture.Name);
      var audioDevices = GetAudioDevices();
      audioOutDeviceBox.DisplayMemberPath = "Value";
      audioOutDeviceBox.SelectedValuePath = "Key";
      audioOutDeviceBox.ItemsSource = audioDevices;
      audioInDeviceBox.DisplayMemberPath = "Value";
      audioInDeviceBox.SelectedValuePath = "Key";
      textToSpeechServiceBox.ItemsSource = webServiceLookup;
      textToSpeechServiceBox.DisplayMemberPath = "Value";
      textToSpeechServiceBox.SelectedValuePath = "Key";
      audioInDeviceBox.ItemsSource = audioDevices;
      textLanguageBox.ItemsSource = languages;
      speechLanguageBox.ItemsSource = languages;
      Applysettings();
    }

    private List<KeyValuePair<int, string>> GetAudioDevices()
    {
      var audioDevices = new List<KeyValuePair<int, string>>();
      for (int n = -1; n < WaveOut.DeviceCount; n++)
      {
        var caps = WaveOut.GetCapabilities(n);
        audioDevices.Add(new KeyValuePair<int, string>(n, caps.ProductName));
      }
      return audioDevices;
    }

    private void Applysettings()
    {
      push2TalkCheckbox.IsChecked = settingsService.settings.generalSettings.IsPush2Talk;
      appPush2TalkCheckbox.IsChecked = settingsService.settings.generalSettings.IsAppPush2Talk;
      audioInDeviceBox.SelectedItem = GetAudioDevices().Where(device =>
      {
        return device.Key == settingsService.settings.generalSettings.AudioInDevice;
      }).First();
      audioOutDeviceBox.SelectedItem = GetAudioDevices().Where(device =>
      {
        return device.Key == settingsService.settings.generalSettings.AudioOutDevice;
      }).First();
      textToSpeechServiceBox.SelectedItem = webServiceLookup.Where(webService =>
      {
        return webService.Key == settingsService.settings.generalSettings.ActiveTextToSpeechService;
      }).First();
      textLanguageBox.SelectedItem = settingsService.settings.generalSettings.TextInputLanguage;
      speechLanguageBox.SelectedItem = settingsService.settings.generalSettings.SpeechInputLanguage;
      populateVoiceLists(settingsService.settings.generalSettings.SpeechInputLanguage);
      //googleVoiceListBox.SelectedItem = 
      push2TalkKeys = new Hotkey(settingsService.settings.generalSettings.Push2TalkKey);
      push2TalkBox.Text = push2TalkKeys.ToString();
      appPush2TalkKeys = new Hotkey(settingsService.settings.generalSettings.AppPush2TalkKey);
      appPush2TalkBox.Text = appPush2TalkKeys.ToString();
      googleAccountKeyBox.Text = settingsService.settings.googleSettings.ServiceAccountKey;
      amazonAccountKeyIdBox.Text = settingsService.settings.amazonSettings.AccessKeyId;
      amazonSecretAccessKeyBox.Text = settingsService.settings.amazonSettings.SecretAccessKey;
      IBMAccountKeyBox.Text = settingsService.settings.ibmSettings.IamAPIKey;
      IBMTexttoSpeechURLBox.Text = settingsService.settings.ibmSettings.textToSpeechURL;
      IBMSpeechtoTextURLBox.Text = settingsService.settings.ibmSettings.speechToTextURL;
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // TODO: dispose managed state (managed objects).
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        disposedValue = true;
      }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~OptionsDialog() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
    #endregion

    private void push2TalkCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      settingsService.settings.generalSettings.IsPush2Talk = ((CheckBox)sender).IsChecked;
    }

    private void appPush2TalkCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      settingsService.settings.generalSettings.IsAppPush2Talk = ((CheckBox)sender).IsChecked;
    }

    private void textLanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var combobox = (ComboBox)sender;
      settingsService.settings.generalSettings.TextInputLanguage = (string)combobox.SelectedValue;
      populateVoiceLists((string)combobox.SelectedValue);
    }

    private async void populateVoiceLists(string language)
    {
      var googleVoices = await googleWebService.GetVoices(language);
      var googleVoicesMapped = googleVoices.Select(voice => new
      {
        Key = voice.Name + ", " + voice.SsmlGender,
        Value = voice
      });
      googleVoiceListBox.ItemsSource = googleVoicesMapped;
      googleVoiceListBox.DisplayMemberPath = "Key";
      googleVoiceListBox.SelectedValuePath = "Value";
      var amazonVoices = await amazonWebService.GetVoices(language);
      var amazonVoicesMapped = amazonVoices.Select(voice => new
      {
        Key = voice.Name + ", " + voice.Gender,
        Value = voice
      });
      amazonVoiceListBox.ItemsSource = amazonVoicesMapped;
      amazonVoiceListBox.DisplayMemberPath = "Key";
      amazonVoiceListBox.SelectedValuePath = "Value";
      var ibmVoices = await ibmWebService.GetVoices(language);
      var ibmVoicesMapped = ibmVoices.Select(voice => new
      {
        Key = voice.Name + ", " + voice.Gender,
        Value = voice
      });
      ibmVoiceListBox.ItemsSource = ibmVoicesMapped;
      ibmVoiceListBox.DisplayMemberPath = "Key";
      ibmVoiceListBox.SelectedValuePath = "Value";
    }

    private void speechLanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var combobox = (ComboBox)sender;
      settingsService.settings.generalSettings.SpeechInputLanguage = (string)combobox.SelectedValue;
    }

    private void promptForGoogleKey_Click(object sender, RoutedEventArgs e)
    {
      if (openFileDialog.ShowDialog() == true)
      {
        settingsService.settings.googleSettings.ServiceAccountKey = openFileDialog.FileName;
        googleAccountKeyBox.Text = openFileDialog.FileName;
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", openFileDialog.FileName);
        googleWebService.createClients();
      }
    }

    private void promptForIBMKey_Click(object sender, RoutedEventArgs e)
    {
      if (openFileDialog.ShowDialog() == true)
      {
        settingsService.settings.ibmSettings.IamAPIKey = openFileDialog.FileName;
        ibmWebService.createClients();
      }
    }

    private void SaveSettings_Click(object sender, RoutedEventArgs e)
    {
      settingsService.saveSettings();
    }

    private void audioOutDeviceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var combobox = (ComboBox)sender;
      settingsService.settings.generalSettings.AudioOutDevice = (int)combobox.SelectedValue;
    }

    private void amazonAccessKeyId_Changed(object sender, TextChangedEventArgs e)
    {
      settingsService.settings.amazonSettings.AccessKeyId = ((TextBox)sender).Text;
      amazonWebService.createClients();
    }

    private void amazonSecretAccessKey_Changed(object sender, TextChangedEventArgs e)
    {
      settingsService.settings.amazonSettings.SecretAccessKey = ((TextBox)sender).Text;
      amazonWebService.createClients();
    }

    private void ibmTextToSpeechURLBox_Changed(object sender, TextChangedEventArgs e)
    {
      settingsService.settings.ibmSettings.textToSpeechURL = ((TextBox)sender).Text;
      ibmWebService.createClients();
    }

    private void ibmSpeechToTextURLBox_Changed(object sender, TextChangedEventArgs e)
    {
      settingsService.settings.ibmSettings.speechToTextURL = ((TextBox)sender).Text;
      ibmWebService.createClients();
    }

    private void audioInDeviceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var combobox = (ComboBox)sender;
      settingsService.settings.generalSettings.AudioInDevice = (int)combobox.SelectedValue;
    }

    private void textToSpeechServiceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var combobox = (ComboBox)sender;
      settingsService.settings.generalSettings.ActiveTextToSpeechService = (int)combobox.SelectedValue;
    }


    private void googleVoiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var listbox = (ListBox)sender;
      settingsService.settings.googleSettings.Voice = (Voice)listbox.SelectedValue;
    }

    private void amazonVoiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var listbox = (ListBox)sender;
      settingsService.settings.amazonSettings.Voice = (Amazon.Polly.Model.Voice)listbox.SelectedValue;
    }

    private void ibmVoiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var listbox = (ListBox)sender;
      settingsService.settings.ibmSettings.Voice = (IBM.WatsonDeveloperCloud.TextToSpeech.v1.Model.Voice)listbox.SelectedValue;
    }

    private void push2talkRecordButton_Click(object sender, RoutedEventArgs e)
    {
      if (!listeningForAppPush2Talk)
      {
        if (listeningForPush2Talk)
        {
          KeyDown -= handlePush2TalkKeyDown;
          KeyUp -= removeDownKey;
          push2talkRecordButton.Content = "Record";
          listeningForPush2Talk = false;
        }
        else
        {
          keysDown.Clear();
          KeyDown += handlePush2TalkKeyDown;
          KeyUp += removeDownKey;
          push2talkRecordButton.Content = "Stop Recording";
          listeningForPush2Talk = true;
        }
      }
    }

    private void removeDownKey(object sender, KeyEventArgs e)
    {
      var keyUp = e.Key == Key.System ? e.SystemKey : e.Key;
      keysDown = keysDown.Where(key => key != keyUp).ToList();
    }

    private void handlePush2TalkKeyDown(object sender, KeyEventArgs e)
    {
      var keyDown = e.Key == Key.System ? e.SystemKey : e.Key;
      updateKeysDown(keyDown);
      push2TalkKeys.HotKeys = keysDown.Select(key => key).ToList();
      push2TalkBox.Text = push2TalkKeys.ToString();
      settingsService.settings.generalSettings.Push2TalkKey = push2TalkKeys.HotKeys;
    }

    private void updateKeysDown(Key key)
    {
      if (keysDown.Contains(key))
        return;
      keysDown.Add(key);
    }

    private void appPush2talkRecordButton_Click(object sender, RoutedEventArgs e)
    {
      if (!listeningForPush2Talk)
      {
        if (listeningForAppPush2Talk)
        {
          KeyDown -= handleAppPush2TalkKeyDown;
          KeyUp -= removeDownKey;
          appPush2talkRecordButton.Content = "Record";
          listeningForAppPush2Talk = false;
        }
        else
        {
          keysDown.Clear();
          KeyDown += handleAppPush2TalkKeyDown;
          KeyUp += removeDownKey;
          appPush2talkRecordButton.Content = "Stop Recording";
          listeningForAppPush2Talk = true;
        }
      }
    }

    private void handleAppPush2TalkKeyDown(object sender, KeyEventArgs e)
    {
      var keyDown = e.Key == Key.System ? e.SystemKey : e.Key;
      updateKeysDown(keyDown);
      appPush2TalkKeys.HotKeys = keysDown.Select(key => key).ToList();
      appPush2TalkBox.Text = appPush2TalkKeys.ToString();
      settingsService.settings.generalSettings.AppPush2TalkKey = appPush2TalkKeys.HotKeys;
    }
  }

}
