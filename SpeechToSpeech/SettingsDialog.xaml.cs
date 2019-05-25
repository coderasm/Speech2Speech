using Google.Cloud.TextToSpeech.V1;
using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
    private CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
    private OpenFileDialog openFileDialog = new OpenFileDialog();

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
      audioInDeviceBox.ItemsSource = audioDevices;
      textLanguageBox.ItemsSource = languages;
      speechLanguageBox.ItemsSource = languages;
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
      audioInDeviceBox.SelectedValue = settingsService.settings.generalSettings.AudioInDevice;
      audioOutDeviceBox.SelectedValue = settingsService.settings.generalSettings.AudioOutDevice;
      textLanguageBox.SelectedValue = settingsService.settings.generalSettings.TextInputLanguage;
      textLanguageBox.SelectedValue = settingsService.settings.generalSettings.SpeechInputLanguage;
      populateVoiceLists(settingsService.settings.generalSettings.SpeechInputLanguage);
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
        Key = voice.Name,
        Value = voice
      });
      googleVoiceListBox.ItemsSource = googleVoicesMapped;
      googleVoiceListBox.DisplayMemberPath = "Key";
      googleVoiceListBox.SelectedValuePath = "Value";
      var amazonVoices = amazonWebService.GetVoices(language);
      var ibmVoices = ibmWebService.GetVoices(language);
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
      }
    }

    private void promptForIBMKey_Click(object sender, RoutedEventArgs e)
    {
      if (openFileDialog.ShowDialog() == true)
      {
        settingsService.settings.ibmSettings.IamAPIKey = openFileDialog.FileName;
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
    }

    private void amazonSecretAccessKey_Changed(object sender, TextChangedEventArgs e)
    {
      settingsService.settings.amazonSettings.SecretAccessKey = ((TextBox)sender).Text;
    }

    private void ibmTextToSpeechURL_Changed(object sender, TextChangedEventArgs e)
    {
      settingsService.settings.ibmSettings.textToSpeechURL = ((TextBox)sender).Text;
    }

    private void ibmSpeechToTextURL_Changed(object sender, TextChangedEventArgs e)
    {
      settingsService.settings.ibmSettings.speechToTextURL = ((TextBox)sender).Text;
    }

    private void audioInDeviceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var combobox = (ComboBox)sender;
      settingsService.settings.generalSettings.AudioInDevice = (int)combobox.SelectedValue;
    }
  }

}
