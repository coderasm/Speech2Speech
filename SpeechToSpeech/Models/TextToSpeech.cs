using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech.Models
{
  [Table("texttospeech")]
  public class TextToSpeech
  {
    [Key]
    public int Id { get; set; }
    public string Text { get; set; }
    public string AudioFile { get; set; }



    public override string ToString()
    {
      return $"Text: \"{Text}\", Audio File: {AudioFile}";
    }
  }
}
