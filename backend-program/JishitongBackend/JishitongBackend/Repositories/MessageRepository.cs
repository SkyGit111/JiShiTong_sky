using JishitongBackend.Models;

namespace JishitongBackend.Repositories
{
    public class MessageRepository
    {
        private readonly MySqlConnection _mySqlConnection;

        public MessageRepository(MySqlConnection mySqlConnection)
        {
            _mySqlConnection = mySqlConnection;
        }

        public async Task<int> MessageCreateRepository(Message message)
        {

            string query = @"
                INSERT INTO message (SENDER_ID, RECEIVER_ID,  MESSAGE_TIME, CONTENT, REQUEST_ID)
                VALUES (?,?,?,?,?)";

            var parameters = new List<object>
                {
                message.SenderId,
                message.ReceiverId,
                message.MessageTime,
                message.Content,
                message.RequestId
                };

            var rowsAffected = await _mySqlConnection.ExecuteNonQueryAsync(query, parameters);

            if (rowsAffected == 0)
            {
                return -1;
            }
            else
            {
                string selectQuery = "SELECT LAST_INSERT_ID()";
                var newMessageIdResult = await _mySqlConnection.ExecuteQueryAsync(selectQuery);

                // 这里假设 LAST_INSERT_ID() 返回一个只有一列的结果
                if (newMessageIdResult.Count > 0)
                {
                    // 从结果中获取第一个字典，获取 'LAST_INSERT_ID()' 对应的值
                    var newMessageId = newMessageIdResult[0].Values.FirstOrDefault();
                    if (newMessageId != null && int.TryParse(newMessageId.ToString(), out int messageId))
                    {
                        return messageId; // 返回新插入的 ID
                    }

                }
                return -1; // 如果无法解析 ID 或者结果为空
            }

        }


        public async Task<int> MessageDeleteRepository(int messageId)
        {
            string query = "DELETE FROM message WHERE message_id = ?";
            var parameters = new List<object> { messageId };

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

        public async Task<List<Message>> GetMessagesByReceiverIdRepository(int receiverId)
        {
            string query = "SELECT * FROM message WHERE RECEIVER_ID = ?";
            var parameters = new List<object> { receiverId };

            try
            {
                var result = await _mySqlConnection.ExecuteQueryAsync(query, parameters);

                var messages = new List<Message>();
                if (result.Count > 0)
                {
                    foreach (var row in result)
                    {
                        var message = new Message
                        {
                            MessageId = Convert.ToInt32(row["MESSAGE_ID"]),
                            SenderId = Convert.ToInt32(row["SENDER_ID"]),
                            ReceiverId = Convert.ToInt32(row["RECEIVER_ID"]),
                            MessageTime = row["MESSAGE_TIME"] != DBNull.Value ? Convert.ToDateTime(row["MESSAGE_TIME"]) : null,
                            Content = row["CONTENT"]?.ToString(),
                            RequestId = Convert.ToInt32(row["REQUEST_ID"])
                        };

                        messages.Add(message);
                    }
                }

                return messages;  // 返回消息列表
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching messages: {ex.Message}");
                return new List<Message>();  // 发生异常时返回空列表
            }
        }


    }
}
