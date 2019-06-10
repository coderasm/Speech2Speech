using SpeechToSpeech.Commands;
using SpeechToSpeech.Models;
using SpeechToSpeech.Repositories;
using SpeechToSpeech.Services;
using SpeechToSpeech.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Unity.Attributes;

namespace SpeechToSpeech.ViewModels
{
  public class MainViewModel : INotifyPropertyChanged, IMainViewModel
  {
    private ISettingsService settingsService { get; set; }
    private ITextToSpeechRepository textToSpeechRepository { get; set; }
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
    [Dependency]
    public IFileManagementService fileManagementService { get; set; }
    private Hotkey hotkeys;
    private Settings settings;
    public ICommand PlayCmd { get; set; }
    public ICommand StopCmd { get; set; }
    public ICommand DeleteCmd { get; set; }
    public RoutedCommand PauseCmd { get; set; } = new RoutedCommand();
    public RoutedCommand MuteCmd { get; set; } = new RoutedCommand();
    public RoutedCommand VolumeCmd { get; set; } = new RoutedCommand();
    public RoutedCommand BalanceCmd { get; set; } = new RoutedCommand();


    public event PropertyChangedEventHandler PropertyChanged;
    private ObservableCollection<TextToSpeech> _textToSpeeches = new ObservableCollection<TextToSpeech>();
    public ObservableCollection<TextToSpeech> TextToSpeeches
    {
      get
      {
        if (_textToSpeeches.Count == 0)
          Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
          {
            var collection = new ObservableCollection<TextToSpeech>();
            var results = await textToSpeechRepository.GetAll();
            results.ForEach(service => collection.Add(service));
            TextToSpeeches = collection;
          });
        return _textToSpeeches;
      }
      set
      {
        _textToSpeeches.Clear();
        _textToSpeeches.AddRange(value);
        //NotifyPropertyChanged("TextToSpeeches");
      }
    }

    public MainViewModel(ISettingsService settingsService, ITextToSpeechRepository textToSpeechRepository)
    {
      this.settingsService = settingsService;
      settings = settingsService.settings;
      this.textToSpeechRepository = textToSpeechRepository;
      PlayCmd = new PlayCommand(this);
      StopCmd = new StopCommand(this);
      DeleteCmd = new DeleteCommand(this);
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
        amazonWebService, googleWebService, ibmWebService
      };
      var activeService = webServices[settings.generalSettings.ActiveTextToSpeechService - 1];
      audioFile = await activeService.ToAudio(text);
      var textTospeech = new TextToSpeech { Text = text, AudioFile = audioFile };
      textTospeech.Id = await textToSpeechRepository.Insert(textTospeech);
      TextToSpeeches.Add(textTospeech);
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
        .OnPlay(() => { hotkeys.BroadcastDown(); })
        .OnPlayStopped(() => {
          if (settings.generalSettings.IsAppPush2Talk)
            Task.Run(async () => {
              await Task.Delay(settings.generalSettings.KeyUpDelay * 1000);
              hotkeys.BroadcastUp();
            });
        })
        .Play(audioFileName, settings.generalSettings.AudioOutDevice);
    }

    public void StopHandler()
    {
      stopAudio();
    }

    private void stopAudio()
    {
      audioService.Stop();
    }

    public void DeleteHandler(object parameter)
    {
      deleteTextToSpeechEntry(parameter as TextToSpeech);
    }

    private async void deleteTextToSpeechEntry(TextToSpeech toRemove)
    {
      var result = await textToSpeechRepository.Delete(toRemove.Id);
      if (result) {
        var remaining = TextToSpeeches.Where(textToSpeech => textToSpeech.Id != toRemove.Id);
        TextToSpeeches.Clear();
        TextToSpeeches.AddRange(remaining);
        fileManagementService.Delete(toRemove.AudioFile);
      }
    }

    private void NotifyPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}
