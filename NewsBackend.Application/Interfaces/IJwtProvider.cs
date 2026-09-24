using NewsBackend.Domain.Entities;

namespace NewsBackend.Application.Interfaces;

public interface IJwtProvider
{
    string GenerateAccessToken(User user);

    (string Token, DateTime ExpiresAt) GenerateRefreshToken();
}