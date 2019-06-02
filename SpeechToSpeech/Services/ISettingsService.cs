namespace SpeechToSpeech.Services
{
  public interface ISettingsService
  {
    Settings settings { get; }
    void LoadSettings();
    void SaveSettings();
  }
}
