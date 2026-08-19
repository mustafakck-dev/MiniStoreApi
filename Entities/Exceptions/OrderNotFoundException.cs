namespace Entities.Exceptions;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException(int orderId)
        : base($"Id değeri {orderId} olan sipariş bulunamadı.")
    {
    }
}