using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpeechToSpeech
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public SettingsDialog OptionsDialog { get; set; }
    public SettingsService settingsService { get; set; } = new SettingsService();
    public MainWindow()
    {
      InitializeComponent();
    }

    private void OnOptionsClicked(object sender, RoutedEventArgs e)
    {
      OptionsDialog = new SettingsDialog(settingsService);
      if (OptionsDialog.ShowDialog() == true)
      {
        settingsService.saveSettings();
      }
    }
  }
}
