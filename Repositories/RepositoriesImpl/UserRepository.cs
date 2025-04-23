using Domain.Entities;
using Domain.Models;
using Repositories.Context;
using Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.RepositoriesImpl
{
    public class UserRepository : IUserRepository
    {
        private readonly Context.Context _context;

        internal UserRepository(Context.Context context)
        {
            _context = context;
        }

        #region User

        public async Task<User> GetUser(int Id)
        {
            var user = await _context.Set<User>().FirstOrDefaultAsync(x => x.Id == Id && x.IsActive == true);
            return user;
        }


        public async Task<User> GetUserByUsername(string userName)
        {
            var user = await _context.Set<User>().FirstOrDefaultAsync(x => x.UserName == userName && x.IsActive == true);
            return user;
        }
        public async Task<User> AddUser(User userEntity)
        {
            var user = _context.User.Add(userEntity);
            return await Task.FromResult(user.Entity);
        }

        public async Task<bool> UserExist(string userName, string email)
        {
            return await _context.Set<User>().AnyAsync(x => (x.UserName == userName && x.Email == email));
        }

        public async Task<User> VerifyUserCredentials(string userName, string passwordHash)
        {
            var entity = await _context.Set<User>().FirstOrDefaultAsync(x => x.IsActive == true && (x.UserName == userName || x.Email == userName) && x.Password.ToUpper() == passwordHash);
            return entity;
        }
        #endregion

        public async Task<AppToken> AddAppToken(AppToken entity)
        {
            var appToken = _context.AppToken.Add(entity);
            return await Task.FromResult(appToken.Entity);
        }

        public async Task<bool> TokenExist(long resourceId, string token)
        {
            return await _context.Set<AppToken>().AnyAsync(x => (x.ResourceId == resourceId && x.Token == token));
        }

    }
}
