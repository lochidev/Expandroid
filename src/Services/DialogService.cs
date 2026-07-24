namespace Expandroid.Services
{
    internal class DialogService : IDialogService
    {
        public async Task<bool> DisplayConfirmAsync(string title, string message, string accept = "OK", string cancel = null)
        {
            Page page = Application.Current.Windows[0].Page;
            if (cancel is null)
            {
                await page.DisplayAlertAsync(title, message, accept);
                return true;
            }
            else
            {
                return await page.DisplayAlertAsync(title, message, accept, cancel);
            }
        }
    }
}
