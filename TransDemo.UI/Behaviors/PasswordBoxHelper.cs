using System.Windows;
using System.Windows.Controls;

namespace TransDemo.UI.Behaviors
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached(
                "BoundPassword",
                typeof(string),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static string GetBoundPassword(DependencyObject dp)
            => (string)dp.GetValue(BoundPassword);

        public static void SetBoundPassword(DependencyObject dp, string value)
            => dp.SetValue(BoundPassword, value);

        private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox pb)
            {
                pb.PasswordChanged -= PasswordChanged;
                if ((bool)pb.GetValue(BindPasswordFlag))
                {
                    pb.Password = e.NewValue as string ?? string.Empty;
                }
                pb.PasswordChanged += PasswordChanged;
            }
        }

        private static readonly DependencyProperty BindPasswordFlag =
            DependencyProperty.RegisterAttached(
                "BindPassword",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false, OnBindPasswordChanged));

        public static bool GetBindPassword(DependencyObject dp)
            => (bool)dp.GetValue(BindPasswordFlag);

        public static void SetBindPassword(DependencyObject dp, bool value)
            => dp.SetValue(BindPasswordFlag, value);

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is PasswordBox pb)
            {
                bool wasBound = (bool)e.OldValue;
                bool needBind = (bool)e.NewValue;
                if (wasBound)
                    pb.PasswordChanged -= PasswordChanged;
                if (needBind)
                    pb.PasswordChanged += PasswordChanged;
            }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                SetBoundPassword(pb, pb.Password);
            }
        }
    }
}
