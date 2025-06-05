// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Text.Json;
using MonitorAppLib;


using var client = new HttpClient();

string url = "http://localhost:5000/";
string? message = Console.ReadLine();

SetMonitorDelay delay = new SetMonitorDelay()
{
    Delay = int.Parse(message!)
};

var jsonSetDelay = JsonSerializer.Serialize(delay);

var content = new StringContent(jsonSetDelay, Encoding.UTF8, "text/plain");

try
{
    var response = await client.PostAsync(url, content);

    Console.WriteLine($"Статус {response.StatusCode}");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при отправке запроса: {ex.Message}");
}



