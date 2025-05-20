using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using RimWorld_Mod_Structure_Builder.InfoClasses;

namespace RimWorld_Mod_Structure_Builder.Convertors;

public class AuthorsToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return string.Join(", ", (value as ObservableCollection<AuthorInfo>)?.Select(x => x.Name) ?? Array.Empty<string>());
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}