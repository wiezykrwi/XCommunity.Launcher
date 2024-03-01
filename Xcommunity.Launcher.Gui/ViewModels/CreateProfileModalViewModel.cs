namespace Xcommunity.Launcher.Gui.ViewModels;

public class CreateProfileModalViewModel : ModalViewModelBase<CreateProfileResult>
{
    private string _name;

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    protected override CreateProfileResult Map() => new(Name);
}

public record CreateProfileResult(string Name);