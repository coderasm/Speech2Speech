﻿using System;
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
    public Options Options { get; set; } = new Options();
    public OptionsDialog OptionsDialog { get; set; }
    public MainWindow()
    {
      InitializeComponent();
      OptionsDialog  = new OptionsDialog(Options);
    }

    private void OnOptionsClicked(object sender, RoutedEventArgs e)
    {
      if (OptionsDialog.ShowDialog() == true)
      {

      }
    }
  }
}