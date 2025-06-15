using JishitongBackend.Models;
using System.Text.Json;

namespace JishitongBackend.Repositories
{
    public class UserRepository
    {
        private readonly MySqlConnection _mySqlConnection;

        public UserRepository(MySqlConnection mySqlConnection)
        {
            _mySqlConnection = mySqlConnection;
        }

        public async Task<Result<User>> GetUserInformationRepository(int userId)
        {
            string query = "SELECT * FROM user WHERE user_id = ?";
            var parameters = new List<object> { userId };

            var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

            if (result.Count == 0)
            {
                return Result<User>.Failure("找不到用户");
            }

            var user = new User
            {
                UserName = result[0]["USER_NAME"].ToString(),
                UserRole = result[0]["USER_ROLE"].ToString(),
                UserPassword = result[0]["USER_PASSWORD"].ToString(),
                UserContact = result[0]["USER_CONTACT"].ToString(),
                Icon = result[0]["ICON"].ToString(),
                UserIntroduction = result[0]["USER_INTRODUCTION"].ToString(),
            };
            if(result[0]["USER_AGE"] != DBNull.Value && result[0]["USER_AGE"] != null)
            {
                user.UserAge = int.Parse(result[0]["USER_AGE"].ToString());
            }
            if(result[0]["USER_ORGANIZATION_ID"] != DBNull.Value && result[0]["USER_ORGANIZATION_ID"] != null)
            {
                user.UserOrganizationId = int.Parse(result[0]["USER_ORGANIZATION_ID"].ToString());
            }

            return Result<User>.Success(user,"获取用户信息成功");
        }

        public async Task<int> UserAddRepository(User user)
        {

            string query = @"
                INSERT INTO user (user_name, user_role, user_password)
                VALUES (?,?,?)";

            var parameters = new List<object>
            {
                user.UserName,
                user.UserRole,
                user.UserPassword
            };

            var rowsAffected = await _mySqlConnection.ExecuteNonQueryAsync(query, parameters);

            if (rowsAffected == 0)
            {
                return -1;
            }
            else
            {
                string selectQuery = "SELECT LAST_INSERT_ID()";
                var newUserIdResult = await _mySqlConnection.ExecuteQueryAsync(selectQuery);

                // 这里假设 LAST_INSERT_ID() 返回一个只有一列的结果
                if (newUserIdResult.Count > 0)
                {
                    // 从结果中获取第一个字典，获取 'LAST_INSERT_ID()' 对应的值
                    var newUserId = newUserIdResult[0].Values.FirstOrDefault();
                    if (newUserId != null && int.TryParse(newUserId.ToString(), out int userId))
                    {
                        return userId; // 返回新插入的用户 ID
                    }
                }

                return -1; // 如果无法解析 ID 或者结果为空

            }

        }

        public async Task<int> AdminAddRepository(User user,string org_pass)
        {
            // 查询组织表，检查是否存在匹配的组织密码
            string query = "SELECT ORGANIZATION_ID FROM organization WHERE ORGANIZATION_PASSWORD = ?";
            var parameters = new List<object> { org_pass };

            var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

            // 如果没有找到匹配的组织密码，返回错误信息
            if (result.Count == 0)
            {
                // 可以根据需求调整返回的错误值或者逻辑
                return -1; // 密码不匹配
            }

            //如果 验证管理员权限正确，进入以下逻辑
            // 从查询结果中提取组织 ID
            var organizationId = result[0]["ORGANIZATION_ID"];

            string insert_query = @"
                INSERT INTO user (user_name, user_role, user_password,user_organization_id)
                VALUES (?,?,?,?)";

            var parameters1 = new List<object>
            {
                user.UserName,
                user.UserRole,
                user.UserPassword,
                organizationId
            };

            var rowsAffected = await _mySqlConnection.ExecuteNonQueryAsync(insert_query, parameters1);
            

            if (rowsAffected == 0)
            {
                return -1;
            }
            else
            {
                string selectQuery = "SELECT LAST_INSERT_ID()";
                var newUserIdResult = await _mySqlConnection.ExecuteQueryAsync(selectQuery);

                // 这里假设 LAST_INSERT_ID() 返回一个只有一列的结果
                if (newUserIdResult.Count > 0)
                {
                    // 从结果中获取第一个字典，获取 'LAST_INSERT_ID()' 对应的值
                    var newUserId = newUserIdResult[0].Values.FirstOrDefault();
                    if (newUserId != null && int.TryParse(newUserId.ToString(), out int userId))
                    {
                        return userId; // 返回新插入的用户 ID
                    }
                }

                return -1; // 如果无法解析 ID 或者结果为空

            }

        }

