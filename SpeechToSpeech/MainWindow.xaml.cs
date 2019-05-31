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
    private Hotkey hotkeys;
    private ITranscribeAndVocalize<Voice>[] webServices;
    private IAudioService audioService;

    public MainWindow(IAudioService audioService)
    {
      InitializeComponent();
      this.audioService = audioService;
      settingsService = SettingsService.Create();
      DatabaseService.Initialize(settingsService.settings);
      googleWebService = GoogleWebService.Create(settingsService.settings);
      amazonWebService = AmazonWebService.Create(settingsService.settings);
      ibmWebService = IBMWebService.Create(settingsService.settings);
      webServices = new ITranscribeAndVocalize<Voice>[]
      {
        googleWebService, amazonWebService, ibmWebService
      };
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
        settingsService.SaveSettings();
      }
    }

    private async void sendTextButton_Click(object sender, RoutedEventArgs e)
    {
      string audioFile = "";
      var activeService = webServices[settingsService.settings.generalSettings.ActiveTextToSpeechService];
      audioFile = await activeService.ToAudio(textToSendBox.Text);
      if (audioFile != "")
        playFile(audioFile);
    }

    private void playFile(string audioFileName)
    {
      hotkeys = Hotkey.Create(settingsService.settings.generalSettings.AppPush2TalkKey);
      audioService
        .OnPlay(() => hotkeys.BroadcastDown())
        .OnPlayStopped(() => hotkeys.BroadcastUp())
        .Play(audioFileName, settingsService.settings.generalSettings.AudioOutDevice);
    }

    private void OnButtonStopClick(object sender, StoppedEventArgs args)
    {
      audioService.Stop();
    }
  }
}
