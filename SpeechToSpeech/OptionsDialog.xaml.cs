﻿using Google.Cloud.TextToSpeech.V1;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;

namespace SpeechToSpeech
{
  /// <summary>
  /// Interaction logic for Options.xaml
  /// </summary>
  public partial class OptionsDialog : Window, IDisposable
  {
    private Options options { get; set; }
    private CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
    private OpenFileDialog openFileDialog = new OpenFileDialog();

    public OptionsDialog(Options options )
    {
      this.options = options;
      InitializeComponent();
      var languages = cultures.Select(culture => culture.Name);
      textLanguageBox.ItemsSource = languages;
      speechLanguageBox.ItemsSource = languages;
      //ListVoices(options.TextInputLanguage);
    }

    private void ListVoices(string language)
    {
      TextToSpeechClient client;
      client = TextToSpeechClient.Create();
      var response = client.ListVoices(new ListVoicesRequest
      {
        LanguageCode = language
      });
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          // TODO: dispose managed state (managed objects).
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        disposedValue = true;
      }
    }

    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~OptionsDialog() {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
    #endregion

    private void push2TalkCheckBox_Checked(object sender, RoutedEventArgs e)
    {
      options.IsPush2Talk = ((CheckBox)sender).IsChecked;
    }

    private void textLanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var combobox = (ComboBox)sender;
      options.TextInputLanguage = (string)combobox.SelectedValue;
    }

    private void speechLanguageBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var combobox = (ComboBox)sender;
      options.SpeechInputLanguage = (string)combobox.SelectedValue;
    }

    private void promptForAmazonKey_Click(object sender, RoutedEventArgs e)
    {
      if(openFileDialog.ShowDialog() == true)
      {
        options.AmazonServiceAccountKey = openFileDialog.FileName;
      }
    }

    private void promptForGoogleKey_Click(object sender, RoutedEventArgs e)
    {
      if (openFileDialog.ShowDialog() == true)
      {
        options.GoogleServiceAccountKey = openFileDialog.FileName;
      }
    }
  }
}
