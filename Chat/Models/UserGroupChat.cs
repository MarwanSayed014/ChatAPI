using ChatAPI.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatAPI.Models
{
    public class UserGroupChat
    {
        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public Guid GroupChatId { get; set; }
        [ForeignKey("GroupChatId")]
        public virtual GroupChat GroupChat { get; set; }
        public virtual GroupUserRolesTypes GroupUserRolesTypes { get; set; }
    }
}
