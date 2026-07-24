namespace Expandroid.Models
{
    // for temporary hacks
    internal interface ICheckIfActivated
    {
        bool IsActivated();
        void OpenSettings();
        bool RequestPermission();
    }
}
