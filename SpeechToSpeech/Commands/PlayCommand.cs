using SpeechToSpeech.ViewModels;
using System;
using System.Windows.Input;

namespace SpeechToSpeech.Commands
{
  public class PlayCommand : ICommand
  {
    private IVocalizedViewModel ViewModel;
    public event EventHandler CanExecuteChanged;

    public PlayCommand(IVocalizedViewModel viewModel)
    {
      ViewModel = viewModel;
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      ViewModel.PlayHandler(parameter);
    }
  }
}
