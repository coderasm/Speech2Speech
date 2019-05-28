using NAudio.Wave;
using System.IO;
using System.Windows;

namespace SpeechToSpeech
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private SettingsDialog OptionsDialog;
    private SettingsService settingsService;
    private GoogleWebService googleWebService;
    private AmazonWebService amazonWebService;
    private IBMWebService ibmWebService;
    private WaveOutEvent outputDevice = null;
    private AudioFileReader audioFile = null;
    private Hotkey hotkeys;

    public MainWindow()
    {
      InitializeComponent();
      settingsService = SettingsService.Create();
      googleWebService = GoogleWebService.Create(settingsService.settings);
      amazonWebService = AmazonWebService.Create(settingsService.settings);
      ibmWebService = IBMWebService.Create(settingsService.settings);
      createFolders();
    }

    private void createFolders()
    {
      if (!Directory.Exists(@".\vocalized"))
        Directory.CreateDirectory(@".\vocalized");
      if (!Directory.Exists(@".\transcribed"))
        Directory.CreateDirectory(@".\transcribed");
    }

    private void OnOptionsClicked(object sender, RoutedEventArgs e)
    {
      OptionsDialog = new SettingsDialog(settingsService, googleWebService, amazonWebService, ibmWebService);
      if (OptionsDialog.ShowDialog() == true)
      {
        settingsService.saveSettings();
      }
    }

    private async void sendTextButton_Click(object sender, RoutedEventArgs e)
    {
      string audioFile = "";
      switch (settingsService.settings.generalSettings.ActiveTextToSpeechService)
      {
        case 0:
          audioFile = await googleWebService.ToAudio(textToSendBox.Text);
          break;
        case 1:
          audioFile = await amazonWebService.ToAudio(textToSendBox.Text);
          break;
        case 2:
          audioFile = await ibmWebService.ToAudio(textToSendBox.Text);
          break;
        default:
          break;
      }
      if (audioFile != "")
        playFile(audioFile);
    }

    private void playFile(string audioFileName)
    {
      if (outputDevice == null)
      {
        outputDevice = new WaveOutEvent() { DeviceNumber = settingsService.settings.generalSettings.AudioOutDevice };
        outputDevice.PlaybackStopped += OnPlaybackStopped;
      }
      if (audioFile == null)
      {
        audioFile = new AudioFileReader(audioFileName);
        outputDevice.Init(audioFile);
      }
      hotkeys = new Hotkey(settingsService.settings.generalSettings.AppPush2TalkKey);
      hotkeys.BroadcastDown();
      outputDevice.Play();
    }

    private void OnPlaybackStopped(object sender, StoppedEventArgs args)
    {
      hotkeys.BroadcastUp();
      outputDevice.Dispose();
      outputDevice = null;
      audioFile.Dispose();
      audioFile = null;
    }

    private void OnButtonStopClick(object sender, StoppedEventArgs args)
    {
      outputDevice?.Stop();
    }
  }
}
