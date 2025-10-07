using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml;
using System;

namespace PrototypPlanerare.Converters
{
    /// <summary>
    /// Returns a Brush for an EngineeringStatus value.
    /// Use ConverterParameter = "bg" | "border" | "fg".
    /// </summary>
    public sealed class EngStatusToBrushConverter : IValueConverter
    {
        // Light, soft palette
        private static readonly SolidColorBrush Bg_NotStarted = FromHex("#EDF1F5");
        private static readonly SolidColorBrush Bd_NotStarted = FromHex("#D5DEE8");

        private static readonly SolidColorBrush Bg_InProgress = FromHex("#E6F0FF");
        private static readonly SolidColorBrush Bd_InProgress = FromHex("#8EB6FF");

        private static readonly SolidColorBrush Bg_Blocked = FromHex("#FFF2E5");
        private static readonly SolidColorBrush Bd_Blocked = FromHex("#FFC48A");

        private static readonly SolidColorBrush Bg_Done = FromHex("#E6F7EC");
        private static readonly SolidColorBrush Bd_Done = FromHex("#8CD39E");

        private static SolidColorBrush FromHex(string hex) =>
            (SolidColorBrush)XamlBindingHelper.ConvertValue(typeof(SolidColorBrush), hex);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var kind = (parameter as string)?.ToLowerInvariant() ?? "bg";

            // Default to NotStarted
            var bg = Bg_NotStarted; var bd = Bd_NotStarted;

            // Accept either enum or its string
            string statusStr = value?.ToString() ?? "NotStarted";
            switch (statusStr)
            {
                case "InProgress": bg = Bg_InProgress; bd = Bd_InProgress; break;
                case "Blocked": bg = Bg_Blocked; bd = Bd_Blocked; break;
                case "Done": bg = Bg_Done; bd = Bd_Done; break;
                default: bg = Bg_NotStarted; bd = Bd_NotStarted; break;
            }

            return kind switch
            {
                "border" => bd,
                "fg" => Application.Current.Resources["TextFillColorPrimaryBrush"] as Brush
                            ?? new SolidColorBrush(Microsoft.UI.Colors.Black),
                _ => bg
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            throw new NotImplementedException();
    }
}
