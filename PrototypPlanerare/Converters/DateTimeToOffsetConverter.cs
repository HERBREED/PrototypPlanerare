using Microsoft.UI.Xaml.Data;
using System;

namespace PrototypPlanerare.Converters
{
    /// Two-way converter between DateTime? (model) and DateTimeOffset (DatePicker.Date)
    public sealed class DateTimeToOffsetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Model -> UI
            if (value == null)
                return new DateTimeOffset(DateTime.Today);

            if (value is DateTime dt)
            {
                if (dt.Kind == DateTimeKind.Unspecified)
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
                return new DateTimeOffset(dt);
            }

            // If your model ever provides a DateTimeOffset already
            if (value is DateTimeOffset dto)
                return dto;

            return new DateTimeOffset(DateTime.Today);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // UI -> Model
            if (value is DateTimeOffset dto)
                return new DateTime(dto.Year, dto.Month, dto.Day); // just the date

            return null;
        }
    }
}
