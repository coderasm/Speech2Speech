using Microsoft.Win32;
using SpeechToSpeech.Services;
using SpeechToSpeech.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity.Attributes;

namespace SpeechToSpeech.Views
{
  /// <summary>
  /// Interaction logic for Options.xaml
  /// </summary>
  public partial class SettingsDialog : Window, IDisposable
  {
    private OpenFileDialog openFileDialog = new OpenFileDialog();
    private IStringValidationService stringValidationService;
    [Dependency]
    public SettingsViewModel ViewModel
    {
      set { DataContext = value; }
      get { return (SettingsViewModel)DataContext; }
    }

    public SettingsDialog(IStringValidationService stringValidationService)
    {
      InitializeComponent();
      this.stringValidationService = stringValidationService;
    }

    private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
    {
      ViewModel.UpdateVoices();
    }

    private void textLanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var combobox = (ComboBox)sender;
      ViewModel.UpdateVoices((string)combobox.SelectedValue);
    }


    private void speechLanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
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

    private void amazonAccessKeyId_Changed(object sender, TextChangedEventArgs e)
    {
      ViewModel.UpdateAmazonWebService();
    }

    private void amazonSecretAccessKey_Changed(object sender, TextChangedEventArgs e)
    {
      ViewModel.UpdateAmazonWebService();
    }

    private void IBMTextToSpeechAPIKeyBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      ViewModel.UpdateIBMWebService();
    }

    private void IBMSpeechToTextAPIKeyBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      ViewModel.UpdateIBMWebService();
    }

    private void ibmTextToSpeechURLBox_Changed(object sender, TextChangedEventArgs e)
    {
      ViewModel.UpdateIBMWebService();
    }

    private void ibmSpeechToTextURLBox_Changed(object sender, TextChangedEventArgs e)
    {
      ViewModel.UpdateIBMWebService();
    }

    private void KeyUpDelayBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = !stringValidationService.isInteger(((TextBox)sender).Text + e.Text);
    }

    private void KeyUpDelayBox_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void push2talkRecordButton_Click(object sender, RoutedEventArgs e)
    {
      if (!ViewModel.ListeningForAppPush2Talk)
      {
        if (ViewModel.ListeningForPush2Talk)
        {
          Action<KeyEventHandler> keyDownLamda = (KeyEventHandler handler) => { KeyDown -= handler; };
          Action<KeyEventHandler> keyUpLamda = (KeyEventHandler handler) => { KeyUp -= handler; };
          ViewModel.StopRecordingPush2TalkKeys(keyDownLamda, keyUpLamda);
          push2talkRecordButton.Content = "Record";
          ViewModel.ListeningForPush2Talk = false;
        }
        else
        {
          Action<KeyEventHandler> keyDownLamda = (KeyEventHandler handler) => { KeyDown += handler; };
          Action<KeyEventHandler> keyUpLamda = (KeyEventHandler handler) => { KeyUp += handler; };
          ViewModel.StartRecordingPush2TalkKeys(keyDownLamda, keyUpLamda);
          push2talkRecordButton.Content = "Stop Recording";
          ViewModel.ListeningForPush2Talk = true;
        }
      }
    }

    private void appPush2talkRecordButton_Click(object sender, RoutedEventArgs e)
    {
      if (!ViewModel.ListeningForPush2Talk)
      {
        if (ViewModel.ListeningForAppPush2Talk)
        {
          Action<KeyEventHandler> keyDownLamda = (KeyEventHandler handler) => { KeyDown -= handler; };
          Action<KeyEventHandler> keyUpLamda = (KeyEventHandler handler) => { KeyUp -= handler; };
          ViewModel.StopRecordingAppPush2TalkKeys(keyDownLamda, keyUpLamda);
          appPush2talkRecordButton.Content = "Record";
          ViewModel.ListeningForAppPush2Talk = false;
        }
        else
        {
          Action<KeyEventHandler> keyDownLamda = (KeyEventHandler handler) => { KeyDown += handler; };
          Action<KeyEventHandler> keyUpLamda = (KeyEventHandler handler) => { KeyUp += handler; };
          ViewModel.StartRecordingAppPush2TalkKeys(keyDownLamda, keyUpLamda);
          appPush2talkRecordButton.Content = "Stop Recording";
          ViewModel.ListeningForAppPush2Talk = true;
        }
      }
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
