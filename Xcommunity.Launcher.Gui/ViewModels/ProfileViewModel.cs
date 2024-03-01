namespace Xcommunity.Launcher.Gui.ViewModels;

public class ProfileViewModel(string name) : ViewModelBase
{
    public string Name { get; } = name;
}