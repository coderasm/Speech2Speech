using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech
{
  public class AmazonSettings
  {
    public string AccessKeyId { get; set; }
    public string SecretAccessKey { get; set; }
    public string Voice { get; set; }
  }
}
