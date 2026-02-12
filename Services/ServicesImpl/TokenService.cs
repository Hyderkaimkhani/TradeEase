using AutoMapper;
using Common;
using Domain.Config;
using Domain.Models;
using Repositories.Interfaces;
using Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.ServicesImpl
{
    public class TokenService : ITokenService
    {
        private readonly IMapper _autoMapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IHttpContextAccessor _context;

        public TokenService(IUnitOfWorkFactory unitOfWorkFactory, IMapper autoMapper, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _autoMapper = autoMapper;
            _unitOfWorkFactory = unitOfWorkFactory;
            _configuration = configuration;
            _context = httpContextAccessor;
        }

        public async Task<ResponseModel<TokenModel>> GenerateTokens(UserModel userModel)
        {
            DateTime now = DateTime.Now;
            TokenProviderOptions tokenOptions = new TokenProviderOptions()
            {
                Audience = _configuration.GetSection("TokenProviderOptions").GetSection("TokenAudience").Value,
                Issuer = _configuration.GetSection("TokenProviderOptions").GetSection("TokenIssuer").Value,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["TokenProviderOptions:UserAuthSecretKey"])), SecurityAlgorithms.HmacSha256),
                AccessExpiration = new TimeSpan(Convert.ToInt32(_configuration["TokenProviderOptions:TokenExpirationDays"]),
                                                Convert.ToInt32(_configuration["TokenProviderOptions:TokenExpirationHours"]),
                                                Convert.ToInt32(_configuration["TokenProviderOptions:TokenExpirationMinutes"]), 0)
            };

            var accessJwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                claims: await GetTokenClaims(userModel, now),
                notBefore: now,
                expires: now.Add(tokenOptions.AccessExpiration),
                signingCredentials: tokenOptions.SigningCredentials);

            var encodedAccessJwt = new JwtSecurityTokenHandler().WriteToken(accessJwt);

            var refreshJwt = new JwtSecurityToken(
                issuer: tokenOptions.Issuer,
                audience: tokenOptions.Audience,
                claims: await GetTokenClaims(userModel, now),
                notBefore: now,
                expires: now.Add(tokenOptions.RefreshExpiration),
                signingCredentials: tokenOptions.SigningCredentials);

            var encodedRefreshJwt = new JwtSecurityTokenHandler().WriteToken(refreshJwt);
            var response = new ResponseModel<TokenModel>();
            try
            {
                using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
                {
                    var result = await unitOfWork.TokenRepository.AddForUser(userModel.Id, encodedRefreshJwt, encodedAccessJwt, DateTime.Now.AddSeconds(tokenOptions.RefreshExpiration.TotalSeconds));

                    if (await unitOfWork.SaveChangesAsync())
                    {
                        TokenModel tokenModelResponse = new TokenModel
                        {
                            AccessToken = encodedAccessJwt,
                            AccessTokenExpiry = (int)tokenOptions.AccessExpiration.TotalSeconds,
                            RefreshToken = encodedRefreshJwt
                        };

                        response.Model = tokenModelResponse;
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to generate token";
                    }
                }

                return response;
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task<string> GetClaimFromToken(string claimType)
        {
            if (!_context.HttpContext.User.Claims.Any())
                throw new UnauthorizedAccessException();

            var userClaims = _context.HttpContext.User;

            string claimValue = userClaims.FindFirst(claim => claim.Type == claimType)?.Value;

            if (string.IsNullOrWhiteSpace(claimValue))
                throw new UnauthorizedAccessException();

            return await Task.FromResult(claimValue);
        }

        public async Task<ResponseModel<TokenModel>> VerifyRefreshToken(TokenModel userTokenRequest)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                int userId = Convert.ToInt32(await GetSubFromToken(userTokenRequest.RefreshToken));

                var userAuth = await unitOfWork.TokenRepository.VerifyRefreshTokenAsync(userId, userTokenRequest.RefreshToken);

                if (userAuth != null)
                {
                    var user = await unitOfWork.UserRepository.GetUser(userId);

                    var userObject = _autoMapper.Map<UserModel>(user);

                    return await GenerateTokens(userObject);
                }
            }
            return null;
        }

        private async Task<Claim[]> GetTokenClaims(UserModel userModel, DateTime dateTime)
        {
            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sid, userModel.Id.ToString()),
                new Claim(ClaimType.Custom_Sub, userModel.UserName == null ? userModel.Email : userModel.UserName),
                new Claim(ClaimType.CompanyId, userModel.CompanyId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, userModel.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, Utilities.ToUnixEpochDate(dateTime).ToString(), ClaimValueTypes.Integer64),
                //new Claim(JwtRegisteredClaimNames.Iss, "Intellliro"),
                //new Claim(JwtRegisteredClaimNames.Aud, "YourAudienceHere"),
            };

            return await Task.FromResult(claims);
        }

        private async Task<string> GetSubFromToken(string token)
        {
            string data = string.Empty;

            if (!string.IsNullOrWhiteSpace(token))
            {
                data = (new JwtSecurityTokenHandler().ReadToken(token) as JwtSecurityToken).Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sid).Value;
            }

            if (string.IsNullOrWhiteSpace(data))
                throw new UnauthorizedAccessException();

            return await Task.FromResult(data);
        }
    }
}
