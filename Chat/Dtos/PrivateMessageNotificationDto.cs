using ChatAPI.Models;

namespace ChatAPI.Dtos
{
    public class PrivateMessageNotificationDto
    {
        public PrivateMessage PrivateMessage { get; set; }
        public Guid AnotherUserId { get; set; }
        public string SenderName { get; set; }
    }
}
