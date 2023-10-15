using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Domain.Interfaces;
using WebAPI.Domain.Models;

namespace WebAPI.Repository;

public class TokenRepository : ITokenRepository
{
    private readonly IConfiguration _configuration;
    private readonly IRepository<RefreshToken> _refreshTokenRepository;

    public TokenRepository(IConfiguration configuration, IRepository<RefreshToken> refreshTokenRepository)
    {
        _configuration = configuration;
        _refreshTokenRepository = refreshTokenRepository;
    }


    public async Task<AuthenticationResponse?> CreateAuthenticationResponseAsync(string refreshTokenId)
    {
        var token = await _refreshTokenRepository.GetAsync(refreshTokenId);

        if (token!.IsValid is false)
        {
            return null;
        }

        var response = new AuthenticationResponse();
        response.userType = token.UserType;
        response.userId = token.UserId;

        SetAccessToken(response);
        await SetRefreshTokenAsync(response);
        return response;
    }

    public async Task<AuthenticationResponse> CreateAuthenticationResponseAsync(UserInfo userInfo)
    {
        var response = new AuthenticationResponse
        {
            userType = userInfo.UserType,
            userId = userInfo.id
        };

        SetAccessToken(response);
        await SetRefreshTokenAsync(response);
        return response;
    }

    private async Task SetRefreshTokenAsync(AuthenticationResponse response)
    {
        var refreshToken = new RefreshToken();
        refreshToken.UserType = response.userType;
        refreshToken.UserId = response.userId;
        refreshToken.IsValid = true;
        refreshToken.ExpirationDateTime = DateTime.UtcNow.AddMonths(6);
        await _refreshTokenRepository.CreateAsync(refreshToken);

        response.refreshTokenId = refreshToken.id;
    }

    private void SetAccessToken(AuthenticationResponse response)
    {
        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.UserData, response.userId),
            new Claim(ClaimTypes.Name, response.userId),
            new Claim(ClaimTypes.Role, response.userType.ToString()),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var expiration = DateTime.UtcNow.AddHours(1);
        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);


        response.accessToken = jwt;
        response.accessTokenExpiration = expiration;
    }
}