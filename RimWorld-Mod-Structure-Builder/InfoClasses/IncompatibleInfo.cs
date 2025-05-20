using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RimWorld_Mod_Structure_Builder.InfoClasses;

public record IncompatibleInfo : INotifyPropertyChanged
{
    private string _id;
    private string _version;
    
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
    
    public IncompatibleInfo() { }
    public IncompatibleInfo(string id, string version)
    {
        Id = id;
        Version = version;
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}