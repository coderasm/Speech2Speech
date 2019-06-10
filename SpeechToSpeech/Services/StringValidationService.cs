using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SpeechToSpeech.Services
{
  public class StringValidationService : IStringValidationService
  {
    public bool isInteger(string text)
    {
      var regex = new Regex($@"^0$|^[1-9]\d*$");
      return regex.IsMatch(text);
    }
  }
}
