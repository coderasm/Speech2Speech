using NAudio.Wave;
using SpeechToSpeech.Services;
using System.IO;
using System.Windows;
using Unity.Attributes;

namespace SpeechToSpeech.Views
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private ISettingsService settingsService { get; set; }
    private IDialogService dialogService { get; set; }
    [Dependency]
    public GoogleWebService googleWebService { get; set; }
    [Dependency]
    public AmazonWebService amazonWebService { get; set; }
    [Dependency]
    public IBMWebService ibmWebService { get; set; }
    private Hotkey hotkeys;
    private IAudioService audioService { get; set; }

    public MainWindow(ISettingsService settingsService, IAudioService audioService, IDialogService dialogService)
    {
      InitializeComponent();
      this.settingsService = settingsService;
      this.dialogService = dialogService;
      this.audioService = audioService;
      DatabaseService.Initialize(settingsService.settings);
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
      if (dialogService.ShowDialog<SettingsDialog>() == true)
      {
        settingsService.SaveSettings();
      }
    }

    private async void sendTextButton_Click(object sender, RoutedEventArgs e)
    {
      string audioFile = "";
      var webServices = new ITranscribeAndVocalize<Voice>[]
      {
        googleWebService, amazonWebService, ibmWebService
      };
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
