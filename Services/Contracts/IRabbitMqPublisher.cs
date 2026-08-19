using Entities.MessageModels;

namespace Services.Contracts;

public interface IRabbitMqPublisher
{
    Task PublishOrderCreatedAsync(OrderCreatedMessage message);
}