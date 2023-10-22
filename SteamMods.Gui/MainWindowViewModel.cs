using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.Input;
using SteamMods.Core;

namespace SteamMods.Gui;

public class MainWindowViewModel : INotifyPropertyChanged
{
    public MainWindowViewModel()
    {
        LoadCommand = new AsyncRelayCommand(LoadAsync);
    }

    public ObservableCollection<ModData> Mods { get; } = new();
    
    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand LoadCommand { get; }

    private async Task LoadAsync()
    {
        var modFinder = new ModFinder();
        var mods = await modFinder.FindModsAsync();
        
        Mods.Clear();
        foreach (var mod in mods)
        {
            Mods.Add(mod);
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}