namespace Entities.Exceptions;

public sealed class InsufficientStockException : BadRequestException
{
    public InsufficientStockException(
        string productName,
        int availableStock,
        int requestedQuantity)
        : base(
            $"{productName} ürünü için yeterli stok bulunmamaktadır. " +
            $"Mevcut stok: {availableStock}, " +
            $"istenen miktar: {requestedQuantity}.")
    {
    }
}