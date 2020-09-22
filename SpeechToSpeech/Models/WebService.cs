using Dapper.Contrib.Extensions;

namespace SpeechToSpeech.Models
{
  [Table("service")]
  public class WebService
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
  }
}
