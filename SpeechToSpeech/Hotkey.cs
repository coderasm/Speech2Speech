using System.Collections.Generic;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;

namespace SpeechToSpeech
{
  class Hotkey
  {
    public List<Key> HotKeys { get; set; }
    private InputSimulator simulator = new InputSimulator();

    private Hotkey(List<Key> keys)
    {
      HotKeys = keys;
    }

    public static Hotkey Create(List<Key> keys)
    {
      return new Hotkey(keys);
    }

    public void BroadcastDown()
    {
      HotKeys.ForEach(key =>
      {
        var virtualKeyCode = KeyInterop.VirtualKeyFromKey(key);
        simulator.Keyboard.KeyDown((VirtualKeyCode)virtualKeyCode);
      });
    }

    public void BroadcastUp()
    {
      HotKeys.ForEach(key =>
      {

        var virtualKeyCode = KeyInterop.VirtualKeyFromKey(key);
        simulator.Keyboard.KeyUp((VirtualKeyCode)virtualKeyCode);
      });
    }

    public override string ToString()
    {
      var text = "";
      HotKeys.ForEach(key =>
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
