using System;
using Microsoft.UI.Xaml.Data;

namespace PrototypPlanerare.Converters
{
    public sealed class DateTimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dt) return dt.ToString("yyyy-MM-dd");
            if (value is DateTimeOffset dto) return dto.LocalDateTime.ToString("yyyy-MM-dd");
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
            => throw new NotImplementedException();
    }
}
