namespace Xcommunity.Launcher.Gui.ViewModels;

public class ViewModelLocator
{
    public static ViewModelLocator Instance { get; } = new();
    private ViewModelLocator() {}

    public MainViewModel Main { get; } = new();
    public ModalViewModel Modal { get; } = new();
    public ModsViewModel Mods { get; } = new();
    public ProfilesViewModel Profiles { get; } = new();

}