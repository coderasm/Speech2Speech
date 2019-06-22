using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using WindowsInput;
using WindowsInput.Native;

namespace SpeechToSpeech
{
  public class Hotkey: INotifyPropertyChanged
  {
    private List<Key> _hotkeys = new List<Key>();
    public List<Key> HotKeys {
      get
      {
        return _hotkeys;
      }
      set
      {
        _hotkeys = value;
        NotifyPropertyChanged("Printed");
      }
    }
    private InputSimulator simulator = new InputSimulator();

    public event PropertyChangedEventHandler PropertyChanged;
    
    public string Printed
    {
      get
      {
        return ToString();
      }
    }

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
        if (key == Key.System)
          simulator.Keyboard.KeyDown(VirtualKeyCode.LMENU);
        else
        {
          var virtualKeyCode = KeyInterop.VirtualKeyFromKey(key);
          simulator.Keyboard.KeyDown((VirtualKeyCode)virtualKeyCode);
        }
      });
    }

    public void BroadcastUp()
    {
      HotKeys.ForEach(key =>
      {
        if (key == Key.System)
          simulator.Keyboard.KeyUp(VirtualKeyCode.LMENU);
        else
        {
          var virtualKeyCode = KeyInterop.VirtualKeyFromKey(key);
          simulator.Keyboard.KeyUp((VirtualKeyCode)virtualKeyCode);
        }
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

    private void NotifyPropertyChanged(string prop)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}
