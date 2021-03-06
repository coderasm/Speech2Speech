﻿using SpeechToSpeech.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeechToSpeech.Commands
{
  public class VolumeCommand : ICommand
  {
    private IMainViewModel ViewModel;
    public event EventHandler CanExecuteChanged;

    public VolumeCommand(IMainViewModel viewModel)
    {
      ViewModel = viewModel;
    }

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      ViewModel.VolumeHandler(parameter);
    }
  }
}
