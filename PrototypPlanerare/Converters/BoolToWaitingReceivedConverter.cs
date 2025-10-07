using Microsoft.UI.Xaml.Data;
using System;

namespace PrototypPlanerare.Converters
{
    public sealed class BoolToWaitingReceivedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // Handle plain bool
            if (value is bool b)
                return b ? "Mottagen" : "Väntar";

            // Handle nullable bool
            bool? nb = value as bool?;
            if (nb.HasValue)
                return nb.Value ? "Mottagen" : "Väntar";

            // Fallback when null/unknown
            return "—";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var s = (value as string)?.Trim();
            return string.Equals(s, "Mottagen", StringComparison.OrdinalIgnoreCase);
        }
    }
}
