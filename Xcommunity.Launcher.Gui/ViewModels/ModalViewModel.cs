using System;
using System.Threading.Tasks;

namespace Xcommunity.Launcher.Gui.ViewModels;

public class ModalViewModel : ViewModelBase
{
    private ModalViewModelBase? _currentModalViewModel;

    public ModalViewModelBase? Current
    {
        get => _currentModalViewModel;
        set => SetField(ref _currentModalViewModel, value);
    }

    public async Task<ModalResult<T>> OpenModalAsync<T>(ModalViewModelBase<T>? modalViewModel)
    {
        Current = modalViewModel ?? throw new ArgumentNullException(nameof(modalViewModel));
        var result = await modalViewModel.ShowAsync();
        Current = null;
        return result;
    }
}

public record ModalResult<T>(bool Success, T? Data)
{
    public static ModalResult<T> Cancelled { get; } = new(false, default);
}