using Google.Cloud.TextToSpeech.V1;
using Microsoft.Win32;
using NAudio.Wave;
using SpeechToSpeech.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Unity;

namespace SpeechToSpeech
{
  /// <summary>
  /// Interaction logic for Options.xaml
  /// </summary>
  public partial class SettingsDialog : Window, IDisposable
  {
    private OpenFileDialog openFileDialog = new OpenFileDialog();

    [Dependency]
    public SettingsViewModel ViewModel
    {
      set { DataContext = value; }
      get { return (SettingsViewModel)DataContext; }
    }

    public SettingsDialog(
      )
    {
      InitializeComponent();
    }

    private void push2TalkCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      ViewModel.settings.generalSettings.IsPush2Talk = ((CheckBox)sender).IsChecked;
    }

    private void appPush2TalkCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      ViewModel.settings.generalSettings.IsAppPush2Talk = ((CheckBox)sender).IsChecked;
    }

    private void textLanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var combobox = (ComboBox)sender;
      ViewModel.UpdateVoices((string)combobox.SelectedValue);
    }


    private void speechLanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ViewModel.settings.generalSettings.SpeechInputLanguage = (string)((ComboBox)sender).SelectedValue;
    }

    private void promptForGoogleKey_Click(object sender, RoutedEventArgs e)
    {
      if (openFileDialog.ShowDialog() == true)
      {
        ViewModel.settings.googleSettings.ServiceAccountKey = openFileDialog.FileName;
        ViewModel.UpdateGoogleWebService();
      }
    }

    private void SaveSettings_Click(object sender, RoutedEventArgs e)
    {
      ViewModel.SaveSettings();
    }

    private void audioOutDeviceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var combobox = (ComboBox)sender;
      ViewModel.settings.generalSettings.AudioOutDevice = (int)combobox.SelectedValue;
    }

    private void amazonAccessKeyId_Changed(object sender, TextChangedEventArgs e)
    {
      ViewModel.settings.amazonSettings.AccessKeyId = ((TextBox)sender).Text;
      ViewModel.UpdateAmazonWebService();
    }

    private void amazonSecretAccessKey_Changed(object sender, TextChangedEventArgs e)
    {
      ViewModel.settings.amazonSettings.SecretAccessKey = ((TextBox)sender).Text;
      ViewModel.UpdateAmazonWebService();
    }

    private void IBMTextToSpeechAPIKeyBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      ViewModel.settings.ibmSettings.textToSpeechAPIKey = ((TextBox)sender).Text;
      ViewModel.UpdateIBMWebService();
    }

    private void IBMSpeechToTextAPIKeyBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      ViewModel.settings.ibmSettings.speechToTextAPIKey = ((TextBox)sender).Text;
      ViewModel.UpdateIBMWebService();
    }

    private void ibmTextToSpeechURLBox_Changed(object sender, TextChangedEventArgs e)
    {
      ViewModel.settings.ibmSettings.textToSpeechURL = ((TextBox)sender).Text;
      ViewModel.UpdateIBMWebService();
    }

    private void ibmSpeechToTextURLBox_Changed(object sender, TextChangedEventArgs e)
    {
      ViewModel.settings.ibmSettings.speechToTextURL = ((TextBox)sender).Text;
      ViewModel.UpdateIBMWebService();
    }

    private void audioInDeviceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ViewModel.settings.generalSettings.AudioInDevice = (int)((ComboBox)sender).SelectedValue;
    }

    private void textToSpeechServiceBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ViewModel.settings.generalSettings.ActiveTextToSpeechService = (int)((ComboBox)sender).SelectedValue;
    }


    private void googleVoiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ViewModel.settings.googleSettings.Voice = (Voice)((ListBox)sender).SelectedValue;
    }

    private void amazonVoiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ViewModel.settings.amazonSettings.Voice = (Voice)((ListBox)sender).SelectedValue;
    }

    private void ibmVoiceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      ViewModel.settings.ibmSettings.Voice = (Voice)((ListBox)sender).SelectedValue;
    }

    private void push2talkRecordButton_Click(object sender, RoutedEventArgs e)
    {
      if (!ViewModel.ListeningForAppPush2Talk)
      {
        if (ViewModel.ListeningForPush2Talk)
        {
          keyDownLamda = (action)
          KeyUp -= removeDownKey;
          push2talkRecordButton.Content = "Record";
          ViewModel.ListeningForPush2Talk = false;
        }
        else
        {
          ViewModel.KeysDown.Clear();
          KeyDown += handlePush2TalkKeyDown;
          KeyUp += removeDownKey;
          push2talkRecordButton.Content = "Stop Recording";
          ViewModel.ListeningForPush2Talk = true;
        }
      }
    }

    private void removeDownKey(object sender, KeyEventArgs e)
    {
      var keyUp = e.Key == Key.System ? e.SystemKey : e.Key;
      ViewModel.KeysDown = ViewModel.KeysDown.Where(key => key != keyUp).ToList();
    }

    private void handlePush2TalkKeyDown(object sender, KeyEventArgs e)
    {
      var key = e.Key == Key.System ? e.SystemKey : e.Key;
      ViewModel.UpdatePush2TalkKeys(key);
    }

    private void appPush2talkRecordButton_Click(object sender, RoutedEventArgs e)
    {
      if (!ViewModel.ListeningForPush2Talk)
      {
        if (ViewModel.ListeningForAppPush2Talk)
        {
          KeyDown -= handleAppPush2TalkKeyDown;
          KeyUp -= removeDownKey;
          appPush2talkRecordButton.Content = "Record";
          ViewModel.ListeningForAppPush2Talk = false;
        }
        else
        {
          ViewModel.KeysDown.Clear();
          KeyDown += handleAppPush2TalkKeyDown;
          KeyUp += removeDownKey;
          appPush2talkRecordButton.Content = "Stop Recording";
          ViewModel.ListeningForAppPush2Talk = true;
        }
      }
    }

    private void handleAppPush2TalkKeyDown(object sender, KeyEventArgs e)
    {
      var key = e.Key == Key.System ? e.SystemKey : e.Key;
      ViewModel.UpdateAppPush2TalkKeys(key);
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
  }

}
