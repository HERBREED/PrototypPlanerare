using System;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI; // Color / Colors

namespace PrototypPlanerare.Converters
{
    public sealed class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var s = (value as string ?? "").ToLowerInvariant();

            Color c = s switch
            {
                "done" => Color.FromArgb(0xFF, 0x39, 0xA8, 0x4A), // green
                "inprogress" => Color.FromArgb(0xFF, 0xCC, 0x9A, 0x06), // amber
                "blocked" => Color.FromArgb(0xFF, 0xD1, 0x34, 0x38), // red
                "notstarted" => Color.FromArgb(0xFF, 0x8A, 0x8A, 0x8A), // gray
                _ => Color.FromArgb(0xFF, 0x8A, 0x8A, 0x8A)
            };

            return new SolidColorBrush(c);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
