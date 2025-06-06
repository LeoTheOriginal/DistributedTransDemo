using System;
using System.Globalization;
using System.Windows.Data;

namespace TransDemo.UI.Converters
{
    /// <summary>
    /// Converts a value to a boolean indicating whether the value is not null.
    /// Returns <c>false</c> if the value is <c>null</c>, otherwise returns <c>true</c>.
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value to a boolean.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use (not used).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// <c>true</c> if <paramref name="value"/> is not <c>null</c>; otherwise, <c>false</c>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Return true if value is not null, otherwise false
            return value != null;
        }

        /// <summary>
        /// Not supported. Throws <see cref="NotSupportedException"/>.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use (not used).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>Nothing. Always throws.</returns>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
