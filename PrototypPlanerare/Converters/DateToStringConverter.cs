using System;
using Microsoft.UI.Xaml.Data;

namespace PrototypPlanerare.Converters
{
    public sealed class DateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is null) return string.Empty;

            if (value is DateTime dt)
                return dt.ToString("yyyy-MM-dd");

            if (value is DateTimeOffset dto)
                return dto.DateTime.ToString("yyyy-MM-dd");

            if (value is string s && DateTime.TryParse(s, out var parsed))
                return parsed.ToString("yyyy-MM-dd");

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string s && DateTime.TryParse(s, out var dt))
                return dt;

            return null; // we’re not editing the date in the UI yet
        }
    }
}
