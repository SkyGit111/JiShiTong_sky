namespace JishitongBackend.Models
{
    public class Message
    {
        private int messageId;
        private int? senderId;
        private int? receiverId;
        private DateTime? messageTime;
        private string? content;
        private int? requestId;

        public int MessageId
        {
            get { return messageId; }
            set { messageId = value; }
        }

        public int? SenderId
        {
            get { return senderId; }
            set { senderId = value; }
        }

        public int? ReceiverId
        {
            get { return receiverId; }
            set { receiverId = value; }
        }

        public DateTime? MessageTime
        {
            get { return messageTime; }
            set { messageTime = value; }
        }

        public string? Content
        {
            get { return content; }
            set { content = value; }
        }

        public int? RequestId
        {
            get { return requestId; }
            set { requestId = value; }
        }
    }
}
