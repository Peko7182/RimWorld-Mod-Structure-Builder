using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RimWorld_Mod_Structure_Builder.InfoClasses;

public record SupportedVersionInfo : INotifyPropertyChanged
{
    private string _version { get; set; }
    
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
    
    public SupportedVersionInfo() {}
    public SupportedVersionInfo(string version)
    {
        Version = version;
    }
    
    public override string ToString() => Version;
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}