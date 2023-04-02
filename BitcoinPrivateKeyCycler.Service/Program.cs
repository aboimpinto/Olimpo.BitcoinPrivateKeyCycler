using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BitcoinPrivateKeyCycler.Service;

class Program
{
    static void Main(string[] args)
    {
        CreateHostBuilder(args)
            .Build()
            .Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) => 
        Host.CreateDefaultBuilder(args)
        .UseSystemd()
        .ConfigureLogging(x => 
        {
            x.ClearProviders();
            x.AddConsole();
            x.AddDebug();

        })
        .ConfigureServices((hostContext, services) => 
        {
            services.AddHostedService<BitcoinPrivateKeyCyclerWorker>();
        });
}