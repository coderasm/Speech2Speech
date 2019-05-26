using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech
{
  class Hotkey
  {
    public List<Key> Keys { get; set; }

    public Hotkey(List<Key> keys)
    {
      Keys = keys;
    }

    public override string ToString()
    {
      var text = "";
      Keys.ForEach(key =>
      {
        switch (key)
        {
          case Key.LeftCtrl:
          case Key.RightCtrl:
            text = "ctrl" + (text != "" ? $" + {text}" : "");
            break;
          case Key.LeftAlt:
          case Key.RightAlt:
          case Key.System:
            text = "alt" + (text != "" ? $" + {text}" : "");
            break;
          case Key.LeftShift:
          case Key.RightShift:
            text = "shift" + (text != "" ? $" + {text}" : "");
            break;
          default:
            text = text + (text != "" ? $" + " : "") + key.ToString().ToLower();
            break;
        }
      });
      return text;
    }
  }
}
