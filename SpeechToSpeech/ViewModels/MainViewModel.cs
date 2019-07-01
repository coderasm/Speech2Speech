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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Unity;
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
    public IAudioPlayer audioService { get; set; }
    [Dependency]
    public IFileManagementService fileManagementService { get; set; }
    private Settings settings;
    private IUnityContainer container;
    public ICommand DeleteCmd { get; set; }


    public event PropertyChangedEventHandler PropertyChanged;
    private ObservableCollection<VocalizedViewModel> _vocalizedViewModels = new ObservableCollection<VocalizedViewModel>();
    public ObservableCollection<VocalizedViewModel> VocalizedViewModels
    {
      get
      {
        if (_vocalizedViewModels.Count == 0)
          Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
          {
            var collection = new ObservableCollection<VocalizedViewModel>();
            var results = await textToSpeechRepository.GetAll();
            var viewModels = results.Select(textToSpeech =>
            {
              return new VocalizedViewModel(textToSpeech, container.Resolve<IAudioPlayer>(), container.Resolve<ISettingsService>());
            });
            collection.AddRange(viewModels);
            VocalizedViewModels = collection;
          });
        return _vocalizedViewModels;
      }
      set
      {
        _vocalizedViewModels.Clear();
        _vocalizedViewModels.AddRange(value);
      }
    }

    public MainViewModel(ISettingsService settingsService, ITextToSpeechRepository textToSpeechRepository, IUnityContainer container)
    {
      this.container = container;
      this.settingsService = settingsService;
      settings = settingsService.settings;
      this.textToSpeechRepository = textToSpeechRepository;
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
      var textToSpeech = new TextToSpeech { Text = text, AudioFile = audioFile};
      textToSpeech.Id = await textToSpeechRepository.Insert(textToSpeech);
      var viewModel = new VocalizedViewModel(textToSpeech, container.Resolve<IAudioPlayer>(), container.Resolve<ISettingsService>());
      VocalizedViewModels.Add(viewModel);
      if (audioFile != "" && settings.generalSettings.IsAutoPlayVocalized)
        viewModel.PlayHandler(audioFile);
    }

    public void DeleteHandler(object parameter)
    {
      deleteTextToSpeechEntry(parameter as VocalizedViewModel);
    }

    private async void deleteTextToSpeechEntry(VocalizedViewModel vmToRemove)
    {
      var result = false;
      if (vmToRemove.TextToSpeech.Id != 0)
        result = await textToSpeechRepository.Delete(vmToRemove.TextToSpeech.Id);
      else
        result = await textToSpeechRepository.DeleteByAudioFile(vmToRemove.TextToSpeech.AudioFile);
      if (result)
      {
        var remaining = VocalizedViewModels.Where(viewModel => viewModel != vmToRemove);
        VocalizedViewModels.Clear();
        VocalizedViewModels.AddRange(remaining);
        vmToRemove.Dispose();
        fileManagementService.Delete(vmToRemove.TextToSpeech.AudioFile);
      }
    }

    private void NotifyPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}
