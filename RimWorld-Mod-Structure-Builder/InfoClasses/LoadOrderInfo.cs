using System.ComponentModel;
using System.Runtime.CompilerServices;
using RimWorld_Mod_Structure_Builder.InfoClasses.Interfaces;

namespace RimWorld_Mod_Structure_Builder.InfoClasses;

public record LoadOrderInfo : IVersioned, INotifyPropertyChanged
{
    private string _id;
    private string _version;
    private bool _forced;
    
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
    public bool Forced
    {
        get => _forced;
        set
        {
            if (value == _forced) return;
            _forced = value;
            OnPropertyChanged();
        }
    }
    
    public LoadOrderInfo() {}
    public LoadOrderInfo(string id, string version = null, bool forced = false)
    {
        Id = id;
        Version = version;
        Forced = forced;
    }
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}