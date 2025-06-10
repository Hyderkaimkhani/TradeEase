using AutoMapper;
using Common;
using Domain.Entities;
using Domain.Models;
using Domain.Models.RequestModel;
using Microsoft.Extensions.Configuration;
using Repositories.Interfaces;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace Services.ServicesImpl
{
    public class UserService : IUserService
    {
        private readonly IMapper _autoMapper;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ITokenService _tokenService;

        public UserService(IUnitOfWorkFactory unitOfWorkFactory,
              IMapper autoMapper,
              IConfiguration configuration,
              ITokenService tokenService
            )
        {
            _autoMapper = autoMapper;
            _unitOfWorkFactory = unitOfWorkFactory;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        public async Task<ResponseModel<UserModel>> AddUser(AddUserRequestModel userRequestModel)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<UserModel>();

                if (await unitOfWork.UserRepository.UserExist(userRequestModel.UserName, userRequestModel.Email))
                {
                    response.IsError = true;
                    response.Message = "User already exists";
                }
                else
                {
                    var user = _autoMapper.Map<User>(userRequestModel);

                    //user.UserCategoryId = (int)UserCategoryEnum.SuperAdmin;
                    user.TotalLogin = 0;
                    user.IsActive = true;
                    user.Status = "Activated";
                    user.Password = Utilities.MD5Hash("helloworld" + _configuration.GetSection("RandomPasswordSeed").Value);
                    user.CreatedBy = "";//await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    user.CreatedDate = DateTime.UtcNow;
                    user.UpdatedBy = "";//await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sub);
                    user.UpdatedDate = DateTime.UtcNow;
                    user.Email = user.Email.ToLower();


                    var addedUser = await unitOfWork.UserRepository.AddUser(user);
                    if (addedUser != null)
                    {
                        //Got to do this to get user id generated
                        if (await unitOfWork.SaveChangesAsync())
                        {
                            response.Message = "User added successfuly";
                        }
                        else
                        {
                            response.IsError = true;
                            response.Message = "Unable to add user";
                        }
                    }
                    else
                    {
                        response.IsError = true;
                        response.Message = "Unable to add user";
                    }

                }
                return response;
            }
        }


        public async Task<ResponseModel<UserModel>> VerifyUser(LoginRequestModel loginRequestModel)
        {
            string passwordHash = Utilities.MD5Hash(loginRequestModel.Password + _configuration.GetSection("RandomPasswordSeed").Value);
            var response = new ResponseModel<UserModel>();
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var userEntity = await unitOfWork.UserRepository.VerifyUserCredentials(loginRequestModel.UserName, passwordHash);

                if (userEntity == null)
                {
                    response.IsError = true;
                    response.Message = "Invalid credentials";
                }
                else
                {
                    userEntity.LastLogin = DateTime.UtcNow;
                    userEntity.TotalLogin += 1;
                    await unitOfWork.SaveChangesAsync();

                    response.Model = _autoMapper.Map<UserModel>(userEntity);
                }

                return response;
            }
        }

        public async Task<ResponseModel<UserModel>> GetUser(int userId)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<UserModel>();

                var user = await unitOfWork.UserRepository.GetUser(userId);

                if (user == null)
                {
                    response.IsError = true;
                    response.Message = "User does not exists";
                }
                else
                {
                    response.Model = _autoMapper.Map<UserModel>(user);
                }

                return response;
            }
        }

        public async Task<ResponseModel<UserModel>> GetUserByUsername(string userName)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<UserModel>();

                var user = await unitOfWork.UserRepository.GetUserByUsername(userName);

                if (user == null)
                {
                    response.IsError = true;
                    response.Message = "User does not exists";
                }
                else
                {
                    response.Model = _autoMapper.Map<UserModel>(user);
                }

                return response;
            }
        }

        public async Task<ResponseModel<UserModel>> GetUserFromToken()
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<UserModel>();

                var userId = Int32.Parse(await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sid));

                var user = await unitOfWork.UserRepository.GetUser(userId);

                if (user == null)
                {
                    response.IsError = true;
                    response.Message = "User does not exists";
                }
                else
                {
                    response.Model = _autoMapper.Map<UserModel>(user);
                }

                return response;
            }
        }

        public async Task<ResponseModel<string>> ChangePassword(ChangePasswordRequestModel request)
        {
            using (var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork())
            {
                var response = new ResponseModel<string>();
                var userId = Convert.ToInt32(await _tokenService.GetClaimFromToken(JwtRegisteredClaimNames.Sid));
                var user = await unitOfWork.UserRepository.GetUser(userId);
                var oldPassword = Utilities.MD5Hash(request.OldPassword + _configuration.GetSection("RandomPasswordSeed").Value);

                if (!user.Password.ToLower().Equals(oldPassword.ToLower()))
                {
                    response.IsError = true;
                    response.Message = "Old password is not correct.";
                }
                else
                {
                    user.Password = Utilities.MD5Hash(request.NewPassword + _configuration.GetSection("RandomPasswordSeed").Value);
                    user.UpdatedBy = await _tokenService.GetClaimFromToken(ClaimType.Custom_Sub);


                    if (await unitOfWork.SaveChangesAsync())
                        response.Message = "Password changed successfully.";
                    else
                        response.Message = "Password change request failed.";
                }
                return response;
            }
        }

        
    }
}
