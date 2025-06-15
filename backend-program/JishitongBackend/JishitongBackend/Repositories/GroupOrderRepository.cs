using JishitongBackend.Models;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text.Json;


namespace JishitongBackend.Repositories
{
    public class GroupOrderRepository
    {
        private readonly MySqlConnection _mySqlConnection;

        public GroupOrderRepository(MySqlConnection mySqlConnection)
        {
            _mySqlConnection = mySqlConnection;
        }


        public async Task<Result<GroupProject>> GetGroupProjectInformationRepository(int requestId)
        {
            string query = @"
        SELECT project_id, request_type, project_name, total_price
        FROM group_project
        WHERE request_id = ?";

            var parameters = new List<object> { requestId };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var groupProject = result[0];

                    int projectId = Convert.ToInt32(groupProject["project_id"]);
                    string requestType = groupProject["request_type"]?.ToString();
                    string projectName = groupProject["project_name"]?.ToString();
                    int? totalPrice = groupProject["total_price"] != DBNull.Value ? Convert.ToInt32(groupProject["total_price"]) : (int?)null;

                    if (requestType == "traffic")
                    {
                        var trafficResult = await GetTrafficGroupInformation(projectId);
                        if (trafficResult.IsSuccess)
                        {
                            var response = new GroupProject
                            {
                                ProjectId = projectId,
                                RequestType = requestType,
                                ProjectName = projectName,
                                TotalPrice = totalPrice,
                                SpecificInfo = trafficResult.Data // 绑定 TrafficGroup 对象
                            };
                            return Result<GroupProject>.Success(response);
                        }
                    }
                    else if (requestType == "product")
                    {
                        var productResult = await GetProductGroupInformation(projectId);
                        if (productResult.IsSuccess)
                        {
                            var response = new GroupProject
                            {
                                ProjectId = projectId,
                                RequestType = requestType,
                                ProjectName = projectName,
                                TotalPrice = totalPrice,
                                SpecificInfo = productResult.Data // 绑定 ProductGroup 对象
                            };
                            return Result<GroupProject>.Success(response);
                        }
                    }
                    else if (requestType == "activity")
                    {
                        var activityResult = await GetActivityGroupInformation(projectId);
                        if (activityResult.IsSuccess)
                        {
                            var response = new GroupProject
                            {
                                ProjectId = projectId,
                                RequestType = requestType,
                                ProjectName = projectName,
                                TotalPrice = totalPrice,
                                SpecificInfo = activityResult.Data // 绑定 ActivityGroup 对象
                            };
                            return Result<GroupProject>.Success(response);
                        }
                    }

                    return Result<GroupProject>.Failure("未找到对应类型的具体信息");
                }
                else
                {
                    return Result<GroupProject>.Failure("未找到拼单项目信息");
                }
            }
            catch (Exception ex)
            {
                return Result<GroupProject>.Failure($"查询出错: {ex.Message}");
            }
        }

        public async Task<Result<TrafficGroup>> GetTrafficGroupInformation(int projectId)
        {
            string query = @"
        SELECT ORIGIN_PLACE, DESTINATION_PLACE, TRAFFIC_TIME
        FROM traffic_group
        WHERE project_id = ?";

            var parameters = new List<object> { projectId };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var trafficGroup = result[0];
                    var traffic = new TrafficGroup
                    {
                        ProjectId = projectId,
                        OriginPlace = trafficGroup["ORIGIN_PLACE"]?.ToString(),
                        DestinationPlace = trafficGroup["DESTINATION_PLACE"]?.ToString(),
                        TrafficTime = trafficGroup["TRAFFIC_TIME"] != DBNull.Value ? Convert.ToDateTime(trafficGroup["TRAFFIC_TIME"]) : (DateTime?)null
                    };

                    return Result<TrafficGroup>.Success(traffic);
                }

                return Result<TrafficGroup>.Failure("未找到交通信息");
            }
            catch (Exception ex)
            {
                return Result<TrafficGroup>.Failure($"查询出错: {ex.Message}");
            }
        }

        public async Task<Result<ProductGroup>> GetProductGroupInformation(int projectId)
        {
            string query = @"
        SELECT PLATFORM, ADDRESS
        FROM product_group
        WHERE project_id = ?";

            var parameters = new List<object> { projectId };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var productGroup = result[0];
                    var product = new ProductGroup
                    {
                        ProjectId = projectId,
                        Platform = productGroup["PLATFORM"]?.ToString(),
                        Address = productGroup["ADDRESS"]?.ToString()
                    };

                    return Result<ProductGroup>.Success(product);
                }

                return Result<ProductGroup>.Failure("未找到产品信息");
            }
            catch (Exception ex)
            {
                return Result<ProductGroup>.Failure($"查询出错: {ex.Message}");
            }
        }

        public async Task<Result<ActivityGroup>> GetActivityGroupInformation(int projectId)
        {
            string query = @"
        SELECT LOCATION, ACTIVITY_TIME
        FROM activity_group
        WHERE project_id = ?";

            var parameters = new List<object> { projectId };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var activityGroup = result[0];
                    var activity = new ActivityGroup
                    {
                        ProjectId = projectId,
                        Location = activityGroup["LOCATION"]?.ToString(),
                        ActivityTime = activityGroup["ACTIVITY_TIME"] != DBNull.Value ? Convert.ToDateTime(activityGroup["ACTIVITY_TIME"]) : (DateTime?)null
                    };

                    return Result<ActivityGroup>.Success(activity);
                }

                return Result<ActivityGroup>.Failure("未找到活动信息");
            }
            catch (Exception ex)
            {
                return Result<ActivityGroup>.Failure($"查询出错: {ex.Message}");
            }
        }

        public async Task<GroupOrder?> GetGroupOrderInformationRepository(int requestId)
        {
            string ini_query = "UPDATE group_order_request    SET participants = IFNULL(participants, '[]')   WHERE request_id = ?;";
            string query = "SELECT * FROM group_order_request WHERE REQUEST_ID = ?";
            var parameters = new List<object> { requestId };

            try
            {
                await _mySqlConnection.ExecuteQueryAsync(ini_query, parameters);
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var row = result[0];  // 假设查询返回的第一行是目标数据

                    // 将查询结果映射到 GroupOrder 对象
                    var groupOrder = new GroupOrder
                    {
                        RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                        PrimeUserId = Convert.ToInt32(row["PRIME_USER_ID"]),
                        RequestType = row["REQUEST_TYPE"]?.ToString(),
                        RequestStates = row["REQUEST_STATES"]?.ToString(),
                        StartTime = row["START_TIME"] != DBNull.Value ? Convert.ToDateTime(row["START_TIME"]) : null,
                        EndTime = row["END_TIME"] != DBNull.Value ? Convert.ToDateTime(row["END_TIME"]) : null,
                        Title = row["TITLE"]?.ToString(),
                        InitiationTime = row["INITIATION_TIME"] != DBNull.Value ? Convert.ToDateTime(row["INITIATION_TIME"]) : null,
                        PersonNum = Convert.ToInt32(row["PERSON_NUM"]),
                        Description = row["DESCRIPTION"]?.ToString(),
                        PriceDistribution = row["PRICE_DISTRIBUTION"]?.ToString(),
                        ExtraRequirement = row["EXTRA_REQUIREMENT"]?.ToString(),
                        Participants = row["PARTICIPANTS"]?.ToString()
                    };

                    // 计算并赋值三个 int 变量
                    int requestTypeValue = 0;
                    if (groupOrder.RequestStates == "open") requestTypeValue = 2;
                    else if (groupOrder.RequestStates == "normal") requestTypeValue = 1;
                    else if (groupOrder.RequestStates == "close") requestTypeValue = 0;

                    int timeStatusValue = 0;
                    if (groupOrder.StartTime != null && groupOrder.EndTime != null)
                    {
                        var currentTime = DateTime.Now;
                        if (currentTime >= groupOrder.StartTime && currentTime <= groupOrder.EndTime) timeStatusValue = 1;
                    }

                    int participantStatusValue = 0;
                    if (groupOrder.Participants != null)
                    {
                        var participantIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(groupOrder.Participants);
                        if (participantIds != null && participantIds.Count < groupOrder.PersonNum) participantStatusValue = 1;
                    }

                    if ((requestTypeValue == 2 || (requestTypeValue == 1 && timeStatusValue == 1)) && participantStatusValue == 1)
                    {
                        groupOrder.RequestStates = "开放";
                    }
                    else if ((requestTypeValue == 2 || (requestTypeValue == 1 && timeStatusValue == 1)) && participantStatusValue == 0)
                    {
                        groupOrder.RequestStates = "满员";
                    }
                    else
                    {
                        groupOrder.RequestStates = "关闭";
                    }


                    // 输出调试信息
                    Console.WriteLine($"RequestTypeValue: {requestTypeValue}, TimeStatusValue: {timeStatusValue}, ParticipantStatusValue: {participantStatusValue}");

                    return groupOrder;  // 返回拼单信息
                }

                return null;  // 如果没有数据，返回 null
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching group order info: {ex.Message}");
                return null;  // 发生异常时返回 null
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetAllRepository()
        {
            string query = "SELECT * FROM group_order_request";
            var parameters = new List<object>();

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var groupOrders = new List<GroupOrder>();

                    foreach (var row in result)
                    {
                        var groupOrder = new GroupOrder
                        {
                            RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                            PrimeUserId = Convert.ToInt32(row["PRIME_USER_ID"]),
                            RequestType = row["REQUEST_TYPE"]?.ToString(),
                            RequestStates = row["REQUEST_STATES"]?.ToString(),
                            StartTime = row["START_TIME"] != DBNull.Value ? Convert.ToDateTime(row["START_TIME"]) : null,
                            EndTime = row["END_TIME"] != DBNull.Value ? Convert.ToDateTime(row["END_TIME"]) : null,
                            Title = row["TITLE"]?.ToString(),
                            InitiationTime = row["INITIATION_TIME"] != DBNull.Value ? Convert.ToDateTime(row["INITIATION_TIME"]) : null,
                            PersonNum = Convert.ToInt32(row["PERSON_NUM"]),
                            Description = row["DESCRIPTION"]?.ToString(),
                            PriceDistribution = row["PRICE_DISTRIBUTION"]?.ToString(),
                            ExtraRequirement = row["EXTRA_REQUIREMENT"]?.ToString(),
                            Participants = row["PARTICIPANTS"]?.ToString()
                        };

                        groupOrders.Add(groupOrder);
                    }

                    return Result<List<GroupOrder>>.Success(groupOrders);  // 返回拼单信息列表
                }

                return Result<List<GroupOrder>>.Failure("未找到拼单信息");  // 如果没有数据，返回失败消息
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching group order info: {ex.Message}");
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");  // 发生异常时返回失败消息
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetAllTrafficRepository()
        {
            string query = "SELECT * FROM group_order_request WHERE request_type = ?";
            var parameters = new List<object>
                {
                "traffic"
                };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var groupOrders = new List<GroupOrder>();

                    foreach (var row in result)
                    {
                        var groupOrder = new GroupOrder
                        {
                            RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                            PrimeUserId = Convert.ToInt32(row["PRIME_USER_ID"]),
                            RequestType = row["REQUEST_TYPE"]?.ToString(),
                            RequestStates = row["REQUEST_STATES"]?.ToString(),
                            StartTime = row["START_TIME"] != DBNull.Value ? Convert.ToDateTime(row["START_TIME"]) : null,
                            EndTime = row["END_TIME"] != DBNull.Value ? Convert.ToDateTime(row["END_TIME"]) : null,
                            Title = row["TITLE"]?.ToString(),
                            InitiationTime = row["INITIATION_TIME"] != DBNull.Value ? Convert.ToDateTime(row["INITIATION_TIME"]) : null,
                            PersonNum = Convert.ToInt32(row["PERSON_NUM"]),
                            Description = row["DESCRIPTION"]?.ToString(),
                            PriceDistribution = row["PRICE_DISTRIBUTION"]?.ToString(),
                            ExtraRequirement = row["EXTRA_REQUIREMENT"]?.ToString(),
                            Participants = row["PARTICIPANTS"]?.ToString()
                        };

                        groupOrders.Add(groupOrder);
                    }

                    return Result<List<GroupOrder>>.Success(groupOrders);  // 返回拼单信息列表
                }

                return Result<List<GroupOrder>>.Failure("未找到拼单信息");  // 如果没有数据，返回失败消息
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching group order info: {ex.Message}");
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");  // 发生异常时返回失败消息
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetAllActivityRepository()
        {
            string query = "SELECT * FROM group_order_request WHERE request_type = ?";
            var parameters = new List<object>
                {
                "activity"
                };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var groupOrders = new List<GroupOrder>();

                    foreach (var row in result)
                    {
                        var groupOrder = new GroupOrder
                        {
                            RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                            PrimeUserId = Convert.ToInt32(row["PRIME_USER_ID"]),
                            RequestType = row["REQUEST_TYPE"]?.ToString(),
                            RequestStates = row["REQUEST_STATES"]?.ToString(),
                            StartTime = row["START_TIME"] != DBNull.Value ? Convert.ToDateTime(row["START_TIME"]) : null,
                            EndTime = row["END_TIME"] != DBNull.Value ? Convert.ToDateTime(row["END_TIME"]) : null,
                            Title = row["TITLE"]?.ToString(),
                            InitiationTime = row["INITIATION_TIME"] != DBNull.Value ? Convert.ToDateTime(row["INITIATION_TIME"]) : null,
                            PersonNum = Convert.ToInt32(row["PERSON_NUM"]),
                            Description = row["DESCRIPTION"]?.ToString(),
                            PriceDistribution = row["PRICE_DISTRIBUTION"]?.ToString(),
                            ExtraRequirement = row["EXTRA_REQUIREMENT"]?.ToString(),
                            Participants = row["PARTICIPANTS"]?.ToString()
                        };

                        groupOrders.Add(groupOrder);
                    }

                    return Result<List<GroupOrder>>.Success(groupOrders);  // 返回拼单信息列表
                }

                return Result<List<GroupOrder>>.Failure("未找到拼单信息");  // 如果没有数据，返回失败消息
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching group order info: {ex.Message}");
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");  // 发生异常时返回失败消息
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetAllProductRepository()
        {
            string query = "SELECT * FROM group_order_request WHERE request_type = ?";
            var parameters = new List<object>
                {
                "product"
                };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var groupOrders = new List<GroupOrder>();

                    foreach (var row in result)
                    {
                        var groupOrder = new GroupOrder
                        {
                            RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                            PrimeUserId = Convert.ToInt32(row["PRIME_USER_ID"]),
                            RequestType = row["REQUEST_TYPE"]?.ToString(),
                            RequestStates = row["REQUEST_STATES"]?.ToString(),
                            StartTime = row["START_TIME"] != DBNull.Value ? Convert.ToDateTime(row["START_TIME"]) : null,
                            EndTime = row["END_TIME"] != DBNull.Value ? Convert.ToDateTime(row["END_TIME"]) : null,
                            Title = row["TITLE"]?.ToString(),
                            InitiationTime = row["INITIATION_TIME"] != DBNull.Value ? Convert.ToDateTime(row["INITIATION_TIME"]) : null,
                            PersonNum = Convert.ToInt32(row["PERSON_NUM"]),
                            Description = row["DESCRIPTION"]?.ToString(),
                            PriceDistribution = row["PRICE_DISTRIBUTION"]?.ToString(),
                            ExtraRequirement = row["EXTRA_REQUIREMENT"]?.ToString(),
                            Participants = row["PARTICIPANTS"]?.ToString()
                        };

                        groupOrders.Add(groupOrder);
                    }

                    return Result<List<GroupOrder>>.Success(groupOrders);  // 返回拼单信息列表
                }

                return Result<List<GroupOrder>>.Failure("未找到拼单信息");  // 如果没有数据，返回失败消息
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching group order info: {ex.Message}");
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");  // 发生异常时返回失败消息
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetOpenRepository(int userId)
        {
            try
            {
                // 获取当前时间
                var currentTime = DateTime.Now;

                // 查询拼单信息，符合两个条件：
                // 1. request_states == "open" 或者 (request_states == "normal" 且当前时间在 start_time 和 end_time 之间)
                // 2. prime_user_id != userId
                string query = @"
             SELECT * FROM group_order_request
             WHERE (request_states = 'open' 
             OR (request_states = 'normal' AND start_time <= ? AND end_time >= ?))
             AND prime_user_id != ?";

                var parameters = new List<object>
        {
             DateTime.Now,
             DateTime.Now,
             userId
        };

                // 执行查询
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                // 如果查询结果为空，返回失败消息
                if (result.Count == 0)
                {
                    return Result<List<GroupOrder>>.Failure("未找到符合条件的拼单信息");
                }

                // 将查询结果映射为 GroupOrder 对象列表
                var groupOrders = new List<GroupOrder>();
                foreach (var row in result)
                {
                    var groupOrder = new GroupOrder
                    {
                        RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                        PrimeUserId = row["PRIME_USER_ID"] != DBNull.Value ? Convert.ToInt32(row["PRIME_USER_ID"]) : (int?)null,
                        RequestType = row["REQUEST_TYPE"]?.ToString(),
                        RequestStates = row["REQUEST_STATES"]?.ToString(),
                        StartTime = row["START_TIME"] != DBNull.Value ? Convert.ToDateTime(row["START_TIME"]) : (DateTime?)null,
                        EndTime = row["END_TIME"] != DBNull.Value ? Convert.ToDateTime(row["END_TIME"]) : (DateTime?)null,
                        Title = row["TITLE"]?.ToString(),
                        InitiationTime = row["INITIATION_TIME"] != DBNull.Value ? Convert.ToDateTime(row["INITIATION_TIME"]) : (DateTime?)null,
                        PersonNum = row["PERSON_NUM"] != DBNull.Value ? Convert.ToInt32(row["PERSON_NUM"]) : (int?)null,
                        Description = row["DESCRIPTION"]?.ToString(),
                        PriceDistribution = row["PRICE_DISTRIBUTION"]?.ToString(),
                        ExtraRequirement = row["EXTRA_REQUIREMENT"]?.ToString(),
                        Participants = row["PARTICIPANTS"]?.ToString()
                    };

                    groupOrders.Add(groupOrder);
                }


                var list_result = Result<List<GroupOrder>>.Success(groupOrders);

                return list_result;  // 返回拼单信息列表
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching open group order info: {ex.Message}");
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");  // 发生异常时返回失败消息
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetOpenTypeRepository(int userId, string type)
        {
            try
            {
                // 获取当前时间
                var currentTime = DateTime.Now;

                // 查询拼单信息，符合两个条件：
                // 1. request_states == "open" 或者 (request_states == "normal" 且当前时间在 start_time 和 end_time 之间)
                // 2. prime_user_id != userId
                string query = @"
             SELECT * FROM group_order_request
             WHERE (request_states = 'open' 
             OR (request_states = 'normal' AND start_time <= ? AND end_time >= ?))
             AND prime_user_id != ?
             AND request_type = ?";

                var parameters = new List<object>
        {
             DateTime.Now,
             DateTime.Now,
             userId,
             type
        };

                // 执行查询
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                // 如果查询结果为空，返回失败消息
                if (result.Count == 0)
                {
                    return Result<List<GroupOrder>>.Failure("未找到符合条件的拼单信息");
                }

                // 将查询结果映射为 GroupOrder 对象列表
                var groupOrders = new List<GroupOrder>();
                foreach (var row in result)
                {
                    var groupOrder = new GroupOrder
                    {
                        RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                        PrimeUserId = row["PRIME_USER_ID"] != DBNull.Value ? Convert.ToInt32(row["PRIME_USER_ID"]) : (int?)null,
                        RequestType = row["REQUEST_TYPE"]?.ToString(),
                        RequestStates = row["REQUEST_STATES"]?.ToString(),
                        StartTime = row["START_TIME"] != DBNull.Value ? Convert.ToDateTime(row["START_TIME"]) : (DateTime?)null,
                        EndTime = row["END_TIME"] != DBNull.Value ? Convert.ToDateTime(row["END_TIME"]) : (DateTime?)null,
                        Title = row["TITLE"]?.ToString(),
                        InitiationTime = row["INITIATION_TIME"] != DBNull.Value ? Convert.ToDateTime(row["INITIATION_TIME"]) : (DateTime?)null,
                        PersonNum = row["PERSON_NUM"] != DBNull.Value ? Convert.ToInt32(row["PERSON_NUM"]) : (int?)null,
                        Description = row["DESCRIPTION"]?.ToString(),
                        PriceDistribution = row["PRICE_DISTRIBUTION"]?.ToString(),
                        ExtraRequirement = row["EXTRA_REQUIREMENT"]?.ToString(),
                        Participants = row["PARTICIPANTS"]?.ToString()
                    };

                    groupOrders.Add(groupOrder);
                }

                return Result<List<GroupOrder>>.Success(groupOrders);  // 返回拼单信息列表
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching open group order info: {ex.Message}");
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");  // 发生异常时返回失败消息
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetMineRepository(int userId)
        {
            string query = @"
                SELECT * FROM group_order_request
                WHERE prime_user_id = ? 
                OR JSON_CONTAINS(participants, ?)";

            var parameters = new List<object>
            {
                userId,                       // prime_user_id = userId
                $"[{userId}]"                  // participants 中包含 userId，JSON 格式数组
            };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var groupOrders = new List<GroupOrder>();

                    foreach (var row in result)
                    {
                        var groupOrder = new GroupOrder
                        {
                            RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                            PrimeUserId = Convert.ToInt32(row["PRIME_USER_ID"]),
                            RequestType = row["REQUEST_TYPE"]?.ToString(),
                            RequestStates = row["REQUEST_STATES"]?.ToString(),
                            StartTime = row["START_TIME"] != DBNull.Value ? Convert.ToDateTime(row["START_TIME"]) : null,
                            EndTime = row["END_TIME"] != DBNull.Value ? Convert.ToDateTime(row["END_TIME"]) : null,
                            Title = row["TITLE"]?.ToString(),
                            InitiationTime = row["INITIATION_TIME"] != DBNull.Value ? Convert.ToDateTime(row["INITIATION_TIME"]) : null,
                            PersonNum = Convert.ToInt32(row["PERSON_NUM"]),
                            Description = row["DESCRIPTION"]?.ToString(),
                            PriceDistribution = row["PRICE_DISTRIBUTION"]?.ToString(),
                            ExtraRequirement = row["EXTRA_REQUIREMENT"]?.ToString(),
                            Participants = row["PARTICIPANTS"]?.ToString()
                        };

                        groupOrders.Add(groupOrder);
                    }

                    return Result<List<GroupOrder>>.Success(groupOrders);
                }

                return Result<List<GroupOrder>>.Failure("未找到相关拼单信息");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching group orders: {ex.Message}");
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetMyCreateRepository(int userId)
        {
            string query = @"
                SELECT * FROM group_order_request
                WHERE prime_user_id = ?  ";

            var parameters = new List<object>
            {
                userId                     // prime_user_id = userId
            };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var groupOrders = new List<GroupOrder>();

                    foreach (var row in result)
                    {
                        var groupOrder = new GroupOrder
                        {
                            RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                            PrimeUserId = Convert.ToInt32(row["PRIME_USER_ID"]),
                            RequestType = row["REQUEST_TYPE"]?.ToString(),
                            RequestStates = row["REQUEST_STATES"]?.ToString(),
                            StartTime = row["START_TIME"] != DBNull.Value ? Convert.ToDateTime(row["START_TIME"]) : null,
                            EndTime = row["END_TIME"] != DBNull.Value ? Convert.ToDateTime(row["END_TIME"]) : null,
                            Title = row["TITLE"]?.ToString(),
                            InitiationTime = row["INITIATION_TIME"] != DBNull.Value ? Convert.ToDateTime(row["INITIATION_TIME"]) : null,
                            PersonNum = Convert.ToInt32(row["PERSON_NUM"]),
                            Description = row["DESCRIPTION"]?.ToString(),
                            PriceDistribution = row["PRICE_DISTRIBUTION"]?.ToString(),
                            ExtraRequirement = row["EXTRA_REQUIREMENT"]?.ToString(),
                            Participants = row["PARTICIPANTS"]?.ToString()
                        };

                        groupOrders.Add(groupOrder);
                    }

                    return Result<List<GroupOrder>>.Success(groupOrders);
                }

                return Result<List<GroupOrder>>.Failure("未找到相关拼单信息");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching group orders: {ex.Message}");
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<List<GroupOrder>>> GroupOrderGetMyParticipateRepository(int userId)
        {
            string query = @"
                SELECT * FROM group_order_request
                WHERE JSON_CONTAINS(participants, ?)";

            var parameters = new List<object>
            {
                $"[{userId}]"                  // participants 中包含 userId，JSON 格式数组
            };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var groupOrders = new List<GroupOrder>();

                    foreach (var row in result)
                    {
                        var groupOrder = new GroupOrder
                        {
                            RequestId = Convert.ToInt32(row["REQUEST_ID"]),
                            PrimeUserId = Convert.ToInt32(row["PRIME_USER_ID"]),
                            RequestType = row["REQUEST_TYPE"]?.ToString(),
                            RequestStates = row["REQUEST_STATES"]?.ToString(),
                            StartTime = row["START_TIME"] != DBNull.Value ? Convert.ToDateTime(row["START_TIME"]) : null,
                            EndTime = row["END_TIME"] != DBNull.Value ? Convert.ToDateTime(row["END_TIME"]) : null,
                            Title = row["TITLE"]?.ToString(),
                            InitiationTime = row["INITIATION_TIME"] != DBNull.Value ? Convert.ToDateTime(row["INITIATION_TIME"]) : null,
                            PersonNum = Convert.ToInt32(row["PERSON_NUM"]),
                            Description = row["DESCRIPTION"]?.ToString(),
                            PriceDistribution = row["PRICE_DISTRIBUTION"]?.ToString(),
                            ExtraRequirement = row["EXTRA_REQUIREMENT"]?.ToString(),
                            Participants = row["PARTICIPANTS"]?.ToString()
                        };

                        groupOrders.Add(groupOrder);
                    }

                    return Result<List<GroupOrder>>.Success(groupOrders);
                }

                return Result<List<GroupOrder>>.Failure("未找到相关拼单信息");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching group orders: {ex.Message}");
                return Result<List<GroupOrder>>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<int> AddUserToGroupOrderRepository(int userId, int requestId)
        {
            string query = @"
                UPDATE group_order_request
                SET participants = IFNULL(participants, '[]')
                WHERE request_id = ?;
                UPDATE group_order_request
                SET participants = JSON_ARRAY_APPEND(participants, '$', ?)
                WHERE request_id = ?";

            var parameters = new List<object>
            {
                requestId,
                userId,            // 要加入拼单的用户ID
                requestId         // 要加入的拼单请求ID
            };

            try
            {
                var result = await _mySqlConnection.ExecuteNonQueryAsync(query, parameters);

                if (result > 0)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user to group order: {ex.Message}");
                return -1;
            }
        }

        public async Task<Result<string>> GetParticipantsByRequestIdRepository(int requestId)
        {
            string query = @"
                SELECT participants
                FROM group_order_request
                WHERE request_id = ?";

            var parameters = new List<object> { requestId };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                if (result.Count > 0)
                {
                    var participantsJson = result[0]["participants"]?.ToString();
                    if (!string.IsNullOrEmpty(participantsJson))
                    {
                        try
                        {
                            // 使用 JsonConvert 反序列化 JSON 字符串为 List<int>
                            var participants = JsonConvert.DeserializeObject<List<int>>(participantsJson);
                            Console.WriteLine($"Participants: {string.Join(", ", participants)}");
                            return Result<string>.Success(participantsJson); // 返回 participants JSON 字符串
                        }
                        catch (Newtonsoft.Json.JsonException jsonEx) // 捕获 JsonException
                        {
                            Console.WriteLine($"JSON 解析错误: {jsonEx.Message}");
                            return Result<string>.Failure($"JSON 解析错误: {jsonEx.Message}");
                        }
                        catch (Exception ex) // 捕获其他异常
                        {
                            Console.WriteLine($"其他错误: {ex.Message}");
                            return Result<string>.Failure($"其他错误: {ex.Message}");
                        }
                    }
                    else
                    {
                        return Result<string>.Failure("Participants 字段为空");
                    }
                }
                else
                {
                    Console.WriteLine("No matching records found");
                    return Result<string>.Failure("未找到相关拼单请求");
                }

            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"查询出错: {ex.Message}");
            }
        }

        public async Task<int> DeleteUserToGroupOrderRepository(int userId, int requestId)
        {
            // 获取当前拼单请求的 participants 字段
            var result = await GetParticipantsByRequestIdRepository(requestId);
            if (!result.IsSuccess)
            {
                return -3; // 如果查询失败，返回 -1
            }

            // 解析 participants 字段
            var participantsJson = result.Data;
            var participantsList = System.Text.Json.JsonSerializer.Deserialize<List<int>>(participantsJson) ?? new List<int>();

            // 删除指定的 userId
            participantsList.Remove(userId);

            // 如果删除后 participants 为空，则设置为一个空的数组
            var updatedParticipantsJson = System.Text.Json.JsonSerializer.Serialize(participantsList);

            // 更新数据库中的 participants 字段
            string query = @"
                UPDATE group_order_request
                SET participants = ?
                WHERE request_id = ?";

            var parameters = new List<object>
            {
                updatedParticipantsJson, // 更新后的 JSON 字符串
                requestId                // 拼单请求 ID
            };

            try
            {
                var updateResult = await _mySqlConnection.ExecuteNonQueryAsync(query, parameters);

                if (updateResult > 0)
                {
                    return 1; // 更新成功
                }
                else
                {
                    return -2; // 如果没有行被更新，返回 -1
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating participants: {ex.Message}");
                return -1; // 更新出错
            }

        }

        public async Task<int> GroupOrderDeleteRepository(int requestId)
        {
            string query = "DELETE FROM group_order_request WHERE request_id = ?";
            var parameters = new List<object> { requestId };

            try
            {
                await _mySqlConnection.ExecuteNonQueryAsync(query, parameters);
                var result = await DeleteGroupProjectRepository(requestId);
                if(result.IsSuccess)
                {
                    return 1;  // 删除成功
                }
                else
                {
                    return -2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing DELETE request: {ex.Message}");
                return -1;
            }
        }

        public async Task<Result<string>> DeleteGroupProjectRepository(int requestId)
        {
            // 开始同步事务
            try
            {
                // 删除对应的具体信息表的记录（根据request_type区分）
                var groupProjectQuery = "SELECT project_id, request_type FROM group_project WHERE request_id = ?";
                var parameters = new List<object> { requestId };

                var groupProjectResult = await _mySqlConnection.ExecuteQueryAsync(groupProjectQuery, parameters);

                if (groupProjectResult.Count == 0)
                {
                    return Result<string>.Failure("未找到对应的GroupProject");
                }

                // 获取项目的ID和请求类型
                var groupProject = groupProjectResult[0];
                var projectId = Convert.ToInt32(groupProject["project_id"]);
                var requestType = groupProject["request_type"].ToString();

                // 根据requestType删除对应的具体信息
                if (requestType == "traffic")
                {
                    // 删除 TrafficGroup
                    var deleteTrafficGroupQuery = "DELETE FROM traffic_group WHERE project_id = ?";
                    await _mySqlConnection.ExecuteNonQueryAsync(deleteTrafficGroupQuery, new List<object> { projectId });
                }
                else if (requestType == "product")
                {
                    // 删除 ProductGroup
                    var deleteProductGroupQuery = "DELETE FROM product_group WHERE project_id = ?";
                    await _mySqlConnection.ExecuteNonQueryAsync(deleteProductGroupQuery, new List<object> { projectId });
                }
                else if (requestType == "activity")
                {
                    // 删除 ActivityGroup
                    var deleteActivityGroupQuery = "DELETE FROM activity_group WHERE project_id = ?";
                    await _mySqlConnection.ExecuteNonQueryAsync(deleteActivityGroupQuery, new List<object> { projectId });
                }

                // 删除 GroupProject
                var deleteGroupProjectQuery = "DELETE FROM group_project WHERE request_id = ?";
                await _mySqlConnection.ExecuteNonQueryAsync(deleteGroupProjectQuery, new List<object> { requestId });

                return Result<string>.Success("删除成功");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"删除操作失败: {ex.Message}");
            }

        }

        public async Task<int> GroupOrderCreateRepository(GroupOrder groupOrder)
        {

            string query = @"
                INSERT INTO group_order_request (PRIME_USER_ID, REQUEST_TYPE, REQUEST_STATES, START_TIME, END_TIME , TITLE, INITIATION_TIME, PERSON_NUM, DESCRIPTION, PRICE_DISTRIBUTION, EXTRA_REQUIREMENT )
                VALUES (?,?,?,?,?,?,?,?,?,?,?)";

            var parameters = new List<object>
                {
                groupOrder.PrimeUserId,
                groupOrder.RequestType,
                "normal",//默认
                groupOrder.StartTime,
                groupOrder.EndTime,
                groupOrder.Title,
                groupOrder.InitiationTime,
                groupOrder.PersonNum,
                groupOrder.Description,
                groupOrder.PriceDistribution,
                groupOrder.ExtraRequirement
                };

            var rowsAffected = await _mySqlConnection.ExecuteNonQueryAsync(query, parameters);

            if (rowsAffected == 0)
            {
                return -1;
            }
            else
            {

                    string selectQuery = "SELECT LAST_INSERT_ID()";
                var newGroupOrderIdResult = await _mySqlConnection.ExecuteQueryAsync(selectQuery);

                // 这里假设 LAST_INSERT_ID() 返回一个只有一列的结果
                if (newGroupOrderIdResult.Count > 0)
                {
                    // 从结果中获取第一个字典，获取 'LAST_INSERT_ID()' 对应的值
                    var newGroupOrderId = newGroupOrderIdResult[0].Values.FirstOrDefault();
                    if (newGroupOrderId != null && int.TryParse(newGroupOrderId.ToString(), out int groupOrderId))
                    {
                        string ini_query = @"
                UPDATE group_order_request
                SET participants = IFNULL(participants, '[]')
                WHERE request_id = ?;";

                        var ini_parameters = new List<object>
                     {
                    newGroupOrderId,

                      };

                        var result = await _mySqlConnection.ExecuteNonQueryAsync(ini_query, ini_parameters);

                        if (!(result > 0))
                        {
                            return -1;
                        }


                        return groupOrderId; // 返回新插入的用户 ID
                    }
                }

                return -1; // 如果无法解析 ID 或者结果为空

            }

        }

        public async Task<int> GroupOrderModifyRepository(JsonElement modify_request)
        {
            if (!modify_request.EnumerateObject().Any())
            {
                return -3;  // 如果请求为空，返回 -3
            }

            // 从请求体中读取 requestId
            if (!modify_request.TryGetProperty("requestId", out var requestIdProperty) || !requestIdProperty.ValueKind.Equals(JsonValueKind.Number))
            {
                return -4;  // 如果没有提供 requestId 或者类型不匹配，返回 -4
            }
            int requestId = requestIdProperty.GetInt32();  // 获取 requestId

            var queryBuilder = new System.Text.StringBuilder("UPDATE group_order_request SET ");
            var parameters = new List<object>();

            Console.WriteLine($"Modify GroupOrder: {modify_request}");

            // 第一个循环生成查询字符串
            foreach (var property in modify_request.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "requestStates":
                        if(property.Value.GetString()=="open"|| property.Value.GetString() == "close"||property.Value.GetString() == "normal")
                        {
                            queryBuilder.Append("REQUEST_STATES = ?, ");
                        }
                        break;
                    case "startTime":
                        queryBuilder.Append("START_TIME = ?, ");
                        break;
                    case "endTime":
                        queryBuilder.Append("END_TIME = ?, ");
                        break;
                    case "title":
                        queryBuilder.Append("TITLE = ?, ");
                        break;
                    case "personNum":
                        queryBuilder.Append("PERSON_NUM = ?, ");
                        break;
                    case "description":
                        queryBuilder.Append("DESCRIPTION = ?, ");
                        break;
                    case "priceDistribution":
                        queryBuilder.Append("PRICE_DISTRIBUTION = ?, ");
                        break;
                    case "extraRequirement":
                        queryBuilder.Append("EXTRA_REQUIREMENT = ?, ");
                        break;
                    default:
                        break;
                }
            }

            // 移除最后一个逗号和空格
            if (queryBuilder.ToString() != "UPDATE group_order_request SET ")
            {
                queryBuilder.Length -= 2;
            }

            queryBuilder.Append(" WHERE REQUEST_ID = ?");  // 根据 requestId 作为条件
            string query = queryBuilder.ToString();  // 完整的 SQL 查询语句
            Console.WriteLine($"Generated Query: {query}");

            // 第二个循环添加参数
            foreach (var property in modify_request.EnumerateObject())
            {
                switch (property.Name)
                {
                    case "requestStates":
                        if (property.Value.GetString() == "open" || property.Value.GetString() == "close" || property.Value.GetString() == "normal")
                        {
                            parameters.Add(property.Value.GetString());
                        }
                        break;
                    case "startTime":
                        parameters.Add(property.Value.GetDateTime());
                        break;
                    case "endTime":
                        parameters.Add(property.Value.GetDateTime());
                        break;
                    case "title":
                        parameters.Add(property.Value.GetString());
                        break;
                    case "personNum":
                        parameters.Add(property.Value.GetInt32());
                        break;
                    case "description":
                        parameters.Add(property.Value.GetString());
                        break;
                    case "priceDistribution":
                        parameters.Add(property.Value.GetString());
                        break;
                    case "extraRequirement":
                        parameters.Add(property.Value.GetString());
                        break;
                    default:
                        break;
                }
            }

            // 添加 requestId 作为 WHERE 条件
            parameters.Add(requestId);

            try
            {
                // 执行更新查询
                var rowsAffected = await _mySqlConnection.ExecuteNonQueryAsync(query, parameters);
                if (rowsAffected == 0)
                {
                    return -2;  // 如果没有更新任何行，返回 -2
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing update request: {ex.Message}");
                return -1;  // 捕获异常并返回 -1
            }

            string ini_query = @"
                UPDATE group_order_request
SET participants = '[]'
WHERE request_id = ?;
";

            var ini_parameters = new List<object>
                     {
                    requestId
                      };

            var result = await _mySqlConnection.ExecuteNonQueryAsync(ini_query, ini_parameters);

            modify_request.TryGetProperty("participants", out var participantsJson);

            // 先将 participantsJson 解析为字符串
            var participantsString = participantsJson.GetString();

            // 再将字符串反序列化为整数列表
            var participants = System.Text.Json.JsonSerializer.Deserialize<List<int>>(participantsString);

            // Step 2: 循环调用接口，将每一个 userId 插入到 participants 中
            foreach (var userId in participants)
            {
                var insertResult = await AddUserToGroupOrderRepository(userId, requestId);
                if (insertResult == -1)
                {
                    return -1;
                }

            }

            return 1;  // 更新成功，返回 1
        }

        public async Task<Result<string>> ModifyGroupProjectRepository(JsonElement requestJson)
        {
            // 输出调试信息
            Console.WriteLine($"Received requestJson: {requestJson}");

            // 从 JSON 中提取参数
            var requestId = requestJson.GetProperty("requestId").GetInt32();
            var projectName = requestJson.GetProperty("projectName").GetString();
            var totalPrice = requestJson.GetProperty("totalPrice").GetInt32();
            var requestType = requestJson.GetProperty("requestType").GetString();
            var specificInfoJson = requestJson.GetProperty("specificInfo");

            try
            {
                // 查询对应的 projectId
                var queryProjectId = "SELECT project_id FROM group_project WHERE request_id = ?";
                var queryProjectIdParameters = new List<object> { requestId };
                var projectIdResult = await _mySqlConnection.ExecuteQueryAsync(queryProjectId, queryProjectIdParameters);

                if (projectIdResult.Count == 0)
                {
                    return Result<string>.Failure("未找到对应的 projectId");
                }

                // 获取 projectId
                var projectId = Convert.ToInt32(projectIdResult[0]["project_id"]);

                // 更新 GroupProject
                var updateGroupProjectQuery = @"
            UPDATE group_project
            SET project_name = ?, total_price = ?, request_type = ?
            WHERE project_id = ?";
                var groupProjectParameters = new List<object>
        {
            projectName,
            totalPrice,
            requestType,
            projectId
        };

                // 输出完整的查询语句进行调试
                Console.WriteLine($"Executing query: {updateGroupProjectQuery}");
                Console.WriteLine("GroupProject update parameters:");
                foreach (var param in groupProjectParameters)
                {
                    Console.WriteLine(param);
                }

                // 执行更新操作
                var groupProjectResult = await _mySqlConnection.ExecuteNonQueryAsync(updateGroupProjectQuery, groupProjectParameters);
                Console.WriteLine($"GroupProject update result: {groupProjectResult}");

                if (groupProjectResult <= 0)
                {
                    return Result<string>.Failure("更新GroupProject失败");
                }

                // 根据 requestType 更新具体信息
                if (requestType == "traffic" && specificInfoJson.ValueKind != JsonValueKind.Null)
                {
                    // 使用类似 requestJson 的方式解析 specificInfoJson
                    var originPlace = specificInfoJson.GetProperty("originPlace").GetString();
                    var destinationPlace = specificInfoJson.GetProperty("destinationPlace").GetString();
                    var trafficTime = specificInfoJson.GetProperty("trafficTime").GetDateTime();

                    Console.WriteLine("Deserialized TrafficGroup:");
                    Console.WriteLine($"OriginPlace: {originPlace}");
                    Console.WriteLine($"DestinationPlace: {destinationPlace}");
                    Console.WriteLine($"TrafficTime: {trafficTime}");

                    var updateTrafficGroupQuery = @"
                UPDATE traffic_group
                SET origin_place = ?, destination_place = ?, traffic_time = ?
                WHERE project_id = ?";
                    var trafficParameters = new List<object>
            {
                originPlace,
                destinationPlace,
                trafficTime,
                projectId
            };

                    // 输出更新参数以调试
                    Console.WriteLine("TrafficGroup update parameters:");
                    foreach (var param in trafficParameters)
                    {
                        Console.WriteLine(param);
                    }

                    // 执行更新操作并检查结果
                    var trafficResult = await _mySqlConnection.ExecuteNonQueryAsync(updateTrafficGroupQuery, trafficParameters);
                    Console.WriteLine($"TrafficGroup update result: {trafficResult}");
                    if (trafficResult <= 0)
                    {
                        return Result<string>.Failure("更新TrafficGroup失败");
                    }
                }
                else if (requestType == "product" && specificInfoJson.ValueKind != JsonValueKind.Null)
                {
                    // 使用类似 requestJson 的方式解析 specificInfoJson
                    var platform = specificInfoJson.GetProperty("platform").GetString();
                    var address = specificInfoJson.GetProperty("address").GetString();

                    Console.WriteLine("Deserialized ProductGroup:");
                    Console.WriteLine($"Platform: {platform}");
                    Console.WriteLine($"Address: {address}");

                    var updateProductGroupQuery = @"
                UPDATE product_group
                SET platform = ?, address = ?
                WHERE project_id = ?";
                    var productParameters = new List<object>
            {
                platform,
                address,
                projectId
            };

                    // 输出更新参数以调试
                    Console.WriteLine("ProductGroup update parameters:");
                    foreach (var param in productParameters)
                    {
                        Console.WriteLine(param);
                    }

                    // 执行更新操作并检查结果
                    var productResult = await _mySqlConnection.ExecuteNonQueryAsync(updateProductGroupQuery, productParameters);
                    Console.WriteLine($"ProductGroup update result: {productResult}");
                    if (productResult <= 0)
                    {
                        return Result<string>.Failure("更新ProductGroup失败");
                    }
                }
                else if (requestType == "activity" && specificInfoJson.ValueKind != JsonValueKind.Null)
                {
                    // 使用类似 requestJson 的方式解析 specificInfoJson
                    var location = specificInfoJson.GetProperty("location").GetString();
                    var activityTime = specificInfoJson.GetProperty("activityTime").GetDateTime();

                    Console.WriteLine("Deserialized ActivityGroup:");
                    Console.WriteLine($"Location: {location}");
                    Console.WriteLine($"ActivityTime: {activityTime}");

                    var updateActivityGroupQuery = @"
                UPDATE activity_group
                SET location = ?, activity_time = ?
                WHERE project_id = ?";
                    var activityParameters = new List<object>
            {
                location,
                activityTime,
                projectId
            };

                    // 输出更新参数以调试
                    Console.WriteLine("ActivityGroup update parameters:");
                    foreach (var param in activityParameters)
                    {
                        Console.WriteLine(param);
                    }

                    // 执行更新操作并检查结果
                    var activityResult = await _mySqlConnection.ExecuteNonQueryAsync(updateActivityGroupQuery, activityParameters);
                    Console.WriteLine($"ActivityGroup update result: {activityResult}");
                    if (activityResult <= 0)
                    {
                        return Result<string>.Failure("更新ActivityGroup失败");
                    }
                }
                else
                {
                    return Result<string>.Failure("无效的请求类型或具体信息");
                }

                return Result<string>.Success("更新成功");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"更新操作失败: {ex.Message}");
            }
        }

        public async Task<Result<string>> CreateGroupProjectRepository(JsonElement requestJson)
        {
            // 输出调试信息
            Console.WriteLine($"Received requestJson: {requestJson}");

            // 从 JSON 中提取参数
            var requestId = requestJson.GetProperty("requestId").GetInt32();
            var projectName = requestJson.GetProperty("projectName").GetString();
            var totalPrice = requestJson.GetProperty("totalPrice").GetInt32();
            var requestType = requestJson.GetProperty("requestType").GetString();
            var specificInfoJson = requestJson.GetProperty("specificInfo");

            try
            {
                // 插入 GroupProject
                var insertGroupProjectQuery = @"
        INSERT INTO group_project (request_id, project_name, total_price, request_type)
        VALUES (?, ?, ?, ?)";
                var groupProjectParameters = new List<object>
        {
            requestId,
            projectName,
            totalPrice,
            requestType
        };

                // 输出插入参数以调试
                Console.WriteLine("GroupProject insert parameters:");
                foreach (var param in groupProjectParameters)
                {
                    Console.WriteLine(param);
                }

                // 执行插入操作
                var groupProjectResult = await _mySqlConnection.ExecuteNonQueryAsync(insertGroupProjectQuery, groupProjectParameters);
                Console.WriteLine($"GroupProject insert result: {groupProjectResult}");

                if (groupProjectResult <= 0)
                {
                    return Result<string>.Failure("插入GroupProject失败");
                }

                string selectQuery = "SELECT LAST_INSERT_ID()";
                var newUserIdResult = await _mySqlConnection.ExecuteQueryAsync(selectQuery);

                // 这里假设 LAST_INSERT_ID() 返回一个只有一列的结果
                if (newUserIdResult.Count > 0)
                {
                    // 从结果中获取第一个字典，获取 'LAST_INSERT_ID()' 对应的值
                    var newProjectId = newUserIdResult[0].Values.FirstOrDefault();
                    if (newProjectId != null && int.TryParse(newProjectId.ToString(), out int projectId))
                    {
                        // 根据requestType插入具体信息
                        if (requestType == "traffic" && specificInfoJson.ValueKind != JsonValueKind.Null)
                        {
                            // 使用类似 requestJson 的方式解析 specificInfoJson
                            var originPlace = specificInfoJson.GetProperty("originPlace").GetString();
                            var destinationPlace = specificInfoJson.GetProperty("destinationPlace").GetString();
                            var trafficTime = specificInfoJson.GetProperty("trafficTime").GetDateTime();

                            Console.WriteLine("Deserialized TrafficGroup:");
                            Console.WriteLine($"OriginPlace: {originPlace}");
                            Console.WriteLine($"DestinationPlace: {destinationPlace}");
                            Console.WriteLine($"TrafficTime: {trafficTime}");

                            var insertTrafficGroupQuery = @"
                    INSERT INTO traffic_group (project_id, origin_place, destination_place, traffic_time)
                    VALUES (?, ?, ?, ?)";
                            var trafficParameters = new List<object>
                    {
                        projectId,
                        originPlace,
                        destinationPlace,
                        trafficTime
                    };

                            // 输出插入参数以调试
                            Console.WriteLine("TrafficGroup insert parameters:");
                            foreach (var param in trafficParameters)
                            {
                                Console.WriteLine(param);
                            }

                            // 执行插入操作并检查结果
                            var trafficResult = await _mySqlConnection.ExecuteNonQueryAsync(insertTrafficGroupQuery, trafficParameters);
                            Console.WriteLine($"TrafficGroup insert result: {trafficResult}");
                            if (trafficResult <= 0)
                            {
                                return Result<string>.Failure("插入TrafficGroup失败");
                            }
                        }
                        else if (requestType == "product" && specificInfoJson.ValueKind != JsonValueKind.Null)
                        {
                            // 使用类似 requestJson 的方式解析 specificInfoJson
                            var platform = specificInfoJson.GetProperty("platform").GetString();
                            var address = specificInfoJson.GetProperty("address").GetString();

                            Console.WriteLine("Deserialized ProductGroup:");
                            Console.WriteLine($"Platform: {platform}");
                            Console.WriteLine($"Address: {address}");

                            var insertProductGroupQuery = @"
                    INSERT INTO product_group (project_id, platform, address)
                    VALUES (?, ?, ?)";
                            var productParameters = new List<object>
                    {
                        projectId,
                        platform,
                        address
                    };

                            // 输出插入参数以调试
                            Console.WriteLine("ProductGroup insert parameters:");
                            foreach (var param in productParameters)
                            {
                                Console.WriteLine(param);
                            }

                            // 执行插入操作并检查结果
                            var productResult = await _mySqlConnection.ExecuteNonQueryAsync(insertProductGroupQuery, productParameters);
                            Console.WriteLine($"ProductGroup insert result: {productResult}");
                            if (productResult <= 0)
                            {
                                return Result<string>.Failure("插入ProductGroup失败");
                            }
                        }
                        else if (requestType == "activity" && specificInfoJson.ValueKind != JsonValueKind.Null)
                        {
                            // 使用类似 requestJson 的方式解析 specificInfoJson
                            var location = specificInfoJson.GetProperty("location").GetString();
                            var activityTime = specificInfoJson.GetProperty("activityTime").GetDateTime();

                            Console.WriteLine("Deserialized ActivityGroup:");
                            Console.WriteLine($"Location: {location}");
                            Console.WriteLine($"ActivityTime: {activityTime}");

                            var insertActivityGroupQuery = @"
                    INSERT INTO activity_group (project_id, location, activity_time)
                    VALUES (?, ?, ?)";
                            var activityParameters = new List<object>
                    {
                        projectId,
                        location,
                        activityTime
                    };

                            // 输出插入参数以调试
                            Console.WriteLine("ActivityGroup insert parameters:");
                            foreach (var param in activityParameters)
                            {
                                Console.WriteLine(param);
                            }

                            // 执行插入操作并检查结果
                            var activityResult = await _mySqlConnection.ExecuteNonQueryAsync(insertActivityGroupQuery, activityParameters);
                            Console.WriteLine($"ActivityGroup insert result: {activityResult}");
                            if (activityResult <= 0)
                            {
                                return Result<string>.Failure("插入ActivityGroup失败");
                            }
                        }
                        else
                        {
                            return Result<string>.Failure("无效的请求类型或具体信息");
                        }

                        return Result<string>.Success("请求成功");
                    }
                }
                return Result<string>.Failure("插入操作失败：");
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"插入操作失败: {ex.Message}");
            }
        }

    }
}
