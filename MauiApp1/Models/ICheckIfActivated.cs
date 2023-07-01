namespace MauiApp1.Models
{
    // for temporary hacks
    internal interface ICheckIfActivated
    {
        public bool IsActivated();
        public void OpenSettings();
        public bool RequestPermission();
    }
}
