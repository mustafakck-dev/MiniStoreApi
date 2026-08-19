namespace Entities.DTOs;

public record TokenDto
{
    public string AccessToken { get; init; } = string.Empty;

    public DateTime ExpiresAt { get; init; }
}