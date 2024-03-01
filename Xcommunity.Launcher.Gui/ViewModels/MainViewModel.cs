namespace Xcommunity.Launcher.Gui.ViewModels;

public class MainViewModel : ViewModelBase
{
    private MainWindowState _state;
    
    public MainWindowState State
    {
        get => _state;
        set => SetField(ref _state, value);
    }

    public ModalViewModel Modal => ViewModelLocator.Instance.Modal;

    public void Save()
    {
        ViewModelLocator.Instance.Mods.Save();
    }
}

public enum MainWindowState
{
    Mods,
    Profiles
}