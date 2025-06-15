using Microsoft.AspNetCore.Mvc;
using JishitongBackend.Models;
using System.Data;
using JishitongBackend.Services;
using Newtonsoft.Json;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;


namespace JishitongBackend.Controllers
{
    [Route("v1/groupOrder")]
    [ApiController]
    public class GroupOrderController : ControllerBase
    {
        private readonly GroupOrderService _groupOrderService;
        private readonly UserService _userService;
        private readonly HttpClient _httpClient; // 添加 HttpClient 实例

        public GroupOrderController(GroupOrderService groupOrderService, UserService userService, HttpClient httpClient)
        {
            _groupOrderService = groupOrderService;
            _userService = userService;
            _httpClient = httpClient; // 注入 HttpClient
        }

        //Get v1/groupOrder/getinformation
        [HttpGet("getinformation")]
        public async Task<IActionResult> GetGroupOrderInformationController([FromHeader(Name = "Authorization")] string authHeader, int requestId)
        {
            if (!_userService.TokenCheckService(authHeader))
            {
                return Unauthorized();  // 校验 token，如果无效返回 401 Unauthorized
            }

            var result = await _groupOrderService.GetCompleteGroupOrderInformationService(requestId);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "获取消息信息成功",
                    data = result.Data
                };
                return Ok(response);
            }
            else
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 如果失败，返回错误信息
            }
        }

        [HttpPost("getmultipleinformation")]
        public async Task<IActionResult> GetMultipleGroupOrderInformationController([FromHeader(Name = "Authorization")] string authHeader, [FromBody] List<int> requestIds)
        {
            if (!_userService.TokenCheckService(authHeader))
            {
                return Unauthorized();  // 校验 token，如果无效返回 401 Unauthorized
            }

            // 调用服务层的方法，并等待结果
            var result = await _groupOrderService.GetMultipleGroupOrderInformationService(requestIds);

            // 如果所有请求都失败，返回一个错误消息
            if (!result.IsSuccess)
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(new { Message = "所有请求都失败", Errors = result.Message });
            }

            // 返回所有成功的结果
            return Ok(new { Data = result.Data, Errors = result.Message });
        }

        //Get v1/groupOrder/getAll
        [HttpGet("getAll")]
        public async Task<IActionResult> GetGroupOrderGetAll([FromHeader(Name = "Authorization")] string authHeader)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Get All GroupOrder");

            var result = await _groupOrderService.GroupOrderGetAllService();

            if (result.IsSuccess)
            {
                // 使用 LINQ 循环遍历并返回新的列表
                var response = new
                {
                    code = 200,
                    msg = "获取拼单信息成功",
                    data = result.Data.Select(order => new
                    {
                        request_id = order.RequestId
                    }).ToList()
                };

                return Ok(response);  // 返回处理后的数据
            }
            else
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回错误信息
            }

        }

        //Get v1/groupOrder/getAllProduct
        [HttpGet("getAllProduct")]
        public async Task<IActionResult> GetGroupOrderGetAllProduct([FromHeader(Name = "Authorization")] string authHeader)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Get All GroupOrder");

            var result = await _groupOrderService.GroupOrderGetAllProductService();

            if (result.IsSuccess)
            {
                // 使用 LINQ 循环遍历并返回新的列表
                var response = new
                {
                    code = 200,
                    msg = "获取拼单信息成功",
                    data = result.Data.Select(order => new
                    {
                        request_id = order.RequestId
                    }).ToList()
                };

                return Ok(response);  // 返回处理后的数据
            }
            else
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回错误信息
            }

        }

        //Get v1/groupOrder/getAllTraffic
        [HttpGet("getAllTraffic")]
        public async Task<IActionResult> GetGroupOrderGetAllTraffic([FromHeader(Name = "Authorization")] string authHeader)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Get All GroupOrder");

            var result = await _groupOrderService.GroupOrderGetAllTrafficService();

            if (result.IsSuccess)
            {
                // 使用 LINQ 循环遍历并返回新的列表
                var response = new
                {
                    code = 200,
                    msg = "获取拼单信息成功",
                    data = result.Data.Select(order => new
                    {
                        request_id = order.RequestId
                    }).ToList()
                };

                return Ok(response);  // 返回处理后的数据
            }
            else
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回错误信息
            }

        }

        //Get v1/groupOrder/getAllActivity
        [HttpGet("getAllActivity")]
        public async Task<IActionResult> GetGroupOrderGetAllActivity([FromHeader(Name = "Authorization")] string authHeader)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Get All GroupOrder");

            var result = await _groupOrderService.GroupOrderGetAllActivityService();

            if (result.IsSuccess)
            {
                // 使用 LINQ 循环遍历并返回新的列表
                var response = new
                {
                    code = 200,
                    msg = "获取拼单信息成功",
                    data = result.Data.Select(order => new
                    {
                        request_id = order.RequestId
                    }).ToList()
                };

                return Ok(response);  // 返回处理后的数据
            }
            else
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回错误信息
            }

        }

        //筛选开放且非本人发起的拼单
        //Get v1/groupOrder/getOpen
        [HttpGet("getOpen")]
        public async Task<IActionResult> GetGroupOrderGetOpen([FromHeader(Name = "Authorization")] string authHeader, int userId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Get Open GroupOrder");

            var result = await _groupOrderService.GroupOrderGetOpenService(userId);

            if (result.IsSuccess)
            {
                // 使用 LINQ 循环遍历并返回新的列表
                var response = new
                {
                    code = 200,
                    msg = "获取拼单信息成功",
                    data = result.Data.Select(order => new
                    {
                        request_id = order.RequestId
                    }).ToList()
                };

                return Ok(response);  // 返回处理后的数据
            }
            else
            {

                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回错误信息
            }

        }

        //Get v1/groupOrder/getOpenTraffic
        [HttpGet("getOpenTraffic")]
        public async Task<IActionResult> GetGroupOrderGetOpenTraffic([FromHeader(Name = "Authorization")] string authHeader, int userId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Get Open GroupOrder");

            var result = await _groupOrderService.GroupOrderGetOpenTrafficService(userId);

            if (result.IsSuccess)
            {
                // 使用 LINQ 循环遍历并返回新的列表
                var response = new
                {
                    code = 200,
                    msg = "获取拼单信息成功",
                    data = result.Data.Select(order => new
                    {
                        request_id = order.RequestId
                    }).ToList()
                };

                return Ok(response);  // 返回处理后的数据
            }
            else
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回错误信息
            }

        }

        //Get v1/groupOrder/getOpenActivity
        [HttpGet("getOpenActivity")]
        public async Task<IActionResult> GetGroupOrderGetOpenActivity([FromHeader(Name = "Authorization")] string authHeader, int userId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Get Open GroupOrder");

            var result = await _groupOrderService.GroupOrderGetOpenActivityService(userId);

            if (result.IsSuccess)
            {
                // 使用 LINQ 循环遍历并返回新的列表
                var response = new
                {
                    code = 200,
                    msg = "获取拼单信息成功",
                    data = result.Data.Select(order => new
                    {
                        request_id = order.RequestId
                    }).ToList()
                };

                return Ok(response);  // 返回处理后的数据
            }
            else
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回错误信息
            }

        }

        //Get v1/groupOrder/getOpenProduct
        [HttpGet("getOpenProduct")]
        public async Task<IActionResult> GetGroupOrderGetOpenProduct([FromHeader(Name = "Authorization")] string authHeader, int userId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Get Open GroupOrder");

            var result = await _groupOrderService.GroupOrderGetOpenProductService(userId);

            if (result.IsSuccess)
            {
                // 使用 LINQ 循环遍历并返回新的列表
                var response = new
                {
                    code = 200,
                    msg = "获取拼单信息成功",
                    data = result.Data.Select(order => new
                    {
                        request_id = order.RequestId
                    }).ToList()
                };

                return Ok(response);  // 返回处理后的数据
            }
            else
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回错误信息
            }

        }

        //筛选本人发起或参与的拼单
        //Get v1/groupOrder/getMine
        [HttpGet("getMine")]
        public async Task<IActionResult> GetGroupOrderGetMine([FromHeader(Name = "Authorization")] string authHeader, int userId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Get Mine GroupOrder");

            var result = await _groupOrderService.GroupOrderGetMineService(userId);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "获取拼单信息成功",
                    data = result.Data // 这里的 data 已经是格式化好的数据
                };

                return Ok(response);  // 返回处理后的数据
            }
            else
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回错误信息
            }
        }

        //Get v1/groupOrder/getMyParticipate
        [HttpGet("getMyParticipate")]
        public async Task<IActionResult> GetGroupOrderGetMyParticipate([FromHeader(Name = "Authorization")] string authHeader, int userId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Get Mine GroupOrder");

            var result = await _groupOrderService.GroupOrderGetMyParticipateService(userId);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "获取拼单信息成功",
                    data = result.Data // 这里的 data 已经是格式化好的数据
                };

                return Ok(response);  // 返回处理后的数据
            }
            else
            {
                if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回错误信息
            }
        }

        //Get v1/groupOrder/getMyCreate
        [HttpGet("getMyCreate")]
        public async Task<IActionResult> GetGroupOrderGetMyCreate([FromHeader(Name = "Authorization")] string authHeader, int userId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Get Mine GroupOrder");

            var result = await _groupOrderService.GroupOrderGetMyCreateService(userId);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "获取拼单信息成功",
                    data = result.Data // 这里的 data 已经是格式化好的数据
                };

                return Ok(response);  // 返回处理后的数据
            }
            else
            {
              if (result.Code == 204)
                {
                    return NoContent();
                }
                return BadRequest(result);  // 失败时返回错误信息
            }
        }

        //POST v1/groupOrder/createRequest
        [HttpPost("createRequest")]
        public async Task<IActionResult> PostGroupOrderCreate([FromHeader(Name = "Authorization")] string authHeader, [FromBody] GroupOrder groupOrder)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Create GroupOrder: {JsonConvert.SerializeObject(groupOrder)}");

            var result = await _groupOrderService.GroupOrderCreateService(groupOrder);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "创建成功",
                    group_order_id = result.Data.RequestId
                };
                return Ok(response);  // 成功时返回 200 和数据
            }
            else
            {
                return BadRequest(result);  // 失败时返回 400 和错误消息
            }
        }

        //POST v1/groupOrder/create
        [HttpPost("create")]
        public async Task<IActionResult> PostGroupOrderCreateWell([FromHeader(Name = "Authorization")] string authHeader, [FromBody] JsonElement requestJson)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Create GroupOrder: {JsonConvert.SerializeObject(requestJson)}");

            var result = await _groupOrderService.CreateGroupOrderWithProjectService(requestJson);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "创建成功",
                    group_order_id = result.Data
                };
                return Ok(response);  // 成功时返回 200 和数据
            }
            else
            {
                return BadRequest(result);  // 失败时返回 400 和错误消息
            }
        }

        //POST v1/groupOrder/modifyRequest
        [HttpPost("modifyRequest")]
        public async Task<IActionResult> PostGroupOrderModify([FromHeader(Name = "Authorization")] string authHeader, [FromBody] JsonElement modifyRequest)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Modify GroupOrder: {JsonConvert.SerializeObject(modifyRequest)}");

            var result = await _groupOrderService.GroupOrderModifyService(modifyRequest);

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

        //POST v1/groupOrder/modify
        [HttpPost("modify")]
        public async Task<IActionResult> PostGroupOrderModifyWell([FromHeader(Name = "Authorization")] string authHeader, [FromBody] JsonElement modifyRequest)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            Console.WriteLine($"Modify GroupBOTH: {JsonConvert.SerializeObject(modifyRequest)}");

            var result = await _groupOrderService.ModifyGroupOrderWithProjectService(modifyRequest);

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

        //POST v1/groupOrder/participate
        [HttpPost("participate")]
        public async Task<IActionResult> PostGroupOrderParticipate([FromHeader(Name = "Authorization")] string authHeader, int userId, int requestId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            var result = await _groupOrderService.AddUserToGroupOrderService(userId,requestId);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "用户加入拼单请求成功"
                };
                return Ok(response);  // 成功时返回 200 和数据
            }
            else
            {
                return BadRequest(result);  // 失败时返回 400 和错误消息
            }

        }

        //POST v1/groupOrder/quit
        [HttpPost("quit")]
        public async Task<IActionResult> PostGroupOrderQuit([FromHeader(Name = "Authorization")] string authHeader, int userId, int requestId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            var result = await _groupOrderService.DeleteUserToGroupOrderService(userId, requestId);

            if (result.IsSuccess)
            {
                var response = new
                {
                    code = 200,
                    msg = "用户退出拼单请求成功"
                };
                return Ok(response);  // 成功时返回 200 和数据
            }
            else
            {
                return BadRequest(result);  // 失败时返回 400 和错误消息
            }

        }

        //DELETE v1/groupOrder/delete
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteGroupOrder([FromHeader(Name = "Authorization")] string authHeader, int requestId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            var result = await _groupOrderService.GroupOrderDeleteService(requestId);

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

        public class VoiceCreateRequest
        {
            public string Text { get; set; }
            public string ActiveIndex { get; set; }
        }

        [HttpPost("voiceCreate")]
        public async Task<IActionResult> VoiceCreateGroupOrder([FromHeader(Name = "Authorization")] string authHeader, [FromBody] VoiceCreateRequest voiceRequest)
        {
            if (!_userService.TokenCheckService(authHeader))
            {
                return Unauthorized();
            }

            string apiKey = "sk-0fe92bf297734396b9b2e66f4dd42169"; // 你的通义千问 API Key
            string requestUrl = "https://dashscope.aliyuncs.com/compatible-mode/v1/chat/completions";

            string prompt = BuildPromptForTongyi(voiceRequest.Text, voiceRequest.ActiveIndex);

            _httpClient.DefaultRequestHeaders.Clear(); // 清除之前的头信息，以防冲突
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var payload = new
            {
                model = "qwen-turbo",
                input = new {
                    messages = new [] {
                        new { role = "user", content = prompt }
                    }
                },
                parameters = new {
                    result_format = "message"
                }
            };

            string jsonPayload = JsonConvert.SerializeObject(payload);
            HttpContent httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, httpContent);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    JObject result = JObject.Parse(responseBody);
                    string extractedJsonString = result?["output"]?["choices"]?[0]?["message"]?["content"]?.ToString();

                    if (string.IsNullOrEmpty(extractedJsonString))
                    {
                        return BadRequest(new { message = "AI未能返回有效内容", rawResponse = responseBody });
                    }

                    if (extractedJsonString.StartsWith("```json"))
                    {
                        extractedJsonString = extractedJsonString.Substring(7);
                    }
                    if (extractedJsonString.EndsWith("```"))
                    {
                        extractedJsonString = extractedJsonString.Substring(0, extractedJsonString.Length - 3);
                    }
                    extractedJsonString = extractedJsonString.Trim();

                    try
                    {
                        var jsonObject = JsonConvert.DeserializeObject(extractedJsonString);
                        return Ok(jsonObject);
                    }
                    catch (JsonReaderException ex)
                    {
                        return BadRequest(new { message = "AI返回内容非标准JSON格式", rawOutput = extractedJsonString, error = ex.Message, fullResponse = responseBody });
                    }
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { message = "调用AI服务失败", details = responseBody });
                }
            }
            catch (HttpRequestException e)
            {
                return StatusCode(500, new { message = "请求AI服务时发生网络错误", details = e.Message });
            }
            catch (Exception ex) // 捕获其他潜在错误，例如JObject.Parse失败
            {
                return StatusCode(500, new { message = "处理AI响应时发生内部错误", details = ex.Message });
            }
        }

        private string BuildPromptForTongyi(string voiceText, string formType)
        {
            string basePrompt = $"请分析以下语音内容，提取出拼单信息并返回JSON格式数据。语音内容：\"{voiceText}\"";
            string fieldsDescription = "";
            string jsonTemplate = "";
            // 获取当前日期，用于辅助大模型进行时间转换
            string currentDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (formType == "1") // 拼好物
            {
                fieldsDescription = @"
拼好物表单包含以下字段：
- title: 拼单标题 (字符串)
- description: 拼单描述 (字符串)
- name: 物品名称 (字符串)
- price: 总金额 (数字，返回字符串格式)
- platform: 平台或商家 (字符串)
- location: 购物或收货地 (字符串)
- customValue: 预期人数 (数字，默认1，返回字符串格式)
- request: 额外要求 (字符串，可为空)
- distribute: 总价分配 (字符串)
- starttime: 开放时间 (ISO格式 YYYY-MM-DDTHH:mm:ss，如果未提及则设为空字符串)
- endtime: 关闭时间 (ISO格式 YYYY-MM-DDTHH:mm:ss，如果未提及则设为空字符串)";
                jsonTemplate = @"{
""title"": """",
""description"": """",
""name"": """",
""price"": """",
""platform"": """",
""location"": """",
""customValue"": ""1"",
""request"": """",
""distribute"": """",
""starttime"": """",
""endtime"": """"
}";
            }
            else if (formType == "2") // 拼交通
            {
                fieldsDescription = @"
拼交通表单包含以下字段：
- title: 拼单标题 (字符串)
- description: 拼单描述 (字符串)
- name: 交通名称 (字符串)
- price: 总金额 (数字，返回字符串格式)
- location1: 始发地 (字符串)
- location2: 目的地 (字符串)
- customValue: 预期人数 (数字，默认1，返回字符串格式)
- request: 额外要求 (字符串，可为空)
- distribute: 总价分配 (字符串)
- starttime: 开放时间 (ISO格式 YYYY-MM-DDTHH:mm:ss)
- endtime: 关闭时间 (ISO格式 YYYY-MM-DDTHH:mm:ss)
- time: 预计出发时间 (ISO格式 YYYY-MM-DDTHH:mm:ss)";
                jsonTemplate = @"{
""title"": """",
""description"": """",
""name"": """",
""price"": """",
""location1"": """",
""location2"": """",
""customValue"": ""1"",
""request"": """",
""distribute"": """",
""starttime"": """",
""endtime"": """",
""time"": """"
}";
            }
            else // 拼活动 (formType == "3")
            {
                fieldsDescription = @"
拼活动表单包含以下字段：
- title: 拼单标题 (字符串)
- description: 拼单描述 (字符串)
- name: 活动名称 (字符串)
- price: 总金额 (数字，返回字符串格式)
- location: 活动地点 (字符串)
- customValue: 预期人数 (数字，默认1，返回字符串格式)
- request: 额外要求 (字符串，可为空)
- distribute: 总价分配 (字符串)
- starttime: 开放时间 (ISO格式 YYYY-MM-DDTHH:mm:ss)
- endtime: 关闭时间 (ISO格式 YYYY-MM-DDTHH:mm:ss)
- time: 预计活动时间 (ISO格式 YYYY-MM-DDTHH:mm:ss)";
                jsonTemplate = @"{
""title"": """",
""description"": """",
""name"": """",
""price"": """",
""location"": """",
""customValue"": ""1"",
""request"": """",
""distribute"": """",
""starttime"": """",
""endtime"": """",
""time"": """"
}";
            }

            return $"{basePrompt}\n\n当前日期是: {currentDate}，请根据此日期转换相对时间（例如“明天”）。\n\n{fieldsDescription}\n\n请严格按照以下JSON格式返回，不要包含任何其他内容或Markdown标记：\n{jsonTemplate}\n\n注意：\n1. 如果语音中没有明确提到某个字段，请将该字段的值设为空字符串。\n2. 时间格式请严格使用 ISO 8601 格式 (YYYY-MM-DDTHH:mm:ss)。\n3. 数字字段（如price, customValue）请返回字符串格式的数字。\n4. 确保返回的是一个纯粹的、没有额外字符或注释的JSON对象。";
        }
    }
}
