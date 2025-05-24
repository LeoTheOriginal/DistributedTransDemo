using System;
using System.Globalization;
using System.Windows.Data;

namespace TransDemo.UI.Converters
{
    /// <summary>
    /// Konwertuje null→false, a każdy inny obiekt→true
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
