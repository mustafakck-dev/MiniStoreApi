using Entities.DTOs;
using Microsoft.AspNetCore.Identity;

public interface IAuthenticationService
{
    Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto);

    Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuthenticationDto);

    Task<TokenDto> CreateTokenAsync();
}