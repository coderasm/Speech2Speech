using SpeechToSpeech.Commands;
using SpeechToSpeech.Models;
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
  public class MainViewModel : INotifyPropertyChanged, IMainViewModel
  {
    private ISettingsService settingsService { get; set; }
    [Dependency]
    public IDialogService dialogService { get; set; }
    [Dependency]
    public GoogleWebService googleWebService { get; set; }
    [Dependency]
    public AmazonWebService amazonWebService { get; set; }
    [Dependency]
    public IBMWebService ibmWebService { get; set; }
    [Dependency]
    public IAudioService audioService { get; set; }
    private Hotkey hotkeys;
    private Settings settings;
    public ICommand PlayCmd { get; set; }
    public ICommand StopCmd { get; set; }
    public RoutedCommand PauseCmd { get; set; } = new RoutedCommand();
    public RoutedCommand MuteCmd { get; set; } = new RoutedCommand();
    public RoutedCommand VolumeCmd { get; set; } = new RoutedCommand();
    public RoutedCommand BalanceCmd { get; set; } = new RoutedCommand();


    public event PropertyChangedEventHandler PropertyChanged;
    public ObservableCollection<TextToSpeech> TextToSpeeches { get; set; } = new ObservableCollection<TextToSpeech>();

    public MainViewModel(ISettingsService settingsService)
    {
      this.settingsService = settingsService;
      settings = settingsService.settings;
      PlayCmd = new PlayCommand(this);
      StopCmd = new StopCommand(this);
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

    public void PlayHandler(object parameter)
    {
      playFile(parameter as string);
    }

    private void playFile(string audioFileName)
    {
      hotkeys = Hotkey.Create(settings.generalSettings.AppPush2TalkKey);
      audioService
        .OnPlay(() => { if (settings.generalSettings.IsAppPush2Talk) hotkeys.BroadcastDown(); })
        .OnPlayStopped(() => { if (settings.generalSettings.IsAppPush2Talk) hotkeys.BroadcastUp(); })
        .Play(audioFileName, settings.generalSettings.AudioOutDevice);
    }

    public void StopHandler()
    {
      stopAudio();
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
