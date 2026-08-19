namespace Entities.MessageModels;

public class OrderCreatedMessage
{
    public int OrderId { get; set; }

    public DateTime CreatedAt { get; set; }
}