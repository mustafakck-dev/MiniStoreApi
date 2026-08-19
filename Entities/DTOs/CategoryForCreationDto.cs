using System.ComponentModel.DataAnnotations;

namespace Entities.DTOs;

public record CategoryForCreationDto // Yeni kategori eklerken kullanılacak.
{
    [Required]
    [MinLength(2)]                           // Id yok çünkü SQL Server oluşturacak.
    [MaxLength(50)]
    public string Name { get; init; } = string.Empty;
}