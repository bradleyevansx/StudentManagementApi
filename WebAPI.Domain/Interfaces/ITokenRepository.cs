using WebAPI.Domain.Models;

namespace WebAPI.Domain.Interfaces;

public interface ITokenRepository
{
    public Task<AuthenticationResponse> CreateAuthenticationResponseAsync(UserInfo userInfo);
    public Task<AuthenticationResponse?> CreateAuthenticationResponseAsync(string refreshTokenId);
}