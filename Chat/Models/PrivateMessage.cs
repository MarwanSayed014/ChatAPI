using ChatAPI.Types;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatAPI.Models
{
    public class PrivateMessage
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

        public Guid PrivateChatId { get; set; }
        [ForeignKey("PrivateChatId")]
        public virtual PrivateChat PrivateChat { get; set; }

        [Required]
        public MessageStatus MessageStatus { get; set; }
    }
}
