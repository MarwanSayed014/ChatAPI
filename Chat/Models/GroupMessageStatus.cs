using ChatAPI.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatAPI.Models
{
    public class GroupMessageStatus
    {
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public Guid GroupChatId { get; set; }
        [ForeignKey("GroupChatId")]
        public virtual GroupChat GroupChat { get; set; }
        [Required]
        public MessageStatus MessageStatus { get; set; }
    }
}
