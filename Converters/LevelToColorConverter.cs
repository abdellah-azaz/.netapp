using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MonAppMultiplateforme.Converters;

public class LevelToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string? level = value?.ToString()?.ToUpper();
        return level switch
        {
            "CRITICAL" or "CRITIQUE" => Brushes.Red,
            "HIGH" or "ÉLEVÉ" or "ELEVÉ" => Brushes.OrangeRed,
            "MEDIUM" or "MOYEN" => Brushes.Orange,
            "LOW" or "FAIBLE" => Brushes.Gray,
            "INFO" => Brushes.LightBlue,
            _ => Brushes.Gray
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
