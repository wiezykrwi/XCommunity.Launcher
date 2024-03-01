using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Xcommunity.Launcher.Core;

namespace Xcommunity.Launcher.Gui.ViewModels;

public class ProfilesViewModel : ViewModelBase
{
    private List<ProfileViewModel> _allProfiles = [];

    public ProfilesViewModel()
    {
        CreateCommand = new AsyncRelayCommand(Create);
        ImportCommand = new RelayCommand(Import);
        LoadCommand = new RelayCommand(Load);
        
        if (File.Exists("profiles.json"))
        {
            using var fileStream = File.Open("profiles.json", FileMode.Open);
            var profiles = JsonSerializer.Deserialize<ProfileData[]>(fileStream);
            if (profiles != null)
            {
                _allProfiles.AddRange(profiles.Select(x => new ProfileViewModel(x.Name)));
            }
        }
        else
        {
            _allProfiles.Add(new ProfileViewModel("Default"));
        }
        Filter();
    }

    private void Load()
    {
        throw new System.NotImplementedException();
    }

    private async Task Create()
    {
        var modal = new CreateProfileModalViewModel();
        var result = await ViewModelLocator.Instance.Modal.OpenModalAsync(modal);
        if (!result.Success || result.Data is null)
        {
            return;
        }
        
        _allProfiles.Add(new ProfileViewModel(result.Data.Name));
        Filter();
    }
    
    public ObservableCollection<ProfileViewModel> Profiles { get; } = [];
    
    public ICommand CreateCommand { get; }

    public ICommand ImportCommand { get; }

    public ICommand LoadCommand { get; }

    private void Import()
    {
        var dialog = new OpenFileDialog();
        var result = dialog.ShowDialog();

        if (result != true)
        {
            return;
        }
        
        // AML read mode
    }

    private void Filter()
    {
        Profiles.Clear();
        foreach (var profileViewModel in _allProfiles)
        {
            Profiles.Add(profileViewModel);
        }
    }
}