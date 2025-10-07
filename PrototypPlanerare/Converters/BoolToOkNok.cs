using Microsoft.UI.Xaml.Data;
using System;

namespace PrototypPlanerare.Converters
{
    public sealed class BoolToOkNok : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => (value as bool? == true) ? "Godkänd" : "Ej Godkänd";

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => string.Equals(value?.ToString(), "Godkänd", StringComparison.OrdinalIgnoreCase);
    }
}