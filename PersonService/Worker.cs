using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PersonService.Consumers;
using PersonService.Options;

namespace PersonService
{
    public class Worker : BackgroundService
    {
        public Worker(ILogger<Worker> logger, IOptions<RabbitMqOptions> options, ILoggerFactory loggerFactory)
        {
            _logger = logger;
            _options = options;
            _loggerFactory = loggerFactory;
            _createPersonQueueName = typeof(CreatePerson).Name;
            _personCreatedQueueName = typeof(PersonCreated).Name;
        }

        private IBusControl _busControl;
        private readonly ILogger<Worker> _logger;
        private readonly IOptions<RabbitMqOptions> _options;
        private readonly ILoggerFactory _loggerFactory;
        private readonly string _createPersonQueueName;
        private readonly string _personCreatedQueueName;

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StartAsync");

            _busControl = Bus.Factory.CreateUsingRabbitMq(
                configurator =>
                {
                    configurator.Host(new Uri(_options.Value.HostAddress),
                        host =>
                        {
                            host.Password(_options.Value.Password);
                            host.Username(_options.Value.Username);
                        });

                    EndpointConvention.Map<CreatePerson>(new Uri($"queue:{_createPersonQueueName}"));
                    EndpointConvention.Map<PersonCreated>(new Uri($"queue:{_personCreatedQueueName}"));

                    configurator.ReceiveEndpoint(_createPersonQueueName,
                        endpoint =>
                        {
                            var logger = _loggerFactory.CreateLogger<CreatePerson>();
                            endpoint.Consumer(() => new CreatePersonConsumer(logger));
                        });

                    configurator.ReceiveEndpoint(_personCreatedQueueName,
                    endpoint =>
                    {
                        var logger = _loggerFactory.CreateLogger<PersonCreated>();
                        endpoint.Consumer(() => new PersonCreatedConsumer(logger));
                    });
                });

            await _busControl.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExecuteAsync");
            await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("StopAsync");
            await _busControl?.StopAsync(cancellationToken);
        }
    }
}
