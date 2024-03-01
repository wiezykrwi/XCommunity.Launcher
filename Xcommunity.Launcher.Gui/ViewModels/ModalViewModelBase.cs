using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Xcommunity.Launcher.Gui.ViewModels;

public abstract class ModalViewModelBase : ViewModelBase
{
    protected ModalViewModelBase()
    {
        OkCommand = new RelayCommand(Ok);
        CancelCommand = new RelayCommand(Cancel);
    }

    public ICommand OkCommand { get; }
    public ICommand CancelCommand { get; }

    protected abstract void Ok();
    protected abstract void Cancel();
}

public abstract class ModalViewModelBase<T> : ModalViewModelBase
{
    private readonly TaskCompletionSource<ModalResult<T>> _source = new();

    protected ModalViewModelBase()
    {
    }
    public Task<ModalResult<T>> ShowAsync()
    {
        return _source.Task;
    }

    protected override void Ok()
    {
        _source.SetResult(new(true, Map()));
    }

    protected override void Cancel()
    {
        _source.SetResult(ModalResult<T>.Cancelled);
    }

    protected abstract T Map();
}