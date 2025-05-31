using System;
using System.Globalization;
using System.Windows.Data;

namespace TransDemo.UI.Converters
{
    public class BranchIdToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                0 => "Central",
                1 => "Branch 1",
                2 => "Branch 2",
                _ => $"Unknown ({value})"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not supported.");
        }
    }
}
