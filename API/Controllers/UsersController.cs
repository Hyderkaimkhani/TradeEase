using Domain.Models;
using Domain.Models.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

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