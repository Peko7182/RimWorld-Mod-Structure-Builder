using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RimWorld_Mod_Structure_Builder.InfoClasses;

/// <summary>
/// Class for storing information about mod description
/// </summary>
public sealed record DescriptionInfo : INotifyPropertyChanged
{
    private string _version;
    private string _description;
    
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
    public string Description
    {
        get => _description;
        set
        {
            if (value == _description) return;
            _description = value;
            OnPropertyChanged();
        }
    }
    
    public DescriptionInfo() { }
    public DescriptionInfo(string description, string version = null)
    {
        Version = version;
        Description = description;
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}