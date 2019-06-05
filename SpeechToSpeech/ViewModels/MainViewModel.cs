using SpeechToSpeech.Services;
using SpeechToSpeech.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using Unity.Attributes;

namespace SpeechToSpeech.ViewModels
{
  public class MainViewModel : INotifyPropertyChanged
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
    private Settings settings;
    public RoutedCommand PlayCmd { get; set; } = new RoutedCommand();
    public RoutedCommand StopCmd { get; set; } = new RoutedCommand();
    public RoutedCommand PauseCmd { get; set; } = new RoutedCommand();
    public RoutedCommand MuteCmd { get; set; } = new RoutedCommand();
    public RoutedCommand VolumeCmd { get; set; } = new RoutedCommand();
    public RoutedCommand BalanceCmd { get; set; } = new RoutedCommand();


    public event PropertyChangedEventHandler PropertyChanged;

    private IAudioService audioService { get; set; }
    public ObservableCollection<TextToSpeech> TextToSpeeches { get; set; } = new ObservableCollection<TextToSpeech>();

    public MainViewModel(ISettingsService settingsService, IAudioService audioService, IDialogService dialogService)
    {

      this.settingsService = settingsService;
      this.settings = settingsService.settings;
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

    public void openSettingsDialog()
    {
      if (dialogService.ShowDialog<SettingsDialog>() == true)
      {
        settingsService.SaveSettings();
      }
    }

    public async void vocalizeText(string text)
    {
      string audioFile = "";
      var webServices = new ITranscribeAndVocalize<Voice>[]
      {
        googleWebService, amazonWebService, ibmWebService
      };
      var activeService = webServices[settings.generalSettings.ActiveTextToSpeechService];
      audioFile = await activeService.ToAudio(text);
      TextToSpeeches.Add(new TextToSpeech { Text = text, AudioFile = audioFile });
      if (audioFile != "" && settings.generalSettings.IsAutoPlayVocalized)
        playFile(audioFile);
    }

    private void playFile(string audioFileName)
    {
      hotkeys = Hotkey.Create(settings.generalSettings.AppPush2TalkKey);
      audioService
        .OnPlay(() => { if (settings.generalSettings.IsAppPush2Talk) hotkeys.BroadcastDown(); })
        .OnPlayStopped(() => { if (settings.generalSettings.IsAppPush2Talk) hotkeys.BroadcastUp(); })
        .Play(audioFileName, settings.generalSettings.AudioOutDevice);
    }

    private void PlayCmdExecuted(object sender, ExecutedRoutedEventArgs e)
    {
      var target = e.Source as Button;
      if (target != null)
      {

      }
    }

    // CanExecuteRoutedEventHandler for the custom color command.
    private void PlayCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
      if (e.Source is Button)
      {
        e.CanExecute = true;
      }
      else
      {
        e.CanExecute = false;
      }
    }

    public void stopAudio()
    {
      audioService.Stop();
    }

    private void NotifyPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}
