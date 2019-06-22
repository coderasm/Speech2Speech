using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech.ViewModels
{
  public interface IVocalizedViewModel
  {
    void PlayHandler(object parameter);
    void PauseHandler();
    void StopHandler();
  }
}
