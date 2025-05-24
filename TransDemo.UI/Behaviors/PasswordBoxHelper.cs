using System;
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

        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdating",
                typeof(bool),
                typeof(PasswordBoxHelper),
                new PropertyMetadata(false));

        private static bool GetIsUpdating(DependencyObject dp)
           => (bool)dp.GetValue(IsUpdatingProperty);
        private static void SetIsUpdating(DependencyObject dp, bool value)
            => dp.SetValue(IsUpdatingProperty, value);


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


        private static void OnBoundPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            if (dp is not PasswordBox pb) return;

            // 1) odłączamy handler, żeby nie wpaść w pętlę
            pb.PasswordChanged -= PasswordChanged;

            // 2) jeśli bindowanie włączone i value różni się od tego co już w kontrolce
            if (GetBindPassword(pb))
            {
                var newPwd = e.NewValue as string ?? string.Empty;
                if (pb.Password != newPwd)
                    pb.Password = newPwd;
            }

            // 3) z powrotem podłączamy handler
            pb.PasswordChanged += PasswordChanged;
        }




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

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is not PasswordBox pb) return;
            if (!GetBindPassword(pb)) return;

            // zaznaczamy, że to MY teraz zmieniamy DP
            SetIsUpdating(pb, true);
            SetBoundPassword(pb, pb.Password);
            SetIsUpdating(pb, false);
        }
    }
}
