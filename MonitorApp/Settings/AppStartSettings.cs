using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorApp.Settings
{
    public class AppStartSettings
    {
        public string NameApp { get; set; } = "WpfMain";
        public string AppPath { get; set; } = @"C:\Users\Gosha and Anya\ADTS-Software\Adts\WpfMain\bin\Debug\net8.0-windows\WpfMain.exe";
        public int TimeInterval { get; set; } = 1200;
    }
}
