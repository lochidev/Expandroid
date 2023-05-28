using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.Models
{
    internal interface ICheckIfActivated
    {
        public bool IsActivated();
        public void OpenSettings();
    }
}
