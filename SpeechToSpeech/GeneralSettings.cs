using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;

namespace SpeechToSpeech
{
  public class GeneralSettings : INotifyPropertyChanged
  {
    private bool? _isPush2Talk = false;
    public bool? IsPush2Talk
    {
      get
      {
        return _isPush2Talk;
      }
      set
      {
        if (_isPush2Talk != value)
        {
          _isPush2Talk = value;
          NotifyPropertyChanged("IsPush2Talk");
        }
      }
    }

    private bool? _isAppPush2Talk = false;
    public bool? IsAppPush2Talk
    {
      get
      {
        return _isAppPush2Talk;
      }
      set
      {
        if (_isAppPush2Talk != value)
        {
          _isAppPush2Talk = value;
          NotifyPropertyChanged("IsAppPush2Talk");
        }
      }
    }
    public List<Key> Push2TalkKey { get; set; } = new List<Key>();
    public List<Key> AppPush2TalkKey { get; set; } = new List<Key>();
    public string SpeechInputLanguage { get; set; } = CultureInfo.CurrentCulture.Name;

    private string _textInputLanguage = CultureInfo.CurrentCulture.Name;
    public string TextInputLanguage
    {
      get
      {
        return _textInputLanguage;
      }
      set
      {
        if (_textInputLanguage != value)
        {
          _textInputLanguage = value;
          NotifyPropertyChanged("TextInputLanguage");
        }
      }
    }
    private int _audioOutDevice = -1;
    public int AudioOutDevice
    {
      get
      {
        return _audioOutDevice;
      }
      set
      {
        if (_audioOutDevice != value)
        {
          _audioOutDevice = value;
          NotifyPropertyChanged("AudioOutDevice");
        }
      }
    }
    private int _audioInDevice = -1;
    public int AudioInDevice
    {
      get
      {
        return _audioInDevice;
      }
      set
      {
        if (_audioInDevice != value)
        {
          _audioInDevice = value;
          NotifyPropertyChanged("AudioInDevice");
        }
      }
    }
    public int ActiveTextToSpeechService { get; set; } = 0;
    public string Database { get; set; } = $@".\speechtospeech.sdf";

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}
