using System.ComponentModel;
using System.Runtime.CompilerServices;
using RimWorld_Mod_Structure_Builder.InfoClasses.Interfaces;

namespace RimWorld_Mod_Structure_Builder.InfoClasses;

public record DependencyInfo : IVersioned, INotifyPropertyChanged
{
    private string _id;
    private string _version;
    private string _displayName;
    private string _steamWorkshopUrl;

    public string Id
    {
        get => _id;
        set
        {
            if (value == _id) return;
            _id = value;
            OnPropertyChanged();
        }
    }
    public string Version
    {
        get => _version;
        set
        {
            if (value == _version) return;
            _version = value;
            OnPropertyChanged();
        }
    }
    public string DisplayName
    {
        get => _displayName;
        set
        {
            if (value == _displayName) return;
            _displayName = value;
            OnPropertyChanged();
        }
    }
    public string SteamWorkshopUrl
    {
        get => _steamWorkshopUrl;
        set
        {
            if (value == _steamWorkshopUrl) return;
            _steamWorkshopUrl = value;
            OnPropertyChanged();
        }
    }
    
    public DependencyInfo() { }
    public DependencyInfo(string id, string displayName, string steamWorkshopUrl)
    {
        Id = id;
        DisplayName = displayName;
        SteamWorkshopUrl = steamWorkshopUrl;
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}