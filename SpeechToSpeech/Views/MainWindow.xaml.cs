using MahApps.Metro.Controls;
using SpeechToSpeech.ViewModels;
using System.Windows;
using System.Windows.Input;
using Unity.Attributes;

namespace SpeechToSpeech.Views
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow
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
      vocalize();
    }

    private void TextToSendBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter)
        vocalize();
    }

    private void vocalize()
    {
      if (textToSendBox.Text != "")
      {
        ViewModel.vocalizeText(textToSendBox.Text);
        textToSendBox.Text = "";
      }
    }
  }
}
