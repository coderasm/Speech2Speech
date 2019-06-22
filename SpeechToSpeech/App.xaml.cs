using Prism.Ioc;
using Prism.Unity;
using SpeechToSpeech.Repositories;
using SpeechToSpeech.Services;
using SpeechToSpeech.Views;
using System.Windows;

namespace SpeechToSpeech
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : PrismApplication
  {
    protected override Window CreateShell()
    {
      return Container.Resolve<MainWindow>();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      Container.Resolve<DatabaseService>();
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
      containerRegistry.RegisterSingleton<IDatabaseService, DatabaseService>();
      containerRegistry.RegisterSingleton<IWebServiceRepository, WebServiceRepository>();
      containerRegistry.RegisterSingleton<ITextToSpeechRepository, TextToSpeechRepository>();
      containerRegistry.RegisterSingleton<IVoiceRepository, VoiceRepository>();
      containerRegistry.RegisterSingleton<IFileManagementService, FileMangementService>();
      containerRegistry.RegisterSingleton<IStringValidationService, StringValidationService>();
      containerRegistry.RegisterSingleton<GoogleWebService>();
      containerRegistry.RegisterSingleton<AmazonWebService>();
      containerRegistry.RegisterSingleton<IBMWebService>();
      containerRegistry.Register<SettingsDialog>();
      containerRegistry.RegisterSingleton<IDialogService, DialogService>();
      containerRegistry.RegisterInstance<IAudioPlayer>(new AudioPlayer());
    }
  }
}
