using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace SpeechToSpeech
{
  public class GeneralSettings : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool _isAutoPlayVocalized = true;
    public List<Key> Push2TalkKey { get; set; } = new List<Key>();
    public List<Key> AppPush2TalkKey { get; set; } = new List<Key>();
    public string SpeechInputLanguage { get; set; } = CultureInfo.CurrentCulture.Name;

    private string _textInputLanguage = CultureInfo.CurrentCulture.Name;

    public bool IsAutoPlayVocalized
    {
      get
      {
        return _isAutoPlayVocalized;
      }
      set
      {
        if (_isAutoPlayVocalized != value)
        {
          _isAutoPlayVocalized = value;
          NotifyPropertyChanged();
        }
      }
    }

    private int _keyUpDelay = 0;
    public int KeyUpDelay
    {
      get
      {
        return _keyUpDelay;
      }
      set
      {
        if (_keyUpDelay != value)
        {
          _keyUpDelay = value;
          NotifyPropertyChanged();
        }
      }
    }

    private bool _isPush2Talk = false;
    public bool IsPush2Talk
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
          NotifyPropertyChanged();
        }
      }
    }

    private bool _isAppPush2Talk = false;
    public bool IsAppPush2Talk
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
          NotifyPropertyChanged();
        }
      }
    }
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
          NotifyPropertyChanged();
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
          NotifyPropertyChanged();
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
          NotifyPropertyChanged();
        }
      }
    }
    private int _activeTextToSpeechService = 1;
    public int ActiveTextToSpeechService
    {
      get
      {
        return _activeTextToSpeechService;
      }
      set
      {
        if (_activeTextToSpeechService != value)
        {
          _activeTextToSpeechService = value;
          NotifyPropertyChanged();
        }
      }
    }
    public string Database { get; set; } = $@".\speechtospeech.sdf";

    public void notifySelectedWebService()
    {
      NotifyPropertyChanged();
    }
  }
}
