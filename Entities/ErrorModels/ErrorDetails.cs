namespace Entities.ErrorModels;

public sealed class ErrorDetails
{
    public int StatusCode { get; set; }

    public string Message { get; set; } = string.Empty;
}