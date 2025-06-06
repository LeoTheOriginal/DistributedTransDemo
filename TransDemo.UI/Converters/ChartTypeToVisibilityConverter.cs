// Converters/ChartTypeToVisibilityConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using static DashboardViewModel;

namespace TransDemo.UI.Converters
{
    /// <summary>
    /// Converts a <see cref="DashboardChartType"/> value to a <see cref="Visibility"/> value
    /// for use in WPF data binding scenarios. Determines whether a UI element should be visible
    /// based on the selected chart type and a string parameter ("Pie" or "Cartesian").
    /// </summary>
    public class ChartTypeToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="DashboardChartType"/> value and a parameter to a <see cref="Visibility"/> value.
        /// Returns <see cref="Visibility.Visible"/> if the selected chart type matches the parameter,
        /// otherwise returns <see cref="Visibility.Collapsed"/>.
        /// </summary>
        /// <param name="value">The source data, expected to be of type <see cref="DashboardChartType"/>.</param>
        /// <param name="targetType">The type of the binding target property (should be <see cref="Visibility"/>).</param>
        /// <param name="parameter">A string parameter indicating the chart type ("Pie" or "Cartesian").</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// <see cref="Visibility.Visible"/> if the chart type matches the parameter; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var selected = (DashboardChartType)value;
            var target = parameter?.ToString();

            return (selected == DashboardChartType.BranchPie && target == "Pie") ||
                   (selected != DashboardChartType.BranchPie && target == "Cartesian")
                   ? Visibility.Visible
                   : Visibility.Collapsed;
        }

        /// <summary>
        /// Not implemented. Throws <see cref="NotImplementedException"/> if called.
        /// </summary>
        /// <param name="value">The value produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Nothing. Always throws.</returns>
        /// <exception cref="NotImplementedException">Always thrown.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}