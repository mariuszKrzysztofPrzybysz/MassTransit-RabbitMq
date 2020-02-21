using System;
using System.Threading.Tasks;
using MassTransit;
using Messages;
using Microsoft.Extensions.Logging;

namespace PersonService.Consumers
{
    public class CreatePersonConsumer : IConsumer<CreatePerson>
    {
        private readonly ILogger<CreatePerson> _logger;

        public CreatePersonConsumer()
        {

        }

        public CreatePersonConsumer(ILogger<CreatePerson> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreatePerson> context)
        {
            var fullName = context.Message.FirstName + (string.IsNullOrWhiteSpace(context.Message.LastName) ? string.Empty : $" {context.Message.LastName}");
            _logger.LogInformation($"Creating person '{fullName}' in progress");

            //TODO: SQL

            var newlyCreatedPerson = new PersonCreated
            {
                Id = NewId.NextGuid(),
                FirstName = context.Message.FirstName,
                LastName = context.Message.LastName
            };

            _logger.LogInformation($"Sending newly created person '{fullName}' in progress");
            await context.Send(newlyCreatedPerson, context.CancellationToken);
        }
    }
}
