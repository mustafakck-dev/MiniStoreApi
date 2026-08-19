using Entities.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IServiceManager _serviceManager;

    public AuthenticationController(IServiceManager serviceManager)
    {
        _serviceManager = serviceManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistrationDto)
    {
        var result = await _serviceManager.AuthenticationService.RegisterUserAsync(userForRegistrationDto);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(
                    error.Code,
                    error.Description);
            }

            return BadRequest(ModelState);
        }

        return StatusCode(StatusCodes.Status201Created);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthenticationDto)
    {
        var isValid = await _serviceManager.AuthenticationService.ValidateUserAsync(userForAuthenticationDto);

        if (!isValid)
        {
            return Unauthorized("Kullanıcı adı veya parola hatalı.");
        }

        var token = await _serviceManager.AuthenticationService.CreateTokenAsync();

        return Ok(token);
    }
}