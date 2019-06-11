using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechToSpeech.ViewModels
{
  public interface IMainViewModel
  {
    void PlayHandler(object parameter);
    void DeleteHandler(object parameter);
    void PauseHandler();
    void VolumeHandler(object parameter);
    void StopHandler();
  }
}