        public async Task<Result<User>> UserLoginRepository(JsonElement login_request)
        {
            // 从 JSON 中提取用户名和密码
            var userId = login_request.GetProperty("userId").ToString();
            var userPassword = login_request.GetProperty("userPassword").GetString();

            //查询用户表，检索与userId匹配的userPassword，然后比对是否与来自login_request的userPassword匹配
            string query = "SELECT user_Password FROM user WHERE user_Id =? ";
            var parameters = new  List<object> { userId };

            var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

            var login_user = new User { };
            if (result.Count == 0)
            {
                return  Result<User>.Failure("找不到用户"); 
            }

            var db_pwd = result[0]["user_Password"].ToString();
            bool pwd_ok = BCrypt.Net.BCrypt.Verify(userPassword, db_pwd);

            if (pwd_ok)
            {
                string search_query = "SELECT user_Name, user_Role FROM user WHERE user_id = ?";
                var search_result = await _mySqlConnection.ExecuteQueryAsync(search_query, parameters);
                var db_name = search_result[0]["user_Name"].ToString();
                var db_role = search_result[0]["user_Role"].ToString();
                login_user.UserRole = db_role;
                login_user.UserName = db_name;
                login_user.UserId = int.Parse(userId);

                return Result<User>.Success(login_user, "登录成功");
            }
            else
            {
                return Result<User>.Failure("密码错误");
            }

        }

        public async Task<int> UserModifyRepository(JsonElement modify_request)
        {
            if (!modify_request.EnumerateObject().Any())
            {
                return -3;
            }
            // 从请求体中读取 userId
            if (!modify_request.TryGetProperty("userId", out var userIdProperty) || !userIdProperty.ValueKind.Equals(JsonValueKind.Number))
            {
                return -4;
            }
            int userId = userIdProperty.GetInt32();


            var queryBuilder = new System.Text.StringBuilder("UPDATE user SET ");
            var parameters = new List<object>();

            Console.WriteLine($"Modify User: {modify_request}");
            // 第一个循环生成查询字符串
            foreach (var property in modify_request.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "userName":
                        queryBuilder.Append("user_name = ?, ");
                        break;
                    case "userPassword":
                        queryBuilder.Append("user_password = ?, ");
                        break;
                    case "icon":
                        queryBuilder.Append("icon = ?, ");
                        break;
                    case "userContact":
                        queryBuilder.Append("user_contact = ?, ");
                        break;
                    case "userIntroduction":
                        queryBuilder.Append("user_introduction = ?, ");
                        break;
                    case "userAge":
                        queryBuilder.Append("user_age = ?, ");
                        break;
                    default:
                        break;
                }
            }

            // 移除最后一个逗号和空格
            if (queryBuilder.ToString() != "UPDATE user SET ") { queryBuilder.Length -= 2; }
            queryBuilder.Append(" WHERE user_id = ?");
            string query = queryBuilder.ToString();
            Console.WriteLine($"Generated Query: {query}");


            // 第二个循环添加参数
            foreach (var property in modify_request.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "userName":
                        parameters.Add(property.Value.GetString());
                        break;
                    case "userPassword":
                        parameters.Add(BCrypt.Net.BCrypt.HashPassword(property.Value.GetString()));
                        break;
                    case "icon":
                        parameters.Add(property.Value.GetString());
                        break;
                    case "userContact":
                        parameters.Add(property.Value.GetString());
                        break;
                    case "userIntroduction":
                        parameters.Add(property.Value.GetString());
                        break;
                    case "userAge":
                        parameters.Add(property.Value.GetInt32());
                        break;
                    default:
                        break;
                }
            }

            // 添加 userId 作为 WHERE 条件
            parameters.Add(userId);

            try
            {
                // 执行查询并更新数据库
                await _mySqlConnection.ExecuteNonQueryAsync(query, parameters);
                return 1;  // 更新成功
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing POST request: {ex.Message}");
                return -1;
            }

        }

        public async Task<int> UserDeleteRepository(int userId)
        {
            string query = "DELETE FROM user WHERE user_id = ?";
            var parameters = new List<object> { userId };

            try
            {
                await _mySqlConnection.ExecuteNonQueryAsync(query, parameters);
                return 1;  // 删除成功
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing DELETE request: {ex.Message}");
                return -1;
            }
        }


    }
}
