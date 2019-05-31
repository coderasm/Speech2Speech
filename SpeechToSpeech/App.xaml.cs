using SpeechToSpeech.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using Unity.Injection;

namespace SpeechToSpeech
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      IUnityContainer container = new UnityContainer();
      container.RegisterType<ISettingsService, SettingsService>();
      container.RegisterType<IAudioService, AudioService>();
      container.RegisterType<ITranscribeAndVocalize<Voice>, GoogleWebService>("GoogleWebService");
      container.RegisterType<ITranscribeAndVocalize<Voice>, AmazonWebService>("AmazonWebService");
      container.RegisterType<ITranscribeAndVocalize<Voice>, IBMWebService>("IBMWebService");

      MainWindow mainWindow = container.Resolve<MainWindow>();
      mainWindow.Show();
    }
  }
}
