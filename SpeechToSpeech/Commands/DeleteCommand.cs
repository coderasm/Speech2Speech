using SpeechToSpeech.ViewModels;
using System;
using System.Windows.Input;

namespace SpeechToSpeech.Commands
{
  public class DeleteCommand : ICommand
  {
    private IMainViewModel ViewModel;
    public event EventHandler CanExecuteChanged;

    public DeleteCommand(IMainViewModel viewModel)
    {
      ViewModel = viewModel;
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      ViewModel.DeleteHandler(parameter);
    }
  }
}
