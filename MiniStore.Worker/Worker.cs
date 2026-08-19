using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Entities.MessageModels;
using System.Text.Json;

namespace MiniStore.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;

    private IConnection? _connection;
    private IChannel? _channel;

    public Worker(ILogger<Worker> logger,IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    // Worker başladığında RabbitMQ'ya bağlanır ve queue'yu dinlemeye başlar.
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:Host"],
            UserName = _configuration["RabbitMq:UserName"],
            Password = _configuration["RabbitMq:Password"]
        };

        // RabbitMQ ile ana bağlantıyı açar.
        _connection = await factory.CreateConnectionAsync();

        // Mesaj tüketmek için channel oluşturur.
        _channel = await _connection.CreateChannelAsync();

        // Aynı anda bu worker'a en fazla 1 işlenmemiş mesaj gönder.
        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false);

        var consumer =
            new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, eventArgs) =>
        {
            try
            {
                // RabbitMQ'dan gelen byte verisini alır.
                var body = eventArgs.Body.ToArray();

                // Byte verisini JSON string'e çevirir.
                var json = Encoding.UTF8.GetString(body);

                // JSON'u OrderCreatedMessage nesnesine çevirir.
                var message =
                    JsonSerializer.Deserialize<OrderCreatedMessage>(json);

                if (message is null)
                {
                    throw new Exception(
                        "OrderCreatedMessage deserialize edilemedi.");
                }
               

                // Şimdilik gerçek background işi yerine log basıyoruz.
                _logger.LogInformation("Sipariş mesajı işlendi. OrderId: {OrderId}, CreatedAt: {CreatedAt}",
                    message.OrderId,
                    message.CreatedAt);

                // İşlem başarılıysa RabbitMQ'ya ACK gönderir.
                await _channel!.BasicAckAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "RabbitMQ mesajı işlenirken hata oluştu.");

                // İşlem başarısızsa mesajı tekrar queue'ya koyar.
                await _channel!.BasicNackAsync(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false,
                    requeue: true);
            }
        };

        // order-created queue'sunu dinlemeye başlar.
        await _channel.BasicConsumeAsync(queue: "order-created",autoAck: false,consumer: consumer);

        _logger.LogInformation("Worker order-created queue'sunu dinliyor.");

        // Worker kapanana kadar uygulamayı canlı tut.
        await Task.Delay(Timeout.Infinite,stoppingToken);
    }

    // Worker kapanırken RabbitMQ kaynaklarını temizler.
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.DisposeAsync();
        }

        if (_connection is not null)
        {
            await _connection.DisposeAsync();
        }

        await base.StopAsync(cancellationToken);
    }
}