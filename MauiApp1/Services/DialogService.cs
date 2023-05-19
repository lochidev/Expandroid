using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Services
{
    internal class DialogService : IDialogService
    {
        public async Task DisplayConfirmAsync(string title, string message)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "Ok");
        }
    }
}
