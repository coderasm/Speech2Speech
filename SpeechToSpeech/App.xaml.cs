﻿using Prism.Ioc;
using Prism.Unity;
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

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      containerRegistry.RegisterSingleton<ISettingsService, SettingsService>();
      containerRegistry.RegisterSingleton<GoogleWebService>();
      containerRegistry.RegisterSingleton<AmazonWebService>();
      containerRegistry.RegisterSingleton<IBMWebService>();
      containerRegistry.Register<SettingsDialog>();
      containerRegistry.RegisterSingleton<IDialogService, DialogService>();
      containerRegistry.RegisterInstance<IAudioService>(new AudioService());
    }
  }
}
