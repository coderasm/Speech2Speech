﻿using NAudio.Wave;
using System;
using System.Collections.Generic;

namespace SpeechToSpeech.Services
{
  public class AudioService : IAudioService
  {
    private WaveOutEvent outputDevice;
    private WaveInEvent inputDevice;
    private AudioFileReader audioFile;
    private Action onPlayStopped = () => { };
    private Action onPlay = () => { };
    private float INITIAL_OUTPUT_VOLUME = 1F;
    private long INITIAL_POSITION = 0;
    private long INITIAL_LENGTH = 0;

    public int OutputDevice
    {
      set
      {
        outputDevice = new WaveOutEvent() { DeviceNumber = value };
      }
    }

    public int InputDevice
    {
      set
      {
        inputDevice = new WaveInEvent() { DeviceNumber = value };
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
        return audioFile != null ? audioFile.Position : INITIAL_POSITION;
      }
      set
      {
        if (audioFile != null)
          audioFile.Position = (long)value;
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
        return audioFile != null ? audioFile.Length : _length;
      }
      set
      {
        _length = value;
      }
    }

    public AudioService() { }


    public AudioService(int inputDevice, int outputDevice)
    {
      this.inputDevice = new WaveInEvent() { DeviceNumber = inputDevice };
      this.outputDevice = new WaveOutEvent() { DeviceNumber = outputDevice, Volume = INITIAL_OUTPUT_VOLUME};
    }

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
      onPlayStopped();
      if (audioFile != null)
        audioFile.Dispose();
      audioFile = null;
      if (outputDevice != null)
        outputDevice.Dispose();
      outputDevice = null;
    }

    public IAudioService Play(string fileName)
    {
      try
      {
        if (outputDevice == null)
          throw new Exception("Output device not set");
        outputDevice.PlaybackStopped += Dispose;
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
        throw e;
      }
      if (audioFile == null)
      {
        audioFile = new AudioFileReader(fileName);
        outputDevice.Init(audioFile);
      }
      onPlay();
      outputDevice.Play();
      return this;
    }

    public IAudioService Play(string fileName, int deviceNumber)
    {
      outputDevice = new WaveOutEvent() { DeviceNumber = deviceNumber, Volume = INITIAL_OUTPUT_VOLUME };
      outputDevice.PlaybackStopped += Dispose;
      audioFile = new AudioFileReader(fileName);
      outputDevice.Init(audioFile);
      onPlay();
      outputDevice.Play();
      return this;
    }

    public IAudioService Pause()
    {
      outputDevice?.Pause();
      return this;
    }

    public IAudioService Stop()
    {
      outputDevice?.Stop();
      return this;
    }

    public IAudioService OnPlayStopped(Action handler)
    {
      onPlayStopped = handler;
      return this;
    }

    public IAudioService OnPlay(Action handler)
    {
      onPlay = handler;
      return this;
    }
  }
}
