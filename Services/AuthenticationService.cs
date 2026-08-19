using AutoMapper;
using Entities.DTOs;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Services.Contracts;
using Entities.ConfigurationModels;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IMapper _mapper;   // DTO’yu User entity’sine çevirir
    private readonly UserManager<User> _userManager; // Kullanıcı oluşturma ve parola işlemlerini yönetir.
    private readonly ILogger<AuthenticationService> _logger; // Kayıt sonucunu loglamak için kullanılır.
    private readonly JwtSettings _jwtSettings;

    private User? _user;
    public AuthenticationService(
    IMapper mapper,
    UserManager<User> userManager,
    ILogger<AuthenticationService> logger,
    IOptions<JwtSettings> jwtSettings)
    {
        _mapper = mapper;
        _userManager = userManager;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto)
    {
        var user = _mapper.Map<User>(userForRegistrationDto);

        var result = await _userManager.CreateAsync(user,userForRegistrationDto.Password);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Kullanıcı kaydı başarısız oldu. UserName: {UserName}",
                userForRegistrationDto.UserName);

            return result;
        }

        var roleResult = await _userManager.AddToRoleAsync(user,"User");
        // Bu kullanıcıyı User isimli role ekle.Yeni kayıt olan herkes otomatik User rolüne atanır.
        if (!roleResult.Succeeded)
        {
            _logger.LogError(
                "Kullanıcı oluşturuldu ancak User rolü atanamadı. UserId: {UserId}",
                user.Id);

            return roleResult;
        }

        _logger.LogInformation(
            "Kullanıcı oluşturuldu ve User rolü atandı. UserId: {UserId}, UserName: {UserName}",
            user.Id,
            user.UserName);

        return result;
    }
    public async Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuthenticationDto)
    {
        _user = await _userManager.FindByNameAsync(userForAuthenticationDto.UserName);

        var isValid =
            _user is not null &&
            await _userManager.CheckPasswordAsync(_user,userForAuthenticationDto.Password);

        if (!isValid)
        {
            _logger.LogWarning(
                "Başarısız giriş denemesi. UserName: {UserName}",
                userForAuthenticationDto.UserName);
        }

        return isValid;
    }
    private async Task<List<Claim>> GetClaimsAsync()
    {
        if (_user is null)
        {
            throw new InvalidOperationException(
                "Token oluşturulmadan önce kullanıcı doğrulanmalıdır.");
        }

        var claims = new List<Claim>
    {
        new(
            ClaimTypes.NameIdentifier,
            _user.Id),

        new(
            ClaimTypes.Name,
            _user.UserName ?? string.Empty),

        new(
            ClaimTypes.Email,
            _user.Email ?? string.Empty)
    };

        var roles = await _userManager.GetRolesAsync(_user);

        foreach (var role in roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role));
        }

        return claims;
    }
    public async Task<TokenDto> CreateTokenAsync()
    {
        var claims = await GetClaimsAsync();

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _jwtSettings.SecretKey));

        var signingCredentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow
            .AddMinutes(_jwtSettings.Expires);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        var accessToken =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        return new TokenDto
        {
            AccessToken = accessToken,
            ExpiresAt = expiresAt
        };
    }
}