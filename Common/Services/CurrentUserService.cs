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

        public int GetCurrentCompanyId()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            // Check if user is authenticated
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return 0;
            }

            var companyIdClaim = httpContext.User.FindFirst(claim => claim.Type == ClaimType.CompanyId);
            return Convert.ToInt32(companyIdClaim?.Value ?? "0");
        }

    }
}
