using Entities.ErrorModels;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace WebApi.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static void ConfigureExceptionHandler(this WebApplication app)
    {
        var logger = app.Services
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("GlobalExceptionHandler");

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.ContentType = "application/json";

                var exceptionFeature = context.Features
                    .Get<IExceptionHandlerFeature>();

                if (exceptionFeature is null)
                {
                    return;
                }

                var exception = exceptionFeature.Error;

                context.Response.StatusCode = exception switch
                {
                    ProductNotFoundException =>
                        StatusCodes.Status404NotFound,

                    CategoryNotFoundException =>
                        StatusCodes.Status404NotFound,

                    BadRequestException =>
                        StatusCodes.Status400BadRequest,

                    _ =>
                        StatusCodes.Status500InternalServerError
                };

                if (context.Response.StatusCode ==
                    StatusCodes.Status500InternalServerError)
                {
                    logger.LogError(
                        exception,
                        "Beklenmeyen sistem hatası. Path: {RequestPath}",
                        context.Request.Path);
                }

                var errorDetails = new ErrorDetails
                {
                    StatusCode = context.Response.StatusCode,

                    Message = context.Response.StatusCode ==
                              StatusCodes.Status500InternalServerError
                        ? "Beklenmeyen bir sunucu hatası oluştu."
                        : exception.Message
                };

                await context.Response
                    .WriteAsJsonAsync(errorDetails);
            });
        });
    }
}