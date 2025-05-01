using Domain.Models;
using Domain.Models.RequestModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponseModel<UserModel>> AddUser(AddUserRequestModel userRequestModel);
        Task<ResponseModel<UserModel>> VerifyUser(LoginRequestModel loginRequestModel);
        Task<ResponseModel<UserModel>> GetUser(int userId);
        Task<ResponseModel<UserModel>> GetUserFromToken();
        Task<ResponseModel<string>> ChangePassword(ChangePasswordRequestModel request);

    }
}
