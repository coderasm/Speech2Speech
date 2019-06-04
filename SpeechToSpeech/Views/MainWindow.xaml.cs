using NAudio.Wave;
using SpeechToSpeech.Services;
using SpeechToSpeech.ViewModels;
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
    [Dependency]
    public MainViewModel ViewModel
    {
      set { DataContext = value; }
      get { return (MainViewModel)DataContext; }
    }

    public MainWindow()
    {
      InitializeComponent();
    }

    private void OnOptionsClicked(object sender, RoutedEventArgs e)
    {
      ViewModel.openSettingsDialog();
    }

    private void sendTextButton_Click(object sender, RoutedEventArgs e)
    {
      ViewModel.vocalizeText(textToSendBox.Text);
    }

    private void OnButtonStopClick(object sender, StoppedEventArgs args)
    {
      ViewModel.stopAudio();
    }
  }
}
