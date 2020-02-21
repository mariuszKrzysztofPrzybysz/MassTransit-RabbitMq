using System;
using System.IO;
using System.Threading.Tasks;
using Bogus;
using MassTransit;
using Messages;
using Microsoft.Extensions.Configuration;

namespace MessageGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IBusControl busControl = null;

            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile(Path.Combine(Environment.CurrentDirectory, "appsettings.json"))
                    .Build();

                var rabbitMqSection = configuration.GetSection("RabbitMq");

                busControl = Bus.Factory.CreateUsingRabbitMq(configurator =>
                {
                    configurator.Host(new Uri(rabbitMqSection["HostAddress"]),
                        host =>
                        {
                            host.Password(rabbitMqSection["Password"]);
                            host.Username(rabbitMqSection["Username"]);
                        });

                    EndpointConvention.Map<CreatePerson>(new Uri($"queue:{nameof(CreatePerson)}"));
                });

                await busControl.StartAsync();

                var people = new Faker<CreatePerson>("pl")
                    .RuleFor(p => p.FirstName, (f, p) => f.Name.FirstName())
                    .RuleFor(p => p.LastName, (f, p) => f.Name.LastName());

                foreach (var person in people.GenerateLazy(20))
                {
                    Console.WriteLine($"Adding a new person '{person.FirstName} {person.LastName}' in progress...");
                    await busControl.Publish<CreatePerson>(person);
                    await Task.Delay(TimeSpan.FromSeconds(5));
                }
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
