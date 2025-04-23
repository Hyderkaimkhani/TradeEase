using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Models.RequestModel;
using Domain.Models;
using Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UsersController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        //[Obsolete]
        [HttpPost()]
        public async Task<IActionResult> AddUser([FromBody] AddUserRequestModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = await _userService.AddUser(requestModel);
            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var responseTokenModel = new ResponseModel<TokenModel>();
            var userResponse = await _userService.VerifyUser(requestModel);
            if (userResponse.IsError)
            {
                return BadRequest(userResponse);
            }
            else
            {
                responseTokenModel = await _tokenService.GenerateTokens(userResponse.Model);
            }

            return Ok(responseTokenModel);
        }

        [Authorize]
        [HttpPost("AppToken")]
        public async Task<IActionResult> AddAppToken(AppTokenRequestModel requestModel)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = await _userService.AddAppToken(requestModel);
            return Ok(response);
        }

        [Authorize]
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangeUserPassword(ChangePasswordRequestModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest("One or more required parameters not passed.");

            var response = await _userService.ChangePassword(request);

            return Ok(response);
        }
    }
}