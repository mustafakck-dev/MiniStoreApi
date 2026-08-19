namespace Entities.Exceptions;

public sealed class ProductNotFoundException : Exception
{
    public ProductNotFoundException(int id)
        : base($"Id değeri {id} olan ürün bulunamadı.")
    {
    }
}