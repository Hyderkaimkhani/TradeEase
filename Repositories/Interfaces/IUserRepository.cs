using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUser(int Id);
        Task<User> GetUserByUsername(string userName);
        Task<User> AddUser(User userEntity);
        Task<bool> UserExist(string userName, string email);
        Task<User> VerifyUserCredentials(string userName, string passwordHash);
        Task<AppToken> AddAppToken(AppToken entity);
        Task<bool> TokenExist(long resourceId, string token);
        
    }
}
