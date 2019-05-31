namespace SpeechToSpeech
{
  public interface ISettingsService
  {
    Settings settings { get; }
    void LoadSettings();
    void SaveSettings();
  }
}
