using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using RimWorld_Mod_Structure_Builder.InfoClasses;

namespace RimWorld_Mod_Structure_Builder.Convertors;

public class DescriptionsToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var descriptions = value as ObservableCollection<DescriptionInfo>;
        if (descriptions == null || !descriptions.Any()) return string.Empty;
        
        var latestDescription = descriptions
            .Where(des => !string.IsNullOrEmpty(des.Version))
            .OrderByDescending(des => Version.Parse(des.Version)) // IDK if Version.Parse is the way
            .FirstOrDefault();
        
        latestDescription = latestDescription ?? descriptions.First();
        
        return latestDescription?.Description ?? "N/A";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}