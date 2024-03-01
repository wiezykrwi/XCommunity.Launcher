using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Xcommunity.Launcher.Core;
using Xcommunity.Launcher.Gui.Services;

namespace Xcommunity.Launcher.Gui.ViewModels;

public class ModsViewModel : ViewModelBase
{
    private readonly List<ModViewModel> _allMods = new();
    private readonly SteamService _steamService = new();
    private string _filterText = string.Empty;
    private ModViewModel? _selectedMod;

    public ModsViewModel()
    {
        LoadCommand = new AsyncRelayCommand(LoadAsync);
        RunCommand = new AsyncRelayCommand(RunAsync);
        ToggleAllCommand = new RelayCommand(ToggleAll);
        ClearFilterCommand = new RelayCommand(ClearFilter);

        if (File.Exists("data.json"))
        {
            using var fileStream = File.Open("data.json", FileMode.Open);
            var mods = JsonSerializer.Deserialize<ModData[]>(fileStream);
            if (mods != null)
            {
                _allMods.AddRange(mods.Select(x => new ModViewModel(this, x)));
                Filter();
            }
        }
    } 
    
    public ObservableCollection<ModViewModel> Mods { get; } = new();

    public ICommand LoadCommand { get; }
    public ICommand ToggleAllCommand { get; }
    public ICommand ClearFilterCommand { get; }
    public ICommand RunCommand { get; }

    public string FilterText
    {
        get => _filterText;
        set
        {
            if (value == _filterText) return;
            _filterText = value;
            Filter();
            OnPropertyChanged();
        }
    }

    public ModViewModel? SelectedMod
    {
        get => _selectedMod;
        set
        {
            if (_selectedMod == value) return;
            SetField(ref _selectedMod, value);

            if (value is null) return;

            value.IsLoadingDetails = true;

            OnPropertyChanged();
            _steamService.GetModDetails(value.Data.Id).ContinueWith(t =>
            {
                if (t is { IsCompletedSuccessfully: true, Result: not null })
                {
                    value.SetDetails(t.Result);
                }
                value.IsLoadingDetails = false;
            });
        }
    }

    public string Counters => $"{_allMods.Count(x => x.IsEnabled)} / {Mods.Count} / {_allMods.Count}";

    public bool AreAllEnabled => Mods.Any() && Mods.All(x => x.IsEnabled);

    private async Task RunAsync()
    {
        var modSaver = new ModSaver();
        await modSaver.SaveAsync(Mods.Where(x => x.IsEnabled).Select(mod => mod.Data).ToList());
    }

    private void ToggleAll()
    {
        var state = Mods.All(x => x.IsEnabled);
        foreach (var mod in Mods) mod.IsEnabled = !state;
    }

    public void NotifyEnabledChanged()
    {
        OnPropertyChanged(nameof(AreAllEnabled));
        OnPropertyChanged(nameof(Counters));
    }

    private void Filter()
    {
        Mods.Clear();
        foreach (var mod in _allMods.Where(mod => mod.Title.Contains(_filterText, StringComparison.InvariantCultureIgnoreCase))) Mods.Add(mod);
        OnPropertyChanged(nameof(Counters));
    }

    private void ClearFilter()
    {
        FilterText = string.Empty;
    }

    private async Task LoadAsync()
    {
        var modFinder = new ModFinder();
        var mods = await modFinder.FindModsAsync();

        _allMods.Clear();
        foreach (var mod in mods) _allMods.Add(new ModViewModel(this, mod));
        Filter();
    }
    
    public void Save()
    {
        using var fileStream = File.Open("data.json", FileMode.Create);
        JsonSerializer.Serialize(fileStream, _allMods.Select(x => x.Data));
    }
}