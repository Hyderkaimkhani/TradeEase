using Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Common.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }

        public string GetCurrentUsername()
        {
            var userClaims = _httpContextAccessor.HttpContext?.User;
            string userName = userClaims?.FindFirst(claim => claim.Type == ClaimType.Custom_Sub)?.Value ?? "System";
            return userName;
        }

    }
}
