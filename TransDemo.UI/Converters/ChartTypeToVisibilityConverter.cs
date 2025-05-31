// Converters/ChartTypeToVisibilityConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using static DashboardViewModel;

namespace TransDemo.UI.Converters
{
    public class ChartTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selected = (DashboardChartType)value;
            var target = parameter?.ToString();

            return (selected == DashboardChartType.BranchPie && target == "Pie") ||
                   (selected != DashboardChartType.BranchPie && target == "Cartesian")
                   ? Visibility.Visible
                   : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}