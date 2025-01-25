using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Models
{
    public class GroupMessage
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(250)]
        public string Text { get; set; }

        [MaxLength(10)]
        public List<string>? AttachmentPaths { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public Guid GroupChatId { get; set; }
        [ForeignKey("GroupChatId")]
        public virtual GroupChat GroupChat { get; set; }

        public Guid SenderId { get; set; }
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }
    }
}
