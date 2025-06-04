using Microsoft.Extensions.Hosting;

namespace Monitor
{
    public static class Program
    {
        public static IHostBuilder CreateHostBuilder(string[] args) => Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(App.ConfigureServices);

    }
}
