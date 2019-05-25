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
    private SettingsDialog OptionsDialog;
    private SettingsService settingsService;
    private GoogleWebService googleWebService;
    private AmazonWebService amazonWebService;
    private IBMWebService ibmWebService;

    public MainWindow()
    {
      InitializeComponent();
      settingsService = SettingsService.Create();
      googleWebService = GoogleWebService.Create(settingsService.settings);
      amazonWebService = AmazonWebService.Create(settingsService.settings);
      ibmWebService = IBMWebService.Create(settingsService.settings);
    }

    private void OnOptionsClicked(object sender, RoutedEventArgs e)
    {
      OptionsDialog = new SettingsDialog(settingsService, googleWebService, amazonWebService, ibmWebService);
      if (OptionsDialog.ShowDialog() == true)
      {
        settingsService.saveSettings();
      }
    }
  }
}
