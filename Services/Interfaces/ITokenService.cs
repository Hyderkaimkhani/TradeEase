using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GetClaimFromToken(string claimType);
        Task<ResponseModel<TokenModel>> GenerateTokens(UserModel userModel);
        Task<ResponseModel<TokenModel>> VerifyRefreshToken(TokenModel userTokenRequest);
    }
}
