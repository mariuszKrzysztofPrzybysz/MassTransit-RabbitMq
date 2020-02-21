using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PersonService.Options;

namespace PersonService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    var configuration = hostContext.Configuration;

                    services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

                    services.AddHostedService<Worker>();
                });
    }
}
