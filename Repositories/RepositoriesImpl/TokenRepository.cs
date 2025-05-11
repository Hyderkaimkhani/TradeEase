using Domain.Entities;
using Repositories.Context;
using Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.RepositoriesImpl
{
    public class TokenRepository : ITokenRepository
    {
        private readonly Context.Context _context;

        internal TokenRepository(Context.Context context)
        {
            _context = context;
        }

        public async Task<int> AddForUser(int userId, string refreshToken, string jwtToken, DateTime refreshToken_ValidTill)
        {
            var user = await _context.UserAuth.Where(x => x.User.Id == userId).FirstOrDefaultAsync();

            if (user != null)
            {
                user.JWTToken = jwtToken;
                user.RefreshToken = refreshToken;
                user.RefreshTokenValidTill = refreshToken_ValidTill;
                user.UpdatedDate = DateTime.UtcNow;
                _context.UserAuth.Update(user);
            }
            else
            {
                var userAuth = new UserAuth();
                userAuth.UserId = userId;
                userAuth.RefreshToken = refreshToken;
                userAuth.JWTToken = jwtToken;
                userAuth.RefreshTokenValidTill = refreshToken_ValidTill;
                userAuth.CreatedDate = DateTime.UtcNow;
                userAuth.UpdatedDate = DateTime.UtcNow;
                _context.UserAuth.Add(userAuth);
            }

            return await Task.FromResult(1);
        }

        public async Task<int> InvalidateTokens(int userId)
        {
            var user = await _context.UserAuth.Where(x => x.UserId == userId).FirstOrDefaultAsync();

            _context.UserAuth.Remove(user);

            return await Task.FromResult(1);
        }

        public async Task<UserAuth> VerifyAccessTokenAsync(string token)
        {
            return await _context.UserAuth.Where(x => x.JWTToken == token).FirstOrDefaultAsync();
        }

        public async Task<UserAuth> VerifyRefreshTokenAsync(int userId, string refreshToken)
        {
            return await _context.UserAuth.Where(x => x.UserId == userId && x.RefreshToken == refreshToken && x.RefreshTokenValidTill >= DateTime.Now).FirstOrDefaultAsync();
        }
    }
}
