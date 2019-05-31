namespace SpeechToSpeech
{
  interface ISettingsService
  {
    Settings settings { get; }
    void LoadSettings();
    void SaveSettings();
  }
}
