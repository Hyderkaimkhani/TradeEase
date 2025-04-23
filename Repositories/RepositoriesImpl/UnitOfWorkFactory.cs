using Repositories.Interfaces;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;

namespace Repositories.RepositoriesImpl
{
    public class UnitOfWorkFactory : IUnitOfWorkFactory
    {
        private IServiceProvider _serviceProvider;

        public UnitOfWorkFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IUnitOfWork CreateUnitOfWork()
        {
            return _serviceProvider.GetService<IUnitOfWork>();
        }
    }
}
