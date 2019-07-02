using NAudio.Wave;
using SpeechToSpeech.SoundTouch;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace SpeechToSpeech.Services
{
  public class AudioPlayer : IAudioPlayer, INotifyPropertyChanged, IDisposable
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private WaveOutEvent outputDevice;
    private VarispeedSampleProvider speedControl;
    private WaveInEvent inputDevice;
    private AudioFileReader _audioFileReader;
    public AudioFileReader AudioFileReader {
      get
      {
        return _audioFileReader;
      }
    }
    private Action onPlayStopped = () => { };
    private Action onPlay = () => { };
    private float INITIAL_OUTPUT_VOLUME = 1F;
    private long INITIAL_POSITION = 0;
    private long INITIAL_LENGTH = 0;

    public int OutputDevice
    {
      set
      {
        outputDevice = new WaveOutEvent() { DeviceNumber = value, Volume = INITIAL_OUTPUT_VOLUME };
        NotifyPropertyChanged("Volume");
      }
    }

    public int InputDevice
    {
      set
      {
        inputDevice = new WaveInEvent() { DeviceNumber = value };
      }
    }

    public string AudioFile
    {
      set
      {
        _audioFileReader = new AudioFileReader(value);
        NotifyPropertyChanged("Position");
        NotifyPropertyChanged("Length");
      }
    }

    public double Volume {
      get
      {
        return outputDevice != null ? outputDevice.Volume : INITIAL_OUTPUT_VOLUME;
      }
      set
      {
        if (outputDevice != null)
          outputDevice.Volume = (float)value;
        else
          INITIAL_OUTPUT_VOLUME = (float)value;
      }
    }

    public double Position
    {
      get
      {
        return AudioFileReader != null ? AudioFileReader.Position : INITIAL_POSITION;
      }
      set
      {
        if (AudioFileReader != null)
          AudioFileReader.Position = (long)value;
        else
          INITIAL_POSITION = (long)value;
      }
    }

    private double _length;
    public double Length
    {
      get
      {
        if (_length == 0)
          _length = INITIAL_LENGTH;
        return AudioFileReader != null ? AudioFileReader.Length : _length;
      }
      set
      {
        _length = value;
      }
    }

    public AudioPlayer() { }


    //public AudioPlayer(int inputDevice, int outputDevice)
    //{
    //  this.inputDevice = new WaveInEvent() { DeviceNumber = inputDevice };
    //  this.outputDevice = new WaveOutEvent() { DeviceNumber = outputDevice, Volume = INITIAL_OUTPUT_VOLUME};
    //}

    public List<KeyValuePair<int, string>> Devices
    {
      get
      {
        return GetDevices();
      }
    }

    private List<KeyValuePair<int, string>> GetDevices()
    {
      var audioDevices = new List<KeyValuePair<int, string>>();
      for (int n = -1; n < WaveOut.DeviceCount; n++)
      {
        var caps = WaveOut.GetCapabilities(n);
        audioDevices.Add(new KeyValuePair<int, string>(n, caps.ProductName));
      }
      return audioDevices;
    }

    private void Dispose(object sender, StoppedEventArgs args)
    {
      Dispose();
    }

    public void Dispose()
    {
      onPlayStopped();
      AudioFileReader?.Dispose();
      _audioFileReader = null;
      outputDevice?.Dispose();
      outputDevice = null;
      speedControl?.Dispose();
      speedControl = null;
    }

    public IAudioPlayer Play(string fileName)
    {
      try
      {
        if (outputDevice == null)
          throw new Exception("Output device not set");
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw e;
      }
      play(fileName);
      return this;
    }

    public IAudioPlayer Play(string fileName, int deviceNumber)
    {
      OutputDevice = deviceNumber;
      play(fileName);
      return this;
    }

    private void play(string fileName)
    {
      outputDevice.PlaybackStopped += Dispose;
      if (AudioFileReader == null)
      {
        AudioFile = fileName;
      }
      if (speedControl == null)
      {
        var useTempo = true;
        speedControl = new VarispeedSampleProvider(AudioFileReader, 100, new SoundTouchProfile(useTempo, false));
      }
      outputDevice.Init(AudioFileReader);
      onPlay();
      outputDevice.Play();
      updatePosition();
    }

    private void updatePosition()
    {
      Task.Run(() =>
      {
        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
          NotifyPropertyChanged("Position");
        }
      });
    }

    public IAudioPlayer Pause()
    {
      outputDevice?.Pause();
      return this;
    }

    public IAudioPlayer Stop()
    {
      outputDevice?.Stop();
      return this;
    }

    public IAudioPlayer OnPlayStopped(Action handler)
    {
      onPlayStopped = handler;
      return this;
    }

    public IAudioPlayer OnPlay(Action handler)
    {
      onPlay = handler;
      return this;
    }

    private void NotifyPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}
