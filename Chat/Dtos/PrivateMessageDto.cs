using System.ComponentModel.DataAnnotations;

namespace ChatAPI.Dtos
{
    public class PrivateMessageDto
    {
        [Required]
        [MinLength(2)]
        [MaxLength(250)]
        public string Text { get; set; }
        public Guid AnotherUserId { get; set; }

    }
}
