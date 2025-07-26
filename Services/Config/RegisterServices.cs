using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services.Interfaces;
using Services.ServicesImpl;

namespace Services.Config
{
    public static class RegisterServices
    {
        public static IServiceCollection _services;
        public static IConfiguration _configuration;

        public static void RegisterComponents(IServiceCollection services, IConfiguration configuration)
        {
            _services = services;
            _configuration = configuration;

            //Register all the services here
            _services.AddTransient<IUserService, UserService>();
            _services.AddTransient<ITokenService, TokenService>();
            _services.AddTransient<IAdminService, AdminService>();
            _services.AddTransient<ISupplyService, SupplyService>();
            _services.AddTransient<IOrderService, OrderService>();
            _services.AddTransient<IPaymentService, PaymentService>();
            _services.AddTransient<IBillService, BillService>();
            _services.AddTransient<ICompanyService, CompanyService>();
            _services.AddTransient<INotificationService, NotificationService>();

        }
    }
}
