using JishitongBackend.Models;
using JishitongBackend.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JishitongBackend.Services
{
    public class UserService
    {

        private readonly IConfiguration _configuration;
        private readonly UserRepository _userRepository;

        public UserService(IConfiguration configuration)
        {
            _configuration = configuration; // 使用 IConfiguration 获取配置信息
            _userRepository = new UserRepository(new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")));
        }

        public async Task<Result<dynamic>> GetUserInformationService(int userId)
        {
            try
            {
                var result = await _userRepository.GetUserInformationRepository(userId);

                if (result == null)
                {
                    return Result<dynamic>.Failure(204, "查询结果为空");
                }
                else
                {
                    var response = new
                    {
                        code = 200,
                        msg = "获取用户信息成功",
                        user_id = userId,
                        user_name = result.Data.UserName,
                        user_role = result.Data.UserRole,
                        user_contact = result.Data.UserContact,
                        icon = result.Data.Icon,
                        user_introduction = result.Data.UserIntroduction,
                        user_age = result.Data.UserAge,
                        user_organization_id = result.Data.UserOrganizationId
                    };

                    return Result<dynamic>.Success(response, "获取用户信息成功");
                }
            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<dynamic>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<User>> UserAddService(User user, string? org_pass)
        {
            try
            {
                // 加密用户密码
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.UserPassword);

                // 设置加密后的密码
                user.UserPassword=hashedPassword;

                int result;
                //区分用户角色
                if (user.UserRole == "student")
                {
                    result = await _userRepository.UserAddRepository(user);
                }
                else if (user.UserRole == "admin")
                {
                    //验证管理员权限
                    result = await _userRepository.AdminAddRepository(user,org_pass);
                }
                else
                {
                    return Result<User>.Failure("用户角色错误");
                }

                // 如果受影响的行数大于 0，表示插入成功
                if (result == -1 )
                {
                    return Result<User>.Failure("注册失败，未插入任何数据");
                }
                else
                {
                    return Result<User>.Success(user, result.ToString());
                }
            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<User>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<User>> UserLoginService([FromBody] JsonElement login_request)
        {
            try
            {
                var result = await _userRepository.UserLoginRepository(login_request);
                var login_user = new User { };

                if (!result.IsSuccess)
                {
                    return Result<User>.Failure("登录失败");
                }
                else
                {
                    var userId = login_request.GetProperty("userId").ToString();
                    var userPassword = login_request.GetProperty("userPassword").GetString();
                    string token = GenerateToken(userId, userPassword);
                    login_user.UserId = result.Data.UserId;
                    login_user.UserName = result.Data.UserName;
                    login_user.UserRole = result.Data.UserRole;

                    return Result<User>.Success(login_user,token);
                }

            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<User>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<String>> UserModifyService([FromBody] JsonElement modify_request)
        {
            try
            {
                var result = await _userRepository.UserModifyRepository(modify_request);

                if (result == -1)
                {
                    return Result<String>.Failure("修改失败");
                }
                else if(result==1)
                {
                    return Result<String>.Success("修改成功");
                }
                else if(result==-3)
                {
                    return Result<String>.Failure("空请求");
                }
                else if (result == -4)
                {
                    return Result<String>.Failure("读不到id");
                }
                else
                {
                    return Result<String>.Failure("修改出错");
                }

            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<String>.Failure($"发生错误: {ex.Message}");
            }
        }

        public bool TokenCheckService(string authHeader)
        {
            try
            {
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return false;
                }
                var token = authHeader.Substring("Bearer ".Length).Trim();
                if (string.IsNullOrEmpty(token))
                {
                    return false;
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "ZJZ2024",
                    ValidAudience = "YourAudience",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (SecurityTokenException)
            {
                // 如果验证失败，返回 false
                return false;
            }

        }

        public async Task<Result<String>> UserDeleteService([FromBody] int userId)
        {
            try
            {
                var result = await _userRepository.UserDeleteRepository(userId);

                if (result == -1)
                {
                    return Result<String>.Failure("删除失败");
                }
                else if (result == 1)
                {
                    return Result<String>.Success("删除成功");
                }
                else if (result == -3)
                {
                    return Result<String>.Failure("空请求");
                }
                else if (result == -4)
                {
                    return Result<String>.Failure("读不到id");
                }
                else
                {
                    return Result<String>.Failure("删除出错");
                }

            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<String>.Failure($"发生错误: {ex.Message}");
            }
        }


        private string GenerateToken(string userId, string userRole)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, userId),
                new Claim(ClaimTypes.Role, userRole)
            };

            var expirationTime = DateTime.UtcNow.AddDays(2); // 设置有效时间为2days

            var jwtToken = new JwtSecurityToken(
                issuer: "ZJZ2024",
                audience: "YourAudience",
                claims: claims,
                expires: expirationTime,  // 设置过期时间
                signingCredentials: signingCredentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(jwtToken);

        }




    }
}
