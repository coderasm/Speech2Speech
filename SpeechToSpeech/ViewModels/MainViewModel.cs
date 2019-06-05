using SpeechToSpeech.Services;
using SpeechToSpeech.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Unity.Attributes;

namespace SpeechToSpeech.ViewModels
{
  public class MainViewModel: INotifyPropertyChanged
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

    public event PropertyChangedEventHandler PropertyChanged;

    private IAudioService audioService { get; set; }
    public ObservableCollection<TextToSpeech> TextToSpeeches { get; set; } = new ObservableCollection<TextToSpeech>();

    public MainViewModel(ISettingsService settingsService, IAudioService audioService, IDialogService dialogService)
    {

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
      var activeService = webServices[settingsService.settings.generalSettings.ActiveTextToSpeechService];
      audioFile = await activeService.ToAudio(text);
      TextToSpeeches.Add(new TextToSpeech { Text = text, AudioFile = audioFile });
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
