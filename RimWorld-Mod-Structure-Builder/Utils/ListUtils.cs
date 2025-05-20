namespace RimWorld_Mod_Structure_Builder.Utils;

using System.Collections.Generic;
using System.Collections.ObjectModel;

public static class ListUtils
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return source == null ? null : new ObservableCollection<T>(source);
    }
}