using System.Threading.Tasks;
using MassTransit;
using Messages;
using Microsoft.Extensions.Logging;

namespace PersonService.Consumers
{
    public class PersonCreatedConsumer : IConsumer<PersonCreated>
    {
        private readonly ILogger<PersonCreated> _logger;

        public PersonCreatedConsumer()
        {

        }

        public PersonCreatedConsumer(ILogger<PersonCreated> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PersonCreated> context)
        {
            var fullName = $"{context.Message.FirstName}" + (string.IsNullOrWhiteSpace(context.Message.LastName) ? string.Empty : $" {context.Message.LastName}");
            _logger.LogInformation($"Created person '{fullName}'");
            await Task.CompletedTask;
        }
    }
}
