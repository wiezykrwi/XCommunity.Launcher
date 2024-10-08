﻿using Xcommunity.Launcher.Core;
using Xcommunity.Launcher.Gui.Services;

namespace Xcommunity.Launcher.Gui.ViewModels;

public class ModViewModel : ViewModelBase
{
    private bool _isLoadingDetails;

    public ModViewModel(ModData modData)
    {
        Data = modData;

        IsEnabled = Data.IsEnabled;
    }

    public bool IsEnabled
    {
        get => Data.IsEnabled;
        set
        {
            if (Data.IsEnabled == value) return;
            Data.IsEnabled = value;
            OnPropertyChanged();
            ViewModelLocator.Instance.Mods.NotifyEnabledChanged();
        }
    }

    public string Title => Data.Title;
    public string Author => Data.Author;
    public ModData Data { get; }

    public bool IsLoadingDetails
    {
        get => _isLoadingDetails;
        set
        {
            if (value == _isLoadingDetails) return;
            _isLoadingDetails = value;
            OnPropertyChanged();
        }
    }
    
    public void SetDetails(DetailedModData detailedModData)
    {
        if (detailedModData.Name != Data.Title)
        {
            Data.Title = detailedModData.Name;
            OnPropertyChanged(nameof(Title));
        }

        if (!string.IsNullOrWhiteSpace(detailedModData.Author) && detailedModData.Author != Data.Author)
        {
            Data.Author = detailedModData.Author;
            OnPropertyChanged(nameof(Author));
        }
    }
}