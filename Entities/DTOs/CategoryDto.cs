namespace Entities.DTOs;

public record CategoryDto   // kategori bilgisini dışarıya döndürürken kullanılacak.
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;
}