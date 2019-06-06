using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech
{
  public class Settings
  {
    public GeneralSettings generalSettings { get; set; } = new GeneralSettings();
    public GoogleSettings googleSettings { get; set; } = new GoogleSettings();
    public AmazonSettings amazonSettings { get; set; } = new AmazonSettings();
    public IBMSettings ibmSettings { get; set; } = new IBMSettings();
    public DatabaseSettings databaseSettings { get; set; } = new DatabaseSettings();
  }
}
