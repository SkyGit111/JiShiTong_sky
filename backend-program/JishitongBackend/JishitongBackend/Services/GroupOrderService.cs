using JishitongBackend.Repositories;
using JishitongBackend.Models;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace JishitongBackend.Services
{
    public class GroupOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly GroupOrderRepository _groupOrderRepository;
        private readonly MessageService _messageService;
        private readonly UserService _userService;

        public GroupOrderService(IConfiguration configuration)
        {
            _configuration = configuration; // 使用 IConfiguration 获取配置信息
            _groupOrderRepository = new GroupOrderRepository(new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")));
            _messageService = new MessageService(configuration);
            _userService = new UserService(configuration);
        }
        
        public async Task<Result<GroupOrder>> GroupOrderCreateService(GroupOrder groupOrder)
        { 
            try
            {
                var result = await _groupOrderRepository.GroupOrderCreateRepository(groupOrder);

                if (result == -1)
                {
                    return Result<GroupOrder>.Failure(204, "查询结果为空");
                }
                else
                {
                    groupOrder.RequestId = result;
                    return Result<GroupOrder>.Success(groupOrder, "创建拼单请求成功");
                }

            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<GroupOrder>.Failure($"发生错误: {ex.Message}");
            }
        
        }

        public async Task<Result<string>> CreateGroupOrderWithProjectService(JsonElement requestJson)
        {
            try
            {
                // 输出完整的requestJson以调试
                Console.WriteLine($"Received requestJson: {requestJson}");

                // Step 1: 从 requestJson 中提取 GroupOrder 和 Project 的信息
                if (!requestJson.TryGetProperty("requestJson", out var innerJson) || innerJson.ValueKind != JsonValueKind.Object)
                {
                    Console.WriteLine("Error: 'requestJson' is not a valid object.");
                    return Result<string>.Failure(204, "查询结果为空");
                }

                if (!innerJson.TryGetProperty("groupOrder", out var groupOrderJson) || groupOrderJson.ValueKind != JsonValueKind.Object)
                {
                    Console.WriteLine("Error: Unable to parse 'groupOrder' from requestJson.");
                    return Result<string>.Failure(204, "查询结果为空");
                }
                Console.WriteLine($"Parsed 'groupOrder': {groupOrderJson}");

                // 逐个提取 GroupOrder 的字段值
                var primeUserId = groupOrderJson.GetProperty("primeUserId").GetInt32();
                var requestType = groupOrderJson.GetProperty("requestType").GetString();
                var startTime = groupOrderJson.GetProperty("startTime").GetDateTime();
                var endTime = groupOrderJson.GetProperty("endTime").GetDateTime();
                var title = groupOrderJson.GetProperty("title").GetString();
                var initiationTime = groupOrderJson.GetProperty("initiationTime").GetDateTime();
                var personNum = groupOrderJson.GetProperty("personNum").GetInt32();
                var description = groupOrderJson.GetProperty("description").GetString();
                var priceDistribution = groupOrderJson.GetProperty("priceDistribution").GetString();
                var extraRequirement = groupOrderJson.TryGetProperty("extraRequirement", out var extraRequirementJson) ? extraRequirementJson.GetString() : null;

                // 创建 GroupOrder 对象
                var groupOrder = new GroupOrder
                {
                    PrimeUserId = primeUserId,
                    RequestType = requestType,
                    StartTime = startTime,
                    EndTime = endTime,
                    Title = title,
                    InitiationTime = initiationTime,
                    PersonNum = personNum,
                    Description = description,
                    PriceDistribution = priceDistribution,
                    ExtraRequirement = extraRequirement
                };

                // 输出解析的 groupOrder 数据
                Console.WriteLine($"Deserialized GroupOrder: PrimeUserId={groupOrder.PrimeUserId}, RequestType={groupOrder.RequestType}, StartTime={groupOrder.StartTime}, EndTime={groupOrder.EndTime}, Title={groupOrder.Title}, InitiationTime={groupOrder.InitiationTime}, PersonNum={groupOrder.PersonNum}, Description={groupOrder.Description}, PriceDistribution={groupOrder.PriceDistribution}, ExtraRequirement={groupOrder.ExtraRequirement}");

                // Step 2: 创建拼单请求
                var groupOrderResult = await GroupOrderCreateService(groupOrder);
                if (!groupOrderResult.IsSuccess)
                {
                    Console.WriteLine($"Error during GroupOrder creation: {groupOrderResult.Message}");
                    return Result<string>.Failure(204, "查询结果为空");
                }
                Console.WriteLine($"GroupOrder created successfully: {groupOrderResult.Data.RequestId}");

                // Step 3: 从 requestJson 中提取项目的公共属性和 specificInfo
                if (!innerJson.TryGetProperty("project", out var projectJson) || projectJson.ValueKind != JsonValueKind.Object)
                {
                    Console.WriteLine("Error: Unable to parse 'project' from requestJson.");
                    return Result<string>.Failure(204, "查询结果为空");
                }
                Console.WriteLine($"Parsed 'project': {projectJson}");

                var requestId = groupOrder.RequestId;
                var projectName = projectJson.GetProperty("projectName").GetString();
                var totalPrice = projectJson.GetProperty("totalPrice").GetInt32();
                var requestTypeProject = projectJson.GetProperty("requestType").GetString();
                var specificInfoJson = projectJson.GetProperty("specificInfo");

                Console.WriteLine($"Extracted project details: RequestId={requestId}, ProjectName={projectName}, TotalPrice={totalPrice}, RequestType={requestTypeProject}");

                // Step 4: 创建项目数据，使用 JsonObject
                var projectData = new JsonObject
                {
                    ["requestId"] = requestId,
                    ["projectName"] = projectName,
                    ["totalPrice"] = totalPrice,
                    ["requestType"] = requestTypeProject
                };

                // Special handling for specificInfoJson
                if (specificInfoJson.ValueKind == JsonValueKind.Object)
                {
                    Console.WriteLine($"Parsing specificInfo: {specificInfoJson}");
                    projectData["specificInfo"] = JsonObject.Parse(specificInfoJson.GetRawText());
                }
                else
                {
                    Console.WriteLine("Error: 'specificInfo' is not a valid object or is missing.");
                    return Result<string>.Failure(204, "查询结果为空");
                }

                // Step 5: 将 JsonObject 转换为 JsonElement
                Console.WriteLine($"Converting projectData to JsonElement: {projectData}");
                JsonElement projectDataElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(projectData.ToString());

                // Step 6: 调用 CreateGroupProjectRepository 创建项目
                Console.WriteLine("Calling CreateGroupProjectRepository...");
                var projectResult = await _groupOrderRepository.CreateGroupProjectRepository(projectDataElement);
                if (!projectResult.IsSuccess)
                {
                    Console.WriteLine($"Error during project creation: {projectResult.Message}");
                    return Result<string>.Failure(204, "查询结果为空");
                }
                Console.WriteLine("Project created successfully.");

                return Result<string>.Success(groupOrderResult.Data.RequestId.ToString(), "拼单请求和项目创建成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return Result<string>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<List<dynamic>>> GetMultipleGroupOrderInformationService(List<int> requestIds)
        {
            var resultList = new List<dynamic>();  // 用于存储所有请求的结果
            var failureMessages = new List<string>();  // 用于存储失败的信息

            foreach (var requestId in requestIds)
            {
                var result = await GetCompleteGroupOrderInformationService(requestId);

                if (result.IsSuccess)
                {
                    resultList.Add(result.Data);  // 将成功的结果加入到结果列表中
                }
                else
                {
                    failureMessages.Add($"请求 {requestId} 失败: {result.Message}");  // 如果失败，记录失败信息
                }
            }

            // 如果所有请求都失败，返回一个错误消息
            if (resultList.Count == 0)
            {
                return Result<List<dynamic>>.Failure(204, "查询结果为空");
            }

            // 返回所有成功的结果
            return Result<List<dynamic>>.Success(resultList);
        }

        public async Task<Result<List<dynamic>>> GetCompleteGroupOrderInformationService(int requestId)
        {
            try
            {
                // 获取拼单信息
                var groupOrderResult = await GetGroupOrderInformationService(requestId);

                if (!groupOrderResult.IsSuccess)
                {
                    return Result<List<dynamic>>.Failure(204, "查询结果为空");
                }

                // 获取项目详细信息
                var projectResult = await GetGroupProjectInformationService(requestId);

                if (!projectResult.IsSuccess)
                {
                    return Result<List<dynamic>>.Failure(204, "查询结果为空");
                }

                // 合并拼单信息和项目信息
                var groupOrder = groupOrderResult.Data;
                var projectData = projectResult.Data;

                // 格式化数据并返回
                var responseData = new List<dynamic>
        {
            new
            {
                request_id = groupOrder.RequestId,
                prime_user_id = groupOrder.PrimeUserId,
                request_type = groupOrder.RequestType,
                request_states = groupOrder.RequestStates,
                start_time = groupOrder.StartTime,
                end_time = groupOrder.EndTime,
                title = groupOrder.Title,
                initiation_time = groupOrder.InitiationTime,
                person_num = groupOrder.PersonNum,
                description = groupOrder.Description,
                price_distribution = groupOrder.PriceDistribution,
                extra_requirement = groupOrder.ExtraRequirement,
                participants = groupOrder.Participants,
                project_name = projectData?.ProjectName,
                total_price = projectData?.TotalPrice,
                specific_info = projectData?.SpecificInfo
            }
        };
                if (groupOrder == null || projectData == null)
                {
                    return Result<List<dynamic>>.Failure(204, "查询结果为空");
                }

                return Result<List<dynamic>>.Success(responseData, "查询成功"); // 返回合并后的数据
            }
            catch (Exception ex)
            {
                return Result<List<dynamic>>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<GroupOrder>> GetGroupOrderInformationService(int requestId)
        {
            try
            {
                // 获取拼单信息
                var result = await _groupOrderRepository.GetGroupOrderInformationRepository(requestId);

                if (result == null)
                {
                    return Result<GroupOrder>.Failure("未找到拼单信息");
                }

                // 获取参与人的详细信息
                var participants = result.Participants;
                if (!string.IsNullOrEmpty(participants))
                {
                    var userIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(participants);

                    var userInfos = new List<object>(); // 使用匿名类型

                    foreach (var userId in userIds)
                    {
                        var userResult = await _userService.GetUserInformationService(userId);
                        if (userResult.IsSuccess)
                        {
                            // 将用户信息进行重新绑定和格式化
                            var userInfo = userResult.Data;

                            // 重新绑定为小写命名，并排除敏感字段
                            var userResponse = new
                            {
                                user_id = userId,
                                user_name = userInfo.user_name,
                                user_contact = userInfo.user_contact,
                                icon = userInfo.icon,
                                user_introduction = userInfo.user_introduction,
                                user_age = userInfo.user_age,
                            };

                            userInfos.Add(userResponse); // 将格式化后的用户信息加入列表
                        }
                        else
                        {
                            // 如果某个用户信息获取失败，可以记录或处理该异常
                            Console.WriteLine($"无法获取用户 {userId} 信息");
                        }
                    }

                    // 将格式化后的用户信息添加到拼单数据中
                    result.ParticipantsDetails = userInfos;
                }

                return Result<GroupOrder>.Success(result);  // 返回拼单信息
            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<GroupOrder>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<GroupProject>> GetGroupProjectInformationService(int requestId)
        {
            try
            {
                // 获取拼单信息
                var result = await _groupOrderRepository.GetGroupProjectInformationRepository(requestId);

                if (result == null)
                {
                    return Result<GroupProject>.Failure("未找到拼单项目信息");
                }

                return Result<GroupProject>.Success(result.Data,"查询成功");  // 返回拼单信息
            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<GroupProject>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetAllService()
        {
            try
            {
                // 获取拼单数据
                var result = await _groupOrderRepository.GroupOrderGetAllRepository();

                if (result.IsSuccess) // 判断是否成功
                {
                    return Result<List<GroupOrder>>.Success(result.Data, "已返回全部拼单信息");
                }
                else
                {
                    return Result<List<GroupOrder>>.Failure(204, "查询结果为空");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败消息
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetAllTrafficService()
        {
            try
            {
                // 获取拼单数据
                var result = await _groupOrderRepository.GroupOrderGetAllTrafficRepository();

                if (result.IsSuccess) // 判断是否成功
                {
                    return Result<List<GroupOrder>>.Success(result.Data, "已返回全部拼单信息");
                }
                else
                {
                    return Result<List<GroupOrder>>.Failure(204, "查询结果为空");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败消息
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetAllActivityService()
        {
            try
            {
                // 获取拼单数据
                var result = await _groupOrderRepository.GroupOrderGetAllActivityRepository();

                if (result.IsSuccess) // 判断是否成功
                {
                    return Result<List<GroupOrder>>.Success(result.Data, "已返回全部拼单信息");
                }
                else
                {
                    return Result<List<GroupOrder>>.Failure(204, "查询结果为空");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败消息
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetAllProductService()
        {
            try
            {
                // 获取拼单数据
                var result = await _groupOrderRepository.GroupOrderGetAllProductRepository();

                if (result.IsSuccess) // 判断是否成功
                {
                    return Result<List<GroupOrder>>.Success(result.Data, "已返回全部拼单信息");
                }
                else
                {
                    return Result<List<GroupOrder>>.Failure(204, "查询结果为空");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败消息
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetOpenService(int userId)
        {

            try
            {
                // 获取开放的拼单数据
                var result = await _groupOrderRepository.GroupOrderGetOpenRepository(userId);

                if (result.IsSuccess) // 判断是否成功
                {
                    return Result<List<GroupOrder>>.Success(result.Data, "已返回开放拼单信息");
                }
                else
                {
                    return Result<List<GroupOrder>>.Failure(204, "查询结果为空");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败消息
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");
            }


        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetOpenTrafficService(int userId)
        {

            try
            {
                // 获取开放的拼单数据
                var result = await _groupOrderRepository.GroupOrderGetOpenTypeRepository(userId,"traffic");

                if (result.IsSuccess) // 判断是否成功
                {
                    return Result<List<GroupOrder>>.Success(result.Data, "已返回开放拼单信息");
                }
                else
                {
                    return Result<List<GroupOrder>>.Failure(204, "查询结果为空");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败消息
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");
            }


        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetOpenActivityService(int userId)
        {

            try
            {
                // 获取开放的拼单数据
                var result = await _groupOrderRepository.GroupOrderGetOpenTypeRepository(userId,"activity");

                if (result.IsSuccess) // 判断是否成功
                {
                    return Result<List<GroupOrder>>.Success(result.Data, "已返回开放拼单信息");
                }
                else
                {
                    return Result<List<GroupOrder>>.Failure(204, "查询结果为空");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败消息
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");
            }


        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetOpenProductService(int userId)
        {

            try
            {
                // 获取开放的拼单数据
                var result = await _groupOrderRepository.GroupOrderGetOpenTypeRepository(userId,"product");

                if (result.IsSuccess) // 判断是否成功
                {
                    return Result<List<GroupOrder>>.Success(result.Data, "已返回开放拼单信息");
                }
                else
                {
                    return Result<List<GroupOrder>>.Failure(204, "查询结果为空");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败消息
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");
            }


        }

        public async Task<Result<List<dynamic>>> GroupOrderGetMineService(int userId)
        {
            try
            {
                // 获取开放的拼单数据
                var result = await _groupOrderRepository.GroupOrderGetMineRepository(userId);

                if (result.IsSuccess)
                {
                    // 将拼单信息进行重新格式化，返回小写命名规则的数据
                    var responseData = result.Data.Select(order => new
                    {
                        request_id = order.RequestId
                    }).Cast<dynamic>().ToList();  // 将匿名类型转换为 dynamic

                    return Result<List<dynamic>>.Success(responseData); // 返回格式化后的拼单信息
                }
                else
                {
                    return Result<List<dynamic>>.Failure(204, "查询结果为空");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败消息
                return Result<List<dynamic>>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<List<dynamic>>> GroupOrderGetMyCreateService(int userId)
        {
            try
            {
                // 获取开放的拼单数据
                var result = await _groupOrderRepository.GroupOrderGetMyCreateRepository(userId);

                if (result.IsSuccess)
                {
                    // 将拼单信息进行重新格式化，返回小写命名规则的数据
                    var responseData = result.Data.Select(order => new
                    {
                        request_id = order.RequestId
                    }).Cast<dynamic>().ToList();  // 将匿名类型转换为 dynamic

                    return Result<List<dynamic>>.Success(responseData); // 返回格式化后的拼单信息
                }
                else
                {
                    return Result<List<dynamic>>.Failure(204, "查询结果为空");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败消息
                return Result<List<dynamic>>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<List<dynamic>>> GroupOrderGetMyParticipateService(int userId)
        {
            try
            {
                // 获取开放的拼单数据
                var result = await _groupOrderRepository.GroupOrderGetMyParticipateRepository(userId);

                if (result.IsSuccess)
                {
                    // 将拼单信息进行重新格式化，返回小写命名规则的数据
                    var responseData = result.Data.Select(order => new
                    {
                        request_id = order.RequestId
                    }).Cast<dynamic>().ToList();  // 将匿名类型转换为 dynamic

                    return Result<List<dynamic>>.Success(responseData); // 返回格式化后的拼单信息
                }
                else
                {
                    return Result<List<dynamic>>.Failure(204, "查询结果为空");
                }
            }
            catch (Exception ex)
            {
                // 捕获异常并返回失败消息
                return Result<List<dynamic>>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<String>> AddUserToGroupOrderService(int userId, int requestId)
        {
            try
            {
                var result = await _groupOrderRepository.AddUserToGroupOrderRepository(userId, requestId);

                if (result == 1)
                {
                    var get_result = await GetGroupOrderInformationService(requestId);
                    var get_user = await _userService.GetUserInformationService(userId);

                    if (get_result.IsSuccess&&get_user.IsSuccess)
                    {
                        var user_name = get_user.Data.user_name;
                        // 检查参与人字段
                        var groupOrder = get_result.Data;
                        if (!string.IsNullOrEmpty(groupOrder.Participants))
                        {
                            // 假设 Participants 字段是一个 JSON 格式的字符串，包含参与人的列表
                            var participants = System.Text.Json.JsonSerializer.Deserialize<List<int>>(groupOrder.Participants);

                            if (participants != null && participants.Any())
                            {
                                // 遍历所有参与人，创建相应的 Message 对象
                                foreach (var participantId in participants)
                                {
                                    var message = new Message
                                    {
                                        SenderId = groupOrder.PrimeUserId,
                                        ReceiverId = participantId,         // 每个参与人作为接收者
                                        MessageTime = DateTime.Now,          // 当前时间
                                        Content = user_name+"加入拼单",          // 消息内容
                                        RequestId = requestId               // 关联的拼单请求 ID
                                    };

                                    Console.WriteLine($" ReceiverId: {JsonConvert.SerializeObject(message.ReceiverId)}");

                                    // 保存 Message 对象（调用消息保存的方法）
                                    var messageResult = await _messageService.MessageCreateService(message);
                                    Console.WriteLine($"消息保存成功");

                                    if (!messageResult.IsSuccess)
                                    {
                                        // 如果保存消息失败，可以记录错误，或者根据需要做其他处理
                                        Console.WriteLine($"Failed to save message for participant {participantId}");
                                    }
                                }

                                return Result<String>.Success("修改拼单请求成功，并通知所有参与人");
                            }
                            else
                            {
                                return Result<String>.Success("修改拼单请求成功，但没有参与人，无需创建消息");
                            }
                        }
                        else
                        {
                            return Result<String>.Success("修改拼单请求成功，但没有参与人，无需创建消息");
                        }
                    }
                    else
                    {
                        return Result<String>.Success("请求成功，但消息发送失败");
                    }
                }
                else
                {
                    return Result<String>.Failure("请求失败");
                }

            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<String>.Failure($"发生错误: {ex.Message}");
            }

        }

        public async Task<Result<String>> DeleteUserToGroupOrderService(int userId, int requestId)
        {
            try
            {
                var result = await _groupOrderRepository.DeleteUserToGroupOrderRepository(userId, requestId);

                if (result == 1)
                {
                    var get_result = await GetGroupOrderInformationService(requestId);
                    var get_user = await _userService.GetUserInformationService(userId);

                    if (get_result.IsSuccess && get_user.IsSuccess)
                    {
                        var user_name = get_user.Data.user_name;
                        // 检查参与人字段
                        var groupOrder = get_result.Data;
                        if (!string.IsNullOrEmpty(groupOrder.Participants))
                        {
                            // 假设 Participants 字段是一个 JSON 格式的字符串，包含参与人的列表
                            var participants = System.Text.Json.JsonSerializer.Deserialize<List<int>>(groupOrder.Participants);

                            if (participants != null && participants.Any())
                            {
                                // 遍历所有参与人，创建相应的 Message 对象
                                foreach (var participantId in participants)
                                {
                                    var message = new Message
                                    {
                                        SenderId = groupOrder.PrimeUserId,
                                        ReceiverId = participantId,         // 每个参与人作为接收者
                                        MessageTime = DateTime.Now,          // 当前时间
                                        Content = user_name + "退出拼单",          // 消息内容
                                        RequestId = requestId               // 关联的拼单请求 ID
                                    };

                                    Console.WriteLine($" ReceiverId: {JsonConvert.SerializeObject(message.ReceiverId)}");

                                    // 保存 Message 对象（调用消息保存的方法）
                                    var messageResult = await _messageService.MessageCreateService(message);
                                    Console.WriteLine($"消息保存成功");

                                    if (!messageResult.IsSuccess)
                                    {
                                        // 如果保存消息失败，可以记录错误，或者根据需要做其他处理
                                        Console.WriteLine($"Failed to save message for participant {participantId}");
                                    }
                                }

                                return Result<String>.Success("修改拼单请求成功，并通知所有参与人");
                            }
                            else
                            {
                                return Result<String>.Success("修改拼单请求成功，但没有参与人，无需创建消息");
                            }
                        }
                        else
                        {
                            return Result<String>.Success("修改拼单请求成功，但没有参与人，无需创建消息");
                        }
                    }
                    else
                    {
                        return Result<String>.Success("请求成功，但消息发送失败");
                    }
                }
                else if(result == -3) 
                {
                    return Result<String>.Failure("查询失败");
                }
                else if (result == -2)
                {
                    return Result<String>.Failure("没有行受到影响");
                }
                else if (result == -1)
                {
                    return Result<String>.Failure("请求出错");
                }
                else
                {
                    return Result<String>.Failure("请求出错");
                }

            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<String>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<String>> GroupOrderDeleteService(int requestId)
        {
            try
            {
                var result = await _groupOrderRepository.GroupOrderDeleteRepository(requestId);

                if (result == -1)
                {
                    return Result<String>.Failure("删除失败");
                }
                else if (result == 1)
                {
                    return Result<String>.Success("删除成功");
                }
                else if (result == -2)
                {
                    return Result<String>.Success("删除请求成功，但删除项目失败");
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

        public async Task<Result<String>> GroupOrderModifyService(JsonElement modify_request)
        {
            try
            {
                var result = await _groupOrderRepository.GroupOrderModifyRepository(modify_request);

                if (result == -1)
                {
                    return Result<String>.Failure(204, "查询结果为空");
                }
                else if (result == 1)
                {
                    Console.WriteLine($"修改拼单成功");
                    var requestId = int.Parse(modify_request.GetProperty("requestId").ToString());
                    var get_result = await GetGroupOrderInformationService(requestId);
                    Console.WriteLine($" get_result: {JsonConvert.SerializeObject(get_result)}");

                    if (get_result.IsSuccess)
                    {
                        // 检查参与人字段
                        var groupOrder = get_result.Data;
                        if (!string.IsNullOrEmpty(groupOrder.Participants))
                        {
                            // 假设 Participants 字段是一个 JSON 格式的字符串，包含参与人的列表
                            var participants = System.Text.Json.JsonSerializer.Deserialize<List<int>>(groupOrder.Participants);

                            if (participants != null && participants.Any())
                            {
                                // 遍历所有参与人，创建相应的 Message 对象
                                foreach (var participantId in participants)
                                {
                                    var message = new Message
                                    {
                                        SenderId = groupOrder.PrimeUserId,
                                        ReceiverId = participantId,         // 每个参与人作为接收者
                                        MessageTime = DateTime.Now,          // 当前时间
                                        Content = "拼单请求更新通知",          // 消息内容
                                        RequestId = requestId               // 关联的拼单请求 ID
                                    };

                                    Console.WriteLine($" ReceiverId: {JsonConvert.SerializeObject(message.ReceiverId)}");

                                    // 保存 Message 对象（调用消息保存的方法）
                                    var messageResult = await _messageService.MessageCreateService(message);
                                    Console.WriteLine($"消息保存成功");

                                    if (!messageResult.IsSuccess)
                                    {
                                        // 如果保存消息失败，可以记录错误，或者根据需要做其他处理
                                        Console.WriteLine($"Failed to save message for participant {participantId}");
                                    }
                                }

                                return Result<String>.Success("修改拼单请求成功，并通知所有参与人");
                            }
                            else
                            {
                                return Result<String>.Success("修改拼单请求成功，但没有参与人，无需创建消息");
                            }
                        }
                        else
                        {
                            return Result<String>.Success("修改拼单请求成功，但没有参与人，无需创建消息");
                        }
                    }
                    else
                    {
                        return Result<String>.Failure(204, "查询结果为空");
                    }
                }
                else
                {
                    return Result<String>.Failure(204, "查询结果为空");
                }

            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<String>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<string>> ModifyGroupOrderWithProjectService(JsonElement modifyRequest)
        {
            try
            {
                // 输出完整的modifyRequest以调试
                Console.WriteLine($"Received modifyRequest: {modifyRequest}");

                // Step 1: 从 modifyRequest 中提取 GroupOrder 和 Project 的信息
                if (!modifyRequest.TryGetProperty("modifyRequest", out var innerJson) || innerJson.ValueKind != JsonValueKind.Object)
                {
                    Console.WriteLine("Error: 'modifyRequest' is not a valid object.");
                    return Result<string>.Failure( "提取信息失败");
                }

                // Step 2: 从 modifyRequest 中提取 GroupOrder 的信息
                if (!innerJson.TryGetProperty("groupOrder", out var groupOrderJson) || groupOrderJson.ValueKind != JsonValueKind.Object)
                {
                    Console.WriteLine("Error: Unable to parse 'groupOrder' from modifyRequest.");
                    return Result<string>.Failure("提取详细grouporder失败");
                }
                Console.WriteLine($"Parsed 'groupOrder': {groupOrderJson}");

                int? requestId = groupOrderJson.TryGetProperty("requestId", out var requestIdJson) && requestIdJson.ValueKind == JsonValueKind.Number
                    ? requestIdJson.GetInt32()
                    : (int?)null;

                int? primeUserId = groupOrderJson.TryGetProperty("primeUserId", out var primeUserIdJson) && primeUserIdJson.ValueKind == JsonValueKind.Number
                    ? primeUserIdJson.GetInt32()
                    : (int?)null;

                string? requestType = groupOrderJson.TryGetProperty("requestType", out var requestTypeJson) && requestTypeJson.ValueKind == JsonValueKind.String
                    ? requestTypeJson.GetString()
                    : null;

                string? requestStates = groupOrderJson.TryGetProperty("requestStates", out var requestStatesJson) && requestStatesJson.ValueKind == JsonValueKind.String
                    ? requestStatesJson.GetString()
                    : null;

                DateTime? startTime = groupOrderJson.TryGetProperty("startTime", out var startTimeJson) && startTimeJson.ValueKind == JsonValueKind.String
                    ? startTimeJson.GetDateTime()
                    : (DateTime?)null;

                DateTime? endTime = groupOrderJson.TryGetProperty("endTime", out var endTimeJson) && endTimeJson.ValueKind == JsonValueKind.String
                    ? endTimeJson.GetDateTime()
                    : (DateTime?)null;

                string? title = groupOrderJson.TryGetProperty("title", out var titleJson) && titleJson.ValueKind == JsonValueKind.String
                    ? titleJson.GetString()
                    : null;

                DateTime? initiationTime = groupOrderJson.TryGetProperty("initiationTime", out var initiationTimeJson) && initiationTimeJson.ValueKind == JsonValueKind.String
                    ? initiationTimeJson.GetDateTime()
                    : (DateTime?)null;

                int? personNum = groupOrderJson.TryGetProperty("personNum", out var personNumJson) && personNumJson.ValueKind == JsonValueKind.Number
                    ? personNumJson.GetInt32()
                    : (int?)null;

                string? description = groupOrderJson.TryGetProperty("description", out var descriptionJson) && descriptionJson.ValueKind == JsonValueKind.String
                    ? descriptionJson.GetString()
                    : null;

                string? priceDistribution = groupOrderJson.TryGetProperty("priceDistribution", out var priceDistributionJson) && priceDistributionJson.ValueKind == JsonValueKind.String
                    ? priceDistributionJson.GetString()
                    : null;

                string? extraRequirement = groupOrderJson.TryGetProperty("extraRequirement", out var extraRequirementJson) && extraRequirementJson.ValueKind == JsonValueKind.String
                    ? extraRequirementJson.GetString()
                    : null;

                string participants = groupOrderJson.TryGetProperty("participants", out var participantsJson)
                    ? System.Text.Json.JsonSerializer.Serialize(participantsJson)
                    : "[]";

                // 创建 GroupOrder 数据
                var groupOrderData = new JsonObject
                {
                    ["requestId"] = requestId,
                    ["primeUserId"] = primeUserId,
                    ["requestType"] = requestType,
                    ["requestStates"] = requestStates,
                    ["startTime"] = startTime,
                    ["endTime"] = endTime,
                    ["title"] = title,
                    ["initiationTime"] = initiationTime,
                    ["personNum"] = personNum,
                    ["description"] = description,
                    ["priceDistribution"] = priceDistribution,
                    ["extraRequirement"] = extraRequirement,
                    ["participants"] = participants
                };

                // 将 GroupOrder 数据转换为 JsonElement
                Console.WriteLine($"Converting groupOrderData to JsonElement: {groupOrderData}");
                JsonElement groupOrderDataElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(groupOrderData.ToString());

                // 修改 GroupOrder
                var groupOrderModifyResult = await GroupOrderModifyService(groupOrderDataElement);
                if (!groupOrderModifyResult.IsSuccess)
                {
                    Console.WriteLine($"Error during GroupOrder modification: {groupOrderModifyResult.Message}");
                    return Result<string>.Failure("Error during GroupOrder modification");
                }
                Console.WriteLine($"GroupOrder modified successfully: {groupOrderModifyResult.Data}");

                // Step 3: 从 modifyRequest 中提取项目的公共属性和 specificInfo
                if (!innerJson.TryGetProperty("project", out var projectJson) || projectJson.ValueKind != JsonValueKind.Object)
                {
                    Console.WriteLine("Error: Unable to parse 'project' from modifyRequest.");
                    return Result<string>.Failure(" Unable to parse 'project' from modifyRequest.");
                }
                Console.WriteLine($"Parsed 'project': {projectJson}");

                var projectName = projectJson.GetProperty("projectName").GetString();
                var totalPrice = projectJson.GetProperty("totalPrice").GetInt32();
                var requestTypeProject = projectJson.GetProperty("requestType").GetString();
                var specificInfoJson = projectJson.GetProperty("specificInfo");

                // 创建项目数据，使用 JsonObject
                var projectData = new JsonObject
                {
                    ["requestId"] = requestId,
                    ["projectName"] = projectName,
                    ["totalPrice"] = totalPrice,
                    ["requestType"] = requestTypeProject
                };

                // Special handling for specificInfoJson
                if (specificInfoJson.ValueKind == JsonValueKind.Object)
                {
                    Console.WriteLine($"Parsing specificInfo: {specificInfoJson}");
                    projectData["specificInfo"] = JsonObject.Parse(specificInfoJson.GetRawText());
                }
                else
                {
                    Console.WriteLine("Error: 'specificInfo' is not a valid object or is missing.");
                    return Result<string>.Failure(" 'specificInfo' is not a valid object or is missing.");
                }

                // 将项目数据转换为 JsonElement
                Console.WriteLine($"Converting projectData to JsonElement: {projectData}");
                JsonElement projectDataElement = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(projectData.ToString());

                // Step 4: 调用 ModifyGroupProjectRepository 修改项目
                Console.WriteLine("Calling ModifyGroupProjectRepository...");
                var projectModifyResult = await _groupOrderRepository.ModifyGroupProjectRepository(projectDataElement);
                if (!projectModifyResult.IsSuccess)
                {
                    Console.WriteLine($"Error during project modification: {projectModifyResult.Message}");
                    return Result<string>.Failure("Error during project modification");
                }
                Console.WriteLine("Project modified successfully.");

                return Result<string>.Success("拼单请求和项目修改成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return Result<string>.Failure($"发生错误: {ex.Message}");
            }
        }

    }
}
