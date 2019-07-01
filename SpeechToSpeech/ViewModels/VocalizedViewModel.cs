using SpeechToSpeech.Commands;
using SpeechToSpeech.Models;
using SpeechToSpeech.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech.ViewModels
{
  public class VocalizedViewModel: IVocalizedViewModel, INotifyPropertyChanged, IDisposable
  {
    public event PropertyChangedEventHandler PropertyChanged;
    public TextToSpeech TextToSpeech { get; }
    public IAudioPlayer AudioPlayer { get;  }
    private ISettingsService settingsService;
    private Settings settings;
    private Hotkey hotkeys;
    public ICommand PlayCmd { get; set; }
    public ICommand StopCmd { get; set; }
    public ICommand PauseCmd { get; set; }
    public RoutedCommand MuteCmd { get; set; } = new RoutedCommand();
    public RoutedCommand BalanceCmd { get; set; } = new RoutedCommand();

    public VocalizedViewModel(TextToSpeech textToSpeech, IAudioPlayer audioPlayer, ISettingsService settingsService)
    {
      TextToSpeech = textToSpeech;
      AudioPlayer = audioPlayer;
      AudioPlayer.OutputDevice = settingsService.settings.generalSettings.AudioOutDevice;
      AudioPlayer.AudioFile = TextToSpeech.AudioFile;
      this.settingsService = settingsService;
      settings = settingsService.settings;
      PlayCmd = new PlayCommand(this);
      StopCmd = new StopCommand(this);
      PauseCmd = new PauseCommand(this);
    }

    public void PlayHandler(object parameter)
    {
      playFile(parameter as string);
    }

    private void playFile(string audioFileName)
    {
      hotkeys = Hotkey.Create(settings.generalSettings.AppPush2TalkKey);
      AudioPlayer
        .OnPlay(() => { hotkeys.BroadcastDown(); })
        .OnPlayStopped(() =>
        {
          if (settings.generalSettings.IsAppPush2Talk)
            Task.Run(async () =>
            {
              await Task.Delay(settings.generalSettings.KeyUpDelay);
              hotkeys.BroadcastUp();
            });
        })
        .Play(audioFileName, settings.generalSettings.AudioOutDevice);
    }

    public void PauseHandler()
    {
      AudioPlayer.Pause();
    }

    public void StopHandler()
    {
      AudioPlayer.Stop();
    }

    private void NotifyPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }

    public void Dispose()
    {
      AudioPlayer.Dispose();
    }
  }
}
