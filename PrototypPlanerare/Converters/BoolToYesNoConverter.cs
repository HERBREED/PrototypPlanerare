using Microsoft.UI.Xaml.Data;
using System;

namespace PrototypPlanerare.Converters
{
    public sealed class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
            => (value as bool? == true) ? "Ja" : "Nej";

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => string.Equals(value?.ToString(), "Ja", StringComparison.OrdinalIgnoreCase);
    }
}
