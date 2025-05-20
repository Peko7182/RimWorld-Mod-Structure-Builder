using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace RimWorld_Mod_Structure_Builder.Utils;

/// <summary>
/// Extension methods for <see cref="IEnumerable{T}"/>
/// </summary>
public static class ListUtils
{
    /// <summary>
    /// Converts a <see cref="IEnumerable{T}"/> to an <see cref="ObservableCollection{T}"/>.
    /// If the source is null, returns null.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the <see cref="IEnumerable{T}"/>.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}"/> to convert.</param>
    /// <returns>The <see cref="ObservableCollection{T}"/> containing the elements of the source.</returns>
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
    {
        return source == null ? null : new ObservableCollection<T>(source);
    }
}