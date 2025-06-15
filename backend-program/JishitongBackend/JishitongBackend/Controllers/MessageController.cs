using Microsoft.AspNetCore.Mvc;
using JishitongBackend.Models;
using JishitongBackend.Services;
using Newtonsoft.Json;

namespace JishitongBackend.Controllers
{
    [Route("v1/message")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessageService _messageService;
        private readonly UserService _userService;

        public MessageController(MessageService messageService, UserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        //GET v1/message/getInformation
        [HttpGet("getInformation")]
        public async Task<IActionResult> GetMessageInformationController([FromHeader(Name = "Authorization")] string authHeader, int userId)
        {
            if (!_userService.TokenCheckService(authHeader))
            {
                return Unauthorized();  // 校验 token，如果无效返回 401 Unauthorized
            }

            var result = await _messageService.GetMessageInformationService(userId);

            if (result.IsSuccess)
            {
                if (result.Data == null || result.Data.Count == 0)
                {
                    return NoContent(); // 如果数据为空，返回 204 No Content
                }

                var response = new
                {
                    code = 200,
                    msg = "获取消息信息成功",
                    messages = result.Data.Select(m => new {
                        message_id = m.MessageId,
                        sender_id = m.SenderId,
                        receiver_id = m.ReceiverId,
                        message_time = m.MessageTime,
                        content = m.Content,
                        request_id = m.RequestId
                    }).ToList()
                };
                return Ok(response);  // 返回消息列表
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

        //DELETE v1/message/delete
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteMessage([FromHeader(Name = "Authorization")] string authHeader, int messageId)
        {
            if (!_userService.TokenCheckService(authHeader)) { return Unauthorized(); }

            var result = await _messageService.MessageDeleteService(messageId);

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
