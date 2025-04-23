using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface ITokenRepository
    {
        Task<int> InvalidateTokens(int userId);
        Task<UserAuth> VerifyAccessTokenAsync(string token);
        Task<UserAuth> VerifyRefreshTokenAsync(int userId, string refreshToken);
        Task<int> AddForUser(int userId, string refreshToken, string jwtToken, DateTime refreshToken_ValidTill);
    }
}
