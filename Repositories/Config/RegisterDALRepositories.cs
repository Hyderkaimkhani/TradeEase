using Repositories.Context;
using Repositories.Interfaces;
using Repositories.RepositoriesImpl;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Repositories.Config
{
    public static class RegisterDALRepositories
    {
        public static IServiceCollection _services;
        public static IConfiguration _configuration;

        public static void RegisterComponents(IServiceCollection services, IConfiguration configuration)
        {
            _services = services;
            _configuration = configuration;

            var connection = _configuration.GetSection("ConnectionStrings").GetSection("SQLSERVERDB").Value;

            //_services.AddEntityFrameworkSqlServer()
            //    .AddDbContext<Context.Context>(
            //        (object options) => options.UseSqlServer(connection)
            //    );

            //Only register our unitofwork and its factory.
            _services.AddTransient<IUnitOfWorkFactory, UnitOfWorkFactory>();
            _services.AddTransient<IUnitOfWork, UnitOfWork>();
        }
    }
}
