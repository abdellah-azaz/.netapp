using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MonAppMultiplateforme.Converters;

public class ResultToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string? result = value?.ToString();
        if (result == "MALWARE" || result == "DANGEREUX" || result == "CRITIQUE")
            return Brushes.Red;
        if (result == "CLEAN" || result == "SAIN" || result == "OK")
            return Brushes.Green;
        if (result == "SUSPICIOUS" || result == "SUSPECT" || result == "ATTENTION")
            return Brushes.Orange;
            
        return Brushes.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
