using Dapper.Contrib.Extensions;

namespace SpeechToSpeech.Models
{
  [Table("texttospeech")]
  public class TextToSpeech
  {
    [Key]
    public int Id { get; set; }
    public string Text { get; set; }
    public string AudioFile { get; set; }
    public int VoiceId { get; set; }
    [Computed]
    public Voice Voice { get; set; }



    public override string ToString()
    {
      return $"Text: \"{Text}\", Audio File: {AudioFile}";
    }
  }
}
