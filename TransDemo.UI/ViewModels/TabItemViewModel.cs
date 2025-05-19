namespace TransDemo.UI.ViewModels
{
    /// <summary>
    /// Bazowy VM dla zakładek – daje Header i Content (UserControl).
    /// </summary>
    public abstract class TabItemViewModel
    {
        public abstract string Header { get; }
        public abstract object Content { get; }
    }
}
