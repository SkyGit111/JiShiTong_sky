using JishitongBackend.Repositories;
using JishitongBackend.Models;

namespace JishitongBackend.Services
{
    public class MessageService
    {
        private readonly IConfiguration _configuration;
        private readonly MessageRepository _messageRepository;

        public MessageService(IConfiguration configuration)
        {
            _configuration = configuration; // 使用 IConfiguration 获取配置信息
            _messageRepository = new MessageRepository(new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")));
        }

        public async Task<Result<Message>> MessageCreateService(Message message)
        {
            try
            {
                var result = await _messageRepository.MessageCreateRepository(message);

                if (result == -1)
                {
                    return Result<Message>.Failure("创建消息失败");
                }
                else
                {
                    message.MessageId = result;
                    return Result<Message>.Success(message, "创建消息成功");
                }

            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<Message>.Failure($"发生错误: {ex.Message}");
            }
        }

        public async Task<Result<String>> MessageDeleteService(int messageId)
        {
            try
            {
                var result = await _messageRepository.MessageDeleteRepository(messageId);

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

        public async Task<Result<List<Message>>> GetMessageInformationService(int userId)
        {
            try
            {
                // 获取拼单信息
                var result = await _messageRepository.GetMessagesByReceiverIdRepository(userId);

                if (result == null || result.Count == 0)
                {
                    return Result<List<Message>>.Failure(204, "查询结果为空");
                }

                return Result<List<Message>>.Success(result);
            }
            catch (Exception ex)
            {
                // 如果发生异常，返回失败消息
                return Result<List<Message>>.Failure($"发生错误: {ex.Message}");
            }
        }




    }
}
