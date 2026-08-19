namespace Entities.Exceptions;

public sealed class InvalidPriceRangeException : Exception
{
    public InvalidPriceRangeException()
        : base("Minimum fiyat, maksimum fiyattan büyük olamaz.")
    {
    }
}