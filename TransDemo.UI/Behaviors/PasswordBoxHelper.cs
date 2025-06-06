using System;
using System.Windows;
using System.Windows.Controls;

namespace TransDemo.UI.Behaviors
{
    /// <summary>
    /// Provides attached properties and behaviors to enable binding the <see cref="PasswordBox.Password"/> property in WPF.
    /// </summary>
    public static class PasswordBoxHelper
    {
        /// <summary>
        /// Identifies the BoundPassword attached property.
        /// Enables binding to the <see cref="PasswordBox.Password"/> property.
        /// </summary>
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        /// <summary>
        /// Gets the value of the BoundPassword attached property.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <returns>The bound password string.</returns>
        public static string GetBoundPassword(DependencyObject dp)
            => (string)dp.GetValue(BoundPassword);

        /// <summary>
        /// Sets the value of the BoundPassword attached property.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <param name="value">The password string to set.</param>
        public static void SetBoundPassword(DependencyObject dp, string value)
            => dp.SetValue(BoundPassword, value);

        /// <summary>
        /// Identifies the IsUpdating attached property.
        /// Used internally to prevent recursive updates.
        /// </summary>
        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdating",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets the value of the IsUpdating attached property.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <returns>True if updating, otherwise false.</returns>
        private static bool GetIsUpdating(DependencyObject dp)
           => (bool)dp.GetValue(IsUpdatingProperty);

        /// <summary>
        /// Sets the value of the IsUpdating attached property.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <param name="value">True if updating, otherwise false.</param>
        private static void SetIsUpdating(DependencyObject dp, bool value)
            => dp.SetValue(IsUpdatingProperty, value);

        /// <summary>
        /// Identifies the BindPassword attached property.
        /// Enables or disables the binding behavior for the <see cref="PasswordBox"/>.
        /// </summary>
        private static readonly DependencyProperty BindPasswordFlag =
            DependencyProperty.RegisterAttached(
                "BindPassword",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        /// <summary>
        /// Gets the value of the BindPassword attached property.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <returns>True if binding is enabled, otherwise false.</returns>
        public static bool GetBindPassword(DependencyObject dp)
            => (bool)dp.GetValue(BindPasswordFlag);

        /// <summary>
        /// Sets the value of the BindPassword attached property.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <param name="value">True to enable binding, otherwise false.</param>
        public static void SetBindPassword(DependencyObject dp, bool value)
            => dp.SetValue(BindPasswordFlag, value);

        /// <summary>
        /// Called when the BoundPassword property changes.
        /// Updates the <see cref="PasswordBox.Password"/> property if binding is enabled.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <param name="e">Event data.</param>
        private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is not PasswordBox pb) return;

            // Detach handler to prevent recursive updates
            pb.PasswordChanged -= PasswordChanged;

            // If binding is enabled and the new value differs from the current password, update it
            if (GetBindPassword(pb))
            {
                var newPwd = e.NewValue as string ?? string.Empty;
                if (pb.Password != newPwd)
                    pb.Password = newPwd;
            }

            // Reattach handler
            pb.PasswordChanged += PasswordChanged;
        }

        /// <summary>
        /// Called when the BindPassword property changes.
        /// Attaches or detaches the PasswordChanged event handler as needed.
        /// </summary>
        /// <param name="dp">The dependency object.</param>
        /// <param name="e">Event data.</param>
        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is not PasswordBox pb) return;

            bool wasBound = (bool)e.OldValue;
            bool needBind = (bool)e.NewValue;

            if (wasBound)
                pb.PasswordChanged -= PasswordChanged;
            if (needBind)
                pb.PasswordChanged += PasswordChanged;
        }

        /// <summary>
        /// Handles the PasswordChanged event of the <see cref="PasswordBox"/>.
        /// Updates the BoundPassword property when the password changes.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">Event data.</param>
        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not PasswordBox pb) return;
            if (!GetBindPassword(pb)) return;

            // Mark as updating to prevent recursive updates
            SetIsUpdating(pb, true);
            SetBoundPassword(pb, pb.Password);
            SetIsUpdating(pb, false);
        }
    }
}
