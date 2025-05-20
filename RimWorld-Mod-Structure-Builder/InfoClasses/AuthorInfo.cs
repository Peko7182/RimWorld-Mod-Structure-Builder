using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RimWorld_Mod_Structure_Builder.InfoClasses;

public record AuthorInfo : INotifyPropertyChanged
{
    private string _name { get; set; }

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;
            OnPropertyChanged();
        }
    }
    
    public AuthorInfo () { }
    public AuthorInfo (string name)
    {
        Name = name;
    }
    
    public override string ToString() => Name;
    
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}