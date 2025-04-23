using Microsoft.IdentityModel.Tokens;

namespace Domain.Config
{
    public class TokenProviderOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public TimeSpan ActiveExpiration { get; set; } = TimeSpan.FromDays(1);
        public TimeSpan ResetExpiration { get; set; } = TimeSpan.FromDays(1);
        public TimeSpan AccessExpiration { get; set; } = TimeSpan.FromMinutes(15);
        public TimeSpan RefreshExpiration { get; set; } = TimeSpan.FromDays(30);
        public SigningCredentials SigningCredentials { get; set; }
    }
}
