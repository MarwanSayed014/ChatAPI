using ChatAPI.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatAPI.Models
{
    public class Friendship
    {
        [Key]
        public Guid FriendshipId { get; set; }
        public Guid RequestorUserId { get; set; }
        [ForeignKey("RequestorUserId")]
        public virtual User RequestorUser { get; set; }
        public Guid RespondentUserId { get; set; }
        [ForeignKey("RespondentUserId")]
        public virtual User RespondentUser { get; set; }
        
        [Required]
        public FriendshipStatus FriendshipStatus { get; set; }
    }
}
