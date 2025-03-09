using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Launcher.Services
{
    public interface IApplicationManager
    {
        void Close();
    }

    public class ApplicationManager : IApplicationManager
    {
        public void Close()
        {
            // TODO: Does not work
            var window = (Application.Current as App)?.Handler?.MauiContext?.Services.GetService<Microsoft.UI.Xaml.Window>();
            window?.Close();
        }
    }
}
