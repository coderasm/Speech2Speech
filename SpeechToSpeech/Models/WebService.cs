using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
