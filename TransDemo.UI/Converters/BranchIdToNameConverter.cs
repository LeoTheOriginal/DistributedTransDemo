using System;
using System.Globalization;
using System.Windows.Data;

namespace TransDemo.UI.Converters
{
    /// <summary>
    /// Converts a branch ID (integer) to its corresponding branch name as a string.
    /// </summary>
    public class BranchIdToNameConverter : IValueConverter
    {
        /// <summary>
        /// Converts a branch ID to a branch name.
        /// </summary>
        /// <param name="value">The branch ID to convert. Expected to be an integer (0, 1, 2).</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A string representing the branch name:
        /// "Central" for 0,
        /// "Branch 1" for 1,
        /// "Branch 2" for 2,
        /// or "Unknown (value)" for any other value.
        /// </returns>
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

        /// <summary>
        /// Not supported. Throws <see cref="NotImplementedException"/>.
        /// </summary>
        /// <param name="value">The value produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Nothing. Always throws.</returns>
        /// <exception cref="NotImplementedException">Always thrown, as ConvertBack is not supported.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack is not supported.");
        }
    }
}
