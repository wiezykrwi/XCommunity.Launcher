namespace Xcommunity.Launcher.Gui.ViewModels;

public class ProfileViewModel(string name, ulong[] mods = null) : ViewModelBase
{
    private ulong[] _mods = mods;
    public string Name { get; } = name;

    public ulong[] Mods
    {
        get => _mods;
        set => SetField(ref _mods, value);
    }
}