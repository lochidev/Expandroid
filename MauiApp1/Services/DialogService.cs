using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Services
{
    internal class DialogService : IDialogService
    {
        public async Task<bool> DisplayConfirmAsync(string title, string message, string accept = "Ok", string cancel = null)
        {
            if (cancel is null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, accept);
                return true;
            }
            else
            {
                return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
            }
        }
    }
}
