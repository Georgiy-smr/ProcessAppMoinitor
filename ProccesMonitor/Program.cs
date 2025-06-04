// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
string processName = "WpfMain"; // без расширения .exe

string appPath = @"C:\Users\Gosha and Anya\ADTS-Software\Adts\WpfMain\bin\Debug\net8.0-windows\WpfMain.exe";
Console.WriteLine("Hello, World!");

while (true)
{
    var processes = Process.GetProcessesByName(processName);
    if (processes.Length == 0)
    {
        // Запускаем приложение
        Process.Start(appPath);
    }
    Thread.Sleep(1000);
}



