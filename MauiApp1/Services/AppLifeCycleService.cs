using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Services
{
    public class AppLifecycleService
    {
        public event Action Paused;
        public void OnPaused(object? pSender, EventArgs pArgs)
        {
            Paused?.Invoke();
        }
        public event Action Stopped;
        public void OnStopped(object? pSender, EventArgs pArgs)
        {
            Stopped?.Invoke();
        }
    }
}
