using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Services
{
    internal interface IDialogService
    {
        Task DisplayConfirmAsync(string title, string message);
    }
}
