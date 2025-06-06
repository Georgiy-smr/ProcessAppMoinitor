// See https://aka.ms/new-console-template for more information
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using MonitorAppLib;
using MonitorAppLib.Services;


IServiceCollection services = new ServiceCollection().AddSendDelayService();
IServiceProvider serviceProvider = services.BuildServiceProvider();
await using var scope = serviceProvider.CreateAsyncScope();
ISendDelay requiredService = scope.ServiceProvider.GetRequiredService<ISendDelay>();


Console.WriteLine("Введите число задержки:\n");
string? message = Console.ReadLine();
var result = await requiredService.SendDelayAsync(TimeSpan.FromSeconds(int.Parse(message!)));
Console.WriteLine(result);


//using var client = new HttpClient();

//string url = "http://localhost:5000/";
//string? message = Console.ReadLine();

//SetMonitorDelay delay = new SetMonitorDelay()
//{
//    Delay = int.Parse(message!)
//};

//var jsonSetDelay = JsonSerializer.Serialize(delay);

//var content = new StringContent(jsonSetDelay, Encoding.UTF8, "text/plain");

//try
//{
//    var response = await client.PostAsync(url, content);

//    Console.WriteLine($"Статус {response.StatusCode}");
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"Ошибка при отправке запроса: {ex.Message}");
//}



