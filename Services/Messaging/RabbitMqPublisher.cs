using Entities.MessageModels;
using RabbitMQ.Client;
using Services.Configuration;
using Services.Contracts;
using System.Text;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Services.Messaging;

public class RabbitMqPublisher : IRabbitMqPublisher, IAsyncDisposable
{
    private const string ExchangeName = "ministore.orders"; // 
    private const string RoutingKey = "order.created"; // Routing key for the order created messages

    private readonly ConnectionFactory _factory;// RabbitMQ connection’ı oluşturacak yapı

    private readonly SemaphoreSlim _connectionLock = new(1, 1);// Semaphore to ensure thread-safe connection creation
    private readonly SemaphoreSlim _publishLock = new(1, 1);// Semaphore to ensure thread-safe message publishing

    private IConnection? _connection; //MiniStoreApi ile RabbitMQ arasındaki ana bağlantı.
    private IChannel? _channel;//Connection açıldıktan sonra publish/consume işlemlerini channel üzerinden yapıyoruz.

    public RabbitMqPublisher(RabbitMqSettings settings)
    {
        _factory = new ConnectionFactory
        {
            HostName = settings.Host,
            UserName = settings.UserName,
            Password = settings.Password,
            ClientProvidedName = "MiniStoreApi Publisher"
        };
    }

    // RabbitMQ bağlantısı ve channel yoksa oluşturur.
    private async Task EnsureConnectionAsync()
    {
        if (_connection?.IsOpen == true && _channel?.IsOpen == true)
        {
            return;
        }

        await _connectionLock.WaitAsync();

        try
        {
            if (_connection?.IsOpen == true && _channel?.IsOpen == true)
            {
                return;
            }

            _connection = await _factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    // OrderCreated mesajını JSON'a çevirip RabbitMQ'ya gönderir.
    public async Task PublishOrderCreatedAsync(OrderCreatedMessage message)
    {
        await EnsureConnectionAsync();

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await _publishLock.WaitAsync();

        try
        {
            await _channel!.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                body: body);
        }
        finally
        {
            _publishLock.Release();
        }
    }

    // Uygulama kapanırken RabbitMQ kaynaklarını kapatır.
    public async ValueTask DisposeAsync()
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        _connectionLock.Dispose();
        _publishLock.Dispose();
    }
}