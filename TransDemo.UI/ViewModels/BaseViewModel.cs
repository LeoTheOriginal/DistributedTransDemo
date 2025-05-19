using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TransDemo.UI.ViewModels
{
    /// <summary>
    /// Klasa bazowa dla wszystkich ViewModeli: udostępnia INotifyPropertyChanged
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Wywołaj, gdy zmieni się wartość właściwości
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Pomocnicza metoda do ustawiania właściwości i powiadamiania UI
        /// </summary>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
