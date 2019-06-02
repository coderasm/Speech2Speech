using Unity;

namespace SpeechToSpeech.Services
{
  class DialogService : IDialogService
  {
    IUnityContainer container;

    public DialogService(IUnityContainer container)
    {
      this.container = container;
    }

    public bool? ShowDialog<T>()
    {
      var window = container.Resolve<T>();
      return ((dynamic)window).ShowDialog();
    }
  }
}
