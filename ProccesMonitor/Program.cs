// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.Json;



using var client = new HttpClient();

string url = "http://localhost:5000/";
string? message = Console.ReadLine();

Point point = new Point(1, 2);

var s = JsonSerializer.Serialize(point);

var content = new StringContent(s, Encoding.UTF8, "text/plain");

try
{
    var response = await client.PostAsync(url, content);
    string responseText = await response.Content.ReadAsStringAsync();

    Console.WriteLine($"Ответ сервера: {responseText}");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка при отправке запроса: {ex.Message}");
}



