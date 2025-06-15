using Microsoft.AspNetCore.Mvc;
using JishitongBackend.Models;
using JishitongBackend.Services;
using Newtonsoft.Json;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace JishitongBackend.Controllers
{
    [Route("v1/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("getinformation")]
        public async Task<IActionResult> GetUserInformationController([FromHeader(Name = "Authorization")] string authHeader, int userId)
        {
            if (!_userService.TokenCheckService(authHeader))
            {
                return Unauthorized();
            }

            var result = await _userService.GetUserInformationService(userId);

            if (result.IsSuccess)
            {
                return Ok(result.Data);  // 成功时返回 200 和数据
            }
            else
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回 400 和错误消息
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> PostUserAdd([FromBody] User user, string? org_pass)
        {
            Console.WriteLine($"Register User: {JsonConvert.SerializeObject(user)}");
            
            var result = await _userService.UserAddService(user, org_pass);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "注册成功",
                    user_id = int.Parse(result.Message)
                };
                return Ok(response);  // 成功时返回 200 和数据
            }
            else
            {
                return BadRequest(result);  // 失败时返回 400 和错误消息
            }

        }

        //POST v1/user/login
        [HttpPost("login")]
        public async Task<IActionResult> PostUserLogin([FromBody] JsonElement login_request)
        {
            Console.WriteLine($"Login User: {JsonConvert.SerializeObject(login_request)}");

            var result = await _userService.UserLoginService(login_request);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "登录成功",
                    user_id = result.Data.UserId,
                    user_name = result.Data.UserName,
                    user_role = result.Data.UserRole,
                    token = result.Message
                };
                return Ok(response);  // 成功时返回 200 和数据
            }
            else
            {
                return BadRequest(result);  // 失败时返回 400 和错误消息
            }
        }

        //POST v1/user/modify
        [HttpPost("modify")]
        public async Task<IActionResult> PostUserModify([FromHeader(Name = "Authorization")] string authHeader, [FromBody] JsonElement modify_request)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized();}

            Console.WriteLine($"Modify User: {JsonConvert.SerializeObject(modify_request)}");

            var result = await _userService.UserModifyService(modify_request);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "修改成功"
                };
                return Ok(response);  // 成功时返回 200 和数据
            }
            else
            {
                return BadRequest(result);  // 失败时返回 400 和错误消息
            }
        }

        //GET v1/user/check
        [HttpPost("check")]
        public IActionResult GetTokenCheck([FromHeader(Name = "Authorization")] string authHeader)
        {
            Console.WriteLine($"Token Check: {authHeader}");
            var result = _userService.TokenCheckService(authHeader);
            if (result)
            {
                var response = new
                {
                    code = 200,
                    msg = "token有效"
                };
                return Ok(response);  // 成功时返回 200 和数据
            }
            else
            {
                var response = new
                {
                    code = 401,
                    msg = "token无效"
                };
                return Unauthorized(response);  // 失败时返回 401 和错误消息
            }


        }

        //DELETE v1/user/delete
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromHeader(Name = "Authorization")] string authHeader, int userId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (userIdClaim != null)
            {
                var userIdinJWT = userIdClaim.Value;  // 获取 userId
                if (userIdinJWT.ToString() != userId.ToString())
                {
                    return Unauthorized();
                }
            }

            var result = await _userService.UserDeleteService(userId);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "删除成功"
                };
                return Ok(response);  // 成功时返回 200 和数据
            }
            else
            {
                return BadRequest(result);  // 失败时返回 400 和错误消息
            }
        }


    }
}
