using System.ComponentModel.DataAnnotations.Schema;

namespace ChatAPI.Models
{
    public class UserPrivateChat
    {
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public Guid PrivateChatId { get; set; }
        [ForeignKey("PrivateChatId")]
        public virtual PrivateChat PrivateChat { get; set; }
    }
}
