namespace Entities.Exceptions;

public sealed class CategoryNotFoundException : Exception
{
    public CategoryNotFoundException(int id)
        : base($"Id değeri {id} olan kategori bulunamadı.")
    {
    }
}