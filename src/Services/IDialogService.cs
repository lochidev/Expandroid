namespace Expandroid.Services
{
    internal interface IDialogService
    {
        Task<bool> DisplayConfirmAsync(string title, string message, string accept = "OK", string cancel = null);
    }
}
